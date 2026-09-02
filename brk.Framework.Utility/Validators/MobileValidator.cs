using System.Text.RegularExpressions;
using System.Text;
using brk.Framework.Utility.Constants;
using brk.Framework.Utility.Extensions;

namespace brk.Framework.Utility.Validators;

/// <summary>
/// Validates mobile, static phone numbers and national identity numbers for Iran and Afghanistan
/// </summary>
public class MobileValidator
{
    #region Mobile Phone Patterns


    // Iran mobile prefixes (without country code)
    private static readonly string[] IranMobilePrefixes =
    {
        "0900", "0901", "0902", "0903", "0904", "0905", "0910", "0911", "0912",
        "0913", "0914", "0915", "0916", "0917", "0918", "0919", "0920", "0921",
        "0922", "0923", "0930", "0933", "0935", "0936", "0937", "0938", "0939",
        "0990", "0991", "0992", "0993", "0994"
    };


    // Afghanistan mobile prefixes
    private static readonly string[] AfghanistanMobilePrefixes =
    {
        "070", "071", "072", "073", "074", "075", "076", "077", "078", "079"
    };

    #endregion

    #region Static/Landline Phone Patterns

    // Common Iran area codes (2-3 digits)
    private static readonly string[] IranAreaCodes =
    {
        "021", // Tehran
        "025", // Qom
        "026", // Alborz
        "028", // Qazvin
        "031", // Isfahan
        "038", // Chaharmahal va Bakhtiari
        "041", // East Azerbaijan
        "044", // West Azerbaijan
        "045", // Ardabil
        "051", // Razavi Khorasan
        "054", // Sistan and Baluchestan
        "056", // South Khorasan
        "058", // North Khorasan
        "061", // Khuzestan
        "066", // Lorestan
        "071", // Fars
        "074", // Kohgiluyeh and Boyer-Ahmad
        "076", // Hormozgan
        "077", // Bushehr
        "081", // Hamedan
        "083", // Kermanshah
        "084", // Ilam
        "086", // Markazi
        "087", // Kurdistan
        "011", // Mazandaran
        "013", // Gilan
        "017", // Golestan
        "023", // Semnan
        "024", // Zanjan
        "034", // Kerman
        "035", // Yazd
        "045", // Ardabil
    };

    // Common Afghanistan area codes
    private static readonly string[] AfghanistanAreaCodes =
    {
        "020", // Kabul
        "030", // Kandahar
        "040", // Herat
        "050", // Mazar-i-Sharif
        "060", // Jalalabad
    };

    #endregion

    #region Public Validation Methods

    /// <summary>
    /// Validates Iranian mobile phone number
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate</param>
    /// <returns>True if valid Iranian mobile number</returns>
    public static bool IsIranMobile(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Normalize the number
        string normalized = NormalizePhoneNumber(phoneNumber, "98");

        // National significant number: 9 followed by nine digits.
        if (normalized.Length != 10 || !normalized.StartsWith('9'))
            return false;

        // Check if it matches mobile pattern
        return Array.Exists(IranMobilePrefixes, prefix => normalized.StartsWith(prefix.AsSpan(1), StringComparison.Ordinal));
    }

    /// <summary>
    /// Validates Afghanistan mobile phone number
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate</param>
    /// <returns>True if valid Afghanistan mobile number</returns>
    public static bool IsAfghanistanMobile(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Normalize the number
        string normalized = NormalizePhoneNumber(phoneNumber, "93");

        // Check length after normalization
        if (normalized.Length != 9 || !normalized.StartsWith("7"))
            return false;

        // Check if it matches mobile pattern
        return Array.Exists(AfghanistanMobilePrefixes, prefix => normalized.StartsWith(prefix.AsSpan(1), StringComparison.Ordinal));
    }


    /// <summary>
    /// Determines whether <paramref name="phoneNumber"/> is a valid Iranian landline (static) phone
    /// number: a leading 0, followed by an area code and subscriber number totaling at least
    /// 9 digits.
    /// </summary>
    /// <param name="phoneNumber">The phone number to validate.</param>
    /// <returns><c>true</c> if the value is non-empty and matches the landline pattern.</returns>
    public static bool IsIranStatic(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        string normalized = NormalizePhoneNumber(phoneNumber, "98");

        // Iranian static numbers are 11 digits starting with area code (after country code removal)
        if (normalized.Length != 10)
            return false;

        // Check if it matches static pattern
        if (!Regex.IsMatch($"0{normalized}", RegexPatterns.IranStaticPhonePattern))
            return false;

        // Validate area code (first 2-3 digits)
        string prefix = $"0{normalized.Substring(0, 3)}";
        if (Array.Exists(IranAreaCodes, code => code.StartsWith(prefix)))
            return true;

        // Check 2-digit area codes
        string twoDigitPrefix = $"0{normalized.Substring(0, 2)}";
        return Array.Exists(IranAreaCodes, code => code == twoDigitPrefix);
    }

    /// <summary>
    /// Validates Afghanistan static/landline phone number
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate</param>
    /// <returns>True if valid Afghanistan static number</returns>
    public static bool IsAfghanistanStatic(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        string normalized = NormalizePhoneNumber(phoneNumber, "93");

        // Afghanistan static numbers are 7-8 digits plus area code
        if (normalized.Length < 8 || normalized.Length > 9)
            return false;

        // Check if it matches static pattern
        if (!Regex.IsMatch($"0{normalized}", RegexPatterns.AfghanistanStaticPhonePattern))
            return false;

        // Validate area code (first 2-3 digits)
        string prefix = $"0{normalized.Substring(0, 2)}";
        return Array.Exists(AfghanistanAreaCodes, code => code.StartsWith(prefix));
    }

    #endregion

    #region Combined Validation Methods

    /// <summary>
    /// Validates any Iranian phone number (mobile or static)
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate</param>
    /// <returns>True if valid Iranian phone number</returns>
    public static bool IsIranPhone(string phoneNumber)
    {
        return IsIranMobile(phoneNumber) || IsIranStatic(phoneNumber);
    }

    /// <summary>
    /// Validates any Afghanistan phone number (mobile or static)
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate</param>
    /// <returns>True if valid Afghanistan phone number</returns>
    public static bool IsAfghanistanPhone(string phoneNumber)
    {
        return IsAfghanistanMobile(phoneNumber) || IsAfghanistanStatic(phoneNumber);
    }

    /// <summary>
    /// Validates phone number for either Iran or Afghanistan
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate</param>
    /// <param name="detectedCountry">Outputs the detected country (Iran/Afghanistan/Unknown)</param>
    /// <param name="phoneType">Outputs the detected phone type (Mobile/Static/Unknown)</param>
    /// <returns>True if valid phone number for either country</returns>
    public static bool ValidatePhoneNumber(string phoneNumber, out string detectedCountry, out string phoneType)
    {
        detectedCountry = "Unknown";
        phoneType = "Unknown";

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Check Iran first
        if (IsIranMobile(phoneNumber))
        {
            detectedCountry = "Iran";
            phoneType = "Mobile";
            return true;
        }

        if (IsIranStatic(phoneNumber))
        {
            detectedCountry = "Iran";
            phoneType = "Static";
            return true;
        }

        // Then check Afghanistan
        if (IsAfghanistanMobile(phoneNumber))
        {
            detectedCountry = "Afghanistan";
            phoneType = "Mobile";
            return true;
        }

        if (IsAfghanistanStatic(phoneNumber))
        {
            detectedCountry = "Afghanistan";
            phoneType = "Static";
            return true;
        }

        return false;
    }

    #endregion

    #region Formatting Methods
    
    /// <summary>
    /// Normalizes a mobile number by stripping separators and converting Persian digits to
    /// English, then formats it in "+98XXXXXXXXXX" form using the last 10 digits.
    /// </summary>
    /// <param name="mobile">The raw mobile number, which may contain separators or Persian digits.</param>
    /// <returns>
    /// The formatted mobile number, or <see cref="string.Empty"/> if <paramref name="mobile"/>
    /// is null/empty or has fewer than 10 digits after normalization.
    /// </returns>
    public static string FormatIranMobile(string mobile)
    {
        if (!IsIranMobile(mobile))
            return mobile;

        string normalized = NormalizePhoneNumber(mobile, "98");
        
        // $"0{normalized.Substring(0, 3)}-{normalized.Substring(3, 3)}-{normalized.Substring(6)}";
        return $"+98{normalized}";
    }

    /// <summary>
    /// Formats Afghanistan mobile number to standard format: 070xxxxxxx
    /// </summary>
    /// <param name="mobile">Phone number to format</param>
    /// <returns>Formatted phone number</returns>
    public static string FormatAfghanistanMobile(string mobile)
    {
        if (!IsAfghanistanMobile(mobile))
            return mobile;
        
        string normalized = NormalizePhoneNumber(mobile, "93");
        return $"+93{normalized}";
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Normalizes phone number by removing country code and non-digit characters
    /// </summary>
    /// <param name="phoneNumber">Phone number to normalize</param>
    /// <param name="countryCode">Country code to remove (e.g., "98" for Iran)</param>
    /// <returns>Normalized phone number without country code</returns>
    private static string NormalizePhoneNumber(string phoneNumber, string countryCode)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return string.Empty;

        var digits = new StringBuilder(phoneNumber.Length);
        foreach (var character in phoneNumber.ToEnglishNumber())
        {
            if (character is >= '0' and <= '9')
                digits.Append(character);
        }

        // Remove country code if present
        var result = digits.ToString();
        if (result.StartsWith("00" + countryCode, StringComparison.Ordinal))
            result = result[(2 + countryCode.Length)..];
        else if (result.StartsWith(countryCode, StringComparison.Ordinal) && result.Length >= countryCode.Length + 9)
            result = result[countryCode.Length..];
        else if (result.StartsWith('0'))
            result = result[1..];

        return result;
    }

    /// <summary>
    /// Extracts the operator/provider name for Iranian mobile numbers
    /// </summary>
    /// <param name="phoneNumber">Iranian mobile number</param>
    /// <returns>Operator name or "Unknown"</returns>
    public static string GetIranMobileOperator(string phoneNumber)
    {
        if (!IsIranMobile(phoneNumber))
            return "Invalid Number";

        string normalized = NormalizePhoneNumber(phoneNumber, "98");
        string prefix = $"0{normalized.Substring(0, 3)}";

        if (prefix.StartsWith("0910") || prefix.StartsWith("0911") ||
            prefix.StartsWith("0912") || prefix.StartsWith("0913") ||
            prefix.StartsWith("0914") || prefix.StartsWith("0915") ||
            prefix.StartsWith("0916") || prefix.StartsWith("0917") ||
            prefix.StartsWith("0918") || prefix.StartsWith("0919"))
            return "Hamrah-e Aval (MCI)";

        if (prefix.StartsWith("0901") || prefix.StartsWith("0902") ||
            prefix.StartsWith("0903") || prefix.StartsWith("0904") ||
            prefix.StartsWith("0905") || prefix.StartsWith("0930") ||
            prefix.StartsWith("0933") || prefix.StartsWith("0935") ||
            prefix.StartsWith("0936") || prefix.StartsWith("0937") ||
            prefix.StartsWith("0938") || prefix.StartsWith("0939"))
            return "Irancell (MTN)";

        if (prefix.StartsWith("0920") || prefix.StartsWith("0921") ||
            prefix.StartsWith("0922") || prefix.StartsWith("0923"))
            return "RighTel";

        return "Other";
    }

    #endregion
}
