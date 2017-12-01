var Ksx;
(function (Ksx) {
    var Track = (function () {
        function Track() {
        }
        Track.pageView = function (path) {
            var appInsights = window["appInsights"];
            var ga = window["ga"];
            if (typeof appInsights != "undefined") {
                appInsights.trackPageView(window.document.title, path);
            }
            if (typeof ga != "undefined") {
                ga("send", "pageview", path);
            }
        };
        Track.pageEvent = function (category, action, label) {
            var appInsights = window["appInsights"];
            var ga = window["ga"];
            if (typeof appInsights != "undefined") {
                appInsights.trackEvent(category + "/" + action + "/" + label);
            }
            if (typeof ga != "undefined") {
                ga("send", "event", category, action, label);
            }
        };
        return Track;
    }());
    Ksx.Track = Track;
})(Ksx || (Ksx = {}));
//# sourceMappingURL=Ksx.Track.js.map