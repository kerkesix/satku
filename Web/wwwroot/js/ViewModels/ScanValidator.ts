/// <reference path="../_references.ts" />
/// <reference path="../ViewModelBase.ts" />
module Ksx {
    export class ScanValidator {

        constructor(public barcode: string) {

        }

        validate(): boolean {
            if (this.isTestBarcode()) {
                Messaging.Queues.success.publish("Testiluku onnistui! Tämä todistaa, että lukija "
                    + "ja selain toimivat yhdessä, yhteyttä palvelimeen ei testattu.");
                Ksx.Track.pageEvent("scanvalidate", "test", this.barcode);
                return false;
            }

            if (!this.personIsLinkedToCurrentHappening()) {
                Messaging.Queues.error.publish("Virheellinen luenta! henkilöä ei löydy osallistujien listalta: " + this.barcode);
                Ksx.Track.pageEvent("scanvalidate", "failnotlinked", this.barcode);
                return false;
            }

            if (this.personHasQuit()) {
                Messaging.Queues.error.publish("Henkilö on keskeyttänyt, lukemia ei voi enää lisätä.");
                Ksx.Track.pageEvent("scanvalidate", "failquit", this.barcode);
                return false;
            }

            if (this.personHasCompleted()) {
                Messaging.Queues.error.publish("Henkilö on jo maalissa, lukemia ei voi enää lisätä.");
                Ksx.Track.pageEvent("scanvalidate", "failcompleted", this.barcode);
                return false;
            }

            // TODO: Check double readings, might need to use report view model directly (if so, scrap quit and completed collections also)

            return true;
        }

        isTestBarcode() {
            return (this.barcode === "A1234567");
        }

        personIsLinkedToCurrentHappening() {
            // Validate, that this person is linked to current happening
            return !!Ksx.ReferenceData.attendeesMap[this.barcode];
        }

        personHasQuit() {
            return !!Ksx.ReferenceData.quitAttendees[this.barcode];
        }

        personHasCompleted() {
            return !!Ksx.ReferenceData.completedAttendees[this.barcode];
        }
    }
}