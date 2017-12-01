namespace KsxEventTracker.Domain.Messages.Events
{
    using System;

    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class AttendeeScanOutTimeChanged : ChangeScanTimeBase
    {
        public AttendeeScanOutTimeChanged()
        {
        }

        public AttendeeScanOutTimeChanged(ScanInfo info, DateTimeOffset newTime)
            : base(info, newTime)
        {
        }
    }
}