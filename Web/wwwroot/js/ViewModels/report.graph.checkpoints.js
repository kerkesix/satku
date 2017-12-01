/*global Highcharts */
(function (context) {
    "use strict";

    var chart;

    var drawBarChart = function(data) {
        var container = this,
            categories = [], 
            fail = [], 
            ok = [], 
            waiting = [];

        if (!data || !data.checkpoints().length) {
            return null;
        }

        // Collect data
        data.checkpoints().forEach(function (checkpoint) {
            var readingCount = checkpoint.visits().length,
                quitters = 0;

            checkpoint.visits().forEach(function(v) {
                if (v.quit()) {
                    quitters++;
                }
            });
            
            // Collect category header and add data
            categories.push(checkpoint.name);
            ok.push(readingCount - quitters);
            waiting.push(checkpoint.waitingFor().length);
            fail.push(quitters);
        });
        
        // Initialize graph
        chart = new Highcharts.Chart({
              chart: {
                 renderTo: container,
                 defaultSeriesType: 'bar'
              },
              credits: {
                  enabled: false
              },
              title: {
                 text: 'Huoltopisteet'
              },
              xAxis: {
                  categories: categories
              },
              yAxis: {
                  min: 0,
                  max: ok.length ? ok[0] : 50,
                  title: {
                     text: 'Kävelijöitä'
                  }
              },
              legend: {
                  layout: 'vertical',
                  reversed: true,
                  floating: true,
                  align: 'right',
                  verticalAlign: 'bottom',
                  y: -50,
                  borderWidth: 0
              },
              plotOptions: {
                 series: {
                    stacking: 'normal'
                 }
              },
              series: [{
                 name: 'Keskeyttäneet',
                 data: fail,
                 color: '#f45b5b',
                 borderWidth: 0
              }, {
                 name: 'Odotetaan',
                 data: waiting,
                 color: '#90ee7e',
                 borderWidth: 0
              }, {
                 name: 'Leimanneet',
                 data: ok,
              color: '#2b908f',
                 borderWidth: 0
              }]
        });

        return chart;
    };

    context.registerChart("checkpointsChart", drawBarChart);
})(Ksx || (Ksx = {}));