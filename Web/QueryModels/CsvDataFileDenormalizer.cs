namespace Web.QueryModels
{
    using System;
    using System.Linq;

    using KsxEventTracker.Domain.Messages;
    using KsxEventTracker.Domain.Messages.Events;
    using KsxEventTracker.Domain.Messages.Handlers;
    using Microsoft.Extensions.Logging;

    public class CsvDataFileDenormalizer : IDenormalizer
    {
        private readonly ILoggerFactory loggerFactory;

        public CsvDataFileDenormalizer(ILoggerFactory loggerFactory) {
            this.loggerFactory = loggerFactory;
        }
        
        public bool TryApply(IEvent @event)
        {
            return RedirectToWhen.InvokeEventIfHandlerFound(this, @event);
        }

        #region Happening
        
        public void When(HappeningCreated e)
        {
            QueryModelRepository.Csv.Add(e.HappeningId, new CsvDataFile());;
        }

        public void When(HappeningDeleted e)
        {
            QueryModelRepository.Csv.Remove(e.HappeningId);
        }

        #endregion

        #region Checkpoint
        public void When(CheckpointCreated e)
        {
            QueryModelRepository.Csv.Checkpoints.Add(
                e.Id,
                new CsvCheckpoint { Name = e.Name, DistanceFromPrevious = e.DistanceFromPrevious });
        }

        public void When(CheckpointDeleted e)
        {
            QueryModelRepository.Csv.Checkpoints.Remove(e.Id);
        }
        #endregion

        #region Person
        public void When(PersonCreated e)
        {
            QueryModelRepository.Csv.People.HandleCreate(e);
        }

        public void When(PersonContactInformationUpdated e)
        {
            QueryModelRepository.Csv.People.HandleUpdate(e);
        }

        public void When(PersonDeleted e)
        {
            QueryModelRepository.Csv.People.HandleDelete(e);
        }

        #endregion

        #region Attendee scans
        public void When(AttendeeScanIn e)
        {
            this.AddScanEvent(e, "Sisään");
        }

        public void When(AttendeeScanOut e)
        {
            this.AddScanEvent(e, "Ulos");
        }

        public void When(AttendeeQuit e)
        {
            this.AddScanEvent(e, "Keskeytys");
        }

        public void When(AttendeeScanInRemoved e)
        {
            RemoveScanEvent(e);
        }

        public void When(AttendeeScanOutRemoved e)
        {
            RemoveScanEvent(e);
        }

        private void AddScanEvent(
            AttendeeScanBase e,            
            string typeText)
        {
            var h = QueryModelRepository.Csv[e.HappeningId];
            var person =
                QueryModelRepository.Csv.People.First(
                    m => m.PersonId.Equals(e.PersonId, StringComparison.OrdinalIgnoreCase));

            h.ScanRows.Add(e.ScanId, new CsvScanRow
                                 {
                                    HappeningId = e.HappeningId,
                                    CheckpointId = e.CheckpointId,
                                    CheckpointName = QueryModelRepository.Csv.Checkpoints[e.CheckpointId].Name,
                                    PersonId = e.PersonId,
                                    PersonName = person.Lastname + " " + person.Firstname,
                                    Timestamp = e.Timestamp,
                                    Text = typeText
                                 });
        }

        private static void RemoveScanEvent(AttendeeScanBase e)
        {
            var h = QueryModelRepository.Csv[e.HappeningId];
            h.ScanRows.Remove(e.ScanId);
        }

        #endregion
    }
}