/// <reference path="../_references.ts" />
/// <reference path="../ViewModelBase.ts" />
module Ksx {
    export class RouteViewModel extends ViewModelBase {
        static instance: RouteViewModel;

        constructor() {
            super("route");

            var self = this,
                route: IRoute = {
                    urlMap: ["/route"],
                    order: 50,
                    text: "Reitti",
                    icon: "fa-map-marker",
                    active: false,
                    root: this.domroot,
                    showOnNavigation: true,
                    onViewOpen: self.onload.bind(this)
                };

            Ksx.Routes.register(this.domroot, route);
        }
    }

    // Expose model to outside world for debugging purposes
    RouteViewModel.instance = new RouteViewModel();
}