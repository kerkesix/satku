(function (context) {
    "use strict";

    context.charts = {
        registered: {},
        forEach: function(iteratorFunction) {
            for (var chartId in this.registered) {
                if (this.registered.hasOwnProperty(chartId)) {
                    iteratorFunction.call(this.registered[chartId], chartId);
                }
            }
        }
    };

    context.registerChart = function(name, f) {
        context.charts.registered[name] = f;
    };

})(Ksx || (Ksx = {}));