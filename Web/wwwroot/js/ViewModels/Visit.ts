/// <reference path="../_references.ts" />
/// <reference path="Dtos.ts" />
/// <reference path="Quitter.ts" />
/// <reference path="ReferenceData.ts" />

interface IVisit {
    id: string;

    
    dateTimeIn: KnockoutObservable<number>;
    dateTimeOut: KnockoutObservable<number>;
    duration: KnockoutComputed<number>;
    lastLegSpeed: KnockoutComputed<number>;

    // This is set by data receiving class
    previousVisitCheckoutTime: KnockoutObservable<number>;
    walkingTime: KnockoutComputed<number>;
    distanceFromPrevious: number;

    quit: KnockoutObservable<IQuitter>;

    // Name of the checkpoint copied from parent for easier templating
    checkpointName: string;
}

module Ksx {
    // TODO: Separating popups to own view models might be good idea
    export class Visit implements IVisit {
        // Attendee's id
        id: string;
        quit: KnockoutObservable<Quitter> = ko.observable<Quitter>();

        dateTimeIn: KnockoutObservable<number>;
        dateTimeOut: KnockoutObservable<number>;
        duration: KnockoutComputed<number>;

        previousVisitCheckoutTime: KnockoutObservable<number> = ko.observable(0);
        walkingTime: KnockoutComputed<number>;
        lastLegSpeed: KnockoutComputed<number>;

        // Used when user types manually a new scan out
        newManualScanTime: KnockoutObservable<string> = ko.observable('');

        // Used when user types a new quitter
        addQuitTime: KnockoutObservable<string> = ko.observable('');
        addQuitWalkedSinceLast: KnockoutObservable<string> = ko.observable('');
        addQuitDescription: KnockoutObservable<string> = ko.observable('');

        // Used, when user types new visit times
        // "In", "Out", "Passthrough"
        visitTimeToChange: KnockoutObservable<string> = ko.observable('');
        changeScanTimeDate: KnockoutObservable<string> = ko.observable('');
        changeScanTimeTime: KnockoutObservable<string> = ko.observable('');

        createVisitCommand(name: string, properties?: any) {
            var prop = properties || {};
            prop.PersonId = this.id;
            prop.CheckpointId = this.checkpointId;
            prop.HappeningId = Ksx.Routes.currentHappening;

            return Ksx.Bus.createCommand(name, prop);
        }

        closeModal() {
            // Close modal window
            // HACK: Should find a MVVM way to do this
            (<any>$(".modal")).modal('hide');
        }

        failOperation(text) {
            Messaging.Queues.error.publish(text);
            this.closeModal();            
        }

        sendAndNotify(command, text) {
            Ksx.Bus.bus.send(command);
            Messaging.Queues.info.publish(text);
            this.closeModal();
        }

        // Called, when visit add out scan modal is opened
        addManualScanOutModalOpen() {
            this.newManualScanTime(Ksx.Time.formatTime(new Date()));
        }

        addManualScanOut() {
            var command = this.createVisitCommand("Scan", {
                ScanTimestamp: Ksx.Time.ToServerTimeFromInputDate(this.newManualScanTime())
            });

            this.sendAndNotify(command, "Tallennetaan lukemaa...");
        }

        changeScanTimeModalOpen() {
            var changeType = null;

            if (this.dateTimeIn() === this.dateTimeOut()) {
                changeType = "Passthrough";
            }
            else if (this.dateTimeIn() && !this.dateTimeOut()) {
                changeType = "In";
            }

            this.visitTimeToChange(changeType);
        }

        changeScanTime() {
            var command = this.createVisitCommand("Change" + this.visitTimeToChange() + "ScanTime", {
                NewTime: Ksx.Time.ToServerTimeFromInputDate(this.changeScanTimeDate(), this.changeScanTimeTime())
            });

            this.sendAndNotify(command, "Siirretään lukeman aikaa...");
        }

        addQuitModalOpen() {
            this.addQuitTime(Ksx.Time.formatTime(new Date()));
            this.addQuitWalkedSinceLast("0,0");
        }

        addQuit() {
            // TODO: Completed and quit are not available at ReferenceData
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
        }

        removeQuit() {
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
        }

        removeScan() {
            if (Ksx.ReferenceData.completedAttendees[this.id]) {
                this.failOperation("Henkilö on jo maalissa, lukemia ei voi enää poistaa.");
                return;
            }

            if (Ksx.ReferenceData.quitAttendees[this.id]) {
                this.failOperation("Henkilö on jo keskeyttänyt, lukemia ei voi enää poistaa.");
                return;
            }

            this.sendAndNotify(this.createVisitCommand("RemoveScan"), "Poistetaan lukemaa...");
        }

        constructor(
            data: IVisitDto,
            public checkpointId: string,
            public checkpointName: string,
            public distanceFromPrevious: number) { 
            var self = this;

            self.id = data.a;

            if (data.quit) {
                self.quit(new Quitter(data.quit, data.a));
            }

            self.dateTimeIn = ko.observable(data.i ? Date.parse(data.i) : 0);
            self.dateTimeOut = ko.observable(data.o ? Date.parse(data.o) : 0);

            // Change time editor when time to change changes
            self.visitTimeToChange.subscribe(v => {
                if (v === "In" || v === "Passthrough") {
                    this.changeScanTimeDate(Ksx.Time.formatDate(this.dateTimeIn()));
                    this.changeScanTimeTime(Ksx.Time.formatTime(this.dateTimeIn()));
                }

                if (v === "Out") {
                    this.changeScanTimeDate(Ksx.Time.formatDate(this.dateTimeOut()));
                    this.changeScanTimeTime(Ksx.Time.formatTime(this.dateTimeOut()));
                }
            });

            self.duration = ko.pureComputed(() => {
                if (this.dateTimeIn() === 0 || this.dateTimeOut() === 0) {
                    return 0;
                }

                return Math.abs(this.dateTimeOut() - this.dateTimeIn());
            });

            self.walkingTime = ko.pureComputed(() => {
                if (this.previousVisitCheckoutTime() && this.dateTimeIn()) {
                    return this.dateTimeIn() - this.previousVisitCheckoutTime();
                }

                return 0;
            });

            self.lastLegSpeed = ko.pureComputed(() => {
                var s = 0,
                    d = this.walkingTime();

                if (!d || this.distanceFromPrevious === 0) {
                    return s;
                }

                // duration is in milliseconds, convert to km/h
                s = this.distanceFromPrevious / (d / 3600000);
                return Math.round(100 * s) / 100;
            });
        }
    }
}