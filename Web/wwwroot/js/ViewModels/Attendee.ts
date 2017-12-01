/// <reference path="../_references.ts" />
/// <reference path="Dtos.ts" />
/// <reference path="Visit.ts" />

interface IAttendee {
    id: string;
    name: string;
    phone: string;
    visits: KnockoutObservableArray<IVisit>;
    restTimes: KnockoutComputed<number[]>;
    speeds: KnockoutComputed<number[]>;
    destructionPercent: KnockoutObservable<number>;
    totalWalkingTime: KnockoutComputed<number>;
    totalRestTime: KnockoutComputed<number>;
    walkedKilometres: KnockoutComputed<number>;
    gravatarHash: KnockoutObservable<string>;

    url: string;
    filtered: KnockoutObservable<boolean>;
}

module Ksx {
    export class Attendee implements IAttendee {
        id: string;
        nfc: string;
        name: string;
        phone: string;
        visits: KnockoutObservableArray<IVisit>;
        destructionPercent: KnockoutObservable<number>;

        // Calculated from visits
        restTimes: KnockoutComputed<number[]>;
        speeds: KnockoutComputed<number[]>;
        totalWalkingTime: KnockoutComputed<number>;
        totalRestTime: KnockoutComputed<number>;
        totalTime: KnockoutComputed<number>;        
        walkedKilometres: KnockoutComputed<number>;
        avgSpeed: KnockoutComputed<number>;

        gravatarHash: KnockoutObservable<string>;
        hasQuit: boolean;
        quitText: string;
        url: string;
        filtered: KnockoutObservable<boolean>;

        sum(prev: number, next: number) : number {
            return (prev || 0) + (next || 0);
        }

        constructor (attendee: IAttendeeDto, visits: IVisit[]) { 
            var self = this,
                quitVisit: IVisit;

            self.id = attendee.id;
            self.nfc = attendee.nfc;
            self.name = attendee.name;
            self.phone = attendee.phone || "";
            self.visits = ko.observableArray(visits || []);
            self.destructionPercent = ko.observable(attendee.dstr || 0);
            self.gravatarHash = ko.observable(attendee.hash);

            quitVisit = self.visits().reduce((found, v) => {
                return v.quit() ? v : found;
            }, null);

            // Calculated properties (from visits)
            self.speeds = ko.pureComputed(() => this.visits().map(v => v.lastLegSpeed()));
            self.restTimes = ko.pureComputed(() => this.visits().map(v => v.duration()));

            // Calculate total rest, walking and overall time
            self.totalRestTime = ko.pureComputed(() => this.restTimes().reduce(self.sum, 0));
            self.totalWalkingTime = ko.computed(() => {
                var legs = this.visits().map(v => v.walkingTime()).reduce(self.sum, 0);

                // Add time from last checkout to quit
                if (quitVisit && quitVisit.quit().walkedSinceLast) {
                    var lastVisit = this.visits()[this.visits().length - 1];
                    legs += quitVisit.quit().time - lastVisit.dateTimeOut();
                }

                return legs;
            });
            self.totalTime = ko.pureComputed(() => self.totalRestTime() + self.totalWalkingTime());

            self.walkedKilometres = ko.computed(() => {
                var legs = this.visits().map(v => v.distanceFromPrevious).reduce(self.sum, 0);

                // Add distance from last checkout to quit
                if (quitVisit && quitVisit.quit().walkedSinceLast) {
                    legs += quitVisit.quit().walkedSinceLast;
                }

                // Sum of all kms is not equal to 100, hack it
                if (legs > 100) {
                    legs = 100;
                }

                return legs;
            });

            // As this information never changes (quit --> not quit)
            // make this information static. This is accessed a lot 
            // on attendee lists. 
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

            self.avgSpeed = ko.pureComputed(() => {
                var total = this.speeds().reduce(self.sum, 0);

                // Include only the ones that have non-zero speed
                return (total/this.speeds().filter(a => a > 0).length);
            });

            self.filtered = ko.observable(false);

            self.url = Ksx.Routes.url("attendees", "", attendee.id);
        }
    }
}