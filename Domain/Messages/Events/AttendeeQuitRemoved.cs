namespace KsxEventTracker.Domain.Messages.Events
{
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class AttendeeQuitRemoved : AttendeeScanBase, IEvent
    {
        public AttendeeQuitRemoved()
        {
        }

        public AttendeeQuitRemoved(ScanInfo info): base(info)
        {            
        }
    }
}