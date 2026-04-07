namespace AutoNext.Core.Helpers
{
    public static class DateTimeHelper
    {
        private static readonly TimeZoneInfo IndiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private static readonly TimeZoneInfo UtcTimeZone = TimeZoneInfo.Utc;

        public static DateTime Now => DateTime.UtcNow;
        public static DateTime Today => DateTime.UtcNow.Date;

        public static DateTime ToIndiaTime(this DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, IndiaTimeZone);
        }

        public static DateTime ToUtc(this DateTime indiaDateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(indiaDateTime, IndiaTimeZone);
        }

        public static DateTime StartOfDay(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        public static DateTime EndOfDay(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(1).AddTicks(-1);
        }

        public static DateTime StartOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
        }
    }
}
