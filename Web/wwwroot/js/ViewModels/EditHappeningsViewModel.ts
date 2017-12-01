/// <reference path="../_references.ts" />
/// <reference path="../ViewModelBase.ts" />
/// <reference path="../DataClient.ts" />
module Ksx {
    export interface IHappeningDto {
        key: string;
        isDefault: boolean;
        checkpointCount: number;
        path: string;
    }

    export class Happening {
        Key: KnockoutObservable<string>;
        CheckpointCount: KnockoutObservable<number>;
        Coordinates: KnockoutObservable<string>;

        CoordinatesOpen: KnockoutObservable<boolean>;

        constructor(dto?: IHappeningDto) {
            this.Key = ko.observable(dto ? dto.key : "");
            this.CheckpointCount = ko.observable(dto ? dto.checkpointCount : 0);
            this.Coordinates = ko.observable(dto ? dto.path : "");
            this.CoordinatesOpen = ko.observable(false);
        }

        public toggleCoordinatesOpen() {
            this.CoordinatesOpen(!this.CoordinatesOpen());
        }

        public parseKmlFormat() {
            var kmlSource = this.Coordinates(),
                parsed: string[][];

            // TODO: Parse also checkpoints from KML?

            // KML coordinates are like '24.6405101,60.1584541,0.0 24.6402526,60.1581445,0.0 24.6380639,60.1585075'...
            parsed = this.extractCoordinatesFromKml(kmlSource)
                .split(',0.0')
                .map(pair => pair.replace(/\s/g, '').split(','))
                .filter(t => t.length > 1);

            // Update back to view, but don't use quotes
            this.Coordinates(JSON.stringify(parsed).replace(/\"/g, ''));
        }

        extractCoordinatesFromKml(kmlSource: string): string {
            // Find all coordinates from XML (if the input is XML). Just combine all LineString elements, as 
            // the route might be split in multiple parts
            var index = kmlSource.indexOf("<LineString>"),
                combinedCoordinates = [];

            if (index <= 0) {
                // Not full KML, assume this is just coordinates
                return kmlSource;
            }

            while (index > 0) {
                var coordinatesStart = kmlSource.indexOf("<coordinates>", index),
                    coordinatesEnd = kmlSource.indexOf("</coordinates>", index),
                    endIndex = kmlSource.indexOf("</LineString>", index);

                combinedCoordinates.push(kmlSource.substring(coordinatesStart + 13, coordinatesEnd));
                index = kmlSource.indexOf("<LineString>", endIndex);
            }

            return combinedCoordinates.join(" ");
        }

        public addCoordinates(): void {
            // Send save command to server
            var command = Ksx.Bus.createCommand("AddCoordinatePath", {
                key: this.Key(),
                path: this.Coordinates().trim()
            });

            Ksx.Bus.bus.send(command);

            Messaging.Queues.info.publish("Lisätään koordinaatteja... ");

            this.CoordinatesOpen(false);
        }
    }

    export class EditHappeningsViewModel extends ViewModelBase {
        static instance: EditHappeningsViewModel;

        happenings: KnockoutObservableArray<Happening>;
        newHappening: Happening = new Happening();

        defaultHappening: KnockoutObservable<string> = ko.observable('');

        public onload(context: any): void {
            var that = this;

            if (this.loaded()) {
                return;
            }

            // Load data from server
            Ksx.DataClient.happenings()
                .done(data => {
                    var def = null;
                    var mapped: Happening[] = (data || []).map((dto: IHappeningDto) => {
                        if (dto.isDefault) {
                            def = dto.key;
                        }
                        return new Happening(dto);
                    });

                    this.happenings = ko.observableArray(mapped);

                    this.applyBindings();

                    // Set initial value for default happening
                    this.defaultHappening(def);
                    this.defaultHappening.subscribe(that.saveDefault);
                });
        }

        public saveNewHappening() {
            // Send save command to server
            var command: any = Ksx.Bus.createCommand("CreateHappening", {
                key: this.newHappening.Key()
            });

            Ksx.Bus.bus.send(command);

            // Assume everything is ok (could subscribe to server events here)
            this.happenings.push(
                new Happening({
                    key: this.newHappening.Key(),
                    isDefault: false,
                    checkpointCount: 0,
                    path: ""
                }));

            // Clear out values
            this.newHappening.Key('');

            Messaging.Queues.info.publish("Tallennetaan uutta tapahtumaa '" + command.key + "'...");
        }

        public saveDefault(newDefault: string) {
            // Send change command to server
            var command = Ksx.Bus.createCommand("ChangeDefaultHappening", {
                key: newDefault
            });

            Ksx.Bus.bus.send(command);
            Messaging.Queues.info.publish("Vaihdetaan oletustapahtumaksi '" + newDefault + "'...");
        }

        public deleteHappening(happening) {
            var command = Ksx.Bus.createCommand("DeleteHappening", {
                key: happening.Key()
            });

            Ksx.Bus.bus.send(command);
            this.happenings.remove(happening);

            Messaging.Queues.info.publish("Poistetaan tapahtumaa '" + happening.Key() + "'...");
        }

        constructor() {
            super("edithappenings");

            var self = this,
                route: IRoute = {
                    urlMap: ["/edit/happenings"],
                    order: 90,
                    text: "Muokkaa tapahtumia",
                    icon: "fa-trophy",
                    active: false,
                    tag: "admin",
                    root: this.domroot,
                    showOnNavigation: true,
                    onViewOpen: self.onload.bind(this)
                };

            // Even though this page does not use happening 
            // information, register it under /hapening/ path
            // as this prevents problems detecting happening 
            // server side when refreshing page.
            Ksx.Routes.register(this.domroot, route);
        }
    }

    // Expose model to outside world for debugging purposes
    EditHappeningsViewModel.instance = new EditHappeningsViewModel();
}