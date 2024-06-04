function PreliminaryInfo() {

    this.totalBallotPapers = 0;
    this.totalProcessedBallotPapers = 0;
    this.percentProcessedBallotPapers = 0;
    this.totalVotes = 0;
    this.totalValidVotes = 0;
    this.totalSpoiledVotes = 0;
    this.preliminary = null;
    this.independentCandidateMinimalThreshold = 0;
    this.partyMinimalThreshold = 0;
    this.partyBloc1MinimalThreshold = 0;
    this.partyBloc2MinimalThreshold = 0;
}

var preliminaryInfo = new PreliminaryInfo();

$(document).ready(function () {

    ConnectWS();

    // webSocketLiveResults.send($('#textMessage').val());

});

function ConnectWS() {
    var webSocketLiveResults = new WebSocket(globalServerSocket + 'api/LiveResults?pollId=0');

    webSocketLiveResults.onopen = onWsOpen;
    webSocketLiveResults.onerror = onWsError;
    webSocketLiveResults.onmessage = onWsMessage;
}

function loadCharts(preliminary) {

    if (preliminaryInfo.resultsProcessingStarted) {
        google.load('visualization', '1', {packages: ['corechart'], callback:drawLabColumnChart(preliminary)});
    } else {
        showMessageBeforPreliminary();
    }
}

function onWsOpen() {
    // $('#messages').prepend('<div>' + username + ' connected.</div>');

}

function onWsError(event) {
    // $('#messages').prepend('<div>CONNECTION ERROR</div>');
    ConnectWS();
}

function onWsMessage(event) {
    var stringData = event.data;
    var json = $.parseJSON(stringData);

    preliminaryInfo.resultsProcessingStarted = json.ResultsProcessingStarted;
    preliminaryInfo.totalBallotPapers = json.TotalBallotPapers;
    preliminaryInfo.totalProcessedBallotPapers = json.TotalProcessedBallotPapers;
    preliminaryInfo.percentProcessedBallotPapers = json.PercentProcessedBallotPapers;
    preliminaryInfo.totalVotes = json.TotalVotes;
    preliminaryInfo.totalValidVotes = json.TotalValidVotes;
    preliminaryInfo.totalSpoiledVotes = json.TotalSpoiledVotes;
    preliminaryInfo.preliminary = json.Preliminary;
    //preliminaryInfo.partyMinimalThreshold = Math.round(json.TotalVotes * 6 / 100);
    //preliminaryInfo.independentCandidateMinimalThreshold = Math.round(json.TotalVotes * 2 / 100);
    //preliminaryInfo.partyBloc1MinimalThreshold = Math.round(json.TotalVotes * 9 / 100);
    //preliminaryInfo.partyBloc2MinimalThreshold = Math.round(json.TotalVotes * 11 / 100);
    preliminaryInfo.partyMinimalThreshold = Math.round(json.TotalValidVotes * 6 / 100);
    preliminaryInfo.independentCandidateMinimalThreshold = Math.round(json.TotalValidVotes * 2 / 100);
    preliminaryInfo.partyBloc1MinimalThreshold = Math.round(json.TotalValidVotes * 9 / 100);
    preliminaryInfo.partyBloc2MinimalThreshold = Math.round(json.TotalValidVotes * 11 / 100);
    
    $('#preliminaryProcessInfo').hide();
    $('#preliminaryProcessResults').show();
    
    $('#totalBallotsProcessed').html(
        preliminaryInfo.totalProcessedBallotPapers + ' din ' +
        preliminaryInfo.totalBallotPapers + ' (' + preliminaryInfo.percentProcessedBallotPapers + ')');
    $('#totalVotes').text(preliminaryInfo.totalVotes);
    $('#totalValidVotes').text(preliminaryInfo.totalValidVotes);
    $('#totalSpoiledVotes').text(preliminaryInfo.totalSpoiledVotes);
    loadCharts(preliminaryInfo.preliminary);
}

function onWsSend(callback) {
    callback($('#textMessage').val());
    $('#textMessage').val('');
}

function drawLabColumnChart(preliminaryData) {
    var arr = [];
    var hideColumnArr = [2, 3, 4, 5];
    var auxPercent = Math.round(preliminaryInfo.totalVotes * 0.25 / 100);
    var labelPartyThreshold = 'Pragul minim pentu Partid: ' + preliminaryInfo.partyMinimalThreshold + ' (6%)';
    var labelIndependentCandidateThreshold = 'Pragul minim pentu Candidați Independenți: ' + preliminaryInfo.independentCandidateMinimalThreshold + ' (2%)';
    var labelPartyBloc1Threshold = 'Pragul minim pentu bloc electoral format din 2 partide: ' + preliminaryInfo.partyBloc1MinimalThreshold + ' (9%)';
    var labelPartyBloc2Threshold = 'Pragul minim pentu bloc electoral format din 3 si mai multe partide: ' + preliminaryInfo.partyBloc2MinimalThreshold + ' (11%)';
    arr.push(['Formațiune', 'Voturi', labelPartyThreshold, labelIndependentCandidateThreshold, labelPartyBloc1Threshold, labelPartyBloc2Threshold, { role: 'style' }]);
    arr.push(['', null, preliminaryInfo.partyMinimalThreshold, preliminaryInfo.independentCandidateMinimalThreshold,
        preliminaryInfo.partyBloc1MinimalThreshold, preliminaryInfo.partyBloc2MinimalThreshold, null]);
    $.each(preliminaryData, function() {
        switch (this.PartyType) {
        case 0:
            if ($.inArray(2, hideColumnArr) > -1) {
                hideColumnArr = $.grep(hideColumnArr, function(el) { return el !== 2; });
            }
            break;
        case 1:
            if ($.inArray(3, hideColumnArr) > -1) {
                if (this.CandidateResult >= preliminaryInfo.independentCandidateMinimalThreshold - auxPercent) {
                    hideColumnArr = $.grep(hideColumnArr, function(el) { return el !== 3; });
                }
            }
            break;
        case 2:
            if ($.inArray(4, hideColumnArr) > -1) {
                if (this.CandidateResult >= preliminaryInfo.partyBloc1MinimalThreshold - auxPercent) {
                    hideColumnArr = $.grep(hideColumnArr, function(el) { return el !== 4; });
                }
            }
            break;
        case 3:
            if ($.inArray(5, hideColumnArr) > -1) {
                if (this.CandidateResult >= preliminaryInfo.partyBloc2MinimalThreshold - auxPercent) {
                    hideColumnArr = $.grep(hideColumnArr, function(el) { return el !== 5; });
                }
            }
            break;
        }
        arr.push([
            this.CandidateName + ' \n(' + this.CandidatePercentResult + '%)',
            this.CandidateResult,
            preliminaryInfo.partyMinimalThreshold,
            preliminaryInfo.independentCandidateMinimalThreshold,
            preliminaryInfo.partyBloc1MinimalThreshold,
            preliminaryInfo.partyBloc2MinimalThreshold,
            this.CandidateColor
        ]);
    });
    arr.push([
        '',
        null,
        preliminaryInfo.partyMinimalThreshold,
        preliminaryInfo.independentCandidateMinimalThreshold,
        preliminaryInfo.partyBloc1MinimalThreshold,
        preliminaryInfo.partyBloc2MinimalThreshold,
        null
    ]);

    var data = google.visualization.arrayToDataTable(arr);

    var view = new google.visualization.DataView(data);
    view.setColumns([0, 1, {
            calc: 'stringify',
            sourceColumn: 6,
            type: 'string',
            role: 'style'
        },
        {
            calc: 'stringify',
            sourceColumn: 1,
            type: 'string',
            role: 'annotation'
        },
        2, 3, 4, 5
    ]);

    view.hideColumns(hideColumnArr);

    var options = {
        fontSize : 14,
        height: 600,
        bar: { groupWidth: "50%" },
        legend: { position: "top", maxLines: 3, textStyle: { fontSize: 14 } },
        animation: {
            duration: 1000,
            easing: 'out'
        },
        vAxis: {
            gridlines: { count: 8 },
            titlecolore: 'blue',
            title: 'Voturi',
            minValue: 0,
            baselineColor: 'brown',
            minorGridLines: { count: 5 }
        },
        series: {
            0: { visibleInLegend: false },
            1: { type: 'line', areaOpacity: 0, color: 'red', enableInteractivity: false, lineWidth: 1, visibleInLegend: true },
            2: { type: 'line', areaOpacity: 0, color: 'green', enableInteractivity: false, lineWidth: 1, visibleInLegend: true },
            3: { type: 'line', areaOpacity: 0, color: 'blue', enableInteractivity: false, lineWidth: 1, visibleInLegend: true },
            4: { type: 'line', areaOpacity: 0, color: 'yellow', enableInteractivity: false, lineWidth: 1, visibleInLegend: true },
        },
        hAxis: {
            viewWindow: {
                min: 1,
                max: preliminaryData.count
            }
        },
        chartArea: {
            left: '10%',
            width: '85%',
            height: '60%'
        },
        tooltip: { isHtml: true, textStyle: { showColorCode: true, fontSize: 14 }, text: 'percentage' },        
    };
    var chart = new google.visualization.ColumnChart(document.getElementById("resultsChart"));

    chart.draw(view, options);
}
 
function showMessageBeforPreliminary() {

    $('#preliminaryProcessInfo').show();
    $('#preliminaryProcessResults').hide();

    $('#informationMessage').html(
        '<h2>Calcularea rezultatelor preliminare încă nu s-a început!<br/> Accesați din nou după închiderea secțiilor de votare.</h2>');
    $('#preliminaryHelpPage').load('UI/CallCenter.html');
}
