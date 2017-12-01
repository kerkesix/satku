var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var Ksx;
(function (Ksx) {
    var ReportViewModel = (function (_super) {
        __extends(ReportViewModel, _super);
        function ReportViewModel() {
            var _this = _super.call(this, "report") || this;
            _this.attendees = {};
            _this.dataLoaded = $.Deferred();
            var self = _this, route = {
                urlMap: ["/"],
                order: 0,
                text: "Etusivu",
                icon: "fa-bar-chart",
                active: true,
                root: _this.domroot,
                showOnNavigation: true,
                onViewOpen: self.onload.bind(_this)
            };
            Ksx.Routes.register(_this.domroot, route);
            var attendeesViewModel = new Ksx.Attendees.AttendeesViewModel(self.dataLoaded);
            Ksx.DataClient.status()
                .done(self.dataInit.bind(self))
                .fail(function () {
                Messaging.Queues.error.publish("Tietojen hakeminen epäonnistui.");
            });
            Messaging.Queues.eventsFromServer.subscribe(function (e) {
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
            Messaging.Queues.checkpointForScanRequest.subscribe(function (r) {
                var info = self.checkpoints().reduce(function (loop, c) {
                    var v = c.getAttendeeVisit(r.PersonId);
                    if (loop.visit && !loop.nextCheckpoint) {
                        loop.nextCheckpoint = c.id;
                    }
                    if (v != null) {
                        loop.visit = v;
                        loop.visitCheckpoint = c.id;
                        loop.nextCheckpoint = null;
                    }
                    return loop;
                }, {});
                if (!info.visit) {
                    var name = _this.attendees[r.PersonId];
                    Messaging.Queues.error.publish("Lukeman huoltopisteen päättely epäonnistui. Lukemaa ei voitu lähettää palvelimelle. "
                        + "Kokeile päivittää sivu ennen lukemisen jatkamista. Lukema: " + (name || r.PersonId));
                    return;
                }
                r.CheckpointId = !info.visit.dateTimeOut() ? info.visitCheckpoint : info.nextCheckpoint;
                Messaging.Queues.checkpointForScanResponse.publish(r);
            });
            return _this;
        }
        ReportViewModel.prototype.onload = function (context) {
            if (this.loaded()) {
                return;
            }
        };
        ReportViewModel.prototype.attendeeName = function (id) {
            return this.attendees[id] ? this.attendees[id].name : "";
        };
        ReportViewModel.prototype.applyScanFromServer = function (scan, scanType) {
            var cp = this.searchScanCheckpoint(scan.checkpointId);
            if (!cp) {
                return;
            }
            var personVisit = cp.getAttendeeVisit(scan.personId);
            if (personVisit) {
                if (scanType === "in") {
                    personVisit.dateTimeIn(Date.parse(scan.scanTimestamp));
                }
                if (scanType === "out") {
                    personVisit.dateTimeOut(Date.parse(scan.scanTimestamp));
                }
                if (scanType === "quit") {
                    var q = new Ksx.Quitter({
                        t: scan.timestamp,
                        d: scan.description,
                        wsl: scan.walkedSinceLast
                    }, scan.personId);
                    personVisit.quit(q);
                }
            }
            else {
                var visitInfo = {
                    a: scan.personId,
                    i: scanType === "out" ? null : scan.scanTimestamp,
                    o: scanType === "out" ? scan.scanTimestamp : null,
                    quit: null
                };
                if (scanType === "passthrough") {
                    visitInfo.i = scan.scanTimestamp;
                    visitInfo.o = scan.scanTimestamp;
                }
                personVisit = new Ksx.Visit(visitInfo, cp.id, cp.name, cp.distanceFromPrevious);
                cp.visits.push(personVisit);
                cp.waitingFor.remove(function (e) { return e.id === scan.PersonId; });
            }
        };
        ReportViewModel.prototype.applyRemoveScanFromServer = function (scan, scanType) {
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
            if (scanType === "in" || !personVisit.dateTimeIn()) {
                cp.visits.remove(personVisit);
            }
            else {
                if (scanType === "out") {
                    personVisit.dateTimeOut(0);
                }
            }
        };
        ReportViewModel.prototype.searchScanCheckpoint = function (checkpointId) {
            return this.checkpoints().reduce(function (a, b) {
                return (!a && b.id === checkpointId) ? b : a;
            }, null);
        };
        ReportViewModel.prototype.dataInit = function (reportData) {
            var self = this;
            var mapped = (reportData.checkpoints || [])
                .map(function (cpDto) { return new Ksx.Checkpoint(cpDto); });
            self.checkpoints = ko.observableArray(mapped);
            self.selectedCheckpoint = ko.observable(mapped[0]);
            var allVisits = mapped.reduce(function (all, c) { return all.concat(c.visits()); }, []);
            var lastCheckout = 0, lastAttendee;
            allVisits.sort(self.sortByAttendeeAndTimestamp).forEach(function (v) {
                if (lastCheckout && lastAttendee === v.id) {
                    v.previousVisitCheckoutTime(lastCheckout);
                }
                lastCheckout = v.dateTimeOut();
                lastAttendee = v.id;
            });
            if (reportData.attendees) {
                self.attendees = reportData.attendees.reduce(function (map, elem) {
                    var visits = mapped.reduce(function (collected, c) {
                        var found = c.getAttendeeVisit(elem.id);
                        if (found) {
                            collected.push(found);
                        }
                        return collected;
                    }, []);
                    map[elem.id] = new Ksx.Attendee(elem, visits);
                    return map;
                }, {});
                var attendeesReferenceData = reportData.attendees.map(function (a) { return ({ id: a.id, name: a.name }); });
                Messaging.Queues.referenceData.publish({ name: "attendees", data: attendeesReferenceData });
                var nfcToPerson = reportData.attendees.filter(function (a) { return a.nfc; })
                    .map(function (a) { return ({ nfc: a.nfc, id: a.id }); });
                Messaging.Queues.referenceData.publish({ name: "nfc", data: nfcToPerson });
            }
            var checkpointsReferenceData = reportData.checkpoints.map(function (c) { return ({ id: c.id, name: c.name }); });
            Messaging.Queues.referenceData.publish({ name: "checkpoints", data: checkpointsReferenceData });
            Messaging.Queues.referenceData.publish({
                name: "quitters",
                data: mapped.reduce(function (collected, c) {
                    return collected.concat(c.quitters());
                }, []).map(function (q) { return q.id; })
            });
            Messaging.Queues.referenceData.publish({
                name: "completed",
                data: mapped.length ? mapped[mapped.length - 1].visits().map(function (v) { return v.id; }) : []
            });
            self.dataLoaded.resolve(self.attendees);
            this.applyBindings();
        };
        ReportViewModel.prototype.sortByAttendeeAndTimestamp = function (a, b) {
            var compare = a.id.localeCompare(b.id);
            if (compare === 0) {
                return (a.dateTimeIn() < b.dateTimeIn()) ? -1 : 1;
            }
            return compare;
        };
        return ReportViewModel;
    }(Ksx.ViewModelBase));
    Ksx.ReportViewModel = ReportViewModel;
    ReportViewModel.instance = new ReportViewModel();
})(Ksx || (Ksx = {}));
//# sourceMappingURL=ReportViewModel.js.map