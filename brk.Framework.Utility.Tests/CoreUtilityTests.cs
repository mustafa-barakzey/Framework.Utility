using brk.Framework.Utility.DateTimes;
using brk.Framework.Utility.Extensions;
using brk.Framework.Utility.Validators;
using Xunit;

namespace brk.Framework.Utility.Tests;

public sealed class CoreUtilityTests
{
    [Theory]
    [InlineData("09123456789")]
    [InlineData("+989123456789")]
    [InlineData("00989123456789")]
    [InlineData("۰۹۱۲۳۴۵۶۷۸۹")]
    public void IsIranMobile_AcceptsSupportedRepresentations(string phoneNumber)
        => Assert.True(MobileValidator.IsIranMobile(phoneNumber));

    [Theory]
    [InlineData("0912345678")]
    [InlineData("08123456789")]
    public void IsIranMobile_RejectsInvalidNumbers(string phoneNumber)
        => Assert.False(MobileValidator.IsIranMobile(phoneNumber));

    [Fact]
    public void MobileFormatting_UsesInternationalForm()
    {
        Assert.Equal("+989123456789", MobileValidator.FormatIranMobile("۰۹۱۲۳۴۵۶۷۸۹"));
        Assert.Equal("+93701234567", MobileValidator.FormatAfghanistanMobile("0701234567"));
    }

    [Fact]
    public void NationalId_AcceptsValidChecksumAndPersianDigits()
    {
        Assert.True(NationalIdValidator.IsIranNationalId("0123456789"));
        Assert.True(NationalIdValidator.IsIranNationalId("۰۱۲۳۴۵۶۷۸۹"));
        Assert.False(NationalIdValidator.IsIranNationalId("1111111111"));
    }

    [Fact]
    public void PersianDateTime_ParsesPersianDigitsAndFlexibleWhitespace()
    {
        var actual = "۱۴۰۳/۰۱/۰۱   ۱۳:۰۵".ToGregorianDateTime();

        Assert.Equal(new DateTime(2024, 3, 20, 13, 5, 0), actual);
    }

    [Fact]
    public void IsNumeric_OnlyAcceptsAsciiDigits()
    {
        Assert.True("12345".IsNumeric());
        Assert.False("۱۲۳۴۵".IsNumeric());
        Assert.False("12.3".IsNumeric());
        Assert.False(((string?)null).IsNumeric());
    }

    [Fact]
    public void IsLengthBetween_RejectsInvertedBounds()
        => Assert.Throws<ArgumentOutOfRangeException>(() => "test".IsLengthBetween(5, 2));
}
