namespace KsxEventTracker.Domain.Aggregates.Happening
{
    using System.Collections.Generic;

    using KsxEventTracker.Domain.Messages.Events;

    public sealed class HappeningState : AggregateRootStateBase
    {
        public string HappeningId { get; private set; }

        public string CoordinatePath { get; private set; }
        public HashSet<string> Started { get; private set; }
        public HashSet<string> Completed { get; private set; }
        public HashSet<string> Quitters { get; private set; }

        public HappeningState()
        {
            this.Started = new HashSet<string>();
            this.Completed = new HashSet<string>();
            this.Quitters = new HashSet<string>();
        }

        public void When(HappeningCreated e)
        {
            this.HappeningId = e.HappeningId;
        }

        public void When(CoordinatePathSet e)
        {
            // Path is GZipped due to large size.
            this.CoordinatePath = e.Path.FromGzip();
        }

        public void When(AttendeeStarted e)
        {
            this.Started.Add(e.PersonId);
        }

        public void When(AttendeeCompleted e)
        {
            this.Completed.Add(e.PersonId);
        }

        public void When(AttendeeQuit e)
        {
            this.Quitters.Add(e.PersonId);
        }

        public void When(AttendeeQuitRemoved e)
        {
            this.Quitters.Remove(e.PersonId);
        }

        public void When(ScanValidated e) { }
        public void When(RemoveScanValidated e) { }
    }
}