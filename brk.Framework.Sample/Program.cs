using brk.Framework.Utility.Helpers;
using brk.Framework.Utility.Validators;

// Test Iranian mobile numbers
Console.WriteLine("=== Iran Mobile Numbers ===");
TestNumber("09123456789", MobileValidator.IsIranMobile);
TestNumber("+989123456789", MobileValidator.IsIranMobile);
TestNumber("00989123456789", MobileValidator.IsIranMobile);
TestNumber("9123456789", MobileValidator.IsIranMobile);
TestNumber("0912345678", MobileValidator.IsIranMobile); // Invalid (9 digits)

// Test Afghanistan mobile numbers
Console.WriteLine("\n=== Afghanistan Mobile Numbers ===");
TestNumber("0701234567", MobileValidator.IsAfghanistanMobile);
TestNumber("+93701234567", MobileValidator.IsAfghanistanMobile);
TestNumber("0093701234567", MobileValidator.IsAfghanistanMobile);
TestNumber("070123456", MobileValidator.IsAfghanistanMobile); // Invalid (7 digits)

// Test Iranian static numbers
Console.WriteLine("\n=== Iran Static Numbers ===");
TestNumber("02112345678", MobileValidator.IsIranStatic);
TestNumber("+982112345678", MobileValidator.IsIranStatic);

// Test Iranian National IDs
Console.WriteLine("\n=== Iran National IDs ===");
TestNumber("0123456789", NationalIdValidator.IsIranNationalId);
TestNumber("9876543210", NationalIdValidator.IsIranNationalId);
TestNumber("1111111111", NationalIdValidator.IsIranNationalId); // Invalid
TestNumber("1234444321", NationalIdValidator.IsIranNationalId); // Valid checksum

// Test Afghanistan National IDs
Console.WriteLine("\n=== Afghanistan National IDs ===");
TestNumber("1400-0901-33104", NationalIdValidator.IsAfghanistanNationalId);
TestNumber("1404-0601-37472", NationalIdValidator.IsAfghanistanNationalId);
TestNumber("1404-0601-37472", NationalIdValidator.IsAfghanistanNationalId);
TestNumber("1404-0601-37350", NationalIdValidator.IsAfghanistanNationalId);
TestNumber("1234567890123", NationalIdValidator.IsAfghanistanNationalId);

// Test combined validation
Console.WriteLine("\n=== Combined Validation ===");
string[] testNumbers =
{
    "09123456789",
    "0701234567",
    "02112345678",
    "1234567890"
};

foreach (var number in testNumbers)
{
    bool isValid = MobileValidator.ValidatePhoneNumber(
        number,
        out string country,
        out string type
    );
    Console.WriteLine($"{number}: Valid={isValid}, Country={country}, Type={type}");
}

// Test formatting
Console.WriteLine("\n=== Formatting ===");
Console.WriteLine(MobileValidator.FormatIranMobile("09123456789"));
Console.WriteLine(MobileValidator.FormatAfghanistanMobile("0701234567"));

// Test operator detection
Console.WriteLine("\n=== Operator Detection ===");
Console.WriteLine($"09123456789: {MobileValidator.GetIranMobileOperator("09123456789")}");
Console.WriteLine($"09361234567: {MobileValidator.GetIranMobileOperator("09361234567")}");

static void TestNumber(string number, Func<string, bool> validator)
{
    Console.WriteLine($"{number,-20} => {validator(number)}");
}


Console.WriteLine("=== Email Validation Examples ===\n");

// Test emails
string[] testEmails = new[]
{
    "user@example.com", // Valid
    "user.name+tag@example.com", // Valid - plus addressing
    "user@domain.co.uk", // Valid - multi-part TLD
    "invalid-email", // Invalid - no @
    "user@.com", // Invalid - domain starts with dot
    "user.@domain.com", // Invalid - local part ends with dot
    "user@domain..com", // Invalid - consecutive dots
    "@domain.com", // Invalid - no local part
    "user@domain", // Invalid - no TLD (in strict mode)
    "user@tempmail.com", // Disposable email
    "very.common@example.com", // Valid
    "disposable.style.email.with+symbol@example.com", // Valid
    "user@[192.168.1.1]", // IP domain
    "user@مثال.ایران", // Internationalized
    "user@gmail.com", // Valid - common domain
    "user@hotmail.con", // Typo - should be .com
};

// Basic validation
Console.WriteLine("1. Basic Validation:");
foreach (var email1 in testEmails)
{
    bool isValid = EmailValidator.IsValid(email1);
    Console.WriteLine($"  {email1,-45} => {(isValid ? "✓ Valid" : "✗ Invalid")}");
}

// Detailed validation
Console.WriteLine("\n2. Detailed Validation:");
var result = EmailValidator.Validate("user.name@example.com");
Console.WriteLine($"  Valid: {result.IsValid}");
Console.WriteLine($"  Local Part: {result.LocalPart}");
Console.WriteLine($"  Domain: {result.Domain}");
Console.WriteLine($"  Normalized: {result.NormalizedEmail}");

// Validation with presets
Console.WriteLine("\n3. Preset Validation (Enterprise):");
var enterpriseResult = EmailValidator.Validate(
    "user@example.com",
    EmailValidator.Presets.Enterprise
);
Console.WriteLine($"  Valid: {enterpriseResult.IsValid}");
Console.WriteLine($"  Errors: {enterpriseResult.Errors.Count}");

// Iran-specific validation
Console.WriteLine("\n4. Iran-Specific Validation:");
var iranResult = EmailValidator.Validate(
    "user@example.ir",
    EmailValidator.Presets.IranSpecific
);
Console.WriteLine($"  Valid: {iranResult.IsValid}");
Console.WriteLine($"  Allowed TLD: .ir is allowed");

// Disposable email detection
Console.WriteLine("\n5. Disposable Email Detection:");
string disposableEmail = "user@mailinator.com";
bool isDisposable = EmailValidator.IsDisposableEmail(disposableEmail);
Console.WriteLine($"  {disposableEmail} is disposable: {isDisposable}");

// Email correction suggestions
Console.WriteLine("\n6. Typo Correction:");
string typoEmail = "user@gmial.com";
var suggestions = EmailValidator.SuggestCorrections(typoEmail);
Console.WriteLine($"  Original: {typoEmail}");
Console.WriteLine($"  Suggestions:");
foreach (var suggestion in suggestions)
{
    Console.WriteLine($"    - {suggestion}");
}

// Extract parts
Console.WriteLine("\n7. Extract Email Parts:");
string email2 = "john.doe@example.com";
Console.WriteLine($"  Email: {email2}");
Console.WriteLine($"  Domain: {EmailValidator.ExtractDomain(email2)}");
Console.WriteLine($"  Local Part: {EmailValidator.ExtractLocalPart(email2)}");

// Normalization
Console.WriteLine("\n8. Email Normalization:");
string gmailEmail = "John.Doe@Gmail.com";
Console.WriteLine($"  Original: {gmailEmail}");
Console.WriteLine($"  Normalized: {EmailValidator.NormalizeEmailAddress(gmailEmail)}");

// Custom validation rules
Console.WriteLine("\n9. Custom Validation Rules:");
var customOptions = new EmailValidator.ValidationOptions();
customOptions.CustomRules.Add(email3 =>
{
    // Custom rule: must contain "allowed" in local part
    var localPart = EmailValidator.ExtractLocalPart(email3);
    return localPart.Contains("allowed");
});

var customResult = EmailValidator.Validate("allowed.user@example.com", customOptions);
Console.WriteLine($"  'allowed.user@example.com' => {customResult.IsValid}");

customResult = EmailValidator.Validate("blocked.user@example.com", customOptions);
Console.WriteLine($"  'blocked.user@example.com' => {customResult.IsValid}");

// Validation with different levels
Console.WriteLine("\n10. Validation Levels:");
string testEmail = "user@example.com";

var basicResult = EmailValidator.Validate(testEmail, EmailValidationLevel.Basic);
Console.WriteLine($"  Basic: {basicResult.IsValid} (Checks: {string.Join(", ", basicResult.Checks.Keys)})");

var standardResult = EmailValidator.Validate(testEmail, EmailValidationLevel.Standard);
Console.WriteLine($"  Standard: {standardResult.IsValid} (Checks: {string.Join(", ", standardResult.Checks.Keys)})");

var advancedResult = EmailValidator.Validate(testEmail, EmailValidationLevel.Advanced);
Console.WriteLine($"  Advanced: {advancedResult.IsValid} (Checks: {string.Join(", ", advancedResult.Checks.Keys)})");


var afghanCulture = CultureHelper.GetAfghanCulture();
var pashtoCulture = CultureHelper.GetPashtoCulture();
var iranCulture = CultureHelper.GetIranCulture();
        
// Test with current date
var now = DateTime.Now;
        
System.Threading.Thread.CurrentThread.CurrentCulture = iranCulture;
Console.WriteLine($"Iran: {now.ToString("D")}"); // Long date in iran

// Afghan Dari format
System.Threading.Thread.CurrentThread.CurrentCulture = afghanCulture;
Console.WriteLine($"Dari: {now.ToString("D")}"); // Long date in Dari
        
// Pashto format
System.Threading.Thread.CurrentThread.CurrentCulture = pashtoCulture;
Console.WriteLine($"Pashto: {now.ToString("D")}"); // Long date in Pashto
