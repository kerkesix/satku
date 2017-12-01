namespace Web.Logic
{
    using System;

    public class TimeConverter : ITimeConverter
    {
        // Helsinki TimeZone
        private const string SerializedTimeZone = @"FLE Standard Time;120;(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius;FLE Standard Time;FLE Daylight Time;[01:01:0001;12:31:9999;60;[0;03:00:00;3;5;0;];[0;04:00:00;10;5;0;];];";

        private static readonly TimeZoneInfo ZoneInfo = TimeZoneInfo.FromSerializedString(SerializedTimeZone);

        public DateTime LocalTime
        {
            get
            {        
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ZoneInfo);
            }
        }

        public DateTime ToUtcTime(DateTime localTime)
        {
            // Without this conversion will fail if DateTime.Kind is Local
            DateTime inputDate = DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(inputDate, ZoneInfo);
        }

        public DateTime FromUtcTime(DateTime utcTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, ZoneInfo);
        }
    }
}