var Ksx;
(function (Ksx) {
    var KoRoute = (function () {
        function KoRoute(route) {
            var self = this;
            self.text = ko.observable(route.text);
            self.icon = ko.observable(route.icon);
            self.url = ko.observable(route.navigationUrl);
            self.order = ko.observable(route.order);
            self.active = ko.observable(route.active);
            self.tag = ko.observable(route.tag);
            self.visible = ko.observable(true);
        }
        return KoRoute;
    }());
    Ksx.KoRoute = KoRoute;
    var NavigationViewModel = (function () {
        function NavigationViewModel() {
            this.routes = ko.observableArray();
            this.loaded = ko.observable(false);
            var self = this, routeFilter = function (tag) {
                return ko.pureComputed(function () {
                    return self.routes().filter(function (item) {
                        return item.tag() === tag;
                    });
                });
            };
            self.publicMenuRoutes = routeFilter("public");
            self.adminMenuRoutes = routeFilter("admin");
            Messaging.Queues.navigated.subscribe(function (route) {
                console.log("Navigated", route);
                if (!NavigationViewModel.instance.loaded()) {
                    (Ksx.Routes.navigableRoutes() || []).forEach(function (r) {
                        self.routes.push(new KoRoute(r));
                    });
                    ko.applyBindings(NavigationViewModel.instance, document.getElementById("navigation"));
                    NavigationViewModel.instance.loaded(true);
                }
                self.routes().forEach(function (elem) {
                    elem.active(elem.url() === route.navigationUrl);
                });
            });
        }
        return NavigationViewModel;
    }());
    Ksx.NavigationViewModel = NavigationViewModel;
    NavigationViewModel.instance = new NavigationViewModel();
})(Ksx || (Ksx = {}));
//# sourceMappingURL=NavigationViewModel.js.map