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
    var Happening = (function () {
        function Happening(dto) {
            this.Key = ko.observable(dto ? dto.key : "");
            this.CheckpointCount = ko.observable(dto ? dto.checkpointCount : 0);
            this.Coordinates = ko.observable(dto ? dto.path : "");
            this.CoordinatesOpen = ko.observable(false);
        }
        Happening.prototype.toggleCoordinatesOpen = function () {
            this.CoordinatesOpen(!this.CoordinatesOpen());
        };
        Happening.prototype.parseKmlFormat = function () {
            var kmlSource = this.Coordinates(), parsed;
            parsed = this.extractCoordinatesFromKml(kmlSource)
                .split(',0.0')
                .map(function (pair) { return pair.replace(/\s/g, '').split(','); })
                .filter(function (t) { return t.length > 1; });
            this.Coordinates(JSON.stringify(parsed).replace(/\"/g, ''));
        };
        Happening.prototype.extractCoordinatesFromKml = function (kmlSource) {
            var index = kmlSource.indexOf("<LineString>"), combinedCoordinates = [];
            if (index <= 0) {
                return kmlSource;
            }
            while (index > 0) {
                var coordinatesStart = kmlSource.indexOf("<coordinates>", index), coordinatesEnd = kmlSource.indexOf("</coordinates>", index), endIndex = kmlSource.indexOf("</LineString>", index);
                combinedCoordinates.push(kmlSource.substring(coordinatesStart + 13, coordinatesEnd));
                index = kmlSource.indexOf("<LineString>", endIndex);
            }
            return combinedCoordinates.join(" ");
        };
        Happening.prototype.addCoordinates = function () {
            var command = Ksx.Bus.createCommand("AddCoordinatePath", {
                key: this.Key(),
                path: this.Coordinates().trim()
            });
            Ksx.Bus.bus.send(command);
            Messaging.Queues.info.publish("Lisätään koordinaatteja... ");
            this.CoordinatesOpen(false);
        };
        return Happening;
    }());
    Ksx.Happening = Happening;
    var EditHappeningsViewModel = (function (_super) {
        __extends(EditHappeningsViewModel, _super);
        function EditHappeningsViewModel() {
            var _this = _super.call(this, "edithappenings") || this;
            _this.newHappening = new Happening();
            _this.defaultHappening = ko.observable('');
            var self = _this, route = {
                urlMap: ["/edit/happenings"],
                order: 90,
                text: "Muokkaa tapahtumia",
                icon: "fa-trophy",
                active: false,
                tag: "admin",
                root: _this.domroot,
                showOnNavigation: true,
                onViewOpen: self.onload.bind(_this)
            };
            Ksx.Routes.register(_this.domroot, route);
            return _this;
        }
        EditHappeningsViewModel.prototype.onload = function (context) {
            var _this = this;
            var that = this;
            if (this.loaded()) {
                return;
            }
            Ksx.DataClient.happenings()
                .done(function (data) {
                var def = null;
                var mapped = (data || []).map(function (dto) {
                    if (dto.isDefault) {
                        def = dto.key;
                    }
                    return new Happening(dto);
                });
                _this.happenings = ko.observableArray(mapped);
                _this.applyBindings();
                _this.defaultHappening(def);
                _this.defaultHappening.subscribe(that.saveDefault);
            });
        };
        EditHappeningsViewModel.prototype.saveNewHappening = function () {
            var command = Ksx.Bus.createCommand("CreateHappening", {
                key: this.newHappening.Key()
            });
            Ksx.Bus.bus.send(command);
            this.happenings.push(new Happening({
                key: this.newHappening.Key(),
                isDefault: false,
                checkpointCount: 0,
                path: ""
            }));
            this.newHappening.Key('');
            Messaging.Queues.info.publish("Tallennetaan uutta tapahtumaa '" + command.key + "'...");
        };
        EditHappeningsViewModel.prototype.saveDefault = function (newDefault) {
            var command = Ksx.Bus.createCommand("ChangeDefaultHappening", {
                key: newDefault
            });
            Ksx.Bus.bus.send(command);
            Messaging.Queues.info.publish("Vaihdetaan oletustapahtumaksi '" + newDefault + "'...");
        };
        EditHappeningsViewModel.prototype.deleteHappening = function (happening) {
            var command = Ksx.Bus.createCommand("DeleteHappening", {
                key: happening.Key()
            });
            Ksx.Bus.bus.send(command);
            this.happenings.remove(happening);
            Messaging.Queues.info.publish("Poistetaan tapahtumaa '" + happening.Key() + "'...");
        };
        return EditHappeningsViewModel;
    }(Ksx.ViewModelBase));
    Ksx.EditHappeningsViewModel = EditHappeningsViewModel;
    EditHappeningsViewModel.instance = new EditHappeningsViewModel();
})(Ksx || (Ksx = {}));
//# sourceMappingURL=EditHappeningsViewModel.js.map