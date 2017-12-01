namespace KsxEventTracker.Domain.Aggregates.Happening
{
    using System;
    using System.Linq;

    using KsxEventTracker.Domain.Messages.Events;

    /// <summary>
    /// Ensures, that list of happenings has unique keys, and only one happening is default.
    /// </summary>
    public sealed class HappeningsInventory : AggregateRootBase<HappeningsInventoryState>
    {
        public HappeningsInventory(Action<IEvent> collectEvent, HappeningsInventoryState state)
            : base(collectEvent, state)
        {
        }

        public void Create(string id, bool setAsDefault)
        {
            if (this.State.Happenings.Any(m => m.Id.Equals(id, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Happening '{id}' exists already");
            }

            this.Apply(new HappeningInventoryItemCreated(id, setAsDefault));
        }

        public void ChangeDefault(string idOfNewDefault)
        {
            var h = this.GetHappening(idOfNewDefault);

            if (h != null && !h.IsDefault)
            {
                this.Apply(new HappeningSetAsDefault(Guid.NewGuid().ToString(), idOfNewDefault));
            }
        }

        public void Delete(string id)
        {
            var h = this.GetHappening(id);

            if (h == null)
            {
                return;
            }

            this.Apply(new HappeningDeleted(id));                

            // If deleted was the default, must change default to last happening before that
            if (h.IsDefault && this.State.Happenings.Any())
            {
                this.Apply(new HappeningSetAsDefault(Guid.NewGuid().ToString(), this.State.Happenings.Last().Id));
            }
        }

        public void Start(string happeningId)
        {
            var h = this.GetHappening(happeningId);

            if (h == null)
            {
                return;
            }

            this.Apply(new HappeningStarted(happeningId));
        }

        public void ValidatePersonUnlink(string happeningId, string personId)
        {
            var h = this.GetHappening(happeningId);

            if (h == null)
            {
                return;
            }

            if (h.Started)
            {
                this.Apply(new PersonUnlinkFailed(happeningId, personId, "Tapahtuma on jo alkanut, ei voida poistaa linkitystä."));
                return;
            }

            this.Apply(new PersonUnlinkValidated(happeningId, personId));
        }

        private HappeningsInventoryState.HappeningInfo GetHappening(string happeningId)
        {
            var h =
                this.State.Happenings.FirstOrDefault(
                    m => m.Id.Equals(happeningId, StringComparison.OrdinalIgnoreCase));
            return h;
        }
    }
}