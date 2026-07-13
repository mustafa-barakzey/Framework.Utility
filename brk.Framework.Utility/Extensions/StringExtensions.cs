using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace brk.Framework.Utility.Extensions;

/// <summary>
/// Provides general-purpose string extension methods: safe numeric parsing, byte-array
/// conversion, case conversion, reflection-based display names, and normalization of
/// Arabic/Persian "Ye" and "Ke" characters to their canonical Persian forms.
/// </summary>
public static class StringExtensions
{
    /// <summary>Arabic Yeh character (ي, U+064A) as commonly typed on Arabic keyboards.</summary>
    private const char ArabicYeChar = (char)1610;

    /// <summary>Canonical Persian Yeh character (ی, U+06CC).</summary>
    private const char PersianYeChar = (char)1740;

    /// <summary>Arabic Kaf character (ك, U+0643) as commonly typed on Arabic keyboards.</summary>
    private const char ArabicKeChar = (char)1603;

    /// <summary>Canonical Persian Kaf character (ک, U+06A9).</summary>
    private const char PersianKeChar = (char)1705;

    private static readonly Dictionary<char, char> FaToEnDigits = new()
    {
        ['۰'] = '0', ['۱'] = '1', ['۲'] = '2', ['۳'] = '3', ['۴'] = '4',
        ['۵'] = '5', ['۶'] = '6', ['۷'] = '7', ['۸'] = '8', ['۹'] = '9'
    };

    // ---- Safe numeric parsing ----------------------------------------------------------

    /// <summary>
    /// Parses <paramref name="input"/> as a <see cref="long"/>, returning
    /// <paramref name="replacement"/> instead of throwing if parsing fails.
    /// </summary>
    /// <param name="input">The string to parse. May be null or malformed.</param>
    /// <param name="replacement">The value to return when parsing fails. Defaults to <see cref="long.MinValue"/>.</param>
    /// <returns>The parsed value, or <paramref name="replacement"/> if parsing fails.</returns>
    public static long ToSafeLong(this string? input, long replacement = long.MinValue) =>
        long.TryParse(input, out long result) ? result : replacement;

    /// <summary>
    /// Parses <paramref name="input"/> as a <see cref="long"/>, returning <c>null</c> instead
    /// of throwing if parsing fails.
    /// </summary>
    /// <param name="input">The string to parse. May be null or malformed.</param>
    /// <returns>The parsed value, or <c>null</c> if parsing fails.</returns>
    public static long? ToSafeNullableLong(this string? input) =>
        long.TryParse(input, out long result) ? result : null;

    /// <summary>
    /// Parses <paramref name="input"/> as an <see cref="int"/>, returning
    /// <paramref name="replacement"/> instead of throwing if parsing fails.
    /// </summary>
    /// <param name="input">The string to parse. May be null or malformed.</param>
    /// <param name="replacement">The value to return when parsing fails. Defaults to <see cref="int.MinValue"/>.</param>
    /// <returns>The parsed value, or <paramref name="replacement"/> if parsing fails.</returns>
    public static int ToSafeInt(this string? input, int replacement = int.MinValue) =>
        int.TryParse(input, out int result) ? result : replacement;

    /// <summary>
    /// Parses <paramref name="input"/> as an <see cref="int"/>, returning <c>null</c> instead
    /// of throwing if parsing fails.
    /// </summary>
    /// <param name="input">The string to parse. May be null or malformed.</param>
    /// <returns>The parsed value, or <c>null</c> if parsing fails.</returns>
    public static int? ToSafeNullableInt(this string? input) =>
        int.TryParse(input, out int result) ? result : null;

    // ---- General string helpers --------------------------------------------------------

    /// <summary>
    /// Returns <paramref name="input"/> unchanged, or <see cref="string.Empty"/> if it is null.
    /// </summary>
    /// <param name="input">The string, which may be null.</param>
    /// <returns><paramref name="input"/>, or an empty string if it was null.</returns>
    public static string ToStringOrEmpty(this string? input) => input ?? string.Empty;

    /// <summary>
    /// Converts a PascalCase or camelCase string to lower snake_case, e.g. "UserId" → "user_id".
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>The snake_case version of <paramref name="input"/>, or the original value if null/empty.</returns>
    public static string ToUnderscoreCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return string.Concat(input.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c : c.ToString())).ToLower();
    }

    /// <summary>
    /// Encodes a string as a UTF-8 byte array.
    /// </summary>
    /// <param name="input">The string to encode.</param>
    /// <returns>The UTF-8 encoded bytes.</returns>
    public static byte[] ToByteArray(this string input) => Encoding.UTF8.GetBytes(input);

    /// <summary>
    /// Decodes a UTF-8 byte array back into a string.
    /// </summary>
    /// <param name="input">The UTF-8 encoded bytes.</param>
    /// <returns>The decoded string.</returns>
    public static string FromByteArray(this byte[] input) => Encoding.UTF8.GetString(input);

    // ---- Reflection / display -----------------------------------------------------------

    /// <summary>
    /// Gets the value of the <see cref="DisplayNameAttribute"/> applied to a property, if any.
    /// </summary>
    /// <param name="property">The property to inspect.</param>
    /// <returns>
    /// The display name, or <see cref="string.Empty"/> if the property has no
    /// <see cref="DisplayNameAttribute"/>.
    /// </returns>
    public static string DisplayName(this PropertyInfo property)
    {
        ArgumentNullException.ThrowIfNull(property);

        return property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? string.Empty;
    }

    // ---- Arabic/Persian normalization ----------------------------------------------------

    /// <summary>
    /// Converts <paramref name="data"/> to a string and normalizes Arabic Yeh/Kaf characters to
    /// their canonical Persian forms. See <see cref="ApplyCorrectYeKe(string?)"/>.
    /// </summary>
    /// <param name="data">The object to convert and normalize. May be null.</param>
    /// <returns>
    /// The normalized string, or <see cref="string.Empty"/> if <paramref name="data"/> is null
    /// (matching the behavior of the <see cref="ApplyCorrectYeKe(string?)"/> overload).
    /// </returns>
    public static string ApplyCorrectYeKe(this object? data) =>
        data?.ToString().ApplyCorrectYeKe() ?? string.Empty;

    /// <summary>
    /// Replaces Arabic Yeh (ي) and Kaf (ك) characters with their canonical Persian
    /// equivalents (ی, ک) and trims the result.
    /// </summary>
    /// <param name="data">The string to normalize. May be null or whitespace.</param>
    /// <returns>The normalized, trimmed string, or <see cref="string.Empty"/> if <paramref name="data"/> is null or whitespace.</returns>
    public static string ApplyCorrectYeKe(this string? data) =>
        string.IsNullOrWhiteSpace(data)
            ? string.Empty
            : data.Replace(ArabicYeChar, PersianYeChar).Replace(ArabicKeChar, PersianKeChar).Trim();

    /// <summary>
    /// Replaces every Farsi (Eastern Arabic-Indic) digit in the given text with its ASCII digit equivalent.
    /// </summary>
    /// <param name="text">The source text containing Farsi digits.</param>
    /// <returns>A new string with Farsi digits replaced by ASCII digits, or the original value if null/empty.</returns>
    public static string ToEnglishNumber(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var chars = text.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (FaToEnDigits.TryGetValue(chars[i], out var enDigit))
                chars[i] = enDigit;
        }

        return new string(chars);
    }


    private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);

    /// <summary>Matches one or more consecutive whitespace characters (spaces, tabs, newlines).</summary>
    private static readonly Regex WhitespaceRunRegex = new(@"\s+", RegexOptions.Compiled, RegexTimeout);

    /// <summary>
    /// Matches any HTML tag except a line break tag (&lt;br&gt;, &lt;br/&gt;, &lt;br /&gt;),
    /// case-insensitively.
    /// </summary>
    private static readonly Regex HtmlTagExceptBreakRegex =
        new(@"<(?!br[\x20/>])[^<>]+>", RegexOptions.Compiled | RegexOptions.IgnoreCase, RegexTimeout);

    /// <summary>
    /// Trims the text and collapses any run of whitespace (spaces, tabs, newlines) into a
    /// single space.
    /// </summary>
    /// <param name="text">The text to clean up.</param>
    /// <returns>The trimmed text with internal whitespace collapsed, or <see cref="string.Empty"/> if <paramref name="text"/> is null.</returns>
    public static string FixText(this string? text) =>
        text == null ? string.Empty : WhitespaceRunRegex.Replace(text.Trim(), " ");

    /// <summary>
    /// Normalizes an email address: trims whitespace, removes internal spaces, and
    /// lowercases it using invariant (culture-independent) casing.
    /// </summary>
    /// <param name="email">The email address to normalize.</param>
    /// <returns>The normalized email address, or <see cref="string.Empty"/> if <paramref name="email"/> is null.</returns>
    public static string FixEmail(this string? email) =>
        email == null ? string.Empty : email.Trim().Replace(" ", string.Empty).ToLowerInvariant();

    /// <summary>
    /// Removes all HTML tags from <paramref name="text"/> except line break tags
    /// (&lt;br&gt;, &lt;br/&gt;, &lt;br /&gt;), which are preserved regardless of case.
    /// </summary>
    /// <param name="text">The HTML text to strip tags from.</param>
    /// <returns>The text with all non-break tags removed, or <see cref="string.Empty"/> if <paramref name="text"/> is null.</returns>
    public static string RemoveHtmlTagsExceptBreak(this string? text) =>
        text == null ? string.Empty : HtmlTagExceptBreakRegex.Replace(text, string.Empty);

    /// <summary>
    /// Converts text into a URL-friendly slug: trims whitespace, collapses any whitespace run
    /// into a single dash, and trims any leading or trailing dashes.
    /// </summary>
    /// <param name="text">The text to convert.</param>
    /// <returns>The URL-friendly slug, or <see cref="string.Empty"/> if <paramref name="text"/> is null.</returns>
    public static string FixTextForUrl(this string? text)
    {
        if (text == null)
            return string.Empty;

        var slug = WhitespaceRunRegex.Replace(text.Trim(), "-");
        return slug.Trim('-');
    }

    /// <summary>
    /// Splits a comma-separated list of tags into individual, trimmed, non-empty entries.
    /// </summary>
    /// <param name="tags">The comma-separated tag list.</param>
    /// <returns>The individual tags, trimmed of surrounding whitespace, with empty entries removed.</returns>
    public static string[] SplitTagsWithComma(this string? tags) =>
        tags == null
            ? Array.Empty<string>()
            : tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    /// <summary>
    /// Splits a dash-separated list of tags into individual, trimmed, non-empty entries.
    /// </summary>
    /// <param name="tags">The dash-separated tag list.</param>
    /// <returns>The individual tags, trimmed of surrounding whitespace, with empty entries removed.</returns>
    public static string[] SplitTagsWithDash(this string? tags) =>
        tags == null
            ? Array.Empty<string>()
            : tags.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}