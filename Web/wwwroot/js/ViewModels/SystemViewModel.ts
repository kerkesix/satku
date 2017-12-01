/// <reference path="../_references.ts" />
/// <reference path="../ViewModelBase.ts" />
/// <reference path="../DataClient.ts" />
module Ksx {
    export class SystemViewModel extends ViewModelBase {
        static instance: SystemViewModel;

        public refresh() {
            Ksx.DataClient.refresh().done(() => {
                Ksx.Track.pageEvent("system", "restart", "");
                setTimeout(() => { window.location.href = "/"; }, 4000);
                Messaging.Queues.success.publish("Järjestelmä käynnistetty uudelleen., sinut ohjataan etusivulle...");
            })
                .fail(Messaging.Queues.error.publish.bind(this, "Järjestelmän käynnistäminen epäonnistui"));
        }

        constructor() {
            super("system");

            var self = this,
                route: IRoute = {
                    urlMap: ["/edit/system"],
                    order: 99,
                    text: "Järjestelmä",
                    icon: "fa-cogs",
                    active: false,
                    tag: "admin",
                    root: this.domroot,
                    showOnNavigation: true,
                    onViewOpen: self.onload.bind(this)
                };

            // Even though this page does not use happening 
            // information, register it under /happening/ path
            // as this prevents problems detecting happening 
            // server side when refreshing page.
            Ksx.Routes.register(this.domroot, route);
        }
    }

    // Expose model to outside world for debugging purposes
    SystemViewModel.instance = new SystemViewModel();
}