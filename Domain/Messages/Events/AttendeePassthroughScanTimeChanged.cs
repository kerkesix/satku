namespace KsxEventTracker.Domain.Messages.Events
{
    using System;

    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class AttendeePassthroughScanTimeChanged : ChangeScanTimeBase
    {
        public AttendeePassthroughScanTimeChanged()
        {
        }

        public AttendeePassthroughScanTimeChanged(ScanInfo info, DateTimeOffset newTime)
            : base(info, newTime)
        {
        }
    }
}