var Ksx;
(function (Ksx) {
    var Attendee = (function () {
        function Attendee(attendee, visits) {
            var _this = this;
            var self = this, quitVisit;
            self.id = attendee.id;
            self.nfc = attendee.nfc;
            self.name = attendee.name;
            self.phone = attendee.phone || "";
            self.visits = ko.observableArray(visits || []);
            self.destructionPercent = ko.observable(attendee.dstr || 0);
            self.gravatarHash = ko.observable(attendee.hash);
            quitVisit = self.visits().reduce(function (found, v) {
                return v.quit() ? v : found;
            }, null);
            self.speeds = ko.pureComputed(function () { return _this.visits().map(function (v) { return v.lastLegSpeed(); }); });
            self.restTimes = ko.pureComputed(function () { return _this.visits().map(function (v) { return v.duration(); }); });
            self.totalRestTime = ko.pureComputed(function () { return _this.restTimes().reduce(self.sum, 0); });
            self.totalWalkingTime = ko.computed(function () {
                var legs = _this.visits().map(function (v) { return v.walkingTime(); }).reduce(self.sum, 0);
                if (quitVisit && quitVisit.quit().walkedSinceLast) {
                    var lastVisit = _this.visits()[_this.visits().length - 1];
                    legs += quitVisit.quit().time - lastVisit.dateTimeOut();
                }
                return legs;
            });
            self.totalTime = ko.pureComputed(function () { return self.totalRestTime() + self.totalWalkingTime(); });
            self.walkedKilometres = ko.computed(function () {
                var legs = _this.visits().map(function (v) { return v.distanceFromPrevious; }).reduce(self.sum, 0);
                if (quitVisit && quitVisit.quit().walkedSinceLast) {
                    legs += quitVisit.quit().walkedSinceLast;
                }
                if (legs > 100) {
                    legs = 100;
                }
                return legs;
            });
            self.hasQuit = !!quitVisit;
            self.quitText = '';
            if (self.hasQuit) {
                var q = quitVisit.quit();
                self.quitText = Ksx.Time.formatDateTime(q.time);
                if (q.walkedSinceLast) {
                    self.quitText += " " + q.walkedSinceLast + " km huoltopisteen " + quitVisit.checkpointName + " jälkeen.";
                }
                else {
                    self.quitText += " huoltopisteellä " + quitVisit.checkpointName + ".";
                }
            }
            self.avgSpeed = ko.pureComputed(function () {
                var total = _this.speeds().reduce(self.sum, 0);
                return (total / _this.speeds().filter(function (a) { return a > 0; }).length);
            });
            self.filtered = ko.observable(false);
            self.url = Ksx.Routes.url("attendees", "", attendee.id);
        }
        Attendee.prototype.sum = function (prev, next) {
            return (prev || 0) + (next || 0);
        };
        return Attendee;
    }());
    Ksx.Attendee = Attendee;
})(Ksx || (Ksx = {}));
//# sourceMappingURL=Attendee.js.map