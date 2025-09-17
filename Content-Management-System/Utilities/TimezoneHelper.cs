using System;
using System.Runtime.InteropServices;

namespace Content_Management_System.Utilities
{
    public static class TimezoneHelper
    {
        private static readonly TimeZoneInfo PhTimeZone;

        static TimezoneHelper()
        {
            try
            {
                PhTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                        ? "Singapore Standard Time"   // Windows name
                        : "Asia/Manila"               // Linux/macOS name
                );
            }
            catch (TimeZoneNotFoundException)
            {
                Console.WriteLine("[TimezoneHelper] PH timezone not found. Falling back to UTC.");
                PhTimeZone = TimeZoneInfo.Utc;
            }
            catch (InvalidTimeZoneException)
            {
                Console.WriteLine("[TimezoneHelper] PH timezone invalid. Falling back to UTC.");
                PhTimeZone = TimeZoneInfo.Utc;
            }
        }

        public static DateTime NowPH()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PhTimeZone);
        }
    }
}
