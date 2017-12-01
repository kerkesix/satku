namespace KsxEventTracker.Domain.Messages.Events
{
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class RemoveScanValidated : EventBase, IEvent
    {
        public ScanInfo ScanInfo { get; set; }

        public RemoveScanValidated()
        {
        }

        public RemoveScanValidated(ScanInfo scanInfo)
        {
            this.ScanInfo = scanInfo;
        }
    }
}