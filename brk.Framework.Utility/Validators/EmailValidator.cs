using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;

namespace brk.Framework.Utility.Validators;

/// <summary>
/// Comprehensive email validation helper class with multiple validation strategies
/// </summary>
public class EmailValidator
{
    #region Configuration

    /// <summary>
    /// Email validation options and settings
    /// </summary>
    public class ValidationOptions
    {
        public int MinLocalPartLength { get; set; } = 1;
        public int MaxLocalPartLength { get; set; } = 64;
        public int MinDomainLength { get; set; } = 4; // e.g., a.co
        public int MaxDomainLength { get; set; } = 255;
        public int MaxTotalLength { get; set; } = 254;
        public int MinTopLevelDomainLength { get; set; } = 2;
        public int MaxTopLevelDomainLength { get; set; } = 63;
        public int MaxSubdomains { get; set; } = 10;

        public bool AllowIpDomain { get; set; } = false;
        public bool AllowQuotedStrings { get; set; } = true;
        public bool AllowComments { get; set; } = false;
        public bool AllowInternationalCharacters { get; set; } = false;
        public bool AllowPlusAddressing { get; set; } = true;
        public bool AllowSpecialCharacters { get; set; } = true;
        public bool AllowDisposableEmails { get; set; } = false;
        public bool RequireTopLevelDomain { get; set; } = true;
        public bool StrictRfcCompliance { get; set; } = false;
        public bool UseBuiltInValidation { get; set; } = true;

        public bool CheckDnsMxRecords { get; set; } = false;
        public bool CheckSmtpConnection { get; set; } = false;
        public int SmtpTimeoutSeconds { get; set; } = 5;
        public int DnsTimeoutSeconds { get; set; } = 3;

        public HashSet<string> AllowedDomains { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> BlockedDomains { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public HashSet<string> AllowedTopLevelDomains { get; set; } =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public HashSet<string> BlockedTopLevelDomains { get; set; } =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public List<string> BlockedPatterns { get; set; } = new List<string>();
        public List<Func<string, bool>> CustomRules { get; set; } = new List<Func<string, bool>>();
    }

    /// <summary>
    /// Validation result with detailed information
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Email { get; set; }
        public string LocalPart { get; set; }
        public string Domain { get; set; }
        public string NormalizedEmail { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public EmailValidationLevel ValidationLevel { get; set; }
        public Dictionary<string, bool> Checks { get; set; } = new Dictionary<string, bool>();
    }
    #endregion

    #region Regex Patterns

    private static class Patterns
    {
        // Basic email pattern
        public const string BasicEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        // Standard email pattern
        public const string StandardEmail =
            @"^[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?$";

        // Strict RFC 5322 email pattern
        public const string StrictEmail =
            @"^(?:[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-zA-Z0-9-]*[a-zA-Z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$";

        // International email (IDN) pattern
        public const string InternationalEmail =
            @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

        // IP domain pattern
        public const string IpDomain = @"^\[(?:[0-9]{1,3}\.){3}[0-9]{1,3}\]$";

        // Quoted string pattern
        public const string QuotedString = @"^"".*""$";

        // Comment pattern
        public const string Comment = @"\([^()]*\)";

        // Domain pattern
        public const string Domain =
            @"^[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
    }

    #endregion

    #region Disposable Email Domains

    private static readonly HashSet<string> DisposableEmailDomains =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "mailinator.com", "guerrillamail.com", "guerrillamail.net", "guerrillamail.org",
            "guerrillamail.biz", "guerrillamailblock.com", "pokemail.net", "spam4.me",
            "tempmail.com", "temp-mail.org", "tempmail.net", "10minutemail.com",
            "10minutemail.org", "yopmail.com", "yopmail.net", "yopmail.org",
            "throwaway.email", "throwawaymail.com", "trashmail.com", "trash-mail.com",
            "dispostable.com", "sharklasers.com", "spambox.us", "spambox.xyz",
            "getairmail.com", "mailnesia.com", "mintemail.com", "mytrashmail.com",
            "nwytg.com", "nwytg.net", "objectmail.com", "obobbo.com",
            "oneoffemail.com", "onewaymail.com", "trashmail.ws", "trashmail.me",
            "fakeinbox.com", "fakemailgenerator.com", "fakemail.net", "maildrop.cc",
            "moakt.com", "mohmal.com", "mohmal.net", "mohmal.org",
            "mohmal.xyz", "mohmal.email", "tmpmail.org", "tmpmail.net"
        };

    #endregion

    #region Common TLDs

    private static readonly HashSet<string> ValidTLDs = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "com", "org", "net", "edu", "gov", "mil", "int",
        "info", "biz", "name", "pro", "aero", "coop", "museum",
        "mobi", "travel", "jobs", "cat", "tel", "asia",
        "ir", "af", "uk", "us", "ca", "au", "de", "fr", "jp",
        "cn", "ru", "br", "in", "it", "es", "nl", "se", "no",
        "dk", "fi", "pl", "ch", "at", "be", "nz", "mx", "ar",
        "online", "store", "blog", "app", "dev", "io", "co"
    };

    #endregion

    #region Default Options

    private static readonly ValidationOptions DefaultOptions = new ValidationOptions();

    #endregion

    #region Main Validation Methods

    /// <summary>
    /// Validates an email address with default options
    /// </summary>
    public static ValidationResult Validate(string email)
    {
        return Validate(email, DefaultOptions, EmailValidationLevel.Standard);
    }

    /// <summary>
    /// Validates an email address with custom options
    /// </summary>
    public static ValidationResult Validate(string email, ValidationOptions options)
    {
        return Validate(email, options, EmailValidationLevel.Standard);
    }

    /// <summary>
    /// Validates an email address with specified validation level
    /// </summary>
    public static ValidationResult Validate(string email, EmailValidationLevel level)
    {
        return Validate(email, DefaultOptions, level);
    }

    /// <summary>
    /// Comprehensive email validation with all options
    /// </summary>
    public static ValidationResult Validate(string email, ValidationOptions options, EmailValidationLevel level)
    {
        var result = new ValidationResult
        {
            Email = email,
            ValidationLevel = level,
            IsValid = true
        };

        try
        {
            // 1. Basic null/empty check
            if (string.IsNullOrWhiteSpace(email))
            {
                result.Errors.Add("Email address is null or empty.");
                result.IsValid = false;
                return result;
            }

            // Normalize email
            email = NormalizeEmail(email);
            result.NormalizedEmail = email;

            // 2. Length checks
            if (!ValidateLength(email, options, result))
            {
                result.IsValid = false;
                return result;
            }

            // 3. Basic structure validation
            if (!ValidateBasicStructure(email, options, result))
            {
                result.IsValid = false;
                return result;
            }

            // 4. Parse email parts
            var parts = ParseEmailParts(email);
            result.LocalPart = parts.Item1;
            result.Domain = parts.Item2;

            // 5. Local part validation
            if (!ValidateLocalPart(result.LocalPart, options, result))
            {
                result.IsValid = false;
                return result;
            }

            // 6. Domain validation
            if (!ValidateDomain(result.Domain, options, result))
            {
                result.IsValid = false;
                return result;
            }

            // 7. Top-level domain validation
            if (options.RequireTopLevelDomain)
            {
                if (!ValidateTopLevelDomain(result.Domain, options, result))
                {
                    result.IsValid = false;
                    return result;
                }
            }

            // 8. System.Net.Mail validation
            if (options.UseBuiltInValidation)
            {
                if (!ValidateWithBuiltIn(email, result))
                {
                    result.IsValid = false;
                    return result;
                }
            }

            // 9. Strict RFC compliance
            if (options.StrictRfcCompliance)
            {
                if (!ValidateRfcCompliance(email, result))
                {
                    result.IsValid = false;
                    return result;
                }
            }

            // 10. Allowed/Blocked domains check
            if (!ValidateDomainLists(result.Domain, options, result))
            {
                result.IsValid = false;
                return result;
            }

            // 11. Blocked patterns check
            if (!ValidateBlockedPatterns(email, options, result))
            {
                result.IsValid = false;
                return result;
            }

            // 12. Disposable email check
            if (!options.AllowDisposableEmails)
            {
                if (!ValidateDisposableEmail(result.Domain, result))
                {
                    result.IsValid = false;
                    return result;
                }
            }

            // 13. DNS MX record check (Advanced level)
            if (level >= EmailValidationLevel.Advanced || options.CheckDnsMxRecords)
            {
                if (!ValidateDnsMxRecords(result.Domain, options, result))
                {
                    result.IsValid = false;
                }
            }

            // 14. SMTP check (Strict level)
            if (level >= EmailValidationLevel.Strict || options.CheckSmtpConnection)
            {
                if (!ValidateSmtpConnection(email, options, result))
                {
                    result.IsValid = false;
                }
            }

            // 15. Custom rules
            if (options.CustomRules.Any())
            {
                foreach (var rule in options.CustomRules)
                {
                    if (!rule(email))
                    {
                        result.Errors.Add("Failed custom validation rule.");
                        result.IsValid = false;
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Unexpected validation error: {ex.Message}");
            result.IsValid = false;
        }

        return result;
    }

    /// <summary>
    /// Simple quick validation without detailed results
    /// </summary>
    public static bool IsValid(string email)
    {
        var result = Validate(email, DefaultOptions, EmailValidationLevel.Basic);
        return result.IsValid;
    }

    /// <summary>
    /// Async validation with DNS and SMTP checks
    /// </summary>
    public static async Task<ValidationResult> ValidateAsync(string email, ValidationOptions options = null)
    {
        options = options ?? DefaultOptions;
        var result = new ValidationResult
        {
            Email = email,
            ValidationLevel = EmailValidationLevel.Advanced,
            IsValid = true
        };

        try
        {
            // Perform synchronous validations first
            var syncResult = Validate(email, options, EmailValidationLevel.Standard);
            if (!syncResult.IsValid)
            {
                return syncResult;
            }

            // Update result with sync validation data
            result = syncResult;
            result.ValidationLevel = EmailValidationLevel.Advanced;

            // Parse domain
            var parts = ParseEmailParts(email);
            result.Domain = parts.Item2;

            // Async DNS check
            if (options.CheckDnsMxRecords)
            {
                var dnsResult = await ValidateDnsMxRecordsAsync(result.Domain, options);
                result.Checks["DNS_MX"] = dnsResult;
                if (!dnsResult)
                {
                    result.Errors.Add("Domain does not have valid MX records.");
                    result.IsValid = false;
                }
            }

            // Async SMTP check
            if (options.CheckSmtpConnection)
            {
                var smtpResult = await ValidateSmtpConnectionAsync(email, options);
                result.Checks["SMTP"] = smtpResult;
                if (!smtpResult)
                {
                    result.Errors.Add("SMTP verification failed.");
                    result.IsValid = false;
                }
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Async validation error: {ex.Message}");
            result.IsValid = false;
        }

        return result;
    }

    #endregion

    #region Private Validation Methods

    private static string NormalizeEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return email;

        // Trim and convert to lowercase
        email = email.Trim().ToLowerInvariant();

        // Remove comments if not allowed
        if (Regex.IsMatch(email, Patterns.Comment))
        {
            email = Regex.Replace(email, Patterns.Comment, "");
        }

        // Normalize whitespace
        email = Regex.Replace(email, @"\s+", "");

        return email;
    }

    private static bool ValidateLength(string email, ValidationOptions options, ValidationResult result)
    {
        if (email.Length > options.MaxTotalLength)
        {
            result.Errors.Add($"Email exceeds maximum length of {options.MaxTotalLength} characters.");
            result.Checks["Length"] = false;
            return false;
        }

        result.Checks["Length"] = true;
        return true;
    }

    private static bool ValidateBasicStructure(string email, ValidationOptions options, ValidationResult result)
    {
        // Check for @ symbol
        int atIndex = email.IndexOf('@');
        if (atIndex < 0)
        {
            result.Errors.Add("Email must contain '@' symbol.");
            result.Checks["BasicStructure"] = false;
            return false;
        }

        // Check for multiple @ symbols
        if (email.LastIndexOf('@') != atIndex)
        {
            result.Errors.Add("Email contains multiple '@' symbols.");
            result.Checks["BasicStructure"] = false;
            return false;
        }

        // Check for dots immediately before or after @
        if (atIndex > 0 && email[atIndex - 1] == '.')
        {
            result.Errors.Add("Local part cannot end with a dot.");
            result.Checks["BasicStructure"] = false;
            return false;
        }

        if (atIndex < email.Length - 1 && email[atIndex + 1] == '.')
        {
            result.Errors.Add("Domain part cannot start with a dot.");
            result.Checks["BasicStructure"] = false;
            return false;
        }

        // Basic pattern match
        if (!Regex.IsMatch(email, Patterns.BasicEmail))
        {
            result.Errors.Add("Email does not match basic email pattern.");
            result.Checks["BasicStructure"] = false;
            return false;
        }

        result.Checks["BasicStructure"] = true;
        return true;
    }

    private static Tuple<string, string> ParseEmailParts(string email)
    {
        int atIndex = email.IndexOf('@');
        string localPart = email.Substring(0, atIndex);
        string domain = email.Substring(atIndex + 1);

        return new Tuple<string, string>(localPart, domain);
    }

    private static bool ValidateLocalPart(string localPart, ValidationOptions options, ValidationResult result)
    {
        // Remove quotes for validation if allowed
        string cleanLocalPart = localPart;
        if (options.AllowQuotedStrings && Regex.IsMatch(localPart, Patterns.QuotedString))
        {
            cleanLocalPart = localPart.Trim('"');
        }

        // Length checks
        if (cleanLocalPart.Length < options.MinLocalPartLength)
        {
            result.Errors.Add($"Local part is too short. Minimum {options.MinLocalPartLength} characters.");
            result.Checks["LocalPart"] = false;
            return false;
        }

        if (cleanLocalPart.Length > options.MaxLocalPartLength)
        {
            result.Errors.Add($"Local part exceeds maximum length of {options.MaxLocalPartLength} characters.");
            result.Checks["LocalPart"] = false;
            return false;
        }

        // Consecutive dots check
        if (cleanLocalPart.Contains(".."))
        {
            result.Errors.Add("Local part contains consecutive dots.");
            result.Checks["LocalPart"] = false;
            return false;
        }

        // Plus addressing check
        if (!options.AllowPlusAddressing && cleanLocalPart.Contains("+"))
        {
            result.Errors.Add("Plus addressing is not allowed.");
            result.Checks["LocalPart"] = false;
            return false;
        }

        // Special characters check
        if (!options.AllowSpecialCharacters)
        {
            if (!Regex.IsMatch(cleanLocalPart, @"^[a-zA-Z0-9.]+$"))
            {
                result.Errors.Add("Special characters are not allowed in local part.");
                result.Checks["LocalPart"] = false;
                return false;
            }
        }

        // International characters check
        if (!options.AllowInternationalCharacters)
        {
            if (cleanLocalPart.Any(c => c > 127))
            {
                result.Errors.Add("International characters are not allowed.");
                result.Checks["LocalPart"] = false;
                return false;
            }
        }

        result.Checks["LocalPart"] = true;
        return true;
    }

    private static bool ValidateDomain(string domain, ValidationOptions options, ValidationResult result)
    {
        // Length checks
        if (domain.Length < options.MinDomainLength)
        {
            result.Errors.Add($"Domain is too short. Minimum {options.MinDomainLength} characters.");
            result.Checks["Domain"] = false;
            return false;
        }

        if (domain.Length > options.MaxDomainLength)
        {
            result.Errors.Add($"Domain exceeds maximum length of {options.MaxDomainLength} characters.");
            result.Checks["Domain"] = false;
            return false;
        }

        // IP domain check
        if (Regex.IsMatch(domain, Patterns.IpDomain))
        {
            if (!options.AllowIpDomain)
            {
                result.Errors.Add("IP addresses as domain are not allowed.");
                result.Checks["Domain"] = false;
                return false;
            }

            // Validate IP
            string ipString = domain.Trim('[', ']');
            if (!IPAddress.TryParse(ipString, out _))
            {
                result.Errors.Add("Invalid IP address format.");
                result.Checks["Domain"] = false;
                return false;
            }

            result.Checks["Domain"] = true;
            return true;
        }

        // Domain pattern check
        if (!Regex.IsMatch(domain, Patterns.Domain))
        {
            result.Errors.Add("Domain contains invalid characters.");
            result.Checks["Domain"] = false;
            return false;
        }

        // Check for consecutive dots
        if (domain.Contains(".."))
        {
            result.Errors.Add("Domain contains consecutive dots.");
            result.Checks["Domain"] = false;
            return false;
        }

        // Check subdomain count
        int subdomainCount = domain.Count(c => c == '.');
        if (subdomainCount > options.MaxSubdomains)
        {
            result.Warnings.Add($"Domain has {subdomainCount} subdomains (max {options.MaxSubdomains}).");
        }

        // Check for hyphens at start or end of labels
        string[] labels = domain.Split('.');
        foreach (var label in labels)
        {
            if (label.StartsWith("-") || label.EndsWith("-"))
            {
                result.Errors.Add($"Domain label '{label}' cannot start or end with a hyphen.");
                result.Checks["Domain"] = false;
                return false;
            }
        }

        result.Checks["Domain"] = true;
        return true;
    }

    private static bool ValidateTopLevelDomain(string domain, ValidationOptions options, ValidationResult result)
    {
        string[] parts = domain.Split('.');
        string tld = parts.Last();

        if (tld.Length < options.MinTopLevelDomainLength)
        {
            result.Errors.Add($"Top-level domain is too short. Minimum {options.MinTopLevelDomainLength} characters.");
            result.Checks["TLD"] = false;
            return false;
        }

        if (tld.Length > options.MaxTopLevelDomainLength)
        {
            result.Errors.Add(
                $"Top-level domain exceeds maximum length of {options.MaxTopLevelDomainLength} characters.");
            result.Checks["TLD"] = false;
            return false;
        }

        // Check if TLD is numeric only (usually invalid)
        if (tld.All(char.IsDigit))
        {
            result.Errors.Add("Top-level domain cannot be all numeric.");
            result.Checks["TLD"] = false;
            return false;
        }

        // Allowed TLDs check
        if (options.AllowedTopLevelDomains.Any())
        {
            if (!options.AllowedTopLevelDomains.Contains(tld))
            {
                result.Errors.Add($"Top-level domain '{tld}' is not in the allowed list.");
                result.Checks["TLD"] = false;
                return false;
            }
        }

        // Blocked TLDs check
        if (options.BlockedTopLevelDomains.Any())
        {
            if (options.BlockedTopLevelDomains.Contains(tld))
            {
                result.Errors.Add($"Top-level domain '{tld}' is blocked.");
                result.Checks["TLD"] = false;
                return false;
            }
        }

        // Common TLD validation
        if (!ValidTLDs.Contains(tld))
        {
            result.Warnings.Add($"Top-level domain '{tld}' is not in the common TLD list.");
        }

        result.Checks["TLD"] = true;
        return true;
    }

    private static bool ValidateWithBuiltIn(string email, ValidationResult result)
    {
        try
        {
            var mailAddress = new System.Net.Mail.MailAddress(email);
            result.Checks["BuiltIn"] = true;
            return true;
        }
        catch (FormatException)
        {
            result.Errors.Add("Invalid email format according to System.Net.Mail.");
            result.Checks["BuiltIn"] = false;
            return false;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Built-in validation error: {ex.Message}");
            result.Checks["BuiltIn"] = false;
            return false;
        }
    }

    private static bool ValidateRfcCompliance(string email, ValidationResult result)
    {
        if (!Regex.IsMatch(email, Patterns.StrictEmail))
        {
            result.Errors.Add("Email does not comply with RFC 5322 standards.");
            result.Checks["RFC5322"] = false;
            return false;
        }

        result.Checks["RFC5322"] = true;
        return true;
    }

    private static bool ValidateDomainLists(string domain, ValidationOptions options, ValidationResult result)
    {
        // Allowed domains check
        if (options.AllowedDomains.Any())
        {
            if (!options.AllowedDomains.Contains(domain))
            {
                result.Errors.Add($"Domain '{domain}' is not in the allowed list.");
                result.Checks["DomainLists"] = false;
                return false;
            }
        }

        // Blocked domains check
        if (options.BlockedDomains.Any())
        {
            if (options.BlockedDomains.Contains(domain))
            {
                result.Errors.Add($"Domain '{domain}' is blocked.");
                result.Checks["DomainLists"] = false;
                return false;
            }
        }

        result.Checks["DomainLists"] = true;
        return true;
    }

    private static bool ValidateBlockedPatterns(string email, ValidationOptions options, ValidationResult result)
    {
        foreach (var pattern in options.BlockedPatterns)
        {
            if (Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase))
            {
                result.Errors.Add($"Email matches blocked pattern: {pattern}");
                result.Checks["BlockedPatterns"] = false;
                return false;
            }
        }

        result.Checks["BlockedPatterns"] = true;
        return true;
    }

    private static bool ValidateDisposableEmail(string domain, ValidationResult result)
    {
        if (DisposableEmailDomains.Contains(domain))
        {
            result.Errors.Add("Disposable/temporary email addresses are not allowed.");
            result.Checks["DisposableEmail"] = false;
            return false;
        }

        result.Checks["DisposableEmail"] = true;
        return true;
    }

    private static bool ValidateDnsMxRecords(string domain, ValidationOptions options, ValidationResult result)
    {
        try
        {
            // Simple DNS check (synchronous)
            var hostEntry = Dns.GetHostEntry(domain);
            result.Checks["DNS_MX"] = true;
            return true;
        }
        catch (SocketException)
        {
            result.Errors.Add($"DNS lookup failed for domain '{domain}'.");
            result.Checks["DNS_MX"] = false;
            return false;
        }
        catch (Exception ex)
        {
            result.Warnings.Add($"DNS check warning: {ex.Message}");
            result.Checks["DNS_MX"] = true; // Don't fail on DNS errors
            return true;
        }
    }

    private static async Task<bool> ValidateDnsMxRecordsAsync(string domain, ValidationOptions options)
    {
        try
        {
            var task = Dns.GetHostEntryAsync(domain);
            if (await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(options.DnsTimeoutSeconds))) == task)
            {
                await task;
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    private static bool ValidateSmtpConnection(string email, ValidationOptions options, ValidationResult result)
    {
        try
        {
            var parts = ParseEmailParts(email);
            var domain = parts.Item2;

            // This is a simplified check - full SMTP verification would require more complex implementation
            using (var client = new TcpClient())
            {
                var task = client.ConnectAsync(domain, 25);
                if (task.Wait(TimeSpan.FromSeconds(options.SmtpTimeoutSeconds)))
                {
                    result.Checks["SMTP"] = true;
                    return true;
                }
            }

            result.Checks["SMTP"] = false;
            return false;
        }
        catch
        {
            result.Checks["SMTP"] = false;
            return false;
        }
    }

    private static async Task<bool> ValidateSmtpConnectionAsync(string email, ValidationOptions options)
    {
        try
        {
            var parts = ParseEmailParts(email);
            var domain = parts.Item2;

            using (var client = new TcpClient())
            {
                var connectTask = client.ConnectAsync(domain, 25);
                if (await Task.WhenAny(connectTask, Task.Delay(TimeSpan.FromSeconds(options.SmtpTimeoutSeconds))) ==
                    connectTask)
                {
                    return true;
                }

                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Extracts domain from email address
    /// </summary>
    public static string ExtractDomain(string email)
    {
        try
        {
            var parts = ParseEmailParts(email);
            return parts.Item2;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Extracts local part from email address
    /// </summary>
    public static string ExtractLocalPart(string email)
    {
        try
        {
            var parts = ParseEmailParts(email);
            return parts.Item1;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Checks if email is from a disposable domain
    /// </summary>
    public static bool IsDisposableEmail(string email)
    {
        try
        {
            var domain = ExtractDomain(email);
            return DisposableEmailDomains.Contains(domain);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Suggests corrections for common email typos
    /// </summary>
    public static List<string> SuggestCorrections(string email)
    {
        var suggestions = new List<string>();

        if (string.IsNullOrWhiteSpace(email))
            return suggestions;

        var normalized = email.ToLowerInvariant().Trim();

        // Common domain corrections
        var commonDomains = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "gmial.com", "gmail.com" },
            { "gmail.comm", "gmail.com" },
            { "gmail.con", "gmail.com" },
            { "gmail.co", "gmail.com" },
            { "gmai.com", "gmail.com" },
            { "yaho.com", "yahoo.com" },
            { "yahooo.com", "yahoo.com" },
            { "yahhoo.com", "yahoo.com" },
            { "hotmai.com", "hotmail.com" },
            { "hotmal.com", "hotmail.com" },
            { "hotmial.com", "hotmail.com" },
            { "outlok.com", "outlook.com" },
            { "outlook.con", "outlook.com" },
        };

        foreach (var correction in commonDomains)
        {
            if (normalized.EndsWith("@" + correction.Key))
            {
                suggestions.Add(normalized.Replace("@" + correction.Key, "@" + correction.Value));
            }
        }

        // Missing @ symbol
        if (!normalized.Contains("@") && normalized.Contains("."))
        {
            var parts = normalized.Split('.');
            if (parts.Length >= 2)
            {
                // Try inserting @ before common domains
                foreach (var domain in new[] { "gmail.com", "yahoo.com", "hotmail.com", "outlook.com" })
                {
                    if (normalized.EndsWith("." + domain))
                    {
                        suggestions.Add(normalized.Replace("." + domain, "@" + domain));
                    }
                }
            }
        }

        return suggestions;
    }

    /// <summary>
    /// Normalizes email address (lowercase, trim, etc.)
    /// </summary>
    public static string NormalizeEmailAddress(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return email;

        email = email.Trim().ToLowerInvariant();
        email = Regex.Replace(email, @"\s+", "");

        // Remove dots from Gmail local part (Gmail ignores dots)
        if (email.EndsWith("@gmail.com") || email.EndsWith("@googlemail.com"))
        {
            var parts = ParseEmailParts(email);
            var localPart = parts.Item1.Replace(".", "");
            return $"{localPart}@gmail.com";
        }

        return email;
    }

    /// <summary>
    /// Creates validation options with common presets
    /// </summary>
    public static class Presets
    {
        public static ValidationOptions Lenient
        {
            get
            {
                return new ValidationOptions
                {
                    AllowIpDomain = true,
                    AllowInternationalCharacters = true,
                    AllowDisposableEmails = true,
                    StrictRfcCompliance = false,
                    RequireTopLevelDomain = false,
                    UseBuiltInValidation = false
                };
            }
        }

        public static ValidationOptions Standard
        {
            get
            {
                return new ValidationOptions
                {
                    AllowDisposableEmails = false,
                    AllowIpDomain = false,
                    StrictRfcCompliance = false,
                    RequireTopLevelDomain = true,
                    UseBuiltInValidation = true
                };
            }
        }

        public static ValidationOptions Strict
        {
            get
            {
                return new ValidationOptions
                {
                    AllowDisposableEmails = false,
                    AllowIpDomain = false,
                    AllowInternationalCharacters = false,
                    AllowPlusAddressing = false,
                    AllowSpecialCharacters = false,
                    StrictRfcCompliance = true,
                    RequireTopLevelDomain = true,
                    UseBuiltInValidation = true,
                    CheckDnsMxRecords = true
                };
            }
        }

        public static ValidationOptions Enterprise
        {
            get
            {
                return new ValidationOptions
                {
                    AllowDisposableEmails = false,
                    AllowIpDomain = false,
                    AllowInternationalCharacters = true,
                    StrictRfcCompliance = true,
                    RequireTopLevelDomain = true,
                    UseBuiltInValidation = true,
                    CheckDnsMxRecords = true,
                    CheckSmtpConnection = false,
                    SmtpTimeoutSeconds = 10
                };
            }
        }

        public static ValidationOptions IranSpecific
        {
            get
            {
                var options = new ValidationOptions
                {
                    AllowDisposableEmails = false,
                    AllowIpDomain = false,
                    AllowInternationalCharacters = false,
                    StrictRfcCompliance = false,
                    RequireTopLevelDomain = true,
                    UseBuiltInValidation = true,
                    CheckDnsMxRecords = false,
                    AllowedTopLevelDomains = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        "ir", "com", "org", "net", "ac.ir", "co.ir", "gov.ir", "edu.ir"
                    },
                    BlockedDomains = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        "spam.com", "tempmail.com"
                    }
                };
                return options;
            }
        }

        public static ValidationOptions AfghanistanSpecific
        {
            get
            {
                var options = new ValidationOptions
                {
                    AllowDisposableEmails = false,
                    AllowIpDomain = false,
                    AllowInternationalCharacters = false,
                    StrictRfcCompliance = false,
                    RequireTopLevelDomain = true,
                    UseBuiltInValidation = true,
                    CheckDnsMxRecords = false,
                    AllowedTopLevelDomains = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        "af", "com", "org", "net", "gov.af", "edu.af"
                    }
                };
                return options;
            }
        }
    }

    #endregion
}

public enum EmailValidationLevel
{
    Basic = 1, // Syntax only
    Standard = 2, // Syntax + domain structure
    Advanced = 3, // Syntax + domain + DNS
    Strict = 4 // Full RFC compliance + all checks
}
