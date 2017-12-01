namespace Web.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Web.Models;

    public class AverageCheckpointTimeCalculator : IAverageCheckpointTimeCalculator
    {
        public int Calculate(
            IReadOnlyList<IReadingCalculation> readingsIn,
            IReadOnlyList<IReadingCalculation> readingsOut)
        {
            int result = 0;

            if (readingsIn == null || readingsOut == null || !readingsIn.Any())
            {
                return result;
            }

            // All readings in
            var attendeesIn = readingsIn.Select(m => m.Id);
            var attendeesOut = readingsOut.Select(m => m.Id);
            var attendeesPassed = attendeesIn.Intersect(attendeesOut);

            var seconds = new List<double>();

            // For each attendee find the exit time and calculate time in seconds
            foreach (var attendee in attendeesPassed)
            {
                var readingIn = readingsIn.First(m => m.Id == attendee);
                var readingOut = readingsOut.First(m => m.Id == attendee);

                seconds.Add(TimeSpan.FromTicks(readingOut.Timestamp.Ticks - readingIn.Timestamp.Ticks).TotalSeconds);
            }

            if (seconds.Count > 0)
            {
                result = (int)seconds.Average();
            }

            return result;
        }
    }
}