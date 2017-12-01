namespace KsxEventTracker.Domain.Messages.Events
{
    using System;

    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class AttendeeScanInTimeChanged : ChangeScanTimeBase
    {
        public AttendeeScanInTimeChanged()
        {
        }

        public AttendeeScanInTimeChanged(ScanInfo info, DateTimeOffset newTime)
            : base(info, newTime)
        {
        }
    }
}