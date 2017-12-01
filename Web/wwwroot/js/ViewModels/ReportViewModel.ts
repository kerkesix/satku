/// <reference path="../_references.ts" />
/// <reference path="../ViewModelBase.ts" />
/// <reference path="Dtos.ts" />
/// <reference path="../DataClient.ts" />
/// <reference path="Checkpoint.ts" />
/// <reference path="Attendee.ts" />
/// <reference path="Quitter.ts" />
/// <reference path="AttendeesViewModel.ts" />
module Ksx {
    declare var ksx: any;

    export class ReportViewModel extends ViewModelBase {
        static instance: ReportViewModel;

        checkpoints: KnockoutObservableArray<Checkpoint>;
        selectedCheckpoint: KnockoutObservable<Checkpoint>;

        attendees = {};

        dataLoaded: JQueryDeferred<any> = $.Deferred();

        constructor () {
            super("report");

            var self = this,
                route: IRoute = {
                    urlMap: ["/"],
                    order: 0,
                    text: "Etusivu",
                    icon: "fa-bar-chart",
                    active: true,
                    root: this.domroot,
                    showOnNavigation: true,
                    onViewOpen: self.onload.bind(this)
                };

            Ksx.Routes.register(this.domroot, route);

            // Hard reference to attendees view model, hack!
            var attendeesViewModel = new Ksx.Attendees.AttendeesViewModel(self.dataLoaded);

            // Load initial state from the server
            DataClient.status()
                .done(self.dataInit.bind(self))
                .fail(() => {
                    Messaging.Queues.error.publish("Tietojen hakeminen epäonnistui.");
                });

            Messaging.Queues.eventsFromServer.subscribe(e => {
                if (e.name === 'AttendeeScanIn') {
                    self.applyScanFromServer(e.data, "in");
                }

                if (e.name === 'AttendeeScanOut') {
                    self.applyScanFromServer(e.data, "out");
                }

                if (e.name === 'AttendeePassthroughScan') {
                    self.applyScanFromServer(e.data, "passthrough");
                }

                if (e.name === 'AttendeeScanInTimeChanged') {
                    e.data.ScanTimestamp = e.data.newTime;
                    self.applyScanFromServer(e.data, "in");
                }

                if (e.name === 'AttendeeScanOutTimeChanged') {
                    e.data.ScanTimestamp = e.data.newTime;
                    self.applyScanFromServer(e.data, "out");
                }

                if (e.name === 'AttendeePassthroughScanTimeChanged') {
                    e.data.ScanTimestamp = e.data.newTime;
                    self.applyScanFromServer(e.data, "passthrough");
                }

                if (e.name === 'AttendeeQuit') {
                    self.applyScanFromServer(e.data, "quit");
                }

                if (e.name === 'AttendeeScanInRemoved') {
                    self.applyRemoveScanFromServer(e.data, "in");
                }

                if (e.name === 'AttendeeScanOutRemoved') {
                    self.applyRemoveScanFromServer(e.data, "out");
                }

                if (e.name === 'AttendeeQuitRemoved') {
                    self.applyRemoveScanFromServer(e.data, "quit");
                }
            });

            Messaging.Queues.checkpointForScanRequest.subscribe(r => {
                var info = self.checkpoints().reduce(
                    // Inline type, could use interface 
                    (loop: { visit: Ksx.Visit; nextCheckpoint; visitCheckpoint; }, c) => {

                    var v: Ksx.Visit = c.getAttendeeVisit(r.PersonId);

                    if (loop.visit && !loop.nextCheckpoint) {
                        loop.nextCheckpoint = c.id;
                    }

                    if (v != null) {
                        loop.visit = v;
                        loop.visitCheckpoint = c.id;
                        loop.nextCheckpoint = null;
                    }

                    return loop;
                }, <any>{});

                // Cannot continue if checkpoint id is not found
                if (!info.visit) {
                    var name = this.attendees[r.PersonId];
                    Messaging.Queues.error.publish(
                        "Lukeman huoltopisteen päättely epäonnistui. Lukemaa ei voitu lähettää palvelimelle. "
                        + "Kokeile päivittää sivu ennen lukemisen jatkamista. Lukema: " + (name || r.PersonId));
                    return;
                }

                r.CheckpointId = !info.visit.dateTimeOut() ? info.visitCheckpoint : info.nextCheckpoint;

                // TODO: Use geolocation as backup (or as the only method, or as refining method to doublecheck)

                Messaging.Queues.checkpointForScanResponse.publish(r);
            });
        }

        public onload(context) : void {
            if (this.loaded()) {
                // Bindings applied already
                return;
            }

            // Do not apply bindings here, do it only after data has been loaded in dataInit method
            // this.applyBindings();
        }

        public attendeeName(id: string) :string {
            return this.attendees[id] ? this.attendees[id].name : "";
        }

        private applyScanFromServer(scan, scanType) {
            var cp = this.searchScanCheckpoint(scan.checkpointId);

            if (!cp) {
                return;
            }

            var personVisit: Visit = cp.getAttendeeVisit(scan.personId);

            if (personVisit) {
                if (scanType === "in") {
                    // This is a change of time for existing scan
                    personVisit.dateTimeIn(Date.parse(scan.scanTimestamp));
                }

                if (scanType === "out") {
                    // Either change, or completely new out scan
                    personVisit.dateTimeOut(Date.parse(scan.scanTimestamp));

                    // TODO: Add expected at to next checkpoint, find next checkpoint from this.checkpoints() with indexOf or similar
                }

                if (scanType === "quit") {
                    var q = new Quitter({
                        t: scan.timestamp,
                        d: scan.description,
                        wsl: scan.walkedSinceLast
                    }, scan.personId);
                    personVisit.quit(q);
                }
            }
            else {
                // This is an in-scan.
                // If this is the start scan, use out time instead of in.
                var visitInfo: IVisitDto = {
                    a: scan.personId,
                    i: scanType === "out" ? null : scan.scanTimestamp,
                    o: scanType === "out" ? scan.scanTimestamp : null,
                    quit: null
                };

                // Passthrough is a special case, both times are equal
                if (scanType === "passthrough") {
                    visitInfo.i = scan.scanTimestamp;
                    visitInfo.o = scan.scanTimestamp;
                }

                personVisit = new Visit(visitInfo, cp.id, cp.name, cp.distanceFromPrevious);
                cp.visits.push(personVisit);
                cp.waitingFor.remove(e => e.id === scan.personId);
            }
        }

        private applyRemoveScanFromServer(scan, scanType) {
            var cp = this.searchScanCheckpoint(scan.checkpointId);

            if (!cp) {
                return;
            }

            var personVisit = cp.getAttendeeVisit(scan.personId);

            if (!personVisit) {
                return;
            }

            if (scanType === "quit") {
                personVisit.quit(null);
                return;
            }

            // Start readings are "out" but they do not have in reading --> remove those completely
            if (scanType === "in" || !personVisit.dateTimeIn()) {
                cp.visits.remove(personVisit);
            }
            else {
                if (scanType === "out") {
                    personVisit.dateTimeOut(0);
                }
            }
        }

        private searchScanCheckpoint(checkpointId): Checkpoint {
            return this.checkpoints().reduce((a, b) => {
                return (!a && b.id === checkpointId) ? b : a;
            }, null);
        }

        private dataInit(reportData) {
            var self = this;

            var mapped: Checkpoint[] = (reportData.checkpoints || [])
                .map((cpDto: ICheckpointDto) => new Checkpoint(cpDto));

            self.checkpoints = ko.observableArray<Checkpoint>(mapped);
            self.selectedCheckpoint = ko.observable<Checkpoint>(mapped[0]);

            // To enable walking time calculation, give next visit 
            // previous visit's checkout time.
            // 1. Start by collecting all visits.
            var allVisits = mapped.reduce((all, c: ICheckpoint) => all.concat(c.visits()), []);

            // 2. Sort by first by attendee id and then by timestamp
            var lastCheckout = 0,
                lastAttendee;
            allVisits.sort(self.sortByAttendeeAndTimestamp).forEach(v => {
                if (lastCheckout && lastAttendee === v.id) {
                    v.previousVisitCheckoutTime(lastCheckout);
                }

                lastCheckout = v.dateTimeOut();
                lastAttendee = v.id;
            });

            // Collect all attendees and create a dictionary out of them. This is 
            // used to decrease the amount of data transferred. 
            if (reportData.attendees) {
                self.attendees = reportData.attendees.reduce((map, elem: IAttendeeDto) => {
                    // Find all checkpoint visits for this attendee (not efficient but works)
                    var visits = mapped.reduce((collected, c: ICheckpoint) => {
                        var found = c.getAttendeeVisit(elem.id);

                        if (found) {
                            collected.push(found);
                        }

                        return collected;
                    }, []);

                    // Map attendee index to actual id and name
                    map[elem.id] = new Attendee(elem, visits);
                    return map;
                }, {});

                // Emit attendees as reference data to other interested parties
                var attendeesReferenceData = reportData.attendees.map(a => ({ id: a.id, name: a.name }));
                Messaging.Queues.referenceData.publish({ name: "attendees", data: attendeesReferenceData });

                // Emit NFC code --> person mapping reference data 
                var nfcToPerson = reportData.attendees.filter(a => a.nfc)
                    .map(a => ({ nfc: a.nfc, id: a.id }));
                Messaging.Queues.referenceData.publish({ name: "nfc", data: nfcToPerson });
            }

            // Emit checkpoints as refererence data
            var checkpointsReferenceData = reportData.checkpoints.map(c => ({ id: c.id, name: c.name }));
            Messaging.Queues.referenceData.publish({ name: "checkpoints", data: checkpointsReferenceData });

            // Emit quitters as reference data
            Messaging.Queues.referenceData.publish({
                name: "quitters",
                data: mapped.reduce((collected, c: ICheckpoint) => {
                    return collected.concat(c.quitters());
                }, []).map(q => q.id )
            });

            // Emit completed as reference data
            Messaging.Queues.referenceData.publish({
                name: "completed",
                data: mapped.length ? mapped[mapped.length - 1].visits().map(v => v.id) : []
            });

            // Give attendee data to attendees view model
            // TODO: Works only once, use callbacks instead for data refresh?
            self.dataLoaded.resolve(self.attendees);

            this.applyBindings();
        }  

        private sortByAttendeeAndTimestamp(a, b) {
            var compare = a.id.localeCompare(b.id);

            if (compare === 0) {
                // Sort by second factor: the timestamp (should never be the same, 
                // therefore ignoring the normal compare function 0 result)
                return (a.dateTimeIn() < b.dateTimeIn()) ? -1 : 1;
            }

            return compare;
        }
    }

    // Expose model to outside world for debugging purposes
    ReportViewModel.instance = new ReportViewModel();
}
