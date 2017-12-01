/// <reference path="../_references.ts" />
/// <reference path="../ViewModelBase.ts" />
module Ksx {
    export class StartScanViewModel extends ViewModelBase {
        static instance: StartScanViewModel;
        allAttendees: KnockoutObservableArray<any> = ko.observableArray([]);
        checkpoints: any[];
        time = ko.observable();

        constructor() {
            super("startscan");

            var self = this,
                route: IRoute = {
                    urlMap: ["/readings/start"],
                    order: 94,
                    text: "Aloituslukema",
                    icon: "fa-list-ol",
                    active: false,
                    root: this.domroot,
                    showOnNavigation: false,
                    onViewOpen: self.onload.bind(this)
                };

            Ksx.Routes.register(this.domroot, route);

            Messaging.Queues.referenceData.subscribe(e => {
                if (e.name === "attendees") {
                    this.allAttendees = ko.observableArray(e.data);
                }
        
                if (e.name === "checkpoints") {
                    this.checkpoints = e.data;
                }
            });
        }

        public add() {
            if (this.allAttendees().length && this.checkpoints.length) {
                var checkpointId = this.checkpoints[0].id,
                    happeningId = Ksx.Routes.currentHappening,
                    scanTime = Ksx.Time.ToServerTimeFromInputDate(this.time());

                this.allAttendees().forEach(a => {
                    var command = Ksx.Bus.createCommand("StartScan", {
                        PersonId: a.id,
                        CheckpointId: checkpointId,
                        HappeningId: happeningId,

                        // Use same timestamp for everyone, take from form.
                        // Do not touch command timestamp, as letting user to 
                        // type that might mess event ordering on server side.
                        ScanTimestamp: scanTime
                    });

                    Ksx.Bus.bus.send(command);
                });

                Messaging.Queues.info.publish("Aloituslukemia tallennetaan...");
            }
            // Redirect
            this.sammyContext.redirect("/");
        }
    }

    // Expose model to outside world for debugging purposes
    StartScanViewModel.instance = new StartScanViewModel();
}