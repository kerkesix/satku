namespace KsxEventTracker.Domain.Messages.Events
{
    using System;

    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public abstract class AttendeeScanBase : PersonAtCheckpointBase, IEvent
    {
        /// <summary>
        /// Individual id that can be used to later on track and delete this scan.
        /// </summary>
        public string ScanId { get; set; }

        /// <summary>
        /// Might differ from event timestamp when the value is user-typed.
        /// </summary>
        public DateTimeOffset ScanTimestamp { get; set; }

        public string ReadBy { get; set; }

        protected AttendeeScanBase()
        {
        }

        protected AttendeeScanBase(ScanInfo info)
        {
            this.HappeningId = info.HappeningId;
            this.PersonId = info.PersonId;
            this.ScanId = info.ScanId;
            this.ReadBy = info.ReadBy;
            this.Timestamp = info.Timestamp;
            this.ScanTimestamp = info.ScanTimestamp;
            this.CheckpointId = info.CheckpointId;
        }
    }
}