/// <reference path="../_references.ts" />
/// <reference path="../ViewModelBase.ts" />
/// <reference path="Attendee.ts" />
module Ksx.Attendees {
    export class AttendeesViewModel extends ViewModelBase {
        static retries: number = 0;

        attendees: KnockoutObservableArray<IAttendee>;
        checkpointNames: KnockoutObservableArray<string>;
        selected: KnockoutObservable<Attendee> = ko.observable<Attendee>();
        filterText: KnockoutObservable<string> = ko.observable('');

        previousSelected: IAttendee;
        sammyContext: any;

        public onload(context : any) : void {
            var id = context.params["id"];
                
            this.sammyContext = context;

            if (!this.attendees) {
                if (AttendeesViewModel.retries < 100) {
                    // Retry, data not initialized
                    AttendeesViewModel.retries++;
                    setTimeout(() => this.onload(context), 100);
                }

                return;
            }

            this.selectAttendee(id);

            // Do not re-apply bindings (e.g. dataLoaded callback applies bindings already)
            if (this.loaded()) {
                return;
            }

            this.applyBindings();
        }

        // This method is bound to form submit event, used if user hit enter
        public filterSubmit(elem) {
            this.filter(this.filterText());
        }

        private selectAttendee(id) {
            var found;

            this.attendees().some(elem => {
                if (elem.id === id) {
                    found = elem;
                    return true;
                }

                return false;
            });

            // Give null if none found
            this.selected(found);
        }

        constructor (data: JQueryDeferred<any>) {
            super("attendees");

            var self = this,
                route: IRoute = {
                    urlMap: ["/attendees/:id", "/attendees"],
                    navigationUrl: "/attendees",
                    icon: "fa-group",
                    order: 5,
                    text: "Kävelijät",
                    active: false,
                    root: this.domroot,
                    showOnNavigation: true,
                    onViewOpen: self.onload.bind(this)
                };

            Ksx.Routes.register(this.domroot, route);
            self.filterText.subscribe(<any>self.filter.bind(self));

            // Store old selected attendee to avoid eternal loops
            self.selected.subscribe(attendee => {
                self.previousSelected = attendee;
            }, self, "beforeChange");

            // Navigate to selected attendee on selection (needed on mobile view with 
            // dropdown based navigation, normal navi is href based).
            self.selected.subscribe(attendee => {
                // Act only if value changed 
                if (self.previousSelected !== attendee && self.sammyContext) {
                    var newUrl = Ksx.Routes.url("attendees", null, attendee ? attendee.id : null);

                    self.sammyContext.redirect(newUrl);
                    Ksx.Track.pageView(newUrl);
                }
            });

            // Listen to reference data refreshes
            Messaging.Queues.referenceData.subscribe(e => {
                if (e.name === "checkpoints") {
                    self.checkpointNames = ko.observableArray<string>(e.data.map(c => c.name));
                }
            });

            // Act, when data is loaded
            data.done(self.loadData.bind(this));
        }

        private loadData(inputAttendees) {
            // Map dictionary to array
            var a: IAttendee[] = Object.getOwnPropertyNames(inputAttendees).map(p => inputAttendees[p]);

            this.attendees = ko.observableArray(a);

            // If this event happened before onload call, bind page here
            this.applyBindings();
        }

        private filter(s: string) {
            if (!s) {
                this.attendees().forEach(elem => {
                    elem.filtered(false);
                });            

                return;
            }

            var rule = new RegExp("^[\\w\\s]*" + s + "[\\w\\s]*$", "i");

            this.attendees().forEach(elem => {
                elem.filtered(!elem.name.match(rule));
            });            
        }
    }
}