/// <reference path="Messaging/Queues.ts" />

interface IRoute {
    // Static on navigation routes, may contain sammy.js syntax on others
    urlMap: string[];
    navigationUrl?: string;
    icon: string;
    order: number;
    tag?: string;
    text: string; 
    active: boolean;
    root: string;
    showOnNavigation: boolean;

    // Use any instead of (context) => void 
    // as typescript does not understand correctly
    // function.bind
    onViewOpen: any;
}

// This object is written to HTML on page render. Ugly, but works to get some route data from the 
// server to JS.
declare var Globals: any;

module Ksx {
    export class Routes {
        static registered: any = {};
        static currentHappening: string = Globals.Happening;
        static defaultHappening: string = Globals.DefaultHappening;

        // Urls that should not get happening appended
        static rootUrls: string[] = ["/"];

        static isRootUrl = (url: string) => {
            return Routes.rootUrls.indexOf(url) != -1;
        };

        static addHappeningToUrl = (url: string) => {
            // Append happening id to url if needed
            if (!Routes.isRootUrl(url) || (url === "/" && Routes.defaultHappening !== Routes.currentHappening)) {
                return "/" + Routes.currentHappening + url;
            }

            return url;
        };

        // Array of configured url creation interceptors.
        // Always include one interceptor, others added via addUrlInterceptor.
        private static _urlInterceptors: { (url: string): string; }[] = [Routes.addHappeningToUrl];
        private static _intercepted: boolean = false;

        static addUrlInterceptor(interceptor: (string) => string) {
            if (typeof interceptor === 'function') {
                Routes._urlInterceptors.push(interceptor);
            }
        }
        
        static register(name: string, route: IRoute): void {
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

            // If all interceptors have already been run(e.g.this is 
            // a dynamically added route after page load), run the 
            // all interceptors now
            if (Routes._intercepted) {
                Routes.interceptRoute(route);
            }

            this.registered[name] = route;
        }

        // Registers a route that operates on root, e.g. not under happening path. 
        // An example of such url would be sign in page.
        static registerRootUrl(name: string, route: IRoute): void {
            Routes.rootUrls.push(route.navigationUrl || route.urlMap[0]);
            Routes.register(name, route);
        }

        static interceptRoute(route: IRoute) {
            Routes._urlInterceptors.forEach(modify => {
                route.navigationUrl = modify(route.navigationUrl);
                route.urlMap.forEach((u, index, array) => array[index] = modify(u));
            });
        }

        static all(): IRoute[] {

            var mapped = <any>$.map(this.registered || [], (r: IRoute) => {
                // Not real interception as this modifies the original object. Take a clone instead?
                if (!Routes._intercepted) {
                    Routes.interceptRoute(r);
                }

                return r;
            });

            Routes._intercepted = true;

            // Sort by order and return
            return mapped.sort((a: IRoute, b: IRoute) => a.order - b.order);
        }

        static navigableRoutes(): IRoute[]{
            return Routes.all().filter((r: IRoute) => r.showOnNavigation);
        }

        static url(name, action, id): string {
            if (!name) {
                throw new Error("Must specify route name for URL creation");
            }

            var s = "";
            s = Routes.appendToUrl(s, name);
            s = Routes.appendToUrl(s, action);
            s = Routes.appendToUrl(s, id);
            
            s = Routes.addHappeningToUrl(s);

            return s && s !== '' ? s : "/";
        }

        private static appendToUrl(current: string, appendedText: string) : string {
            if (appendedText && appendedText !== '') {
                current = current + "/" + appendedText;
            }

            return current;
        }
    }
}