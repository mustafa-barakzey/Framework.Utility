using System.Globalization;
using System.Reflection;

namespace brk.Framework.Utility.Helpers;

public class CultureHelper
{
    /// <summary>
    /// Creates a <see cref="CultureInfo"/> configured for Iran with Persian calendar and number formatting.
    /// </summary>
    /// <returns>A culture configured for Iranian Persian conventions.</returns>
    public static CultureInfo GetIranCulture()
    {
        var culture = new CultureInfo("fa-IR");
        var formatInfo = culture.DateTimeFormat;
        formatInfo.AbbreviatedDayNames = ["ی", "د", "س", "چ", "پ", "ج", "ش"];
        formatInfo.DayNames = ["یکشنبه", "دوشنبه", "سه شنبه", "چهار شنبه", "پنجشنبه", "جمعه", "شنبه"];
        var monthNames = new[]
        {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند", ""
        };
        formatInfo.AbbreviatedMonthNames = formatInfo.MonthNames =
            formatInfo.MonthGenitiveNames = formatInfo.AbbreviatedMonthGenitiveNames = monthNames;
        formatInfo.AMDesignator = "ق.ظ";
        formatInfo.PMDesignator = "ب.ظ";
        formatInfo.ShortDatePattern = "yyyy/MM/dd";
        formatInfo.LongDatePattern = "dddd, dd MMMM,yyyy";
        formatInfo.FirstDayOfWeek = DayOfWeek.Saturday;
        Calendar cal = new PersianCalendar();
        var fieldInfo = culture.GetType().GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo != null)
            fieldInfo.SetValue(culture, cal);
        var info = formatInfo.GetType().GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
        if (info != null)
            info.SetValue(formatInfo, cal);
        culture.NumberFormat.NumberDecimalSeparator = "/";
        culture.NumberFormat.DigitSubstitution = DigitShapes.NativeNational;
        culture.NumberFormat.NumberNegativePattern = 0;
        return culture;
    }

    /// <summary>
    /// Creates a <see cref="CultureInfo"/> configured for Afghan Dari conventions.
    /// </summary>
    public static CultureInfo GetAfghanCulture()
    {
        var culture = new CultureInfo("prs-AF"); // Dari (Afghan Persian)
        var formatInfo = culture.DateTimeFormat;

        // Day names in Dari
        formatInfo.AbbreviatedDayNames = new[] { "ی", "د", "س", "چ", "پ", "ج", "ش" };
        formatInfo.DayNames = new[] { "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه" };

        // Afghan Persian month names (Dari)
        var monthNames = new[]
        {
            "حمل", "ثور", "جوزا", "سرطان", "اسد", "سنبله",
            "میزان", "عقرب", "قوس", "جدی", "دلو", "حوت",""
        };

        formatInfo.AbbreviatedMonthNames =
            formatInfo.MonthNames =
                formatInfo.MonthGenitiveNames =
                    formatInfo.AbbreviatedMonthGenitiveNames = monthNames;

        formatInfo.AMDesignator = "ق.ظ";
        formatInfo.PMDesignator = "ب.ظ";
        formatInfo.ShortDatePattern = "yyyy/MM/dd";
        formatInfo.LongDatePattern = "dddd, dd MMMM yyyy";
        formatInfo.FirstDayOfWeek = DayOfWeek.Saturday;

        // Set Persian (Solar Hijri) calendar
        Calendar cal = new PersianCalendar();
        var fieldInfo = culture.GetType().GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo != null)
            fieldInfo.SetValue(culture, cal);

        var info = formatInfo.GetType().GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
        if (info != null)
            info.SetValue(formatInfo, cal);

        // Number formatting
        culture.NumberFormat.NumberDecimalSeparator = "/";
        culture.NumberFormat.DigitSubstitution = DigitShapes.NativeNational;
        culture.NumberFormat.NumberNegativePattern = 0;

        return culture;
    }

    /// <summary>
    /// Creates a <see cref="CultureInfo"/> configured for Afghan Pashto conventions.
    /// </summary>
    public static CultureInfo GetPashtoCulture()
    {
        var culture = new CultureInfo("ps-AF");
        var formatInfo = culture.DateTimeFormat;

        // Day names in Pashto
        formatInfo.AbbreviatedDayNames = new[] { "ی", "د", "س", "چ", "پ", "ج", "ش" };
        formatInfo.DayNames = new[] { "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه" };

        // Pashto month names
        var monthNames = new[]
        {
            "وری", "غویي", "غبرګولی", "چنګاښ", "زمری", "وږی",
            "تله", "لړم", "لیندۍ", "مرغومی", "سلواغه", "کب", ""
        };

        formatInfo.AbbreviatedMonthNames =
            formatInfo.MonthNames =
                formatInfo.MonthGenitiveNames =
                    formatInfo.AbbreviatedMonthGenitiveNames = monthNames;

        formatInfo.AMDesignator = "غ.م";
        formatInfo.PMDesignator = "غ.و";
        formatInfo.ShortDatePattern = "yyyy/MM/dd";
        formatInfo.LongDatePattern = "dddd, dd MMMM yyyy";
        formatInfo.FirstDayOfWeek = DayOfWeek.Saturday;

        // Set Persian (Solar Hijri) calendar
        Calendar cal = new PersianCalendar();
        var fieldInfo = culture.GetType().GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo != null)
            fieldInfo.SetValue(culture, cal);

        var info = formatInfo.GetType().GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
        if (info != null)
            info.SetValue(formatInfo, cal);

        // Number formatting
        culture.NumberFormat.NumberDecimalSeparator = "/";
        culture.NumberFormat.DigitSubstitution = DigitShapes.NativeNational;
        culture.NumberFormat.NumberNegativePattern = 0;

        return culture;
    }
}
