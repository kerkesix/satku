(function () {
    var sammy = Sammy || {};

    // A simple plugin that pings tracker every time a route is triggered. 
    sammy.Track = function () {

        this.helpers({
            track: Ksx.Track.pageView
        });

        this.bind('event-context-after', function () {
            this.track(this.path);
        });
    };

})();