namespace KsxEventTracker.Domain.Aggregates.Happening
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using KsxEventTracker.Domain.Messages.Events;

    public sealed class HappeningsInventoryState : AggregateRootStateBase
    {
        public List<HappeningInfo> Happenings { get; private set; }

        public HappeningsInventoryState()
        {
            this.Happenings = new List<HappeningInfo>();
        }

        public void When(HappeningInventoryItemCreated e)
        {
            if (e.IsDefault)
            {
                this.MarkAllAsNotDefault();
            }

            this.Happenings.Add(new HappeningInfo { Id = e.Id, IsDefault = e.IsDefault });
        }

        public void When(HappeningSetAsDefault e)
        {
            this.MarkAllAsNotDefault();
            this.Happenings.Find(m => m.Id == e.KeyOfNewDefault).IsDefault = true;
        }

        public void When(HappeningDeleted e)
        {
            this.Happenings.RemoveAll(m => m.Id.Equals(e.HappeningId, StringComparison.OrdinalIgnoreCase));
        }

        public void When(HappeningStarted e)
        {
            var h = this.Happenings.First(m => m.Id.Equals(e.HappeningId, StringComparison.OrdinalIgnoreCase));
            h.Started = true;
        }

        public void When(PersonUnlinkFailed e) { }
        public void When(PersonUnlinkValidated e) { }

        private void MarkAllAsNotDefault()
        {
            this.Happenings.ForEach(m => m.IsDefault = false);
        }

        public class HappeningInfo
        {
            public string Id { get; set; }
            public bool IsDefault { get; set; }
            public bool Started { get; set; }
        }
    }
}