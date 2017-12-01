// Attendee graph implemented as a knockout binding
(function () {
    "use strict";

    var graphRoot = null,
        chart = null,
        round = function (n) {
            return Math.round(100 * n) / 100;
        };
    
    function drawBarChart(container, title, checkpointNames, speeds, times) {
        if (!speeds || !times || !speeds.length) {
            return;
        }

        // Add first speed to make the graph look better
        if (speeds.length > 1) {
            speeds[0] = speeds[1];
        }


        // Initialize graph
        chart = new Highcharts.Chart({
            chart: {
                renderTo: container
            },
            credits: {
                enabled: false
            },
            title: {
                text: title
            },
            xAxis: {
                categories: checkpointNames
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'km/h / min'
                }
            },
            legend: {
                borderWidth: 0
            },
            series: [{
                name: 'Huoltoaika (min)',
                type: 'bar',
                // Convert millisecond times to minutes
                data: times.map(function (t) {
                    return round(t / 60000);
                }),
                color: '#2b908f',
                borderWidth: 0
            }, {
                name: 'Keskivauhti (km/h)',
                type: 'spline',
                data: speeds,
                color: '#90ee7e'
            }]
        });
    };
    
    ko.bindingHandlers.attendeeGraph = {
        init: function (element) {
            graphRoot = document.createElement("div");
            element.appendChild(graphRoot);
        },
        update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
            var attendee = bindingContext.$data,
                checkpoints = ko.utils.unwrapObservable(valueAccessor());

            // Destroy current graph (if any)
            if (chart && chart.destroy()) {
                chart = null;
            }
            
            // Create new graph
            // For more descriptive name use e.g. "Vauhti ja lepo - " + attendee.name()
            chart = drawBarChart(graphRoot, null, checkpoints, attendee.speeds(), attendee.restTimes());
        }
    };
})();