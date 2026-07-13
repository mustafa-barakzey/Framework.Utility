using System.Text.RegularExpressions;
using brk.Framework.Utility.Constants;

namespace brk.Framework.Utility.Extensions;

public static class StringValidatorExtensions
{
    // ---- Generic numeric / length checks --------------------------------------------------

    /// <summary>
    /// Determines whether <paramref name="input"/> consists entirely of ASCII digits.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns><c>true</c> if non-empty and every character is a digit.</returns>
    public static bool IsNumeric(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;
        try
        {

            return new Regex(RegexPatterns.NumericPattern, RegexOptions.Compiled, TimeSpan.FromSeconds(1)).IsMatch(input);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Determines whether the length of <paramref name="input"/> is within the inclusive
    /// range [<paramref name="minLength"/>, <paramref name="maxLength"/>].
    /// </summary>
    public static bool IsLengthBetween(this string input, int minLength, int maxLength)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Length >= minLength && input.Length <= maxLength;
    }

    /// <summary>Determines whether <paramref name="input"/> is shorter than <paramref name="length"/>.</summary>
    public static bool IsLengthLessThan(this string input, int length)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Length < length;
    }

    /// <summary>Determines whether <paramref name="input"/> is shorter than or equal to <paramref name="length"/>.</summary>
    public static bool IsLengthLessThanOrEqual(this string input, int length)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Length <= length;
    }

    /// <summary>Determines whether <paramref name="input"/> is longer than <paramref name="length"/>.</summary>
    public static bool IsLengthGreaterThan(this string input, int length)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Length > length;
    }

    /// <summary>Determines whether <paramref name="input"/> is longer than or equal to <paramref name="length"/>.</summary>
    public static bool IsLengthGreaterThanOrEqual(this string input, int length)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Length >= length;
    }

    /// <summary>Determines whether the length of <paramref name="input"/> equals <paramref name="length"/>.</summary>
    public static bool IsLengthEqual(this string input, int length)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.Length == length;
    }
}