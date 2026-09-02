using brk.Framework.Utility.Validators;
using Xunit;

namespace brk.Framework.Utility.Tests;

public sealed class MobileValidatorTests
{
    [Fact]
    public void CountrySpecificValidationAndFormatting_Work() { Assert.True(MobileValidator.IsIranMobile("۰۹۱۲۳۴۵۶۷۸۹")); Assert.True(MobileValidator.IsAfghanistanMobile("0701234567")); Assert.True(MobileValidator.IsIranStatic("02112345678")); Assert.True(MobileValidator.IsAfghanistanStatic("0201234567")); Assert.True(MobileValidator.IsIranPhone("09123456789")); Assert.True(MobileValidator.IsAfghanistanPhone("0701234567")); Assert.Equal("+989123456789", MobileValidator.FormatIranMobile("09123456789")); Assert.Equal("+93701234567", MobileValidator.FormatAfghanistanMobile("0701234567")); }

    [Fact]
    public void ValidatePhoneNumberAndOperator_ReturnClassification() { Assert.True(MobileValidator.ValidatePhoneNumber("09123456789", out var country, out var type)); Assert.Equal("Iran", country); Assert.Equal("Mobile", type); Assert.Equal("Hamrah-e Aval (MCI)", MobileValidator.GetIranMobileOperator("09123456789")); Assert.Equal("Invalid Number", MobileValidator.GetIranMobileOperator("invalid")); }
}
