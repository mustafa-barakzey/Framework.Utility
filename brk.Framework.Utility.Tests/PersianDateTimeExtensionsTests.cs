using brk.Framework.Utility.DateTimes;
using brk.Framework.Utility.Enums;
using Xunit;

namespace brk.Framework.Utility.Tests;

public sealed class PersianDateTimeExtensionsTests
{
    [Fact]
    public void PersianFormattingAndNumbers_Work() { var date = new DateTime(2024, 3, 20, 13, 5, 0); Assert.Equal("1403/01/01", date.ToShamsiDate()); Assert.Equal("1403/01/01", DateOnly.FromDateTime(date).ToShamsiDate()); Assert.Equal("1403/01/01 13:05", date.ToShamsiDateTime()); Assert.Equal("13:05", date.ToShortTime()); Assert.Contains("1403", date.ToShamsiDateString()); Assert.Equal("۱۲۳", "123".ToFarsiNumber()); }

    [Fact]
    public void MonthAndDayMappings_Work() { Assert.Equal("فروردین", PersianDateTimeExtensions.GetMonthOfYear()[PersianMonthOfYear.Farvardin]); Assert.InRange((int)PersianDateTimeExtensions.GetCurrentMonth(), 1, 12); Assert.Equal(DayOfWeek.Saturday, DayOfWeekPersian.Saturday.ToDayOfWeek()); Assert.Equal(DayOfWeekPersian.Friday, DayOfWeek.Friday.ToDayOfWeekPersian()); }
}
