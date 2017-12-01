/// <reference path="../lib/sammy/lib/sammy.js"/>
/// <reference path="../lib/sammy/lib/plugins/sammy.title.js"/>
/// <reference path="Ksx.Routes.js"/>
/// <reference path="Messaging/Queues.js"/>

// Wire up all registered routes to sammy application
var app = Sammy('#maincontent', function () {
    var self = this;

    var showView = function (route, sammy) {
        var viewName = route.root,
            viewToOpen = document.getElementById(viewName),
            jqViewToOpen;

        if (viewToOpen) {
            jqViewToOpen = $(viewToOpen);

            if (!jqViewToOpen.is(":visible")) {
                // Must get all pages at runtime as pages might be injected dynamically
                $(".spapage").hide();
                jqViewToOpen.show();
                
                // Hide all open modals (if modal library has been loaded)
                var modals = $(".modal");
                if (modals.modal) {
                    modals.modal('hide');
                }
            }

            // Not given on 404 routes
            if (sammy) {
                // Change page title
                sammy.title(route.text);
            }
        }
    };

    self.use(Sammy.Title);
    self.setTitle("KSX Satkuseuranta -");

    self.use('Track');

    // Sammy.js emulates pushState on non-supporting browsers, but it requires 
    // registering routes without prefixing slash. Sammy stores this information 
    // to [initialized sammy app]._location_proxy.has_history
    // Remove prefixing slash if pushState is not supported
    if (!self._location_proxy.has_history) {
        Ksx.Routes.addUrlInterceptor(function (s) {
            // Use hashbang
            if (s && s.charAt(0) === '/') {
                s = "#!" + s;
            }
            
            return s;
        });
    }

    function mapRoute(route) {
        // One view model might map multiple urls
        route.urlMap.forEach(function (url) {
            self.get(url, function (sammy) {
                showView(route, sammy);

                if (typeof route.onViewOpen === "function") {
                    route.onViewOpen(sammy);
                }

                Messaging.Queues.navigated.publish(route);

                // Do not follow link
                return false;
            });
        });
    }

    Ksx.Routes.all().forEach(mapRoute);

    // Define 404 route
    self.notFound = function () {
        showView({ root: "404", text: "Sivua ei löytynyt (404)" } );
    };
});

// Use server routed url for start, but default to hashbang on old browsers
app.run((!app._location_proxy.has_history && window.location.pathname === '/') ? "/#!/" : window.location.pathname);
    