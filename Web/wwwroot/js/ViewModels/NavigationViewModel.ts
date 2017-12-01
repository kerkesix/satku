/// <reference path="../_references.ts" />

module Ksx {
    export class KoRoute {
        text: KnockoutObservable<string>;
        icon: KnockoutObservable<string>;
        url: KnockoutObservable<string>;
        order: KnockoutObservable<number>;
        active: KnockoutObservable<boolean>;
        tag: KnockoutObservable<string>;
        visible: KnockoutObservable<boolean>;

        constructor (route: IRoute) {
            var self = this;

            self.text = ko.observable(route.text);
            self.icon = ko.observable(route.icon);
            self.url = ko.observable(route.navigationUrl);
            self.order = ko.observable(route.order);
            self.active = ko.observable(route.active);
            self.tag = ko.observable(route.tag);

            // This is changed externally
            self.visible = ko.observable(true);
        }
    }

    export class NavigationViewModel {

        static instance: NavigationViewModel;
        routes: KnockoutObservableArray<KoRoute> = ko.observableArray<KoRoute>();
        publicMenuRoutes: KnockoutComputed<KoRoute[]>;
        adminMenuRoutes: KnockoutComputed<KoRoute[]>;
        loaded: KnockoutObservable<boolean> = ko.observable(false);

        constructor () {
            var self = this,
                routeFilter = (tag: string) => {
                    return ko.pureComputed(() => {
                        return self.routes().filter((item) => {
                            return item.tag() === tag;
                        });
                    });
                };
            
            self.publicMenuRoutes = routeFilter("public");
            self.adminMenuRoutes = routeFilter("admin");

            // Change active route on view change
            Messaging.Queues.navigated.subscribe((route: any) => {
                console.log("Navigated", route);
                
                // Initialize routes if not done. As every page load - even 
                // the first one - issues navigation event, this is 
                // ok to do here. If this would be done before navigation, 
                // some data would be missing (like pushState interceptors).
                if (!NavigationViewModel.instance.loaded()) {
                    (Ksx.Routes.navigableRoutes() || []).forEach((r: IRoute) => {
                        self.routes.push(new KoRoute(r));
                    });

                    ko.applyBindings(NavigationViewModel.instance, document.getElementById("navigation"));
                    NavigationViewModel.instance.loaded(true);
                }

                self.routes().forEach((elem) => {
                    elem.active(elem.url() === route.navigationUrl);
                });
            });
        }
    }

    // Expose model to outside world for debugging purposes
    NavigationViewModel.instance = new NavigationViewModel();
}
