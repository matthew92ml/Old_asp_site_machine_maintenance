var blue = '#348fe2',
    blueLight = '#5da5e8',
    blueDark = '#003596',
    aqua = '#49b6d6',
    aquaLight = '#6dc5de',
    aquaDark = '#3a92ab',
    green = '#00acac',
    greenLight = '#33bdbd',
    greenDark = '#016b10',
    orange = '#f59c1a',
    orangeLight = '#f7b048',
    orangeDark = '#c47d15',
    dark = '#2d353c',
    grey = '#b6c2c9',
    purple = '#727cb6',
    purpleLight = '#8e96c5',
    purpleDark = '#5b6392',
    white = '#ffffff',
    yellow = '#ffdb31',
    brown = '#763c04',
    red = '#ff5b57';

function CreateInteractiveChart(objid, timelineData, miny, maxy) {
    "use strict";

    if (timelineData == null) return;

    function showTooltip(x, y, contents) {
        $('<div id="tooltip" class="flot-tooltip">' + contents + '</div>').css({
            top: y - 45,
            left: x - 55
        }).appendTo("body").fadeIn(200);
    }

    if ($(objid).length !== 0) {
        var i = 0;
        var timelineStructure = [];
        for (i = 0; i < timelineData.length; i++) {
            var timelineItem = {
                label: timelineData[i].label,
                data: timelineData[i].data,
                color: timelineData[i].color,
                lines: { show: true, fill: false, lineWidth: 1.5 },
                points: { show: false, radius: 3, fillColor: '#fff' },
                shadowSize: 0
            }

            timelineStructure.push(timelineItem);
        }

        if (miny == null && maxy == null) {
            $.plot($(objid), timelineStructure,
                {
                    xaxis: { mode: "time", ticks: 5, tickDecimals: 0, tickColor: 'rgba(0,0,0,0.2)' },
                    yaxis: { tickColor: 'rgba(0,0,0,0.2)', ticks: 5 },
                    grid: {
                        hoverable: true,
                        clickable: true,
                        tickColor: "rgba(0,0,0,0.2)",
                        borderWidth: 1,
                        backgroundColor: '#fafafa',
                        borderColor: '#ddd'
                    },
                    legend: {
                        labelBoxBorderColor: '#ddd',
                        margin: 10,
                        noColumns: 1,
                        position: "nw",
                        backgroundOpacity: 0.5,
                        show: false
                    }
                }
            );
        } else {
            $.plot($(objid), timelineStructure,
                {
                    xaxis: { mode: "time", ticks: 5, tickDecimals: 0, tickColor: 'rgba(0,0,0,0.2)' },
                    yaxis: { tickColor: 'rgba(0,0,0,0.2)', ticks: 5, min: miny, max: maxy },
                    grid: {
                        hoverable: true,
                        clickable: true,
                        tickColor: "rgba(0,0,0,0.2)",
                        borderWidth: 1,
                        backgroundColor: '#fafafa',
                        borderColor: '#ddd'
                    },
                    legend: {
                        labelBoxBorderColor: '#ddd',
                        margin: 10,
                        noColumns: 1,
                        position: "nw",
                        backgroundOpacity: 0.5,
                        show: false
                    }
                }
            );
        }
        
        var previousPoint = null;
        $(objid).bind("plothover", function (event, pos, item) {
            $("#x").text(pos.x.toFixed(2));
            $("#y").text(pos.y.toFixed(2));
            if (item) {
                if (previousPoint !== item.dataIndex) {
                    previousPoint = item.dataIndex;
                    $("#tooltip").remove();
                    var y = item.datapoint[1].toFixed(2);

                    var content = y + " Kg";
                    showTooltip(item.pageX, item.pageY, content);
                }
            } else {
                $("#tooltip").remove();
                previousPoint = null;
            }
            event.preventDefault();
        });
    }
};

function CreateDashboardSparkline(garbagedata) {
    "use strict";

    if (garbagedata == null) return;

    var options = {
        height: '50px',
        width: '100%',
        fillColor: 'transparent',
        lineWidth: 2,
        spotRadius: '4',
        spotColor: false,
        minSpotColor: false,
        maxSpotColor: false
    };

    function renderDashboardSparkline(p_garbagedata) {        
        options.type = 'line';
        options.height = '23px';
        options.width = '100%';
        options.lineColor = aquaDark;
        options.highlightLineColor = aquaLight;
        options.highlightSpotColor = aqua;

        var i = 0;

        for (i = 0; i < p_garbagedata.length; i++) {
            if ($('#sparkline-' + p_garbagedata[i][0]) != null) $('#sparkline-' + p_garbagedata[i][0]).sparkline(p_garbagedata[i][1], options);
        }
    }

    renderDashboardSparkline(garbagedata);

    $(window).on('resize', function () {
        var i = 0;

        for (i = 0; i < garbagedata.length; i++) {
            if ($('#sparkline-' + garbagedata[i][0]) != null) $('#sparkline-' + garbagedata[i][0]).empty();
        }
        renderDashboardSparkline(garbagedata);
    });
}

function CreateDonutChart(garbagedata) {
    "use strict";

    if (garbagedata == null) return;

    if ($('#donut-chart').length !== 0) {
        var donutData = garbagedata;

        $.plot('#donut-chart', donutData, {
            series: {
                pie: {
                    innerRadius: 0.5,
                    show: true,
                    label: {
                        show: false
                    }
                }
            },
            legend: {
                position: "nw",
                backgroundOpacity: 0.5,
                show: true
            }
        });
    }
}
