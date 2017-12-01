namespace KsxEventTracker.Domain.Messages.Events
{
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class AttendeeDoubleScan : AttendeeScanBase, IEvent
    {
        public double DifferenceSinceLastRead { get; set; }
        public string Message { get; set; }

        public AttendeeDoubleScan()
        {
        }

        public AttendeeDoubleScan(ScanInfo scanInfo) : base(scanInfo)
        {
        }
    }
}