namespace brk.Framework.Utility.Extensions;

public static class NumberExtensions
{
    /// <summary>
    /// Determines whether the specified value is <see langword="null"/>.
    /// </summary>
    public static bool IsNull<T>(this T obj) => obj == null;

    /// <summary>
    /// Determines whether the specified value is not <see langword="null"/>.
    /// </summary>
    public static bool IsNotNull<T>(this T obj) => obj != null;

    /// <summary>
    /// Determines whether the specified integer is greater than zero.
    /// </summary>
    public static bool IsGreaterThanZero(this int value) => value > 0;

    /// <summary>
    /// Determines whether the specified nullable integer has a value greater than zero.
    /// </summary>
    public static bool IsGreaterThanZero(this int? value) => value is > 0;

    /// <summary>
    /// Determines whether the specified decimal value is greater than zero.
    /// </summary>
    public static bool IsGreaterThanZero(this decimal value) => value > 0;

    /// <summary>
    /// Determines whether the specified nullable decimal has a value greater than zero.
    /// </summary>
    public static bool IsGreaterThanZero(this decimal? value) => value is > 0;

    /// <summary>
    /// Normalizes the specified decimal value.
    /// </summary>
    public static decimal Normalize(this decimal value)
    {
        return value / 1.000000000000000000000000000000000m;
    }
}
