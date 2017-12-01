namespace KsxEventTracker.Domain.Messages.Events
{
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class CheckpointEventBase : EventBase, IEvent
    {
        public string HappeningId { get; set; }

        public int Order { get; set; }

        public CheckpointType CheckpointType { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public decimal DistanceFromPrevious { get; set; }

        public decimal DistanceFromStart { get; set; }
    }
}