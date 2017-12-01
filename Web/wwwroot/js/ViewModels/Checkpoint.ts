/// <reference path="../_references.ts" />

/// <reference path="Dtos.ts" />
/// <reference path="Visit.ts" />
/// <reference path="Quitter.ts" />
/// <reference path="Expected.ts" />

interface ICheckpoint {
    id: string;
    name: string;
    distanceFromStart: number;
    distanceFromPrevious: number;
    quitters: KnockoutComputed<IQuitter[]>;
    visits: KnockoutObservableArray<IVisit>;
    
    waitingFor: KnockoutObservableArray<IExpected>;
    avgSpeed: KnockoutComputed<number>;
    avgTime: KnockoutComputed<number>;
    latitude: string;
    longitude: string;
    checkpointType: number;

    status: KnockoutComputed<string>;

    getAttendeeVisit: (id: string) => IVisit;
}

module Ksx {
    export class Checkpoint implements ICheckpoint  {
        id: string;
        name: string;
        distanceFromStart: number;
        distanceFromPrevious: number;
        quitters: KnockoutComputed<Quitter[]>;
        visits: KnockoutObservableArray<Visit>;

        waitingFor: KnockoutObservableArray<Expected>;

        avgSpeed: KnockoutComputed<number>;
        avgTime: KnockoutComputed<number>;

        latitude: string;
        longitude: string;
        checkpointType: number;

        // Computed representations of data
        inside: KnockoutComputed<number>;
        status: KnockoutComputed<string>;

        // Used when user types manually a new scan in
        newManualScanDate: KnockoutObservable<string> = ko.observable<string>();
        newManualScanTime: KnockoutObservable<string> = ko.observable<string>();
        selectedNewScanAttendee: KnockoutObservable<Expected> = ko.observable<Expected>();

        getAttendeeVisit(id: string): Visit {
            var found: Visit;

            this.visits().some((visit:Visit) => {
                if (visit.id === id) {
                    found = visit;
                    return true;
                }
            });

            return found;
        }

        mapVisits(dtos: IVisitDto[], checkpointId: string, checkpointName: string): Visit[]{
            return (dtos || []).map((cpDto: IVisitDto) => {
                return new Visit(cpDto, checkpointId, checkpointName, this.distanceFromPrevious);
            });
        }

        selectQuitters(): IQuitter[] {
            return this.visits().filter(v => <any>v.quit()).map(v => v.quit());
        }

        mapWaitingFor(dtos): Expected[]{
            var mapped: Expected[] = [];

            for (var p in dtos) {
                if (dtos.hasOwnProperty(p)) {
                    mapped.push(new Expected({ id: p, t: dtos[p] }));
                }
            }

            return mapped;
        }

        sum(prev: number, next: number): number {
            return (prev || 0) + (next || 0);
        }

        visitAvg = lambda => {
            // Include only the ones that have non-zero values
            var mapped = this.visits().map(lambda),
                nonZeroMapped = mapped.filter(v => v > 0),
                total = <number>nonZeroMapped.reduce(this.sum, 0);

            return total / nonZeroMapped.length;
        };

        // Called, when checkpoint add scan modal is opened
        addManualScanInModalOpen() {
            var d = new Date();
            this.newManualScanDate(Ksx.Time.formatDate(d));
            this.newManualScanTime(Ksx.Time.formatTime(d));
        }

        addManualScanIn() {
            var command = Ksx.Bus.createCommand("Scan", {
                PersonId: this.selectedNewScanAttendee().id,
                CheckpointId: this.id,
                HappeningId: Ksx.Routes.currentHappening,
                ScanTimestamp: Ksx.Time.ToServerTimeFromInputDate(this.newManualScanDate(), this.newManualScanTime())
            });

            Ksx.Bus.bus.send(command);

            Messaging.Queues.info.publish("Tallennetaan lukemaa...");

            // TODO: There are multiple versions of this task around the site, harmonize all to just one closeModal
            // Close modal window
            // HACK: Should find a MVVM way to do this
            (<any>$(".modal")).modal('hide');
        }

        constructor (data: ICheckpointDto) { 
            var self = this;

            self.id = data.id;
            self.name = data.name;
            self.distanceFromStart = data.distanceFromStart;
            self.distanceFromPrevious = data.distanceFromPrevious;

            self.visits = ko.observableArray(self.mapVisits(data.visits, data.id, data.name));

            // Collect quitters from attendee checkpoint at runtime
            self.quitters = ko.pureComputed(self.selectQuitters, self);

            // TODO: Calculate at client side?
            self.waitingFor = ko.observableArray(self.mapWaitingFor(data.waitingFor));

            self.avgSpeed = ko.pureComputed(() => self.visitAvg(v => v.lastLegSpeed()));
            self.avgTime = ko.pureComputed(() => self.visitAvg(v => v.duration()));
            self.inside = ko.pureComputed(() => {
                var inside = this.visits().filter(
                    elem => (!elem.dateTimeOut() && !elem.quit())).length;

                // Correct currently inside if this is the last checkpoint (finish)
                return (this.distanceFromStart == 100) ? 0 : inside;
            });

            self.status = ko.pureComputed(() => {
                var visits = this.visits().length,
                    quits = this.quitters().length,
                    waitingFor = this.waitingFor().length,
                    inside = this.inside();

                return visits + "/" + (visits + waitingFor)
                    + (quits ? " kesk. " + quits : "")
                    + (inside ? " huoltaa " + inside : "");
            });

            self.latitude = data.latitude;
            self.longitude = data.longitude;
            self.checkpointType = data.checkpointType;
        }
    }
}