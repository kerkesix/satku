namespace KsxEventTracker.Domain.Messages.Events
{
    using System;

    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public abstract class ChangeScanTimeBase : PersonAtCheckpointBase, IEvent
    {
        public DateTimeOffset NewTime { get; set; }

        protected ChangeScanTimeBase()
        {
        }

        protected ChangeScanTimeBase(ScanInfo info, DateTimeOffset newTime)
            : base(info.HappeningId, info.CheckpointId, info.PersonId)
        {
            this.NewTime = newTime;
        }
    }
}