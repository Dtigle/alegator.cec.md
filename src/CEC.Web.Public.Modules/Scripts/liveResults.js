function StatInfo() {

    this.baseInfo = null;
    this.percentInfo = null;
    this.preliminaryInfo = null;
    this.totalConnections = 0;

    this.preliminaryTotalCalculated = 0;
    this.totalParticipants = 0;
    this.percentCalculated = '';
}

var statInfo = new StatInfo();

$(document).ready(function () {
    
    var webSocketLiveResults = new WebSocket(globalServerSocket + 'api/LiveResults?pollId=0');
    
    webSocketLiveResults.onopen = onWsOpen;
    webSocketLiveResults.onerror = onWsError;
    webSocketLiveResults.onmessage = onWsMessage;

    // webSocketLiveResults.send($('#textMessage').val());
   
});

function reloadCharts() {

    $('#totalConnected').html(statInfo.totalConnections);

    if (statInfo.baseInfo != null) {
        MorrisChartCEC1('#morris-chart-cec-1');
    }

    if (statInfo.percentInfo != null) {
        MorrisChartCEC2('#morris-chart-cec-2');
    }

    if (statInfo.preliminaryInfo != null) {
        MorrisChartCEC3('#morris-chart-cec-3');
        
    } else {
        showNoPreliminaryMessage();
    }

    $('img.imageCache').imageCache();

}

function onWsOpen() {
    // $('#messages').prepend('<div>' + username + ' connected.</div>');

}

function onWsError(event) {
    // $('#messages').prepend('<div>CONNECTION ERROR</div>');
}

function onWsMessage(event) {
    var stringData = event.data;
    var json = $.parseJSON(stringData);

    statInfo.baseInfo = json.BaseInfo;
    statInfo.percentInfo = json.PercentInfo;
    statInfo.preliminaryInfo = json.Preliminary;
    statInfo.totalConnections = json.TotalConnections;

    statInfo.preliminaryTotalCalculated = json.PreliminaryTotalCalculated;
    statInfo.totalParticipants = json.TotalParticipants;
    statInfo.percentCalculated = json.PercentCalculated;

    reloadCharts();
    
    // $('#messages').prepend('<div>' + json.Message + '</div>');
}

function onWsSend(callback) {
    callback($('#textMessage').val());
    $('#textMessage').val('');
}

function MorrisChartCEC1(graphControl) {

    if (existMorris(graphControl) == false) return;

    $(graphControl).empty();

    Morris.Bar({
        element: graphControl.substr(1),
        data: statInfo.baseInfo,
        barColors: ['#666666', '#aaaaaa'],
        xkey: 'TimeOfData',
        ykeys: [ 'VotersByBaseList', 'VotersByAddList'],
        labels: [ 'Lista de baza', 'Lista adaugatoare'],
        xLabelAngle: 0
    });
}

function MorrisChartCEC2(graphControl) {
     
    if (existMorris(graphControl) == false) return;

    $(graphControl).empty();

    $('#totalVoters').html(statInfo.percentInfo.TotalCount);
    $('#lastRefreshDate').html(statInfo.percentInfo.LastTime);
    
    var total=statInfo.percentInfo.TotalCount;
    var votat = statInfo.percentInfo.VotedCount;
    var notVoted = total - votat;

    var results2 = [
			{ value: notVoted, label: 'N-au votat', formatted: statInfo.percentInfo.NotVotedPercent },
			{ value: votat, label: 'Au votat', formatted: statInfo.percentInfo.VotedPercent },
    ];

    Morris.Donut({
        element: graphControl.substr(1),
        colors: ['#999999', '#642887'],
        data: results2,
        formatter: function (x, data) { return data.value + ' | ' + data.formatted + '%'; }
    });

}


function MorrisChartCEC3(graphControl) {

    if (existMorris(graphControl) == false) return;

    $('#preliminaryInfo').hide();
    $('#preliminaryResults').show();

    $(graphControl).empty();
    $(graphControl+'-legend').empty();

    var dd = statInfo.preliminaryInfo;

    var result = [{ 'Votare': '' }];

    var colors = [];
    var partNames = []; 
     
    for (var i = 0; i < dd.length; i++) {
        colors[i] = dd[i].CandidateColor;
        partNames[i] = dd[i].CandidateName;
        result[0][dd[i].CandidateName] = dd[i].CandidatePercentResult; 
    }

    $('#preCalculatTotal').html(
        statInfo.preliminaryTotalCalculated + ' din ' +
        statInfo.totalParticipants + ' (' + statInfo.percentCalculated + '%)');

    var bar = Morris.Bar({
        element: graphControl.substr(1),
        data: result,
        barColors: colors,
        xkey: 'Votare',
        ykeys: partNames,
        labels: partNames,
        xLabelAngle: 0
    });

    bar.options.labels.forEach(function (label, i) {

        var legendItem = '<div class="partidLogoDiv box "><img class="imageCache partidLogo" src="'
            + dd[i].CandidateId + '" /><H2>'
            + dd[i].CandidatePercentResult + '% </H2><H4  class="btn info-container">'
            + dd[i].CandidateResult + '</H4></div>';
        $(graphControl+'-legend').append(legendItem);
    });

    $('.morris-hover').hide();
}
 
function showNoPreliminaryMessage() {

    $('#preliminaryInfo').show();
    $('#preliminaryResults').hide();

    $('#infoMessage').html(
        '<h2>Calcularea rezultatelor preliminare încă nu s-a început!<br/> Accesați din nou după închiderea secțiilor de votare.</h2>');
    $('#helpPage').load('UI/CallCenter.html');
}

function existMorris(graphControl) {
    
    if ($(graphControl).length > 0)
        return true;
    else {
        return false;
    }
}

(function ($) {
    $.fn.imageCache = function(options) {
        this.config = {
            base64ImageEncoderPath: globalServerResult + 'images/file?logoId='
        };
        $.extend(this.config, options);

        var self = this;
        var $self = $(this);

        $(document).ready(function() {
            $(self).each(function(i, img) {
                var src = $(img).attr('src') || $(img).attr('data-src');
                if (localStorage) {
                    var localSrc = localStorage[src];
                    if (localSrc && localSrc != 'pending') {
                        $(img).attr('src', localSrc);
                    } else {
                        $(img).attr('src', src);
                        if (localStorage[src] !== 'pending') {
                            localStorage[src] = 'pending';
                            $.get(self.config.base64ImageEncoderPath + src,
                                function(data) {
                                    localStorage[src] = data;
                                    $(img).attr('src', data);
                                });
                        }
                    }
                }
            });
        });

        return this;
    };
})(jQuery);