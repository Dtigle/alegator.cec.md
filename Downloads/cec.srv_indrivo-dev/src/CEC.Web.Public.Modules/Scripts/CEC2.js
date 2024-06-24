$(document).ready(function () {
   

});


function InitializeRegionControl()
{
    clearErrors();
   
    $('#txtRegion').change(function () {
        clearErrors();
        setInfoVisibility(false, false);
        $('#region2DIV').show();
        $('#streetDIV').hide();
        $('#psDIV').hide();

        $('#txtStreet').select2('data', null);
        $('#txtRegion2').select2('data', null);
    });

    $('#txtRegion').select2({
        minimumInputLength: 0,
        placeholder: 'raion / municipiu',        
        id: function (region) { return region.Id; },
        ajax: {
            url: function(regionName) {
               var region = normalizeInputSearch(regionName);
                return globalServerRootUrl + "WhereToVote/RegionsOfLevel1/" + encodeURIComponent((region.length > 0) ? region : "_");
            },              
            dataType: 'json',
            type: "GET",
            quietMillis: 100,
            results: function (regions) {
                return { results: regions }
            }
        },
        formatResult: function (region) {
            return "<div class='select2-user-result'>" + region.Name + "</div>";
        },
        formatSelection: function (region) {
            return region.Name;
        },
        formatInputTooShort: function (input, min) { var n = min - input.length; return "Introduceți " + n + " sau mai multe simboluri (litere sau spații)"; },
        formatNoMatches: function () { return "Nici un rezultat găsit"; },
        formatSearching: function () { return "În căutare…"; }
    });
}

function InitializeRegion2Control() {
    $('#region2DIV').hide();

    $('#txtRegion2').change(function () {
        clearErrors();
        setInfoVisibility(false, false);
        $('#txtStreet').select2('data', null);
        $('#txtPollingStation').select2('data', null);
        setStreetAndPollingStationsVisibility();
    });

    $('#txtRegion2').select2({
        minimumInputLength: 0,
        placeholder: 'localitatea',
        id: function (region2) { return region2.Id; },
        ajax: {
            url: function (region2Name) {
                var parentId = $('#txtRegion').val();
                var region2 = normalizeInputSearch(region2Name);
                return globalServerRootUrl + "WhereToVote/Regions/" + parentId + "/" + encodeURIComponent((region2.length > 0) ? region2 : "_");
            },
            dataType: 'json',
            type: "GET",
            quietMillis: 100,
            results: function (region2) { return { results: region2 } }
        },
        formatResult: function (region2) {
            return "<div class='select2-user-result'>" + region2.Name + "</div>";
        },
        formatSelection: function (region2) {
            return region2.Name;
        },
        formatInputTooShort: function (input, min) { var n = min - input.length; return "Introduceți " + n + " sau mai multe simboluri (litere sau spații)"; },
        formatNoMatches: function () { return "Nici un rezultat găsit"; },
        formatSearching: function () { return "În căutare…"; }
    });
}

function InitializeStreetControl() {
    $('#streetDIV').hide();

    $('#txtStreet').change(function () {
        getAddresses();
    });
	
    $('#txtStreet').select2({
    	placeholder: 'strada',
    	ajax: {
	    	url: globalServerRootUrl + "WhereToVote/GetRegions",
	    	dataType: 'json',
	    	type: "POST",
	    	data: function (term, page) {
	    		return {
	    			regionId:$('#txtRegion2').val(),
	    			q: term,
	    			pageLimit: 10,
	    			page: page
	    		};
	    	},
	    	results: function (data, page) {
		    	var more = (page * 10) < data.Data.Total;
		    	return {
		    		results: data.Data.Items,
		    		more: more
		    	};
		    }
	    },
	    formatNoMatches: function () { return "Nici un rezultat găsit"; },
	    formatSearching: function () { return "În căutare…"; },
	    formatLoadMore: function () { return "Se încarcă mai multe rezultate ..."; },
	});
}

function getAddresses()
{
    clearErrors();

    setInfoVisibility(false, false);

    var streetId = $('#txtStreet').val();

    if ((streetId != null) && (streetId != '')) {
        $.ajax({
            url: globalServerRootUrl + "WhereToVote/Addresses/" + streetId,
            type: "GET",
            dataType: "json",

            success: function (json) {
                AddAddressesInTheList(json);
            },

            error: function (xhr, status, errorThrown) {
                $('#streetError').html("Eroare de server. Căutarea adreselor a eșuat. ");
            },
            complete: function (xhr, status) {
            }
        });
    }
}

function AddAddressesInTheList(addresses)
{
    setStreetsVisibility(false);
    $('div#streetList').empty();

    $(addresses).each(function () {
        $("<button></button>").addClass("btn btn-primary").attr("type", "button").text(this.Name).appendTo($('div#streetList'))
            .click(function (address) { return function () { addressSelected.call(this, address); }; }(this));
    });
    setStreetsVisibility(true);
}

function addressSelected(address)
{
    var pollingStation = address.PollingStation;
    showPollingStationInfo(pollingStation);
}

function getPollingStations() {
   
    var regionId = $('#txtRegion2').val();
 
    if (regionId != null) {
        $.ajax({
            url: globalServerRootUrl + "WhereToVote/PollingStations/" + regionId,
            type: "GET",
            dataType: "json",

            success: function (json) {
                AddPollingStationsInTheList(json);
            },

            error: function (xhr, status, errorThrown) {
                $('#region2Error').html("Eroare de server. Căutarea secțiilor de votare a eșuat. ");
            },
            complete: function (xhr, status) {
            }
        });
    }
}

function AddPollingStationsInTheList(pollingStations) {
    $('#psDIV').empty();

    $(pollingStations).each(function () {
        var divElement = $("<div/>").addClass("col-md-5 box-content").attr("style", "height:275px").appendTo($('#psDIV'));
        
        var tableElement = $("<table/>").addClass("table table-hover").appendTo(divElement);
        var tbodyElement = $("<tbody/>").appendTo(tableElement);

        var trElement = $("<tr/>").appendTo(tbodyElement);
        var tdElement = $("<td/>").appendTo(trElement);
        $("<h4/>").text("Secția de votare").appendTo(tdElement);
        tdElement = $("<td/>").appendTo(trElement);
        $("<h4/>").text('№ ' + this.Number).appendTo(tdElement);

        trElement = $("<tr/>").appendTo(tbodyElement);
        $("<td/>").text("Localul secției de votare").appendTo(trElement);
        tdElement = $("<td/>").appendTo(trElement);
        $("<div/>").addClass("info-container").text(this.LocationDescription).appendTo(tdElement);

        trElement = $("<tr/>").appendTo(tbodyElement);
        $("<td/>").text("Adresa").appendTo(trElement);
        tdElement = $("<td/>").appendTo(trElement);
        $("<div/>").addClass("info-container").text(this.Address).appendTo(tdElement);

        trElement = $("<tr/>").appendTo(tbodyElement);
        $("<td/>").text("Circumscriptia").appendTo(trElement);
        tdElement = $("<td/>").appendTo(trElement);
        $("<div/>").addClass("info-container").text(this.Circumscription).appendTo(tdElement);

        trElement = $("<tr/>").appendTo(tbodyElement);
        $("<td/>").text("Contacte").appendTo(trElement);
        tdElement = $("<td/>").appendTo(trElement);
        $("<div/>").addClass("info-container").text(this.PhoneNumber).appendTo(tdElement);
    });
}

function showPollingStationInfo(pollingStation) {
    clearPollingStationInfo();

    if (pollingStation != null) {
        $('#stationNumber').html('№ ' + pollingStation.Number);
        $('div#locationDescription').html(pollingStation.LocationDescription);
        $('div#circumscription').html(pollingStation.Circumscription);
        $('div#addressFull').html(pollingStation.Address);
        $('div#phoneNumber').html(pollingStation.PhoneNumber);

        if (pollingStation.LatX == null || pollingStation.LongY == null) {
            hideMap();
        }
        else {
            $('div#map-2').show();
            initGoogleMaps(pollingStation.LatX, pollingStation.LongY, pollingStation);
        }
    }
    else {
        $('#stationNumber').html("Datele lipsesc");
    }
}

function setPollingStationVisibility(visibility)
{
    setVisibility($('div#PollingStationDiv'), visibility);
}

function setStreetsVisibility(visibility)
{
    setVisibility($('div#StreetDiv'), visibility);
}

function setPollingStationsVisibility(visibility) {
    setVisibility($('div#PollingStationListDiv'), visibility);
}

function setInfoVisibility(pollingStationVisibility, streetsVisibility)
{
    setPollingStationVisibility(pollingStationVisibility);
    setStreetsVisibility(streetsVisibility);
}

function setVisibility(div, visibility)
{
    if (visibility)
    {
        div.slideDown();
    }
    else
    {
        div.hide();
    }
}

function clearPollingStationInfo() {
    $('#stationNumber').empty();
    $('div#locationDescription').empty();
    $('div#addressFull').empty();
    $('div#phoneNumber').empty();
    $('div#map-2').empty();

    setPollingStationVisibility(true);
}

function clearErrors() {
    $('#streetError').empty();
    $('#regionError').empty();
}

function normalizeInputSearch(filter) {
    return filter.replace(/ +(?= )/g, '').trim();
}

function setStreetAndPollingStationsVisibility()
{
    var regionId = $('#txtRegion2').val();
    $.ajax({
        url: globalServerRootUrl + "WhereToVote/HasStreets/" + regionId,
        type: "GET",
        dataType: "json",

        success: function (json) {
            if (json) {
                $('#streetDIV').show();
                $('#psDIV').hide();
            } else {
                $('#streetDIV').hide();
                $('#psDIV').show();
                getPollingStations();
            }
        },

        error: function (xhr, status, errorThrown) {
            $('#streetError').html("Eroare de server. Căutarea adreselor a eșuat. ");
        },
        complete: function (xhr, status) {
        }
    });
}