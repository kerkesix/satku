namespace Web.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using QueryModels;

    public class AttendeeSpeedCalculator : IAttendeeSpeedCalculator
    {
        public AttendeeSpeed Calculate(
            IOrderedEnumerable<DashboardCheckpoint> checkpoints, 
            string personId)
        {
            DateTimeOffset last = DateTimeOffset.MinValue;
            var result = new AttendeeSpeed { Id = personId };
            var restTimes = new List<double>();

            foreach (var entry in checkpoints)
            {
                var visit = entry.Visits.FirstOrDefault(m => m.PersonId.Equals(personId));

                if (visit == null)
                {
                    continue;
                }

                if (entry.DistanceFromPrevious > 0 && last > DateTimeOffset.MinValue && visit.TimeIn.HasValue)
                {
                    TimeSpan time = visit.TimeIn.Value - last;
                    result.WalkingTimes.Add(time);
                    result.WalkedKilometres.Add(entry.DistanceFromPrevious);
                    result.CheckpointSpeeds.Add(entry.Id, (double)entry.DistanceFromPrevious / time.TotalHours);
                }

                last = visit.TimeIn ?? DateTimeOffset.MinValue;

                // If this attendee has quit on this checkpoint, add the last walked 
                // kilometers to total
                if (visit.Quit != null && visit.Quit.WalkedSinceLastCheckpoint > 0)
                {
                    result.WalkedKilometres.Add(visit.Quit.WalkedSinceLastCheckpoint);
                }

                if (visit.Duration > 0)
                {
                    restTimes.Add(visit.Duration);
                }
            }

            return result;
        }
    }
}