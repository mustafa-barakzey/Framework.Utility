using brk.Framework.Utility.Extensions;

namespace brk.Framework.Utility.Validators;

public class NationalIdValidator
{
    #region National Identity Patterns

    // Iran National ID (Code Melli)
    // 10 digits, follows specific validation algorithm
    #endregion

    /// <summary>
    /// Determines whether every character in <paramref name="digits"/> is the same
    /// (e.g. "0000000000"), which is never a valid national code or national ID.
    /// </summary>
    private static bool IsAllSameDigit(string digits)
    {
        for (var index = 1; index < digits.Length; index++)
            if (digits[index] != digits[0])
                return false;

        return digits.Length > 0;
    }

    /// <summary>
    /// Determines whether the specified value is a valid Iranian national ID using the official checksum algorithm.
    /// </summary>
    /// <param name="nationalId">National ID to validate</param>
    /// <returns>True if valid Iranian National ID</returns>
    public static bool IsIranNationalId2(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId))
            return false;

        var normalized = nationalId.Trim().Replace("-", string.Empty).Replace(" ", string.Empty);
        return normalized.Length == 10 && IsIranNationalId(normalized);
    }

    /// <summary>
    /// Determines whether the specified value is a valid Iranian national ID.
    /// </summary>
    public static bool IsIranNationalId(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId) || !nationalId.IsLengthBetween(8, 10))
            return false;

        nationalId = nationalId.ToEnglishNumber().PadLeft(10, '0');

        if (!nationalId.IsNumeric())
            return false;

        if (IsAllSameDigit(nationalId))
            return false;

        return IsChecksumValid(nationalId);

        static bool IsChecksumValid(string code)
        {
            var weightedSum = 0;
            for (int i = 0; i < 9; i++)
                weightedSum += (code[i] - '0') * (10 - i);

            var remainder = weightedSum % 11;
            var checkDigit = code[9] - '0';

            return (remainder < 2 && checkDigit == remainder)
                   || (remainder >= 2 && 11 - remainder == checkDigit);
        }
    }


    /// <summary>
    /// Determines whether the specified value has a valid Afghan national ID (Tazkira) format.
    /// </summary>
    /// <param name="nationalId">National ID to validate</param>
    /// <returns>True if valid Afghanistan National ID</returns>
    public static bool IsAfghanistanNationalId(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId))
            return false;

        // Remove any whitespace or special characters
        nationalId = nationalId.Trim().Replace("-", "").Replace(" ", "").ToEnglishNumber();

        // Check basic pattern (10-13 digits)
        if (nationalId.Length is < 10 or > 13 || !nationalId.IsNumeric())
            return false;

        // No public checksum specification is available for all Tazkira formats.
        return !IsAllSameDigit(nationalId);
    }
}
// public class NationalIdValidator
// {
//     
//     /// <summary>
//     /// صحت سنجی کد ملی
//     /// </summary>
//     /// <param name="nationalCode">کد ملی</param>
//     /// <returns>درست یا غلط</returns>
//     public static bool IsNationalCode(this string nationalCode)
//     {
//         if (string.IsNullOrWhiteSpace(nationalCode) || !nationalCode.IsLengthBetween(8, 10))
//             return false;
//
//         nationalCode = nationalCode.PadLeft(10, '0');
//
//         if (!nationalCode.IsNumeric())
//             return false;
//
//         if (IsAllSameDigit(nationalCode))
//             return false;
//
//         return IsChecksumValid(nationalCode);
//
//         static bool IsChecksumValid(string code)
//         {
//             var digits = code.Select(c => c - '0').ToArray();
//
//             var weightedSum = 0;
//             for (int i = 0; i < 9; i++)
//                 weightedSum += digits[i] * (10 - i);
//
//             var remainder = weightedSum % 11;
//             var checkDigit = digits[9];
//
//             return (remainder < 2 && checkDigit == remainder)
//                    || (remainder >= 2 && 11 - remainder == checkDigit);
//         }
//     }
//
//     /// <summary>
//     /// صحت سنجی شناسه ملی شرکت‌ها
//     /// </summary>
//     /// <param name="nationalId">شناسه ملی شرکت</param>
//     /// <returns>درست یا غلط</returns>
//     public static bool IsLegalNationalIdValid(this string nationalId)
//     {
//         if (string.IsNullOrWhiteSpace(nationalId) || !nationalId.IsLengthEqual(11))
//             return false;
//
//         if (!nationalId.IsNumeric())
//             return false;
//
//         if (IsAllSameDigit(nationalId))
//             return false;
//
//         return IsChecksumValid(nationalId);
//
//         static bool IsChecksumValid(string id)
//         {
//             // Weights repeat the 29,27,23,19,17 sequence twice, per the standard
//             // Iranian legal-entity national ID (Shenase Meli) checksum algorithm.
//             var weights = new[] { 29, 27, 23, 19, 17, 29, 27, 23, 19, 17 };
//             var digits = id.Select(c => c - '0').ToArray();
//
//             var controlDigit = digits[10];
//             var factor = digits[9] + 2;
//
//             var sum = 0;
//             for (int i = 0; i < 10; i++)
//                 sum += (factor + digits[i]) * weights[i];
//
//             var remaining = sum % 11;
//             if (remaining == 10)
//                 remaining = 0;
//
//             return remaining == controlDigit;
//         }
//     }
//
// }
