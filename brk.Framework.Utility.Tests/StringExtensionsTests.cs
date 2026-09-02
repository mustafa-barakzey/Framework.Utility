using System.ComponentModel;
using brk.Framework.Utility.DateTimes;
using brk.Framework.Utility.Extensions;
using Xunit;

namespace brk.Framework.Utility.Tests;

public sealed class StringExtensionsTests
{
    [Fact]
    public void SafeParsingAndNullHandling_ReturnExpectedValues() { Assert.Equal(42, "42".ToSafeInt()); Assert.Equal(-1, "x".ToSafeInt(-1)); Assert.Equal(42L, "42".ToSafeLong()); Assert.Null("x".ToSafeNullableInt()); Assert.Null("x".ToSafeNullableLong()); Assert.Equal(string.Empty, ((string?)null).ToStringOrEmpty()); }

    [Fact]
    public void TextAndByteHelpers_TransformValues() { Assert.Equal("user_id", "UserId".ToUnderscoreCase()); Assert.Equal("text", "text".ToByteArray().FromByteArray()); Assert.Equal("ی ک", " ي ك ".ApplyCorrectYeKe()); object objectInput = "ي"; Assert.Equal("ی", objectInput.ApplyCorrectYeKe()); Assert.Equal("۱۲۳", "123".ToFarsiNumber()); Assert.Equal("123", "۱۲۳".ToEnglishNumber()); Assert.Equal("one two", " one\n two ".FixText()); Assert.Equal("a@b.com", " A @B.COM ".FixEmail()); Assert.Equal("a<br>b", "<p>a</p><br><b>b</b>".RemoveHtmlTagsExceptBreak()); Assert.Equal("hello-world", " hello  world ".FixTextForUrl()); Assert.Equal(["a", "b"], " a, b ,,".SplitTagsWithComma()); Assert.Equal(["a", "b"], " a- b --".SplitTagsWithDash()); }

    [Fact]
    public void DisplayName_ReadsPropertyAttribute() => Assert.Equal("Friendly", typeof(DisplayModel).GetProperty(nameof(DisplayModel.Value))!.DisplayName());

    private sealed class DisplayModel { [DisplayName("Friendly")] public string Value { get; set; } = string.Empty; }
}
