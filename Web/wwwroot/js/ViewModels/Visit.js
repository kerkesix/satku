var Ksx;
(function (Ksx) {
    var Visit = (function () {
        function Visit(data, checkpointId, checkpointName, distanceFromPrevious) {
            var _this = this;
            this.checkpointId = checkpointId;
            this.checkpointName = checkpointName;
            this.distanceFromPrevious = distanceFromPrevious;
            this.quit = ko.observable();
            this.previousVisitCheckoutTime = ko.observable(0);
            this.newManualScanTime = ko.observable('');
            this.addQuitTime = ko.observable('');
            this.addQuitWalkedSinceLast = ko.observable('');
            this.addQuitDescription = ko.observable('');
            this.visitTimeToChange = ko.observable('');
            this.changeScanTimeDate = ko.observable('');
            this.changeScanTimeTime = ko.observable('');
            var self = this;
            self.id = data.a;
            if (data.quit) {
                self.quit(new Ksx.Quitter(data.quit, data.a));
            }
            self.dateTimeIn = ko.observable(data.i ? Date.parse(data.i) : 0);
            self.dateTimeOut = ko.observable(data.o ? Date.parse(data.o) : 0);
            self.visitTimeToChange.subscribe(function (v) {
                if (v === "In" || v === "Passthrough") {
                    _this.changeScanTimeDate(Ksx.Time.formatDate(_this.dateTimeIn()));
                    _this.changeScanTimeTime(Ksx.Time.formatTime(_this.dateTimeIn()));
                }
                if (v === "Out") {
                    _this.changeScanTimeDate(Ksx.Time.formatDate(_this.dateTimeOut()));
                    _this.changeScanTimeTime(Ksx.Time.formatTime(_this.dateTimeOut()));
                }
            });
            self.duration = ko.pureComputed(function () {
                if (_this.dateTimeIn() === 0 || _this.dateTimeOut() === 0) {
                    return 0;
                }
                return Math.abs(_this.dateTimeOut() - _this.dateTimeIn());
            });
            self.walkingTime = ko.pureComputed(function () {
                if (_this.previousVisitCheckoutTime() && _this.dateTimeIn()) {
                    return _this.dateTimeIn() - _this.previousVisitCheckoutTime();
                }
                return 0;
            });
            self.lastLegSpeed = ko.pureComputed(function () {
                var s = 0, d = _this.walkingTime();
                if (!d || _this.distanceFromPrevious === 0) {
                    return s;
                }
                s = _this.distanceFromPrevious / (d / 3600000);
                return Math.round(100 * s) / 100;
            });
        }
        Visit.prototype.createVisitCommand = function (name, properties) {
            var prop = properties || {};
            prop.PersonId = this.id;
            prop.CheckpointId = this.checkpointId;
            prop.HappeningId = Ksx.Routes.currentHappening;
            return Ksx.Bus.createCommand(name, prop);
        };
        Visit.prototype.closeModal = function () {
            $(".modal").modal('hide');
        };
        Visit.prototype.failOperation = function (text) {
            Messaging.Queues.error.publish(text);
            this.closeModal();
        };
        Visit.prototype.sendAndNotify = function (command, text) {
            Ksx.Bus.bus.send(command);
            Messaging.Queues.info.publish(text);
            this.closeModal();
        };
        Visit.prototype.addManualScanOutModalOpen = function () {
            this.newManualScanTime(Ksx.Time.formatTime(new Date()));
        };
        Visit.prototype.addManualScanOut = function () {
            var command = this.createVisitCommand("Scan", {
                ScanTimestamp: Ksx.Time.ToServerTimeFromInputDate(this.newManualScanTime())
            });
            this.sendAndNotify(command, "Tallennetaan lukemaa...");
        };
        Visit.prototype.changeScanTimeModalOpen = function () {
            var changeType = null;
            if (this.dateTimeIn() === this.dateTimeOut()) {
                changeType = "Passthrough";
            }
            else if (this.dateTimeIn() && !this.dateTimeOut()) {
                changeType = "In";
            }
            this.visitTimeToChange(changeType);
        };
        Visit.prototype.changeScanTime = function () {
            var command = this.createVisitCommand("Change" + this.visitTimeToChange() + "ScanTime", {
                NewTime: Ksx.Time.ToServerTimeFromInputDate(this.changeScanTimeDate(), this.changeScanTimeTime())
            });
            this.sendAndNotify(command, "Siirretään lukeman aikaa...");
        };
        Visit.prototype.addQuitModalOpen = function () {
            this.addQuitTime(Ksx.Time.formatTime(new Date()));
            this.addQuitWalkedSinceLast("0,0");
        };
        Visit.prototype.addQuit = function () {
            if (Ksx.ReferenceData.completedAttendees[this.id]) {
                this.failOperation("Henkilö on jo maalissa, keskeytystä ei voi enää lisätä.");
                return;
            }
            if (Ksx.ReferenceData.quitAttendees[this.id]) {
                this.failOperation("Henkilö on jo keskeyttänyt, keskeytystä ei voi enää lisätä.");
                return;
            }
            var command = this.createVisitCommand("Quit", {
                QuitTimestamp: Ksx.Time.ToServerTimeFromInputDate(this.addQuitTime()),
                WalkedSinceLast: parseFloat(this.addQuitWalkedSinceLast().replace(",", ".")),
                Description: this.addQuitDescription()
            });
            this.sendAndNotify(command, "Keskeytetään kävelyä...");
        };
        Visit.prototype.removeQuit = function () {
            if (Ksx.ReferenceData.completedAttendees[this.id]) {
                this.failOperation("Henkilö on jo maalissa, keskeytystä ei voida poistaa.");
                return;
            }
            var removeCommand = Ksx.Bus.createCommand("RemoveQuit", {
                CheckpointId: this.checkpointId,
                PersonId: this.id,
                HappeningId: Ksx.Routes.currentHappening,
            });
            this.sendAndNotify(removeCommand, "Poistetaan keskeytystä...");
        };
        Visit.prototype.removeScan = function () {
            if (Ksx.ReferenceData.completedAttendees[this.id]) {
                this.failOperation("Henkilö on jo maalissa, lukemia ei voi enää poistaa.");
                return;
            }
            if (Ksx.ReferenceData.quitAttendees[this.id]) {
                this.failOperation("Henkilö on jo keskeyttänyt, lukemia ei voi enää poistaa.");
                return;
            }
            this.sendAndNotify(this.createVisitCommand("RemoveScan"), "Poistetaan lukemaa...");
        };
        return Visit;
    }());
    Ksx.Visit = Visit;
})(Ksx || (Ksx = {}));
//# sourceMappingURL=Visit.js.map