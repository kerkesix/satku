/// <reference path="../_references.ts" />
module Ksx {
    export class EventsToNotifications {
        checkpoints = {};
        attendees = {};
        persons = {};

        HappeningSetAsDefault(e) {
            Messaging.Queues.success.publish(e.keyOfNewDefault + " asetettu oletustapahtumaksi.");
        }

        HappeningCreated(e) {
            Messaging.Queues.success.publish("Tapahtuma " + e.happeningId + " luotu.");
        }

        HappeningDeleted(e) {
            Messaging.Queues.success.publish("Tapahtuma " + e.happeningId + " poistettu.");
        }

        PersonCreated(e) {
            var fullName = this.StoreFullName(e);
            Messaging.Queues.success.publish("Henkilö " + fullName + " (" + e.personId + ") luotu.");
        }

        PersonContactInformationUpdated(e) {
            var fullName = this.StoreFullName(e);
            Messaging.Queues.success.publish("Henkilön " + fullName + " (" + e.personId + ") tiedot päivitetty."); 
        }

        PersonLinkedToHappening(e) {
            Messaging.Queues.success.publish("Henkilö " + this.Name(e.personId) + " sidottu satkuun " + e.happeningId + ".");
        }

        PersonUnlinkedFromHappening(e) {
            Messaging.Queues.success.publish("Henkilö " + this.Name(e.personId) + " poistettu satkusta " + e.happeningId + ".");
        }

        PersonUnlinkFailed(e) {
            Messaging.Queues.error.publish(e.reason + " (" + e.personId + ")");
        }

        AttendeeScanIn(e) {
            this.ScanMessage(e, "luettu sisään huoltopisteelle");
        }

        AttendeeScanOut(e) {
            this.ScanMessage(e, "luettu ulos huoltopisteeltä");
        }

        AttendeePassthroughScan(e) {
            this.ScanMessage(e, "luettu läpi huoltopisteestä");
        }

        AttendeeScanInTimeChanged(e) {
            this.TimeChangedMessage(e, "Sisäänluenta-aika");
        }

        AttendeeScanOutTimeChanged(e) {
            this.TimeChangedMessage(e, "Ulosluenta-aika");
        }

        AttendeePassthroughScanTimeChanged(e) {
            this.TimeChangedMessage(e, "Läpiluenta-aika");
        }

        AttendeeDoubleScan(e) {
            Messaging.Queues.warning.publish(e.message + this.AttendeeSuffix(e));
        }

        AttendeeScanOutPreceedsScanIn(e) {
            Messaging.Queues.error.publish("Uloslukema ei voi olla ennen sisäänlukemaa. Tarkista aika. " + this.AttendeeSuffix(e));
        }

        AttendeeScanInRemoved(e) {
            this.ScanMessage(e, "lukema sisään poistettu huoltopisteeltä");
        }

        AttendeeScanOutRemoved(e) {
            this.ScanMessage(e, "lukema ulos poistettu huoltopisteeltä");
        }

        AttendeeQuit(e) {
            this.ScanMessage(e, "keskeytti huoltopisteellä");
        }

        AttendeeQuitRemoved(e) {
            Messaging.Queues.info.publish("Keskeytys poistettu. " + this.AttendeeSuffix(e));
        }

        ScanMessage(e, text) {
            Messaging.Queues.info.publish(this.attendees[e.personId]
                + " " + text + " " + this.checkpoints[e.checkpointId] + ".");
        }

        TimeChangedMessage(e, text) {
            Messaging.Queues.info.publish(
                this.checkpoints[e.checkpointId] + "/" + this.attendees[e.personId]
                + ": " + text + " vaihdettu aikaan " + Ksx.Time.formatTime(e.newTime) + ".");
        }

        StoreFullName(e) {
            // Store persons for this session as other events need it
            var fullName = e.lastname + " " + e.firstname;
            this.persons[e.personId] = fullName;
            return fullName;
        }

        Name(id: string) {
            // Person information might not be available for existing persons
            var name = this.persons[id];
            return name || id;
        }

        AttendeeSuffix(e) {
            return " (" + this.attendees[e.personId] + ")";
        }

        constructor() {
            Messaging.Queues.eventsFromServer.subscribe(e => {
                if (typeof this[e.name] === 'function') {
                    this[e.name](e.data);
                }
            });

            // Take reference data to be able to give meaningful messages (ie. not just ids)
            Messaging.Queues.referenceData.subscribe(e => {
                if (e.name === "checkpoints") {
                    // Change from array to map for easier further usage
                    e.data.forEach(c => this.checkpoints[c.id] = c.name);
                }

                if (e.name === "attendees") {
                    // Change from array to map for easier further usage
                    e.data.forEach(a => this.attendees[a.id] = a.name);
                }
            });
        }
    }

    var listeningNotifications = new Ksx.EventsToNotifications();
}