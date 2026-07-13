using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Reflection;

namespace brk.Framework.Utility.Extensions;

/// <summary>
/// Provides helper extension methods for working with <see cref="Enum"/> types: counting set
/// bits in flag enums, converting between flag enums and collections of individual values,
/// building dictionaries of enum values keyed by their underlying integer or by the enum value
/// itself, and reading <see cref="DisplayAttribute"/> / <see cref="DescriptionAttribute"/>
/// metadata from enum members.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Counts the number of flags set in a flag enum value (i.e., the number of set bits).
    /// </summary>
    /// <typeparam name="TEnum">The flag enum type.</typeparam>
    /// <param name="skillsToCount">The flag enum value to count.</param>
    /// <returns>The number of individual flags set in <paramref name="skillsToCount"/>.</returns>
    public static int Count<TEnum>(this TEnum skillsToCount) where TEnum : struct, Enum
    {
        ulong bits = Convert.ToUInt64(skillsToCount);
        return BitOperations.PopCount(bits);
    }

    /// <summary>
    /// Combines a sequence of individual flag enum values into a single flag enum value using
    /// bitwise OR.
    /// </summary>
    /// <typeparam name="TEnum">The flag enum type.</typeparam>
    /// <param name="values">The individual flag values to combine. May be null or empty.</param>
    /// <returns>
    /// A single <typeparamref name="TEnum"/> value with every flag from <paramref name="values"/>
    /// set, or <c>default</c> (typically the zero/"None" value) if <paramref name="values"/> is
    /// null or empty.
    /// </returns>
    public static TEnum ToFlag<TEnum>(this IEnumerable<TEnum>? values) where TEnum : struct, Enum
    {
        if (values == null)
            return default;

        ulong combined = values.Aggregate(0UL, (acc, value) => acc | Convert.ToUInt64(value));
        return (TEnum)Enum.ToObject(typeof(TEnum), combined);
    }

    /// <summary>
    /// Builds a dictionary of all values of enum <typeparamref name="T"/>, keyed by the enum
    /// value itself, with the display name (see <see cref="GetDisplayName"/>) as the value.
    /// </summary>
    /// <typeparam name="T">The enum type to enumerate.</typeparam>
    /// <returns>A dictionary mapping each enum member to its display name.</returns>
    public static Dictionary<T, string> GetItemsAsDictionary<T>() where T : struct, Enum
    {
        var dic = new Dictionary<T, string>();

        foreach (var item in Enum.GetValues<T>())
        {
            dic[item] = item.GetDisplayName();
        }

        return dic;
    }

    /// <summary>
    /// Gets the value of the <see cref="DescriptionAttribute"/> applied to the enum member, if any.
    /// </summary>
    /// <param name="enumValue">The enum member to inspect.</param>
    /// <returns>
    /// The description text, or <see cref="string.Empty"/> if the member has no
    /// <see cref="DescriptionAttribute"/> or could not be resolved (e.g. for a combined flags value).
    /// </returns>
    public static string GetDescription(this Enum enumValue)
    {
        ArgumentNullException.ThrowIfNull(enumValue);

        var fieldInfo = GetFieldInfo(enumValue);
        if (fieldInfo == null)
            return string.Empty;

        return fieldInfo.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
    }

    /// <summary>
    /// Gets a human-friendly display name for the enum member: the <see cref="DisplayAttribute"/>
    /// name if present, otherwise the <see cref="DisplayNameAttribute"/> name if present,
    /// otherwise the member's own name (<see cref="Enum.ToString()"/>).
    /// </summary>
    /// <param name="enumValue">The enum member to inspect.</param>
    /// <returns>The resolved display name for the enum member.</returns>
    public static string GetDisplayName(this Enum enumValue)
    {
        ArgumentNullException.ThrowIfNull(enumValue);

        var fieldInfo = GetFieldInfo(enumValue);
        if (fieldInfo == null)
            return enumValue.ToString();

        return fieldInfo.GetCustomAttribute<DisplayAttribute>()?.Name
               ?? fieldInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
               ?? enumValue.ToString();
    }

    /// <summary>
    /// Converts a flag enum value into the list of individual, non-zero flags it contains.
    /// </summary>
    /// <typeparam name="T">The flag enum type.</typeparam>
    /// <param name="values">The combined flag enum value.</param>
    /// <returns>
    /// A list of every defined, non-zero <typeparamref name="T"/> member that is set in
    /// <paramref name="values"/>.
    /// </returns>
    public static List<T> ToList<T>(this T values) where T : struct, Enum
    {
        var list = new List<T>();
        ulong valuesBits = Convert.ToUInt64(values);

        foreach (var item in Enum.GetValues<T>())
        {
            ulong itemBits = Convert.ToUInt64(item);

            // Skip the zero/"None" member explicitly: HasFlag(0) is always true for any
            // value, so without this check "None" would incorrectly appear in every result.
            if (itemBits != 0 && (valuesBits & itemBits) == itemBits)
                list.Add(item);
        }

        return list;
    }

    /// <summary>
    /// Looks up (and caches) the <see cref="FieldInfo"/> for the field backing an enum member.
    /// </summary>
    /// <param name="enumValue">The enum member whose field to resolve.</param>
    /// <returns>
    /// The <see cref="FieldInfo"/> for the member, or null if it could not be resolved
    /// (e.g. the value is a combination of flags with no single matching member name).
    /// </returns>
    private static FieldInfo? GetFieldInfo(Enum enumValue)
    {
        var type = enumValue.GetType();
        var name = enumValue.ToString();

        return type.GetField(name);
    }
}