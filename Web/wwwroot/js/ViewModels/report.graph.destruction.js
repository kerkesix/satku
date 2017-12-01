/*global Highcharts */
(function (context) {
    "use strict";

    var chart,
        round = function (n) {
            return Math.round(100*n)/100;
        };

    var drawDestruction = function(data) {
        var container = this,
            categories = [],
            time = [],
            speed = [],
            quit = [];

        if (!data || !data.checkpoints().length) {
            return null;
        }
       
        // Collect data
        data.checkpoints().forEach(function (c) {
            var quitters = 0;
            
            if (c.avgTime()) {
                c.visits().forEach(function (v) {
                    if (v.quit()) {
                        quitters++;
                    }
                });

                categories.push(c.name);
                time.push(round(c.avgTime()/60000));
                speed.push(round(c.avgSpeed()));
                quit.push(quitters);
            }
        });

        chart = new Highcharts.Chart({
          chart: {
             renderTo: container,
             defaultSeriesType: 'spline',
             marginBottom: 120
          },
          credits: {
                enabled: false
          },
          title: {
             text: 'Tummuminen'
          },
          xAxis: {
             categories: categories,
            labels: {
                rotation: -40,
                x: -20,
                y: 40
            }
          },
          yAxis: {
             min: 0,
             title: {
                text: 'kävelijöitä / min / km/h'
             },
             plotLines: [{
                value: 0,
                width: 1,
                color: '#808080'
             }]
          },
          tooltip: {
             formatter: function() {
                       return '<b>'+ this.series.name +'</b><br/>'+
                   this.x +': '+ this.y;
             }
          },
          legend: {
             layout: 'horizontal',
             verticalAlign: 'bottom',
             borderWidth: 0
          },
          series: [{
             name: 'Keskeyttäneitä',
             data: quit,
             color: '#f45b5b'
          }, {
             name: 'Vauhti (km/h)',
             data: speed,
             color: '#90ee7e'
          }, {
             name: 'Huoltoaika (min)',
             data: time,
             color: '#2b908f'
          }]
        });
        
        return chart;
    };

    context.registerChart("destructionChart", drawDestruction);
})(Ksx || (Ksx = {}));