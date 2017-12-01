namespace KsxEventTracker.Domain.Messages.Events
{
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class CheckpointCreated : CheckpointEventBase
    {
        public CheckpointCreated(
            string happeningId, 
            string id, 
            CheckpointType checkpointType, 
            int order, 
            string name, 
            double latitude, 
            double longitude, 
            decimal distanceFromPrevious, 
            decimal distanceFromStart)
        {
            this.HappeningId = happeningId;
            this.Id = id;
            this.Order = order;
            this.CheckpointType = checkpointType;
            this.Name = name;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.DistanceFromPrevious = distanceFromPrevious;
            this.DistanceFromStart = distanceFromStart;
        }
    }
}