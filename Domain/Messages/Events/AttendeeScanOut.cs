namespace KsxEventTracker.Domain.Messages.Events
{
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class AttendeeScanOut : AttendeeScanBase, IEvent
    {
        public AttendeeScanOut()
        {
        }
        public AttendeeScanOut(ScanInfo info): base(info)
        {
        }
    }
}