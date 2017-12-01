namespace Web.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AttendeeSpeed
    {
        private const double DefaultSpeed = 6d;

        public AttendeeSpeed()
        {
            this.CheckpointSpeeds = new Dictionary<string, double>();
            this.WalkingTimes = new List<TimeSpan>();
            this.WalkedKilometres = new List<decimal>();
        }

        public Dictionary<string, double> CheckpointSpeeds { get; private set; }

        public List<TimeSpan> WalkingTimes { get; private set; }

        public List<decimal> WalkedKilometres { get; private set; }

        public double WeightedSpeed
        {
            get
            {
                if (!this.CheckpointSpeeds.Any())
                {
                    // Default speed
                    return DefaultSpeed;
                }

                if (this.CheckpointSpeeds.Count == 1)
                {
                    return this.CheckpointSpeeds.Last().Value;
                }

                // Calculate average
                // Last leg weights 50 %
                var allButLast = this.CheckpointSpeeds.Take(this.CheckpointSpeeds.Count - 1).Average(m => m.Value);
                return (new[] { allButLast, this.CheckpointSpeeds.Last().Value }).Average();
            }
        }

        /// <summary>
        /// Calculates the change in average speed
        /// </summary>
        public double LastAverageSpeedChangePercent
        {
            get
            {
                if (this.CheckpointSpeeds.Count <= 1)
                {
                    // No change yet
                    return 0;
                }

                var secondLast = this.CheckpointSpeeds.Select(m => m.Value).ToArray()[this.CheckpointSpeeds.Count - 2];
                var last = this.CheckpointSpeeds.Last().Value;

                return (last - secondLast) / secondLast;
            }
        }

        public string Id { get; set; }

        public double[] RestTimes { get; set; }

        public TimeSpan TotalWalkingTime
        {
            get
            {
                long ticks = this.WalkingTimes.Sum(m => m.Ticks);
                TimeSpan time = TimeSpan.FromTicks(ticks);
                return time;
            }
        }
    }
}