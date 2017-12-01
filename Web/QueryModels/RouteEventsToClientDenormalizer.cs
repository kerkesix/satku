namespace Web.QueryModels
{
    using KsxEventTracker.Domain.Messages;
    using KsxEventTracker.Domain.Messages.Events;
    using KsxEventTracker.Domain.Messages.Handlers;
    using Microsoft.Extensions.Logging;
    using Web.Hubs;

    public class RouteEventsToClientDenormalizer : IDenormalizer, ISkipReplay
    {
        private readonly ILogger logger;

        public RouteEventsToClientDenormalizer(ILoggerFactory loggerFactory) 
        {
            this.logger = loggerFactory.CreateLogger("RouteEventsToClientDenormalizer");
        }

        public bool TryApply(IEvent @event)
        {
            this.logger.LogInformation("Routing event {0}", @event);
            return RedirectToWhen.InvokeEventIfHandlerFound(this, @event);
        }

        public void When(HappeningSetAsDefault e) => PublishToClients(e);
        public void When(HappeningCreated e) => PublishToClients(e);
        public void When(CoordinatePathSet e) => PublishToClients(e);
        public void When(HappeningDeleted e) => PublishToClients(e);
        public void When(PersonCreated e) => PublishToContributorClients(e);
        public void When(PersonContactInformationUpdated e) => PublishToContributorClients(e);
        public void When(PersonLinkedToHappening e) => PublishToClients(e);
        public void When(PersonUnlinkedFromHappening e) => PublishToContributorClients(e);
        public void When(PersonUnlinkFailed e) => PublishToContributorClients(e);
        public void When(AttendeeScanIn e) => PublishToClients(e);
        public void When(AttendeeScanOut e) => PublishToClients(e);
        public void When(AttendeePassthroughScan e) => PublishToClients(e);
        public void When(AttendeeScannedAtFinishCheckpoint e) => PublishToClients(e);
        public void When(AttendeeScanInTimeChanged e) => PublishToClients(e);
        public void When(AttendeeScanOutTimeChanged e) => PublishToClients(e);
        public void When(AttendeePassthroughScanTimeChanged e) => PublishToClients(e);
        public void When(AttendeeDoubleScan e) => PublishToContributorClients(e);
        public void When(AttendeeScanOutPreceedsScanIn e) => PublishToContributorClients(e);
        public void When(AttendeeQuit e) => PublishToClients(e);
        public void When(AttendeeQuitRemoved e) => PublishToClients(e);
        public void When(AttendeeScanInRemoved e) => PublishToClients(e);
        public void When(AttendeeScanOutRemoved e) => PublishToClients(e);

        private static void PublishToClients(IEvent e)
        {
            var clientEvent = new ClientEvent(e);
            Startup.CommandBus.Clients.All
                .InvokeAsync("publishClientEvent", new object[] { clientEvent })
                .Wait();
        }

        private static void PublishToContributorClients(IEvent e)
        {
            var clientEvent = new ClientEvent(e);
            Startup.CommandBus.Clients.Group(CommandBus.AuthenticatedGroupName)
                .InvokeAsync("publishClientEvent", new object[] { clientEvent })
                .Wait();
        }
    }

    public class ClientEvent
    {
        public string Name { get; set; }
        public object Data { get; set; }

        public ClientEvent(string name, object data)
        {
            this.Name = name;
            this.Data = data;
        }

        public ClientEvent(IEvent data) : this(data.GetType().Name, data)
        {
        }
    }
}