$(document).ready(function () {

});


function Init3() {

    var markers = getMarkers();

    $("#googleMap").gmap3({
        map: {
            options: {
                center: [47.079175, 28.9716983],
                zoom: 7,
                mapTypeId: google.maps.MapTypeId.TERRAIN
            }
        },
        marker: {
            values: markers,
            cluster: {
                radius: 100,
                // This style will be used for clusters with more than 0 markers
                0: {
                    content: "<div class='cluster cluster-1'>CLUSTER_COUNT</div>",
                    width: 53,
                    height: 52
                },
                // This style will be used for clusters with more than 20 markers
                20: {
                    content: "<div class='cluster cluster-2'>CLUSTER_COUNT</div>",
                    width: 56,
                    height: 55
                },
                // This style will be used for clusters with more than 50 markers
                50: {
                    content: "<div class='cluster cluster-3'>CLUSTER_COUNT</div>",
                    width: 66,
                    height: 65
                },
                events: {
                    click: function (cluster) {
                        var map = $(this).gmap3("get");
                        map.setCenter(cluster.main.getPosition());
                        map.setZoom(map.getZoom() + 1);
                    }
                }
            },
            options: {
                //icon: new google.maps.MarkerImage("http://maps.gstatic.com/mapfiles/icon_green.png")
                icon: new google.maps.MarkerImage("Content/img/m_00.png")
                //icon: new google.maps.Circle()
            },
            events: {
                mouseover: function (marker, event, context) {
                    $(this).gmap3(
                      { clear: "overlay" },
                      {
                          overlay: {
                              latLng: marker.getPosition(),
                              options: {
                                  content: "<div class='infobulle'></div>" +
                                            "<div class='arrow'></div>",
                                  offset: {
                                      x: -46,
                                      y: -73
                                  }
                              }
                          }
                      });
                },
                mouseout: function () {
                    $(this).gmap3({ clear: "overlay" });
                }
            }
        }
    });
}

function getMarkers() {
    var result = [];

    $.ajax({
        url: globalServerRootUrl + "Statistic/Statistic",
        type: "GET",
        async: false,
        dataType: "json",

        success: function (json) {
            result = json;
        },

        error: function (xhr, status, errorThrown) {
            console.log('error on get markers');
        }

    });

    return result;
}

function initChartStatistics() {
    debugger;

    var data = getStatsData();
    Morris.Bar({
        element: 'chart',
        data: data,
        xkey: 'Country',
        ykeys: ['Count'],
        xLabelMargin: 5,
        xLabelAngle: 25,
        padding: 60,
        labels: ['Nr. declarații']
    });

    $('#datatable').html('<table cellpadding="0" cellspacing="0" border="0" class="display" id="example"></table>');

    var dataSet = $.map(data, function(value, key) {
        return [[value.Country, value.Count]];
    });

    $('#example').dataTable({
        "data": dataSet,
        "columns": [
            { "title": "Țara" },
            { "title": "Numărul de declarații" },
        ],
        "language": {
            "search": "Căutare",
            "lengthMenu": "_MENU_  înregistrări pe pagină",
            "info": "_START_ - _END_ din _TOTAL_ înregistrări",
            "infoFiltered": "(găsit din _MAX_  înregistrări)",
            "infoEmpty": "0 înregistrări",
            "paginate": {
                "first": "Prima",
                "previous": "Precedent",
                "next": "Următorul",
                "last": "Ultima"
            }
        }
    });
}

function getStatsData() {
    var result = [];
    $.ajax({
        url: globalServerRootUrl + "Statistic/StatisticGrouped",
        type: "GET",
        async: false,
        dataType: "json",

        success: function (json) {
            result = json;
        },

        error: function (xhr, status, errorThrown) {
            ShowErrorMessage();
        }

    });
    return result;
}

function ShowErrorMessage() {
    $('#datatable').hide();
    $('#chart').hide();
    $('#datatableError').html('Eroare de afișare de date');
    $('#chartError').html('Eroare de afișare de date');
}

function drawChart() {
    var jsonData = $.ajax({
        url: globalServerRootUrl + "Statistic/TimelineStatistics",
        dataType: "json",
        async: false
    }).responseJSON;

    // Create our data table out of JSON data loaded from server.
    var data = new google.visualization.DataTable();
    data.addColumn('date', 'Data');
    data.addColumn('number', 'Număr de înregistrări');
    
    var array = [];
    array = $.map(jsonData, function (value, key) {
        return [[new Date(value.Date), value.Count]];
    });

    data.addRows(array);

    var chart = new google.visualization.AnnotationChart(document.getElementById('chart_div'));

    var options = {
        displayAnnotations: true,
        displayZoomButtons: false
    };

    chart.draw(data, options);
}