namespace brk.Framework.Utility.Constants;

/// <summary>
/// Centralized regex patterns used across the string validation extensions.
/// </summary>
/// <remarks>
/// Renamed from <c>RegexPatterns</c> to <c>RegexPattern</c>, and
/// <c>PersianMobilePattern</c> renamed to <c>MobilePattern</c>, to match the names
/// referenced by <c>StringValidatorExtensions</c> (<c>RegexPattern.MobilePattern</c>,
/// <c>RegexPattern.EmailPattern</c>). Rename back on both sides if you'd rather keep
/// the original naming.
/// </remarks>
public static class RegexPatterns
{
    /// <summary>
    /// Iran mobile numbers pattern: 09xx xxx xxxx (11 digits)
    /// </summary>
    /// <remarks>
    /// Matches an Iranian mobile number with an optional country-code prefix
    /// (<c>+98</c>, <c>0098</c>, or <c>98</c>) or a leading <c>0</c>, followed by exactly
    /// 9 digits. Accepts both ASCII (0-9) and Persian (۰-۹) digits, and their Persian
    /// equivalents in the prefix itself.
    /// </remarks>
    public const string IranMobilePattern =@"(\+989|00989|989|09|\+۹۸۹|۰۰۹۸۹|۹۸۹|۰۹)[0-9,۰,١,۲,۳,۴,۵,۶,۷,۸,۹]{9,9}$";
    public const string IranStaticPhonePattern = @"^(\+98|0098|0)?[1-8][1-9][0-9]{8}$";
    
    /// <summary>
    /// Afghanistan Mobile Number Validation : 07xx xxx xxx (10 digits with 0, or 7xx xxx xxx without)
    /// </summary>
    /// <remarks>
    /// Country code: +93
    /// International formats: +937XXXXXXXX, 00937XXXXXXXX, 937XXXXXXXX
    /// Local format: 07XXXXXXXX
    /// Mobile numbers start with: 7
    /// Total digits after country code: 9 (starts with 7 followed by 8 digits)
    /// </remarks>
    public const string AfghanistanMobilePattern = @"(\+937|00937|937|07|\+۹۳۷|۰۰۹۳۷|۹۳۷|۰۷)[0-9۰۱۲۳۴۵۶۷۸۹]{8}$";
    public const string AfghanistanStaticPhonePattern = @"^(\+93|0093|0)?[2-6][0-9]{7,8}$";
    
    /// <summary>
    /// pattern for email
    /// </summary>
    public const string EmailPattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
    
    public const string NumericPattern = @"^\d+$";
    public const string NonDigitPattern = @"[^0-9۰-۹]";
}