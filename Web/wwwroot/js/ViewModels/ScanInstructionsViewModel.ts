/// <reference path="../_references.ts" />
/// <reference path="../ViewModelBase.ts" />
module Ksx {
    export class ScanInstructionsViewModel extends ViewModelBase {
        static instance: ScanInstructionsViewModel;

        constructor() {
            super("scaninstructions");

            var self = this,
                route: IRoute = {
                    urlMap: ["/scaninstructions"],
                    order: 94,
                    text: "Luentaohjeet",
                    icon: "fa-info-sign",
                    active: false,
                    root: this.domroot,
                    showOnNavigation: false,
                    onViewOpen: self.onload.bind(this)
                };

            Ksx.Routes.register(this.domroot, route);
        }
    }

    // Expose model to outside world for debugging purposes
    ScanInstructionsViewModel.instance = new ScanInstructionsViewModel();
}