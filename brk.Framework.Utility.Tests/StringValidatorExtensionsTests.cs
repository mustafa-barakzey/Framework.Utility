using brk.Framework.Utility.Extensions;
using Xunit;

namespace brk.Framework.Utility.Tests;

public sealed class StringValidatorExtensionsTests
{
    [Fact]
    public void IsNumeric_ValidatesAsciiDigitsOnly() { Assert.True("123".IsNumeric()); Assert.False("۱۲۳".IsNumeric()); Assert.False(((string?)null).IsNumeric()); }

    [Fact]
    public void LengthMethods_CompareLength() { const string value = "test"; Assert.True(value.IsLengthBetween(4, 4)); Assert.True(value.IsLengthLessThan(5)); Assert.True(value.IsLengthLessThanOrEqual(4)); Assert.True(value.IsLengthGreaterThan(3)); Assert.True(value.IsLengthGreaterThanOrEqual(4)); Assert.True(value.IsLengthEqual(4)); }

    [Fact]
    public void IsLengthBetween_RejectsInvertedBounds() => Assert.Throws<ArgumentOutOfRangeException>(() => "test".IsLengthBetween(5, 2));
}
