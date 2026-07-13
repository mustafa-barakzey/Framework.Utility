
using System;
using System.Collections.Generic;
using System.Globalization;
using brk.Framework.Utility.Enums;
using brk.Framework.Utility.Extensions;

namespace brk.Framework.Utility.DateTimes;

/// <summary>
/// Provides extension methods for converting between the Gregorian and Persian (Shamsi/Jalali)
/// calendars, formatting dates and times in Persian, converting ASCII digits to Farsi glyphs,
/// and mapping between standard <see cref="DayOfWeek"/> values and their Persian equivalents.
/// </summary>
public static class PersianDateTimeExtensions
{
    /// <summary>Persian month names, indexed 0-11 for months 1-12.</summary>
    private static readonly string[] MonthNames =
    {
        "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
        "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
    };

    /// <summary>Persian names for each <see cref="DayOfWeek"/>.</summary>
    private static readonly Dictionary<DayOfWeek, string> DayNames = new()
    {
        [DayOfWeek.Saturday] = "شنبه",
        [DayOfWeek.Sunday] = "یکشنبه",
        [DayOfWeek.Monday] = "دوشنبه",
        [DayOfWeek.Tuesday] = "سه شنبه",
        [DayOfWeek.Wednesday] = "چهارشنبه",
        [DayOfWeek.Thursday] = "پنجشنبه",
        [DayOfWeek.Friday] = "جمعه"
    };

    /// <summary>Lookup table mapping ASCII digits to their Farsi (Eastern Arabic-Indic) equivalents.</summary>
    private static readonly Dictionary<char, char> EnToFaDigits = new()
    {
        ['0'] = '۰', ['1'] = '۱', ['2'] = '۲', ['3'] = '۳', ['4'] = '۴',
        ['5'] = '۵', ['6'] = '۶', ['7'] = '۷', ['8'] = '۸', ['9'] = '۹'
    };

    /// <summary>
    /// Converts a <see cref="DateTime"/> to its Shamsi (Persian) date representation in "yyyy/MM/dd" format.
    /// </summary>
    /// <param name="value">The Gregorian date/time to convert.</param>
    /// <returns>The Persian calendar date formatted as "yyyy/MM/dd".</returns>
    public static string ToShamsiDate(this DateTime value) => FormatShamsiDate(value);

    /// <summary>
    /// Converts a <see cref="DateOnly"/> to its Shamsi (Persian) date representation in "yyyy/MM/dd" format.
    /// </summary>
    /// <param name="date">The Gregorian date to convert.</param>
    /// <returns>The Persian calendar date formatted as "yyyy/MM/dd".</returns>
    public static string ToShamsiDate(this DateOnly date) => FormatShamsiDate(date.ToDateTime(TimeOnly.MinValue));

    /// <summary>
    /// Converts a <see cref="DateTime"/> to its Shamsi (Persian) date and time representation
    /// in "yyyy/MM/dd HH:mm" format.
    /// </summary>
    /// <param name="value">The Gregorian date/time to convert.</param>
    /// <returns>The Persian calendar date and time formatted as "yyyy/MM/dd HH:mm".</returns>
    public static string ToShamsiDateTime(this DateTime value) => $"{FormatShamsiDate(value)} {value:HH:mm}";

    /// <summary>
    /// Formats the time portion of a <see cref="DateTime"/> as "HH:mm".
    /// </summary>
    /// <param name="value">The date/time whose time portion is formatted.</param>
    /// <returns>The time formatted as "HH:mm".</returns>
    public static string ToShortTime(this DateTime value) => value.ToString("HH:mm");

    /// <summary>
    /// Converts a <see cref="DateTime"/> to a full Persian display string that includes the
    /// weekday name, day of month, month name, and year, e.g. "سه شنبه 18 تیر 1403".
    /// </summary>
    /// <param name="dt">The Gregorian date/time to convert.</param>
    /// <returns>A human-readable Persian date string.</returns>
    public static string ToShamsiDateString(this DateTime dt)
    {
        var persianCalendar = new PersianCalendar();
        int dayOfMonth = persianCalendar.GetDayOfMonth(dt);
        int year = persianCalendar.GetYear(dt);
        string monthName = MonthNames[persianCalendar.GetMonth(dt) - 1];
        string dayName = DayNames.TryGetValue(persianCalendar.GetDayOfWeek(dt), out var name) ? name : string.Empty;

        return $"{dayName} {dayOfMonth} {monthName} {year}";
    }

    /// <summary>
    /// Returns the Persian months of the year as a dictionary keyed by month number (1-12).
    /// </summary>
    /// <returns>A dictionary mapping month number to Persian month name.</returns>
    public static Dictionary<PersianMonthOfYear, string> GetMonthOfYear() => EnumExtensions.GetItemsAsDictionary<PersianMonthOfYear>();

    /// <summary>
    /// Returns the current Persian (Shamsi) month based on today's date.
    /// </summary>
    /// <returns>The current <see cref="PersianMonthOfYear"/>.</returns>
    public static PersianMonthOfYear GetCurrentMonth() => (PersianMonthOfYear)new PersianCalendar().GetMonth(DateTime.Now);

    /// <summary>
    /// Replaces every ASCII digit (0-9) in the given text with its Farsi (Eastern Arabic-Indic) digit equivalent.
    /// </summary>
    /// <param name="text">The source text containing ASCII digits.</param>
    /// <returns>A new string with ASCII digits replaced by Farsi digits, or the original value if null/empty.</returns>
    public static string ToFarsiNumber(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var chars = text.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (EnToFaDigits.TryGetValue(chars[i], out var faDigit))
                chars[i] = faDigit;
        }

        return new string(chars);
    }

    /// <summary>
    /// Converts a custom <see cref="DayOfWeekPersian"/> value to the equivalent standard <see cref="DayOfWeek"/>.
    /// </summary>
    /// <param name="persianDay">The Persian day-of-week value.</param>
    /// <returns>The equivalent <see cref="DayOfWeek"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="persianDay"/> is not a recognized value.</exception>
    public static DayOfWeek ToDayOfWeek(this DayOfWeekPersian persianDay)
        => persianDay switch
        {
            DayOfWeekPersian.Saturday => DayOfWeek.Saturday,
            DayOfWeekPersian.Sunday => DayOfWeek.Sunday,
            DayOfWeekPersian.Monday => DayOfWeek.Monday,
            DayOfWeekPersian.Tuesday => DayOfWeek.Tuesday,
            DayOfWeekPersian.Wednesday => DayOfWeek.Wednesday,
            DayOfWeekPersian.Thursday => DayOfWeek.Thursday,
            DayOfWeekPersian.Friday => DayOfWeek.Friday,
            _ => throw new ArgumentOutOfRangeException(nameof(persianDay), persianDay, null),
        };

    /// <summary>
    /// Converts a standard <see cref="DayOfWeek"/> value to the equivalent custom <see cref="DayOfWeekPersian"/>.
    /// </summary>
    /// <param name="dayOfWeek">The standard day-of-week value.</param>
    /// <returns>The equivalent <see cref="DayOfWeekPersian"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="dayOfWeek"/> is not a recognized value.</exception>
    public static DayOfWeekPersian ToDayOfWeekPersian(this DayOfWeek dayOfWeek)
        => dayOfWeek switch
        {
            DayOfWeek.Saturday => DayOfWeekPersian.Saturday,
            DayOfWeek.Sunday => DayOfWeekPersian.Sunday,
            DayOfWeek.Monday => DayOfWeekPersian.Monday,
            DayOfWeek.Tuesday => DayOfWeekPersian.Tuesday,
            DayOfWeek.Wednesday => DayOfWeekPersian.Wednesday,
            DayOfWeek.Thursday => DayOfWeekPersian.Thursday,
            DayOfWeek.Friday => DayOfWeekPersian.Friday,
            _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null),
        };

    /// <summary>
    /// Formats a Gregorian <see cref="DateTime"/> as a Persian (Shamsi) date string in "yyyy/MM/dd" format.
    /// </summary>
    /// <param name="value">The Gregorian date/time to convert.</param>
    /// <returns>The Persian calendar date formatted as "yyyy/MM/dd".</returns>
    private static string FormatShamsiDate(DateTime value)
    {
        var persianCalendar = new PersianCalendar();
        return
            $"{persianCalendar.GetYear(value)}/{persianCalendar.GetMonth(value):00}/{persianCalendar.GetDayOfMonth(value):00}";
    }
}