namespace KsxEventTracker.Domain.Aggregates.Checkpoint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using KsxEventTracker.Domain.Messages.Events;

    public class CheckpointListState : AggregateRootStateBase
    {
        public string HappeningId { get; set; }

        public List<Checkpoint> Checkpoints { get; set; }

        public CheckpointListState()
        {
            this.Checkpoints = new List<Checkpoint>();
        }

        public void When(CheckpointValidationListCreated e)
        {
            this.HappeningId = e.Id;
        }

        public void When(CheckpointValidated e)
        {
            this.Checkpoints.Add(
                new Checkpoint { Id = e.Id, CheckpointType =  e.CheckpointType });
        }

        public void When(CheckpointTypeChangeValidated e)
        {
            this.Checkpoints.First(m => m.Id == e.Id)
                .CheckpointType = e.NewType;
        }

        public void When(CheckpointDeleted e)
        {
            this.Checkpoints.RemoveAll(m => m.Id.Equals(e.Id, StringComparison.OrdinalIgnoreCase));
        }

        public void When(HappeningDeletionValidated e)
        {
            // Do nothing in purpose, this was a pure validation event that does not affect state
        }

        public class Checkpoint
        {
            public string Id { get; set; }
            public CheckpointType CheckpointType { get; set; }
        }
    }
}