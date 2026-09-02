using System.Globalization;
using brk.Framework.Utility.Extensions;

namespace brk.Framework.Utility.DateTimes;

/// <summary>
/// Provides extension methods for converting Persian (Solar Hijri) date strings
/// to Gregorian <see cref="DateOnly"/> and <see cref="DateTime"/> instances,
/// as well as formatting date and time values.
/// </summary>
public static class DateTimeConverter
{
    /// <summary>
    /// Converts a Persian date string to a Gregorian <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="date">
    /// The Persian date string in <c>yyyy/MM/dd</c> or <c>yyyy-MM-dd</c> format.
    /// Persian digits are also supported.
    /// </param>
    /// <returns>
    /// A <see cref="DateOnly"/> representing the corresponding Gregorian date.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="date"/> is null or empty.
    /// </exception>
    /// <exception cref="FormatException">
    /// Thrown when the date is not in a valid Persian date format.
    /// </exception>
    public static DateOnly ToGregorian(this string date)
    {
        if (string.IsNullOrEmpty(date))
            throw new ArgumentException("Date cannot be null or empty.", nameof(date));

        // Example format: "1402/01/01" or "1402-01-01"
        var dateParts = date.ToEnglishNumber().Split('/', '-');
        if (dateParts.Length != 3)
        {
            throw new FormatException("Invalid Persian date format. Expected format: YYYY/MM/DD");
        }

        var year = int.Parse(dateParts[0]);
        var month = int.Parse(dateParts[1]);
        var day = int.Parse(dateParts[2]);

        return new DateOnly(year, month, day, new PersianCalendar());
    }

    /// <summary>
    /// Converts a Persian date or date-time string to a Gregorian <see cref="DateTime"/>.
    /// </summary>
    /// <param name="persianDateTime">
    /// The Persian date string in one of the following formats:
    /// <list type="bullet">
    /// <item><description><c>yyyy/MM/dd</c></description></item>
    /// <item><description><c>yyyy-MM-dd</c></description></item>
    /// <item><description><c>yyyy/MM/dd HH:mm</c></description></item>
    /// <item><description><c>yyyy/MM/dd HH:mm:ss</c></description></item>
    /// </list>
    /// Persian digits are also supported.
    /// </param>
    /// <returns>
    /// A <see cref="DateTime"/> representing the corresponding Gregorian date and time.
    /// </returns>
    /// <exception cref="FormatException">
    /// Thrown when the supplied value is not in a supported Persian date or date-time format.
    /// </exception>
    public static DateTime ToGregorianDateTime(this string persianDateTime)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(persianDateTime);
        var dateAndTime = persianDateTime.ToEnglishNumber().Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

        // date only
        if (dateAndTime.Length == 1)
        {
            var dateOnly = dateAndTime[0].ToGregorian();
            return new DateTime(dateOnly.Year, dateOnly.Month, dateOnly.Day);
        }

        // date and time
        if (dateAndTime.Length == 2)
        {
            var dateOnly = dateAndTime[0].ToGregorian();

            if (TimeOnly.TryParseExact(dateAndTime[1], ["HH:mm", "HH:mm:ss"], CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var time))
                return dateOnly.ToDateTime(time);
        }

        throw new FormatException("Invalid Persian date format.");
    }

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to a string in <c>HH:mm</c> format.
    /// If the duration exceeds 24 hours, the total number of hours is returned.
    /// </summary>
    /// <param name="ts">The time interval to format.</param>
    /// <returns>
    /// A string representing the time interval in <c>HH:mm</c> format.
    /// </returns>
    public static string ToShortTime(this TimeSpan ts)
    {
        var hours = ts.Hours;
        if (ts.Days > 0)
        {
            hours += ts.Days * 24;
            return hours.ToString("") + ":" + ts.Minutes.ToString("00");
        }

        return ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00");
    }

    /// <summary>
    /// Converts a <see cref="TimeOnly"/> to a string in <c>HH:mm</c> format.
    /// </summary>
    /// <param name="ts">The time value to format.</param>
    /// <returns>
    /// A string representing the time in <c>HH:mm</c> format.
    /// </returns>
    public static string ToShortTime(this TimeOnly ts)
    {
        return ts.Hour.ToString("00") + ":" + ts.Minute.ToString("00");
    }

    /// <summary>
    /// Formats a <see cref="DateOnly"/> as <c>yyyy-MM-dd</c>.
    /// </summary>
    /// <param name="ts">The date to format.</param>
    /// <returns>
    /// A string representing the date in <c>yyyy-MM-dd</c> format.
    /// </returns>
    public static string ToShortDate(this DateOnly ts) => ts.ToString("yyyy-MM-dd");

    /// <summary>
    /// Formats a <see cref="DateTime"/> as <c>yyyy-MM-dd</c>.
    /// </summary>
    /// <param name="ts">The date to format.</param>
    /// <returns>
    /// A string representing the date in <c>yyyy-MM-dd</c> format.
    /// </returns>
    public static string ToShortDate(this DateTime ts) => ts.ToString("yyyy-MM-dd");
}
