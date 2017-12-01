/// <reference path="../_references.ts" />
module Ksx {
    export class AboutViewModel extends ViewModelBase {
        static instance: AboutViewModel;
        
        constructor () {
            super("about");

            var self = this,
                route: IRoute = {
                    urlMap: ["/about"],
                    order: 90,
                    text: "Mikä?",
                    icon: "fa-info-circle",
                    active: false,
                    root: this.domroot,
                    showOnNavigation: true,
                    onViewOpen: self.onload.bind(this)
                };

            Ksx.Routes.register(this.domroot, route);
        }
    }
    
    // Expose model to outside world for debugging purposes
    AboutViewModel.instance = new AboutViewModel();
}