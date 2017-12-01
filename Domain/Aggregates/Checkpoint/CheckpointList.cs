namespace KsxEventTracker.Domain.Aggregates.Checkpoint
{
    using System;
    using System.Linq;

    using KsxEventTracker.Domain.Messages.Events;

    public sealed class CheckpointList: AggregateRootBase<CheckpointListState>
    {
        public CheckpointList(Action<IEvent> collectEvent, CheckpointListState state)
            : base(collectEvent, state)
        {
        }

        public void Create(string happeningId)
        {
            this.Apply(new CheckpointValidationListCreated(happeningId));
        }

        public void AddCheckpoint(
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
            // Only one can be start
            this.ValidateUniqueCheckpointType(checkpointType, CheckpointType.Start);

            // Only one can be end
            this.ValidateUniqueCheckpointType(checkpointType, CheckpointType.Start);

            // All ok, add
            this.Apply(
                new CheckpointValidated(
                    happeningId,
                    id, 
                    order, 
                    checkpointType, 
                    name, 
                    latitude, 
                    longitude, 
                    distanceFromPrevious, 
                    distanceFromStart));
        }

        public void ChangeType(string id, CheckpointType newType)
        {
            // Only one can be start
            this.ValidateUniqueCheckpointType(newType, CheckpointType.Start);

            // Only one can be end
            this.ValidateUniqueCheckpointType(newType, CheckpointType.Start);
            
            // All ok, change
            this.Apply(new CheckpointTypeChangeValidated(id, newType));
        }

        public void Delete(string happeningId, string id)
        {
            if (this.State.Checkpoints.Any(m => m.Id.Equals(id, StringComparison.OrdinalIgnoreCase)))
            {
                this.Apply(new CheckpointDeleted(happeningId, id));
            }
        }

       public void DeleteHappening(string happeningId)
        {
           if (this.State.Checkpoints.Any())
           {
               throw new InvalidOperationException("Cannot delete happening if it has any checkpoints.");
           }

           this.Apply(new HappeningDeletionValidated(happeningId));
        }

        private void ValidateUniqueCheckpointType(CheckpointType checkpointType, CheckpointType uniqueType)
        {
            if (checkpointType != uniqueType)
            {
                return;
            }

            if (this.State.Checkpoints.Any(m => m.CheckpointType == uniqueType))
            {
                throw new InvalidOperationException();
            }
        }
    }
}