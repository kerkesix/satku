var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var Ksx;
(function (Ksx) {
    var ScanLogRow = (function () {
        function ScanLogRow(id, person, timestamp) {
            this.id = id;
            this.person = person;
            this.timestamp = timestamp;
            this.status = ko.observable("new");
            this.text = ko.observable();
            this.error = ko.observable();
        }
        return ScanLogRow;
    }());
    Ksx.ScanLogRow = ScanLogRow;
    var ScanViewModel = (function (_super) {
        __extends(ScanViewModel, _super);
        function ScanViewModel() {
            var _this = _super.call(this, "scan") || this;
            _this.rows = ko.observableArray([]);
            _this.windowActive = ko.observable(true);
            _this.beep = new Beep(1700, 200);
            var self = _this, route = {
                urlMap: ["/scanlog"],
                order: 94,
                text: "Luentaloki",
                icon: "fa-list-ol",
                active: false,
                root: _this.domroot,
                showOnNavigation: false,
                onViewOpen: self.onload.bind(_this)
            };
            Ksx.Routes.register(_this.domroot, route);
            _this.barcodeReader = new Barcode(function (c) { return _this.barcodeComplete(c); }, 8);
            _this.nfcCodeReader = new Barcode(function (c) { return _this.barcodeComplete(_this.nfcTagToPersonId(c)); }, 10);
            console.log("Bar code reader initialized, waiting...");
            Ksx.Bus.initialize.then(function () {
                Messaging.Queues.checkpointForScanResponse.subscribe(Ksx.Bus.bus.send.bind(Ksx.Bus.bus));
            });
            Messaging.Queues.commandAck.subscribe(function (data) {
                _this.rows().forEach(function (r) {
                    if (r.id === data.id) {
                        r.status("ack");
                    }
                });
            });
            Messaging.Queues.eventsFromServer.subscribe(function (e) {
                if (typeof ScanViewModel.successEvents[e.name] !== 'undefined') {
                    _this.setRowStatus(e.data, ScanViewModel.successEvents[e.name]);
                    Ksx.Track.pageEvent("scanresponse", "ok", e.name);
                    return;
                }
                if (typeof ScanViewModel.errorEvents[e.name] !== 'undefined') {
                    _this.setRowStatus(e.data, null, ScanViewModel.errorEvents[e.name]);
                    Ksx.Track.pageEvent("scanresponse", "fail", e.name);
                    return;
                }
            });
            window.onfocus = function () { return _this.windowActive(true); };
            window.onblur = function () { return _this.windowActive(false); };
            return _this;
        }
        ScanViewModel.prototype.nfcTagToPersonId = function (nfc) {
            Ksx.Track.pageEvent("nfc", "complete", nfc);
            return Ksx.ReferenceData.nfcMap.hasOwnProperty(nfc) ? Ksx.ReferenceData.nfcMap[nfc] : nfc;
        };
        ScanViewModel.prototype.barcodeComplete = function (code) {
            console.log("barcode", code);
            Ksx.Track.pageEvent("barcode", "complete", code);
            var validator = new Ksx.ScanValidator(code);
            if (!validator.validate()) {
                return;
            }
            var command = Ksx.Bus.createCommand("Scan", {
                HappeningId: Ksx.Routes.currentHappening,
                PersonId: code
            });
            var logRow = new ScanLogRow(command.id, code, command.timestamp);
            this.rows.push(logRow);
            Messaging.Queues.checkpointForScanRequest.publish(command);
            this.beep.play(1);
            Ksx.Track.pageEvent("scan", "send", code);
        };
        ScanViewModel.prototype.findRow = function (e) {
            return this.rows().reduce(function (found, r) {
                return (r.person === e.personId && r.status() === "ack") ? r : found;
            }, null);
        };
        ScanViewModel.prototype.setRowStatus = function (eventData, success, error) {
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
        };
        ScanViewModel.errorEvents = {
            AttendeeDoubleScan: "Tuplaluenta",
            AttendeeScanOutPreceedsScanIn: "Ulosluenta ennen sisäänluentaa"
        };
        ScanViewModel.successEvents = {
            AttendeeScanIn: "sisään",
            AttendeeScanOut: "ulos",
            AttendeePassthroughScan: "läpi"
        };
        return ScanViewModel;
    }(Ksx.ViewModelBase));
    Ksx.ScanViewModel = ScanViewModel;
    ScanViewModel.instance = new ScanViewModel();
})(Ksx || (Ksx = {}));
//# sourceMappingURL=ScanViewModel.js.map