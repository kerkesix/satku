namespace KsxEventTracker.Domain.Messages.Events
{
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class AttendeeScanOutPreceedsScanIn : AttendeeScanBase, IEvent
    {
        public AttendeeScanOutPreceedsScanIn()
        {
        }

        public AttendeeScanOutPreceedsScanIn(ScanInfo info): base(info)
        {
        }
    }
}