namespace KsxEventTracker.Domain
{
    using System;

    public static class DateTimeExtensions
    {
        public static DateTimeOffset ToUtcOffset(this DateTime value)
        {
            return new DateTimeOffset(value, TimeSpan.FromHours(0));
        }
    }
}