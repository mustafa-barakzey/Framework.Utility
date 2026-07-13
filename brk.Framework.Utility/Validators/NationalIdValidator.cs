using System.Text.RegularExpressions;
using brk.Framework.Utility.Extensions;

namespace brk.Framework.Utility.Validators;

public class NationalIdValidator
{
    #region National Identity Patterns

    // Iran National ID (Code Melli)
    // 10 digits, follows specific validation algorithm
    private const string IranNationalIdPattern = @"^\d{10}$";

    // Afghanistan National ID (Tazkira)
    // Usually 10-13 digits
    private const string AfghanistanNationalIdPattern = @"^\d{10,13}$";

    #endregion

    /// <summary>
    /// Determines whether every character in <paramref name="digits"/> is the same
    /// (e.g. "0000000000"), which is never a valid national code or national ID.
    /// </summary>
    private static bool IsAllSameDigit(string digits) => digits.Distinct().Count() <= 1;

    /// <summary>
    /// Validates Iranian National ID (Code Melli) using the official algorithm
    /// </summary>
    /// <param name="nationalId">National ID to validate</param>
    /// <returns>True if valid Iranian National ID</returns>
    public static bool IsIranNationalId2(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId))
            return false;

        // Remove any whitespace or special characters
        nationalId = nationalId.Trim().Replace("-", "").Replace(" ", "");

        // Check basic pattern
        if (!Regex.IsMatch(nationalId, IranNationalIdPattern))
            return false;

        // Check for repetitive numbers (all same digits)
        if (nationalId == new string(nationalId[0], 10))
            return false;

        try
        {
            // Algorithm: Iranian National ID validation
            // Position: 10 9 8 7 6 5 4 3 2 1
            // Multiply each digit (except the last) by its position
            // Sum all products
            // Calculate remainder of sum divided by 11
            // If remainder < 2, the control digit should equal remainder
            // If remainder >= 2, the control digit should equal 11 - remainder

            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += int.Parse(nationalId[i].ToString()) * (10 - i);
            }

            int remainder = sum % 11;
            int controlDigit = int.Parse(nationalId[9].ToString());

            if (remainder < 2)
                return controlDigit == remainder;
            else
                return controlDigit == (11 - remainder);
        }
        catch
        {
            return false;
        }
    }

    public static bool IsIranNationalId(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId) || !nationalId.IsLengthBetween(8, 10))
            return false;

        nationalId = nationalId.PadLeft(10, '0');

        if (!nationalId.IsNumeric())
            return false;

        if (IsAllSameDigit(nationalId))
            return false;

        return IsChecksumValid(nationalId);

        static bool IsChecksumValid(string code)
        {
            var digits = code.Select(c => c - '0').ToArray();

            var weightedSum = 0;
            for (int i = 0; i < 9; i++)
                weightedSum += digits[i] * (10 - i);

            var remainder = weightedSum % 11;
            var checkDigit = digits[9];

            return (remainder < 2 && checkDigit == remainder)
                   || (remainder >= 2 && 11 - remainder == checkDigit);
        }
    }


    /// <summary>
    /// Validates Afghanistan National ID (Tazkira)
    /// </summary>
    /// <param name="nationalId">National ID to validate</param>
    /// <returns>True if valid Afghanistan National ID</returns>
    public static bool IsAfghanistanNationalId(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId))
            return false;

        if (IsAllSameDigit(nationalId))
            return false;

        // Remove any whitespace or special characters
        nationalId = nationalId.Trim().Replace("-", "").Replace(" ", "");

        // Check basic pattern (10-13 digits)
        if (!Regex.IsMatch(nationalId, AfghanistanNationalIdPattern))
            return false;

        // Additional validation can be added here based on Tazkira format
        // Currently checking if it's all digits and within length range
        return long.TryParse(nationalId, out _);
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