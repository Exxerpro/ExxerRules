using IndTrace.Application.Common.Interfaces;
using System.Globalization;

namespace IndTrace.DataSeeder;

//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate SeederDateTimeMachine logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.

public static class DateTimeExtensions
{
    public static DateTime ParseExactDateTime(this string date, string format, CultureInfo cultureInfo)
    {
        return DateTime.ParseExact(date, format, cultureInfo);
    }


}
