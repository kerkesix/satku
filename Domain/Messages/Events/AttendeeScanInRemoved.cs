namespace KsxEventTracker.Domain.Messages.Events
{
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class AttendeeScanInRemoved : AttendeeScanIn, IEvent
    {
        public AttendeeScanInRemoved()
        {
        }
        public AttendeeScanInRemoved(ScanInfo info) : base(info)
        {
        }
    }
}