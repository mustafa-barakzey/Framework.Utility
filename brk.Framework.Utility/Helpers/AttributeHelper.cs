using System.Reflection;

namespace brk.Framework.Utility.Helpers;

/// <summary>
/// Provides reflection-based helpers for inspecting whether a class's properties are
/// decorated with a given attribute.
/// </summary>
public static class AttributeHelper
{
    /// <summary>
    /// Checks whether the property with the given name, on the given class, is decorated
    /// with the specified attribute.
    /// </summary>
    /// <typeparam name="TClass">The class to inspect.</typeparam>
    /// <typeparam name="TAttribute">The attribute type to look for.</typeparam>
    /// <param name="propertyName">The property name to look up (case-insensitive).</param>
    /// <returns>
    /// <c>true</c> if a property with this name exists and has <typeparamref name="TAttribute"/>
    /// (or a type derived from it) applied to it; otherwise <c>false</c>.
    /// </returns>
    public static bool HasThisAttribute<TClass, TAttribute>(string propertyName)
        where TClass : class
        where TAttribute : Attribute
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        var property = typeof(TClass)
            .GetProperties()
            .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));

        return property != null && property.IsDefined(typeof(TAttribute), inherit: true);
    }

    /// <summary>
    /// Gets all properties of the given class that are decorated with the specified attribute.
    /// </summary>
    /// <typeparam name="TClass">The class to inspect.</typeparam>
    /// <typeparam name="TAttribute">The attribute type to look for.</typeparam>
    /// <returns>
    /// The list of properties that have <typeparamref name="TAttribute"/> (or a type derived
    /// from it) applied to them.
    /// </returns>
    public static List<PropertyInfo> GetPropertiesByAttribute<TClass, TAttribute>()
        where TClass : class
        where TAttribute : Attribute
        => typeof(TClass)
            .GetProperties()
            .Where(p => p.IsDefined(typeof(TAttribute), inherit: true))
            .ToList();
}