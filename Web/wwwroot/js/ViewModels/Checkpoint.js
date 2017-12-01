var Ksx;
(function (Ksx) {
    var Checkpoint = (function () {
        function Checkpoint(data) {
            var _this = this;
            this.newManualScanDate = ko.observable();
            this.newManualScanTime = ko.observable();
            this.selectedNewScanAttendee = ko.observable();
            this.visitAvg = function (lambda) {
                var mapped = _this.visits().map(lambda), nonZeroMapped = mapped.filter(function (v) { return v > 0; }), total = nonZeroMapped.reduce(_this.sum, 0);
                return total / nonZeroMapped.length;
            };
            var self = this;
            self.id = data.id;
            self.name = data.name;
            self.distanceFromStart = data.distanceFromStart;
            self.distanceFromPrevious = data.distanceFromPrevious;
            self.visits = ko.observableArray(self.mapVisits(data.visits, data.id, data.name));
            self.quitters = ko.pureComputed(self.selectQuitters, self);
            self.waitingFor = ko.observableArray(self.mapWaitingFor(data.waitingFor));
            self.avgSpeed = ko.pureComputed(function () { return self.visitAvg(function (v) { return v.lastLegSpeed(); }); });
            self.avgTime = ko.pureComputed(function () { return self.visitAvg(function (v) { return v.duration(); }); });
            self.inside = ko.pureComputed(function () {
                var inside = _this.visits().filter(function (elem) { return (!elem.dateTimeOut() && !elem.quit()); }).length;
                return (_this.distanceFromStart == 100) ? 0 : inside;
            });
            self.status = ko.pureComputed(function () {
                var visits = _this.visits().length, quits = _this.quitters().length, waitingFor = _this.waitingFor().length, inside = _this.inside();
                return visits + "/" + (visits + waitingFor)
                    + (quits ? " kesk. " + quits : "")
                    + (inside ? " huoltaa " + inside : "");
            });
            self.latitude = data.latitude;
            self.longitude = data.longitude;
            self.checkpointType = data.checkpointType;
        }
        Checkpoint.prototype.getAttendeeVisit = function (id) {
            var found;
            this.visits().some(function (visit) {
                if (visit.id === id) {
                    found = visit;
                    return true;
                }
            });
            return found;
        };
        Checkpoint.prototype.mapVisits = function (dtos, checkpointId, checkpointName) {
            var _this = this;
            return (dtos || []).map(function (cpDto) {
                return new Ksx.Visit(cpDto, checkpointId, checkpointName, _this.distanceFromPrevious);
            });
        };
        Checkpoint.prototype.selectQuitters = function () {
            return this.visits().filter(function (v) { return v.quit(); }).map(function (v) { return v.quit(); });
        };
        Checkpoint.prototype.mapWaitingFor = function (dtos) {
            var mapped = [];
            for (var p in dtos) {
                if (dtos.hasOwnProperty(p)) {
                    mapped.push(new Ksx.Expected({ id: p, t: dtos[p] }));
                }
            }
            return mapped;
        };
        Checkpoint.prototype.sum = function (prev, next) {
            return (prev || 0) + (next || 0);
        };
        Checkpoint.prototype.addManualScanInModalOpen = function () {
            var d = new Date();
            this.newManualScanDate(Ksx.Time.formatDate(d));
            this.newManualScanTime(Ksx.Time.formatTime(d));
        };
        Checkpoint.prototype.addManualScanIn = function () {
            var command = Ksx.Bus.createCommand("Scan", {
                PersonId: this.selectedNewScanAttendee().id,
                CheckpointId: this.id,
                HappeningId: Ksx.Routes.currentHappening,
                ScanTimestamp: Ksx.Time.ToServerTimeFromInputDate(this.newManualScanDate(), this.newManualScanTime())
            });
            Ksx.Bus.bus.send(command);
            Messaging.Queues.info.publish("Tallennetaan lukemaa...");
            $(".modal").modal('hide');
        };
        return Checkpoint;
    }());
    Ksx.Checkpoint = Checkpoint;
})(Ksx || (Ksx = {}));
//# sourceMappingURL=Checkpoint.js.map