var Ksx;
(function (Ksx) {
    var ScanValidator = (function () {
        function ScanValidator(barcode) {
            this.barcode = barcode;
        }
        ScanValidator.prototype.validate = function () {
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
            return true;
        };
        ScanValidator.prototype.isTestBarcode = function () {
            return (this.barcode === "A1234567");
        };
        ScanValidator.prototype.personIsLinkedToCurrentHappening = function () {
            return !!Ksx.ReferenceData.attendeesMap[this.barcode];
        };
        ScanValidator.prototype.personHasQuit = function () {
            return !!Ksx.ReferenceData.quitAttendees[this.barcode];
        };
        ScanValidator.prototype.personHasCompleted = function () {
            return !!Ksx.ReferenceData.completedAttendees[this.barcode];
        };
        return ScanValidator;
    }());
    Ksx.ScanValidator = ScanValidator;
})(Ksx || (Ksx = {}));
//# sourceMappingURL=ScanValidator.js.map