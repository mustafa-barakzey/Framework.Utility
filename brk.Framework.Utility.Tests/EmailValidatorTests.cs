using brk.Framework.Utility.Validators;
using Xunit;

namespace brk.Framework.Utility.Tests;

public sealed class EmailValidatorTests
{
    [Fact]
    public void ValidateOverloadsAndIsValid_HandleSyntax() { Assert.True(EmailValidator.Validate("user@example.com").IsValid); Assert.True(EmailValidator.Validate("user@example.com", new EmailValidator.ValidationOptions()).IsValid); Assert.True(EmailValidator.Validate("user@example.com", EmailValidationLevel.Basic).IsValid); Assert.False(EmailValidator.Validate("invalid", new EmailValidator.ValidationOptions(), EmailValidationLevel.Standard).IsValid); Assert.True(EmailValidator.IsValid("user@example.com")); }

    [Fact]
    public async Task ValidateAsync_UsesNonNetworkOptions() { var result = await EmailValidator.ValidateAsync("user@example.com", new EmailValidator.ValidationOptions { CheckDnsMxRecords = false, CheckSmtpConnection = false }); Assert.True(result.IsValid); }

    [Fact]
    public void EmailUtilitiesAndPresets_Work() { Assert.Equal("example.com", EmailValidator.ExtractDomain("user@example.com")); Assert.Equal("user", EmailValidator.ExtractLocalPart("user@example.com")); Assert.True(EmailValidator.IsDisposableEmail("user@mailinator.com")); Assert.Contains("user@gmail.com", EmailValidator.SuggestCorrections("user@gmial.com")); Assert.Equal("johndoe@gmail.com", EmailValidator.NormalizeEmailAddress(" John.Doe@Gmail.com ")); Assert.True(EmailValidator.Presets.Lenient.AllowDisposableEmails); Assert.True(EmailValidator.Presets.Standard.RequireTopLevelDomain); Assert.True(EmailValidator.Presets.Strict.StrictRfcCompliance); Assert.True(EmailValidator.Presets.Enterprise.CheckDnsMxRecords); Assert.Contains("ir", EmailValidator.Presets.IranSpecific.AllowedTopLevelDomains); Assert.Contains("af", EmailValidator.Presets.AfghanistanSpecific.AllowedTopLevelDomains); }
}
