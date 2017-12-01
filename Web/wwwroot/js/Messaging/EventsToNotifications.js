var Ksx;
(function (Ksx) {
    var EventsToNotifications = (function () {
        function EventsToNotifications() {
            var _this = this;
            this.checkpoints = {};
            this.attendees = {};
            this.persons = {};
            Messaging.Queues.eventsFromServer.subscribe(function (e) {
                if (typeof _this[e.name] === 'function') {
                    _this[e.name](e.data);
                }
            });
            Messaging.Queues.referenceData.subscribe(function (e) {
                if (e.name === "checkpoints") {
                    e.data.forEach(function (c) { return _this.checkpoints[c.id] = c.name; });
                }
                if (e.name === "attendees") {
                    e.data.forEach(function (a) { return _this.attendees[a.id] = a.name; });
                }
            });
        }
        EventsToNotifications.prototype.HappeningSetAsDefault = function (e) {
            Messaging.Queues.success.publish(e.keyOfNewDefault + " asetettu oletustapahtumaksi.");
        };
        EventsToNotifications.prototype.HappeningCreated = function (e) {
            Messaging.Queues.success.publish("Tapahtuma " + e.happeningId + " luotu.");
        };
        EventsToNotifications.prototype.HappeningDeleted = function (e) {
            Messaging.Queues.success.publish("Tapahtuma " + e.happeningId + " poistettu.");
        };
        EventsToNotifications.prototype.PersonCreated = function (e) {
            var fullName = this.StoreFullName(e);
            Messaging.Queues.success.publish("Henkilö " + fullName + " (" + e.personId + ") luotu.");
        };
        EventsToNotifications.prototype.PersonContactInformationUpdated = function (e) {
            var fullName = this.StoreFullName(e);
            Messaging.Queues.success.publish("Henkilön " + fullName + " (" + e.personId + ") tiedot päivitetty.");
        };
        EventsToNotifications.prototype.PersonLinkedToHappening = function (e) {
            Messaging.Queues.success.publish("Henkilö " + this.Name(e.personId) + " sidottu satkuun " + e.happeningId + ".");
        };
        EventsToNotifications.prototype.PersonUnlinkedFromHappening = function (e) {
            Messaging.Queues.success.publish("Henkilö " + this.Name(e.personId) + " poistettu satkusta " + e.happeningId + ".");
        };
        EventsToNotifications.prototype.PersonUnlinkFailed = function (e) {
            Messaging.Queues.error.publish(e.reason + " (" + e.personId + ")");
        };
        EventsToNotifications.prototype.AttendeeScanIn = function (e) {
            this.ScanMessage(e, "luettu sisään huoltopisteelle");
        };
        EventsToNotifications.prototype.AttendeeScanOut = function (e) {
            this.ScanMessage(e, "luettu ulos huoltopisteeltä");
        };
        EventsToNotifications.prototype.AttendeePassthroughScan = function (e) {
            this.ScanMessage(e, "luettu läpi huoltopisteestä");
        };
        EventsToNotifications.prototype.AttendeeScanInTimeChanged = function (e) {
            this.TimeChangedMessage(e, "Sisäänluenta-aika");
        };
        EventsToNotifications.prototype.AttendeeScanOutTimeChanged = function (e) {
            this.TimeChangedMessage(e, "Ulosluenta-aika");
        };
        EventsToNotifications.prototype.AttendeePassthroughScanTimeChanged = function (e) {
            this.TimeChangedMessage(e, "Läpiluenta-aika");
        };
        EventsToNotifications.prototype.AttendeeDoubleScan = function (e) {
            Messaging.Queues.warning.publish(e.message + this.AttendeeSuffix(e));
        };
        EventsToNotifications.prototype.AttendeeScanOutPreceedsScanIn = function (e) {
            Messaging.Queues.error.publish("Uloslukema ei voi olla ennen sisäänlukemaa. Tarkista aika. " + this.AttendeeSuffix(e));
        };
        EventsToNotifications.prototype.AttendeeScanInRemoved = function (e) {
            this.ScanMessage(e, "lukema sisään poistettu huoltopisteeltä");
        };
        EventsToNotifications.prototype.AttendeeScanOutRemoved = function (e) {
            this.ScanMessage(e, "lukema ulos poistettu huoltopisteeltä");
        };
        EventsToNotifications.prototype.AttendeeQuit = function (e) {
            this.ScanMessage(e, "keskeytti huoltopisteellä");
        };
        EventsToNotifications.prototype.AttendeeQuitRemoved = function (e) {
            Messaging.Queues.info.publish("Keskeytys poistettu. " + this.AttendeeSuffix(e));
        };
        EventsToNotifications.prototype.ScanMessage = function (e, text) {
            Messaging.Queues.info.publish(this.attendees[e.personId]
                + " " + text + " " + this.checkpoints[e.checkpointId] + ".");
        };
        EventsToNotifications.prototype.TimeChangedMessage = function (e, text) {
            Messaging.Queues.info.publish(this.checkpoints[e.checkpointId] + "/" + this.attendees[e.personId]
                + ": " + text + " vaihdettu aikaan " + Ksx.Time.formatTime(e.newTime) + ".");
        };
        EventsToNotifications.prototype.StoreFullName = function (e) {
            var fullName = e.lastname + " " + e.firstname;
            this.persons[e.personId] = fullName;
            return fullName;
        };
        EventsToNotifications.prototype.Name = function (id) {
            var name = this.persons[id];
            return name || id;
        };
        EventsToNotifications.prototype.AttendeeSuffix = function (e) {
            return " (" + this.attendees[e.personId] + ")";
        };
        return EventsToNotifications;
    }());
    Ksx.EventsToNotifications = EventsToNotifications;
    var listeningNotifications = new Ksx.EventsToNotifications();
})(Ksx || (Ksx = {}));
//# sourceMappingURL=EventsToNotifications.js.map