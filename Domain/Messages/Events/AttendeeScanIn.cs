namespace KsxEventTracker.Domain.Messages.Events
{
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class AttendeeScanIn : AttendeeScanBase, IEvent
    {
        public AttendeeScanIn()
        {
        }
        public AttendeeScanIn(ScanInfo info) : base(info)
        {
        }
    }
}