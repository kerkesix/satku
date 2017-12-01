namespace KsxEventTracker.Domain.Messages.Events
{
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class AttendeeScanOutRemoved : AttendeeScanOut, IEvent
    {
        public AttendeeScanOutRemoved()
        {
        }
        public AttendeeScanOutRemoved(ScanInfo info) : base(info)
        {
        }
    }
}