
function Turnout() {

    this.totalPollingStations = 0;
    this.totalVotersOnBaseList = 0;
    this.totalVotersReceivedBallots = 0;
    this.totalSupplementaryVoters = 0;
    this.percentCalculated = '';
    this.pollingStationsTurnout = null;
}
var turnout = new Turnout();
var map;
var progressIntervalId;
var updateInterval = 60;
var timeLeft = updateInterval;
var progressTimer = null;

$(document).ready(function () {
    var webSocketLiveResults = new WebSocket(globalServerSocket + 'api/LiveElectionTurnout');
    webSocketLiveResults.onmessage = onWsMessage;

    $(window).resize(function () {
        drawColumnChart(turnout.totalVotersOnBaseList, turnout.totalVotersReceivedBallots);
    });
});

function onWsMessage(event) {
    timeLeft = updateInterval;
    if (progressTimer != null) {
        clearInterval(progressTimer);
    }
    progressTimer = setInterval(function () { updateProgress(); }, 1000);

    var stringData = event.data;
    var json = $.parseJSON(stringData);
    if (progressIntervalId != undefined)
        clearInterval(progressIntervalId);

    turnout.totalPollingStations = json.TotalPollingStations;
    turnout.totalVotersOnBaseList = json.TotalVotersOnBaseList;
    turnout.totalVotersReceivedBallots = json.TotalVotersReceivedBallots;
    turnout.totalSupplementaryVoters = json.TotalVotersOnSupplementaryList;
    turnout.percentCalculated = json.TotalPercentCalculated;

    turnout.pollingStationsTurnout = json.PollingStationsTurnout;

    var screenHeight = "88vh";
    $('#googleMap').css('height', screenHeight);
    setTimeout(function () {
        $('#totalPS').text(turnout.totalPollingStations);
        $('#totalBaseVoters').text(turnout.totalVotersOnBaseList);
        $('#totalVoted').text(turnout.totalVotersReceivedBallots);
        $('#totalSupplementaryList').text(turnout.totalSupplementaryVoters);
        $('#votedPercentage').text(turnout.percentCalculated);
    
        initChartStatistics();
        if (map == undefined) {
            Init3();
        } else {
            turnout.pollingStationsTurnout.forEach(function(ps) {
                var popup = $('#' + ps.Number.replace('/', ''));
                if (popup.get(0)) {
                    var p = popup.find('p').eq(1);
                    popup.find('p').eq(1).text(ps.VotersOnBaseList + ' / ' + ps.VotersReceivedBallots);
                }
            });
        }
        drawColumnChart(turnout.totalVotersOnBaseList, turnout.totalVotersReceivedBallots);

    }, 1000);
}

function updateProgress() {
    var countdown = $('#countdown');

    var percentElapsed = timeLeft * 100 / updateInterval;

    $('.progress-bar-success').css("width", percentElapsed + '%');
    $('.progress-bar-success span').text('');
    $('.progress-bar-success').attr('aria-valuenow', percentElapsed);


    if (timeLeft === 0) {
        timeLeft = updateInterval;
    }

    countdown.html('<span>Următoarea actualizare în ' + timeLeft + ' secunde</span>');
    timeLeft--;
}

function Init3() {
    var markers = getMarkers();
    var osmap1 = new OpenLayers.Layer.OSM("OpenStreetMap"); //создание слоя карты

    var map2Layers = [osmap1];
    
    map = new OpenLayers.Map("googleMap");
    map.addLayers(map2Layers);
    
    var projectFrom = new OpenLayers.Projection("EPSG:4326"); // WGS 1984 projection

    var projectTo = map.getProjectionObject();

    var zoom = 10; 
    map.zoomToMaxExtent();
    
    var lonlatStartPosition = new OpenLayers.LonLat(28.9716983, 47.079175);
    map.setCenter(lonlatStartPosition.transform(projectFrom, projectTo), zoom);

    markers.forEach(function (marker) {
        var pollingStationsLocation = new OpenLayers.Geometry.Point(marker[1], marker[0]).transform(projectFrom, projectTo);
        var popup = new OpenLayers.Popup.FramedCloud("Popup",
            pollingStationsLocation.getBounds().getCenterLonLat(),
            new OpenLayers.Size(155, 65),
            marker[2],
            null,
            false // <-- true if we want a close (X) button, false otherwise
        );
        //popup.autoSize = true;
        map.addPopup(popup);
    });
}

function getMarkers() {
    var dataInfo;
    var popupId;
    var result = $.map(turnout.pollingStationsTurnout, function (v, k) {
        if (v.LocationLatitude != null && v.LocationLongitude != null) {
            popupId = v.Number.replace('/', '');
            dataInfo = '<div id="' + popupId + '" style="line-height:1.0"><p style="text-align: center;font-weight:bold;">' + v.Number + '</p><p style="text-align: center">' + v.VotersOnBaseList + ' / ' + v.VotersReceivedBallots + '</p></div>';
            return [[v.LocationLatitude, v.LocationLongitude, dataInfo]];
        }
    });

    return result;
}

function initChartStatistics() {
    var dataSet = $.map(turnout.pollingStationsTurnout, function(value, key) {
        return [[value.Number, value.Circumscription, value.Locality, value.VotersOnBaseList, value.VotersReceivedBallots, value.VotersOnSupplementaryList, value.PercentCalculated, value.LocationLatitude, value.LocationLongitude]];
    });

    var oTable = $('#pollingStationsTable').DataTable();
    oTable.clear();
    oTable.rows.add(dataSet).draw();
}


function ShowErrorMessage() {
    $('#datatable').hide();
    $('#totalStatistic').hide();
}

function drawColumnChart(voterOnBaseList, votersReceivedBallots) {
    var data = google.visualization.arrayToDataTable([
        ["Element", "Alegători", { role: "style" }],
        ["Total Alegători", voterOnBaseList, "#6600FF"],
        ["Au votat", votersReceivedBallots, "CC99FF"]
    ]);

    var formatter = new google.visualization.NumberFormat(
         {pattern: '###,###,###'});
    formatter.format(data, 1);
      
    var view = new google.visualization.DataView(data);
    view.setColumns([0, 1,
                     {
                         calc: "stringify",
                         sourceColumn: 1,
                         type: "string",
                         role: "annotation"
                     },
                     2]);
    var options = {
        fontSize: 18,
        title: "Alegători",
        height: $('#totalStatistic').parent().height(),
        width: $('#totalStatistic').parent().width(),
        bar: { groupWidth: "50%" },
        legend: { position: "none" },
        tooltip: { text: 'percentage' },
        chartArea: {
            left: '15%',
            width: '75%',
            height: '80%'
        },
        vAxis: {
            viewWindow: {
                min: 0,
                max: 3000000
            }
        }
    };
    var chart = new google.visualization.ColumnChart(document.getElementById("totalStatistic"));

    chart.draw(view, options);

}

