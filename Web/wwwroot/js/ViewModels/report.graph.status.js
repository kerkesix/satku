/*global Highcharts */
(function (context) {
    "use strict";

    var chart;
    var round = function(n) {
        return Math.round(100*n)/100;
    };

    var drawStatus = function(data) {
        var container = this,
            calculateOverallStatus = function(dashboardViewModel) {
                if (!dashboardViewModel || !dashboardViewModel.checkpoints().length) {
                    // Avoid errors for happening that is not even started yet
                    return {
                        attendeesTotal: 0,
                        finished: 0,
                        quit: 0,
                        walking: 0
                    };
                }

                var checkpoints = dashboardViewModel.checkpoints(),
                    cps = checkpoints.length,
                    attendeesTotal = cps ? checkpoints[0].visits().length : 0,
                    finished = cps ? checkpoints[cps - 1].visits().length : 0,
                    quit = checkpoints.reduce(function(prev, current) {
                        return prev + current.quitters().length;
                    }, 0);

                return {
                    attendeesTotal: attendeesTotal,
                    finished: finished,
                    quit: quit,
                    walking: attendeesTotal - (finished + quit)
                };
            },
            status = calculateOverallStatus(data),
            pieData = [
                ['Ei lähtenyt', round(100 * (status.attendeesTotal - (status.walking + status.quit + status.finished)) / status.attendeesTotal)],
                ['Perillä', round(100 * status.finished / status.attendeesTotal)],
                ['Kävelee', round(100 * status.walking / status.attendeesTotal)],
                {
                    name: 'Keskeyttänyt',
                    y: round(100 * status.quit / status.attendeesTotal),
                    sliced: true,
                    selected: true
                }
            ];
            
        // Do not show "Did not start" if it is zero. Normally this is zero and just confuses
        if (pieData[0][1] === 0) {
            pieData.shift();
        }

        // Do not draw if data is not valid (error or event not started)
        if (!status || status.attendeesTotal === 0) {
            return null;
        }
        
        chart = new Highcharts.Chart({
                    chart: {
                        renderTo: container,
                        plotBackgroundColor: null,
                        plotBorderWidth: null,
                        plotShadow: false
                    },
                    credits: {
                        enabled: false
                    },
                    title: {
                        text: 'Kokonaistilanne'
                    },
                    legend: {
                        borderWidth: 0                        
                    },
                    tooltip: {
                        formatter: function() {
                            return '<b>'+ this.point.name +'</b>: '+ this.y +' %';
                        }
                    },
                    plotOptions: {
                        pie: {
                            allowPointSelect: true,
                            cursor: 'pointer',
                            dataLabels: {
                                enabled: false
                            },
                            showInLegend: true
                        }
                    },
                    series: [{
                        type: 'pie',
                        name: 'Status',
                        data: pieData,
                        borderWidth: 0
                    }]
        });

        return chart;
    };

    context.registerChart("statusChart", drawStatus);
})(Ksx || (Ksx = {}));