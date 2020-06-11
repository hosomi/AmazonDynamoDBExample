using System;
using TimeZoneConverter;

namespace AmazonDynamoDBExample
{
    public struct DateTimeWithZone
    {
        public DateTimeWithZone(DateTime dateTime, TimeZoneInfo timeZone)
        {
            var dateTimeUnspec = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            UniversalTime = TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspec, timeZone);
            this.TimeZone = timeZone;
        }

        public DateTime UniversalTime { get; }

        public TimeZoneInfo TimeZone { get; }

        public DateTime LocalTime
        {
            get
            {
                return TimeZoneInfo.ConvertTime(UniversalTime, TimeZone);
            }
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime LocalTime(this DateTime value)
        {
            return new DateTimeWithZone(value, TZConvert.GetTimeZoneInfo("Tokyo Standard Time")).LocalTime;
        }
    }
}
