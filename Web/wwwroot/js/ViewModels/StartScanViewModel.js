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
    var StartScanViewModel = (function (_super) {
        __extends(StartScanViewModel, _super);
        function StartScanViewModel() {
            var _this = _super.call(this, "startscan") || this;
            _this.allAttendees = ko.observableArray([]);
            _this.time = ko.observable();
            var self = _this, route = {
                urlMap: ["/readings/start"],
                order: 94,
                text: "Aloituslukema",
                icon: "fa-list-ol",
                active: false,
                root: _this.domroot,
                showOnNavigation: false,
                onViewOpen: self.onload.bind(_this)
            };
            Ksx.Routes.register(_this.domroot, route);
            Messaging.Queues.referenceData.subscribe(function (e) {
                if (e.name === "attendees") {
                    _this.allAttendees = ko.observableArray(e.data);
                }
                if (e.name === "checkpoints") {
                    _this.checkpoints = e.data;
                }
            });
            return _this;
        }
        StartScanViewModel.prototype.add = function () {
            if (this.allAttendees().length && this.checkpoints.length) {
                var checkpointId = this.checkpoints[0].id, happeningId = Ksx.Routes.currentHappening, scanTime = Ksx.Time.ToServerTimeFromInputDate(this.time());
                this.allAttendees().forEach(function (a) {
                    var command = Ksx.Bus.createCommand("StartScan", {
                        PersonId: a.id,
                        CheckpointId: checkpointId,
                        HappeningId: happeningId,
                        ScanTimestamp: scanTime
                    });
                    Ksx.Bus.bus.send(command);
                });
                Messaging.Queues.info.publish("Aloituslukemia tallennetaan...");
            }
            this.sammyContext.redirect("/");
        };
        return StartScanViewModel;
    }(Ksx.ViewModelBase));
    Ksx.StartScanViewModel = StartScanViewModel;
    StartScanViewModel.instance = new StartScanViewModel();
})(Ksx || (Ksx = {}));
//# sourceMappingURL=StartScanViewModel.js.map