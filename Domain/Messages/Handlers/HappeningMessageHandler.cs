namespace KsxEventTracker.Domain.Messages.Handlers
{
    using System;
    using System.Threading.Tasks;
    using KsxEventTracker.Domain.Aggregates.Checkpoint;
    using KsxEventTracker.Domain.Aggregates.Happening;
    using KsxEventTracker.Domain.Messages.Commands;
    using KsxEventTracker.Domain.Messages.Events;
    using KsxEventTracker.Domain.Repositories;

    public class HappeningMessageHandler: ICommandHandler
    {
        public const string RootAggregateId = "root";

        private readonly AggregateRepository<HappeningsInventory, HappeningsInventoryState> happeningsRepository;

        private readonly AggregateRepository<Happening, HappeningState> repository;

        public HappeningMessageHandler(IEventPublisher eventPublisher, AzureTableStorageOptions options)
        {
            happeningsRepository = new AggregateRepository<HappeningsInventory, HappeningsInventoryState>(eventPublisher, options);
            repository = new AggregateRepository<Happening, HappeningState>(eventPublisher, options);
        }

        public bool TryExecute(ICommand command) => RedirectToWhen.InvokeCommandIfHandlerFound(this, command);
        public bool TryApply(IEvent e) => RedirectToWhen.InvokeEventIfHandlerFound(this, e);

        public Task Execute(CreateHappening c) =>
            // Do not use given happening id as aggregate id as this is the root list
            this.happeningsRepository.Execute(RootAggregateId, h => h.Create(c.Key, c.IsDefault));

        public Task Execute(AddCoordinatePath c) => this.repository.Execute(c.Key, h => h.SetCoordinates(c.Key, c.Path));

        public Task Execute(ChangeDefaultHappening c) =>
            // Do not use given happening id as aggregate id as this is the root list
            this.happeningsRepository.Execute(RootAggregateId, h => h.ChangeDefault(c.Key));

        public Task Execute(UnlinkPersonFromHappening c) =>
            // Validate first, PersonMessageHandler takes care of the next step
            happeningsRepository.Execute(
                    RootAggregateId,
                    h => h.ValidatePersonUnlink(c.HappeningId, c.PersonId));

        public Task Execute(StartScan c) => repository.Execute(c.HappeningId, h => h.ValidateStartScan(c.ToScanInfo()));

        public Task Execute(Scan c) => repository.Execute(c.HappeningId, h => h.ValidateScan(c.ToScanInfo()));

        public Task Execute(RemoveScan c) => repository.Execute(c.HappeningId,
                h => h.ValidateRemoveScan(
                    new ScanInfo(c.HappeningId, c.CheckpointId, c.PersonId, c.Id, c.Timestamp, c.Timestamp, c.Context.User)));

        public Task Execute(Quit c)
        {
            var quitTime = c.QuitTimestamp ?? c.Timestamp;
            var scanInfo = new ScanInfo(
                c.HappeningId, c.CheckpointId, c.PersonId, c.Id, c.Timestamp, quitTime, c.Context.User);

            return this.repository.Execute(c.HappeningId,
                h => h.Quit(
                    scanInfo,
                    c.WalkedSinceLast,
                    c.Description));
        }

        public Task Execute(RemoveQuit c)
        {
            var scanInfo = new ScanInfo(
                c.HappeningId, c.CheckpointId, c.PersonId, c.Id, c.Timestamp, c.Timestamp, c.Context.User);

            return this.repository.Execute(c.HappeningId,
                h => h.RemoveQuit(scanInfo));
        }

        public Task When(HappeningInventoryItemCreated e) => this.repository.CreateAndExecute(e.Id, h => h.Create(e.Id));

        public Task When(HappeningDeletionValidated e) =>
            // Do not use given happening id as aggregate id as this is the root list
            this.happeningsRepository.Execute(RootAggregateId, h => h.Delete(e.HappeningId));

        public Task When(FirstAttendeeForHappeningScanned e) => this.happeningsRepository.Execute(RootAggregateId, h => h.Start(e.HappeningId));

        public Task When(AttendeeScannedAtStartCheckpoint e) => this.repository.Execute(e.HappeningId, h => h.AttendeeStarted(e.HappeningId, e.PersonId));

        public Task When(AttendeeScannedAtFinishCheckpoint e) => this.repository.Execute(e.HappeningId, h => h.AttendeeCompleted(e.HappeningId, e.PersonId));
    }

    public static class ScanExtensions
    {
        public static ScanInfo ToScanInfo(this Scan c)
        {
            var scanTime = c.ScanTimestamp ?? c.Timestamp;
            var scanInfo = new ScanInfo(
                c.HappeningId,
                c.CheckpointId,
                c.PersonId,
                c.Id,
                c.Timestamp,
                scanTime,
                c.Context.User);
            return scanInfo;
        }
    }
}