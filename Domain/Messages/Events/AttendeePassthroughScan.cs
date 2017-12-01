namespace KsxEventTracker.Domain.Messages.Events
{
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class AttendeePassthroughScan : AttendeeScanBase, IEvent
    {
        public AttendeePassthroughScan()
        {
        }
        public AttendeePassthroughScan(ScanInfo info)
            : base(info)
        {
        }
    }
}