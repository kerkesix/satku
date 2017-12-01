var Ksx;
(function (Ksx) {
    var Routes = (function () {
        function Routes() {
        }
        Routes.addUrlInterceptor = function (interceptor) {
            if (typeof interceptor === 'function') {
                Routes._urlInterceptors.push(interceptor);
            }
        };
        Routes.register = function (name, route) {
            if (!name) {
                throw new Error("Must give name for route.");
            }
            if (!route) {
                return;
            }
            if (this.registered[name]) {
                throw new Error("Duplicate route: " + name);
            }
            if (!route.navigationUrl) {
                route.navigationUrl = route.urlMap[0];
            }
            if (!route.tag) {
                route.tag = "public";
            }
            if (Routes._intercepted) {
                Routes.interceptRoute(route);
            }
            this.registered[name] = route;
        };
        Routes.registerRootUrl = function (name, route) {
            Routes.rootUrls.push(route.navigationUrl || route.urlMap[0]);
            Routes.register(name, route);
        };
        Routes.interceptRoute = function (route) {
            Routes._urlInterceptors.forEach(function (modify) {
                route.navigationUrl = modify(route.navigationUrl);
                route.urlMap.forEach(function (u, index, array) { return array[index] = modify(u); });
            });
        };
        Routes.all = function () {
            var mapped = $.map(this.registered || [], function (r) {
                if (!Routes._intercepted) {
                    Routes.interceptRoute(r);
                }
                return r;
            });
            Routes._intercepted = true;
            return mapped.sort(function (a, b) { return a.order - b.order; });
        };
        Routes.navigableRoutes = function () {
            return Routes.all().filter(function (r) { return r.showOnNavigation; });
        };
        Routes.url = function (name, action, id) {
            if (!name) {
                throw new Error("Must specify route name for URL creation");
            }
            var s = "";
            s = Routes.appendToUrl(s, name);
            s = Routes.appendToUrl(s, action);
            s = Routes.appendToUrl(s, id);
            s = Routes.addHappeningToUrl(s);
            return s && s !== '' ? s : "/";
        };
        Routes.appendToUrl = function (current, appendedText) {
            if (appendedText && appendedText !== '') {
                current = current + "/" + appendedText;
            }
            return current;
        };
        Routes.registered = {};
        Routes.currentHappening = Globals.Happening;
        Routes.defaultHappening = Globals.DefaultHappening;
        Routes.rootUrls = ["/"];
        Routes.isRootUrl = function (url) {
            return Routes.rootUrls.indexOf(url) != -1;
        };
        Routes.addHappeningToUrl = function (url) {
            if (!Routes.isRootUrl(url) || (url === "/" && Routes.defaultHappening !== Routes.currentHappening)) {
                return "/" + Routes.currentHappening + url;
            }
            return url;
        };
        Routes._urlInterceptors = [Routes.addHappeningToUrl];
        Routes._intercepted = false;
        return Routes;
    }());
    Ksx.Routes = Routes;
})(Ksx || (Ksx = {}));
//# sourceMappingURL=Ksx.Routes.js.map