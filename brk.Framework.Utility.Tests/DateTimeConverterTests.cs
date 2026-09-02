using brk.Framework.Utility.DateTimes;
using Xunit;

namespace brk.Framework.Utility.Tests;

public sealed class DateTimeConverterTests
{
    [Fact]
    public void GregorianConversions_ParseDateAndTime() { Assert.Equal(new DateOnly(2024, 3, 20), "۱۴۰۳/۰۱/۰۱".ToGregorian()); Assert.Equal(new DateTime(2024, 3, 20, 13, 5, 0), "۱۴۰۳/۰۱/۰۱   ۱۳:۰۵".ToGregorianDateTime()); Assert.Throws<FormatException>(() => "bad".ToGregorian()); }

    [Fact]
    public void ShortFormatters_FormatDateAndTime() { Assert.Equal("25:02", new TimeSpan(1, 1, 2, 0).ToShortTime()); Assert.Equal("13:05", new TimeOnly(13, 5).ToShortTime()); Assert.Equal("2024-03-20", new DateOnly(2024, 3, 20).ToShortDate()); Assert.Equal("2024-03-20", new DateTime(2024, 3, 20).ToShortDate()); }
}
