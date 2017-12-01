namespace KsxEventTracker.Domain.Messages.Events
{
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class AttendeeQuit: AttendeeScanBase, IEvent
    {
        public AttendeeQuit()
        {
        }
        public AttendeeQuit(
            ScanInfo info,
            decimal walkedSinceLast, 
            string description): base(info)
        {
            this.WalkedSinceLast = walkedSinceLast;
            this.Description = description;
        }

        public decimal WalkedSinceLast { get; set; }
        public string Description { get; set; }
    }
}