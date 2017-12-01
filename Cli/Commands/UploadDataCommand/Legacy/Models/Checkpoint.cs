namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy.Models
{
    using System;
    using System.Collections.Generic;

    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    /// <summary>
    /// One point where attendees are tracked.
    /// </summary>
    public class Checkpoint
    {
        public Checkpoint()
        {
            this.Readings = new List<Reading>();
            this.Quitters = new List<Quitter>();
        }

        public Guid Id { get; set; }

        public string HappeningId { get; set; }

        public CheckpointType CheckpointType { get; set; }

        public decimal DistanceFromPreviousCheckpoint { get; set; }

        public decimal DistanceFromStart { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public SimpleGeography Location { get; set; }

        public virtual ICollection<Quitter> Quitters { get; set; }

        public virtual ICollection<Reading> Readings { get; set; }
    }
}