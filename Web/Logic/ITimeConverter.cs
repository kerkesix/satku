namespace Web.Logic
{
    using System;

    public interface ITimeConverter
    {
        DateTime FromUtcTime(DateTime utcTime);
        DateTime ToUtcTime(DateTime localTime);
        DateTime LocalTime { get; }
    }
}
