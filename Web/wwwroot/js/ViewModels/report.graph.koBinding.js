// <reference path="report.graph.js"/>
// Summary page graph implemented as a knockout binding
(function () {
    "use strict";

    var chartContainers = {},
        charts = [];
    
    function refreshGraphWidths() {
        charts.forEach(refreshGraphWidth);
    }

    // Force graph layout recalculation
    function refreshGraphWidth(c) {
        if (!c) {
            // There is no data at the page just exit
            return;
        }

        var container = $(c.container).parent(),
            width = container.width(),
            height = container.height();

        if (width) {
            c.setSize(width, height);
        }
    } 

    ko.bindingHandlers.summaryGraph = {
        init: function (element) {
            
            // Loop through all the registered charts and create container for each
            Ksx.charts.forEach(function(chartId) { 
                chartContainers[chartId] = $("<div class='graphContainer'/>", { id: chartId }).appendTo(element).get(0);
            });
        },
        update: function (element, valueAccessor) {
            var data = ko.utils.unwrapObservable(valueAccessor());

            Messaging.Queues.navigated.unsubscribe(refreshGraphWidths);

            // Destroy current graphs (if any)
            charts.forEach(function (item) {
                if (item) {
                    item.destroy();
                }
            });

            charts = [];
            
            // Loop through all the registered charts
            Ksx.charts.forEach(function (chartId) {
                var registeredChartCreator = this,
                    chart = registeredChartCreator.call(chartContainers[chartId], data);

                // Needed on update
                charts.push(chart);
            });
            
            // Listen to page changes, and fix graph width on page change.
            // Do after timeout to avoid breaking current rendering.
            setTimeout(function() {
                Messaging.Queues.navigated.subscribe(refreshGraphWidths);
            }, 100);
        }
    };
})();