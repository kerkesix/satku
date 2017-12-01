/// <reference path="../_references.ts" />
/// <reference path="../ViewModelBase.ts" />
/// <reference path="ScanValidator.ts" />
module Ksx {
    declare var Barcode: (callback: (code: string) => void, length: number) => void;
    declare var Beep: (frequency: number, length: number) => void;


    export class ScanLogRow {
        status = ko.observable("new");
        text = ko.observable();
        error = ko.observable();

        constructor(public id, public person, public timestamp) {
        }
    }

    export class ScanViewModel extends ViewModelBase {
        static instance: ScanInstructionsViewModel;

        rows: KnockoutObservableArray<ScanLogRow> = ko.observableArray([]);
        
        windowActive: KnockoutObservable<boolean> = ko.observable(true);
        barcodeReader: any;
        nfcCodeReader: any;
        beep : any = new Beep(1700, 200);

        static errorEvents = {
            AttendeeDoubleScan: "Tuplaluenta",
            AttendeeScanOutPreceedsScanIn: "Ulosluenta ennen sisäänluentaa"
        };

        static successEvents = {
            AttendeeScanIn: "sisään",
            AttendeeScanOut: "ulos",
            AttendeePassthroughScan: "läpi"
        };

        nfcTagToPersonId(nfc: string): string {
            Ksx.Track.pageEvent("nfc", "complete", nfc);

            // Map NFC tag to person id, or just pass through if not found
            return Ksx.ReferenceData.nfcMap.hasOwnProperty(nfc) ? Ksx.ReferenceData.nfcMap[nfc] : nfc;
        }

        barcodeComplete(code: string) {
            console.log("barcode", code);
            Ksx.Track.pageEvent("barcode", "complete", code);

            var validator = new ScanValidator(code);

            if (!validator.validate()) {
                return;
            }

            var command = Ksx.Bus.createCommand("Scan", {
                HappeningId: Ksx.Routes.currentHappening,
                PersonId: code

                // Appended on other view models:
                // CheckpointId: "",
            });

            // Add row to log
            var logRow = new ScanLogRow(command.id, code, command.timestamp);
            this.rows.push(logRow);

            // Command needs more information, sending is done on response to this request.
            Messaging.Queues.checkpointForScanRequest.publish(command);

            // Beep, to notify operator that this was received
            this.beep.play(1);

            // Send analytics event
            Ksx.Track.pageEvent("scan", "send", code);
        }

        findRow(e) {
            return this.rows().reduce((found, r) => {
                return (r.person === e.personId && r.status() === "ack") ? r : found;
            }, null);
        }

        setRowStatus(eventData, success?, error?) {
            var r = this.findRow(eventData);

            if (!r) {
                return;
            }

            if (success) {
                r.status("ok");
                r.text(success);
            }

            if (error) {
                r.status("error");
                r.error(error);
            }

            // TODO: Update same status to the local storage log rows
        }

        constructor() {
            super("scan");

            var self = this,
                route: IRoute = {
                    urlMap: ["/scanlog"],
                    order: 94,
                    text: "Luentaloki",
                    icon: "fa-list-ol",
                    active: false,
                    root: this.domroot,
                    showOnNavigation: false,
                    onViewOpen: self.onload.bind(this)
                };

            Ksx.Routes.register(this.domroot, route);
            
            // Wire up barcode reader. Expect barcodes of length 8
            this.barcodeReader = new Barcode(c => this.barcodeComplete(c), 8);

            // Wire up NFC code reader, but map codes to persons before executing handler function
            this.nfcCodeReader = new Barcode(c => this.barcodeComplete(this.nfcTagToPersonId(c)), 10);

            console.log("Bar code reader initialized, waiting...");

            Ksx.Bus.initialize.then(() => {
                // When current checkpoint info has been appended, send the command
                Messaging.Queues.checkpointForScanResponse.subscribe(Ksx.Bus.bus.send.bind(Ksx.Bus.bus));
            });

            // When command has been acknowledged at server, change log row status
            Messaging.Queues.commandAck.subscribe(data => {
                this.rows().forEach(r => {
                    if (r.id === data.id) {
                        r.status("ack");
                    }
                });
            });

            // When we get scan answers, change log row status
            Messaging.Queues.eventsFromServer.subscribe(e => {
                if (typeof ScanViewModel.successEvents[e.name] !== 'undefined') {
                    this.setRowStatus(e.data, ScanViewModel.successEvents[e.name]);
                    Ksx.Track.pageEvent("scanresponse", "ok", e.name);
                    return;
                }
                if (typeof ScanViewModel.errorEvents[e.name] !== 'undefined') {
                    this.setRowStatus(e.data, null, ScanViewModel.errorEvents[e.name]);
                    Ksx.Track.pageEvent("scanresponse", "fail", e.name);
                    return;
                }
            });

            // Track window focus to be able to show warning message
            window.onfocus = () => this.windowActive(true);
            window.onblur = () => this.windowActive(false);

            // TODO: If needed, use page visibility api and navigator online api usage to detect other issues
            // Wire up showing warning when connection is lost.
            // Note: this is not definitive, this does not guarantee that browser 
            // can actually send requests all the way to the target server.
            //window.addEventListener("offline", function () { warningMessageElement.show(); });
            //window.addEventListener("online", function () { warningMessageElement.hide(); });
        }
    }

    // Expose model to outside world for debugging purposes
    ScanViewModel.instance = new ScanViewModel();
}