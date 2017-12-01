namespace KsxEventTracker.Domain.Messages.Events
{
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class ScanValidated : EventBase, IEvent
    {
        public ScanInfo ScanInfo { get; set; }

        public ScanValidated()
        {
        }

        public ScanValidated(ScanInfo scanInfo)
        {
            this.ScanInfo = scanInfo;
        }
    }
}