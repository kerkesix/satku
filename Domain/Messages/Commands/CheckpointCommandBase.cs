namespace KsxEventTracker.Domain.Messages.Commands
{
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class CheckpointCommandBase : CommandBase
    {
        protected CheckpointCommandBase()
        {
        }

        public int Order { get; set; }

        public CheckpointType CheckpointType { get; set; }

        public string CheckpointName { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public decimal DistanceFromPrevious { get; set; }

        public decimal DistanceFromStart { get; set; }
    }
}