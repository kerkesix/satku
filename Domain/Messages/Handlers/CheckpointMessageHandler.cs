namespace KsxEventTracker.Domain.Messages.Handlers
{
    using System.Threading.Tasks;

    using KsxEventTracker.Domain.Aggregates.Checkpoint;
    using KsxEventTracker.Domain.Messages.Commands;
    using KsxEventTracker.Domain.Messages.Events;
    using KsxEventTracker.Domain.Repositories;

    public class CheckpointMessageHandler: ICommandHandler 
    {
        private readonly AggregateRepository<CheckpointList, CheckpointListState> checkpointTypesRepository;
        private readonly AggregateRepository<Checkpoint, CheckpointState> checkpointRepository;

        public CheckpointMessageHandler(IEventPublisher eventPublisher, AzureTableStorageOptions options)
        { 
            checkpointTypesRepository = new AggregateRepository<CheckpointList, CheckpointListState>(eventPublisher, options);
            checkpointRepository = new AggregateRepository<Checkpoint, CheckpointState>(eventPublisher, options);
        }
        
        public bool TryExecute(ICommand command) => RedirectToWhen.InvokeCommandIfHandlerFound(this, command);
        public bool TryApply(IEvent e) => RedirectToWhen.InvokeEventIfHandlerFound(this, e);

        public Task Execute(CreateCheckpoint c) => this.checkpointTypesRepository.Execute(c.HappeningId, checkpoint => checkpoint.AddCheckpoint(
                                                                 c.HappeningId,
                                                                 c.Id,
                                                                 c.CheckpointType,
                                                                 c.Order,
                                                                 c.CheckpointName,
                                                                 c.Latitude,
                                                                 c.Longitude,
                                                                 c.DistanceFromPrevious,
                                                                 c.DistanceFromStart));

        public Task Execute(DeleteCheckpoint c) => this.checkpointTypesRepository.Execute(c.HappeningId, list => list.Delete(c.HappeningId, c.Id));

        public Task Execute(DeleteHappening c) => this.checkpointTypesRepository.Execute(c.Key, checkpoint => checkpoint.DeleteHappening(
                                                                c.Key));

        public Task Execute(ChangeInScanTime c) => this.ExecuteScanTimeChange(c, ScanTimeType.In);
        public Task Execute(ChangeOutScanTime c) => this.ExecuteScanTimeChange(c, ScanTimeType.Out);
        public Task Execute(ChangePasshtroughScanTime c) => this.ExecuteScanTimeChange(c, ScanTimeType.Passthrough);

        public Task When(HappeningCreated e) =>
            // Must create new, empty list of checkpoints for the new happening
            this.checkpointTypesRepository.CreateAndExecute(e.Id, checkpoint => checkpoint.Create(e.Id));

        public Task When(CheckpointValidated e) => this.checkpointRepository.CreateAndExecute(e.Id, checkpoint => checkpoint.Create(
                                                                 e.HappeningId,
                                                                 e.Id,
                                                                 e.CheckpointType,
                                                                 e.Order,
                                                                 e.Name,
                                                                 e.Latitude,
                                                                 e.Longitude,
                                                                 e.DistanceFromPrevious,
                                                                 e.DistanceFromStart));

        public Task When(CheckpointDeleted e) => Task.CompletedTask;

        public Task When(ScanValidated e) => this.checkpointRepository.Execute(e.ScanInfo.CheckpointId,
                checkpoint => checkpoint.AddScan(e.ScanInfo));

        public Task When(RemoveScanValidated e) => this.checkpointRepository.Execute(e.ScanInfo.CheckpointId,
                checkpoint => checkpoint.RemoveScan(e.ScanInfo));

        private Task ExecuteScanTimeChange(ChangeScanTimeCommandBase c, ScanTimeType timeType)
        {
            var scanInfo = new ScanInfo(
                c.HappeningId, c.CheckpointId, c.PersonId, c.Id, c.Timestamp, c.NewTime, c.Context.User);

            return this.checkpointRepository.Execute(
                    c.CheckpointId, checkpoint => checkpoint.ChangeScanTime(scanInfo, timeType));
        }
    }
}