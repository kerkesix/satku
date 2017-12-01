namespace KsxEventTracker.Domain.Messages.Events
{
    using KsxEventTracker.Domain.Aggregates.Checkpoint;

    public class CheckpointValidated : CheckpointEventBase
    {
        public CheckpointValidated(
            string happeningId, 
            string id, 
            int order, 
            CheckpointType checkpointType, 
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