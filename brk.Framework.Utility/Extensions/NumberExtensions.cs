namespace brk.Framework.Utility.Extensions;

public static class NumberExtensions
{
    public static bool IsNull<T>(this T obj) => obj == null;
    public static bool IsNotNull<T>(this T obj) => obj != null;
    public static bool IsGreaterThanZero(this int value) => value > 0;
    public static bool IsGreaterThanZero(this int? value) => value is > 0;
    public static bool IsGreaterThanZero(this decimal value) => value > 0;
    public static bool IsGreaterThanZero(this decimal? value) => value is > 0;
    public static decimal Normalize(this decimal value)
    {
        return value / 1.000000000000000000000000000000000m;
    }
}