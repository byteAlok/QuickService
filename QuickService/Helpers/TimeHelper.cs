using System;

namespace QuickService.Helpers
{
    public static class TimeHelper
    {
        private static readonly TimeZoneInfo IstTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        //public static DateTime ConvertToIst(DateTime utcDateTime)
        //{
        //    return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, IstTimeZone);
        //}
        public static DateTime ConvertToIst(DateTime utcDateTime)
        {
            utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, IstTimeZone);
        }
        public static string GetTimeAgo(DateTime? dateTime)
        {
            if (!dateTime.HasValue) return "Never";

            // Convert to IST for comparison if needed, or stick to UTC for diff
            var timeSpan = DateTime.UtcNow - dateTime.Value;

            if (timeSpan.TotalMinutes < 1)
                return "just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hours ago";
            
            return $"{(int)timeSpan.TotalDays} days ago";
        }

        public static string GetStatusText(bool status)
        {
            return status ? "Active" : "Inactive";
        }
    }
}
