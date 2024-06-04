$(document).ready(function () {

});

function initCheckTheList() {

    addValidationToIdnp();

    //$('#informationPart').load('UI/CallCenter.html #subHelpPart');

    $('#goSearchByIdnp').click(getPersonByIdnp1);
    $('#txtIdnpPersonal').enterKey(getPersonByIdnp1);
    $('#marketplace').hide();

    $('#showIdnpHelpImage').click(showOrHideIdnpHelpImage);
    $('#idnpHelpImage').hide();

    hideResultTables();
    hideErrorByIdnp();
}

function addValidationToIdnp(e) {
    $.mask.definitions['~'] = '[02]';
    $("#txtIdnpPersonal").mask("~999999999999");
}

function initGoogleMaps(lat, long, pollingStation) {

    // Create map in element with ID - map-2
    var osmap1 = new OpenLayers.Layer.OSM("OpenStreetMap"); //создание слоя карты

    var map2_layers = [osmap1];
    var map2 = drawMap(long, lat, "map-2", map2_layers, 18, pollingStation);

    $("#map-2").resize(function () {
        setTimeout(map2.updateSize(), 500);
    });
}

var onloadCallback = function () {
    $.ajax({
        url: globalServerRootUrl + "PollingStation/captchaKey/",
        dataType: "json",
        type: "GET",
        success: function (json) {
            grecaptcha.render('captchadiv', {
                'sitekey': json
            });
        }
    });
};

function getPersonByIdnp1() {

    var idnp = $('#txtIdnpPersonal').val();
    var challenge = '';//Recaptcha.get_challenge();
    //var response = Recaptcha.get_response();

    // 11.04.2018 - Trecerea la Recaprcha v2
    var response = $("#g-recaptcha-response").val();
    //var response = ""; //"g-recaptcha-response";

    $('#marketplace').show();
    $('#searchingProgress').show();

    // If dont match Idnp length or RegExp 
    if (checkForInputErrors(idnp) == false) {
        return;
    }

    $.ajax({
        url: globalServerRootUrl + "PollingStation/Assigned/",
        dataType: "json",
        type: "POST",
        data: {
            Idnp: idnp,
            Challenge: challenge,
            Response: response
        },

        success: function (json) {

            //Recaptcha.reload();
            grecaptcha.reset();
            getPersonPollingStation(json, idnp);
        },

        error: function (xhr, status, errorThrown) {
            showCriticalErrorMessage();
        }
    });
    // JOHN
    console.log("ready for next phase...");
}

function getPersonPollingStation(json, idnp) {

    clearFields();

    if (json != null && json.Name != null && json !== undefined && json.WarningMessage != null) {

        showResultTables();
        hideErrorByIdnp();

        $('div#initialePersonal').html(json.Name);
        $('div#anulNasterii').html(json.DOB);
        $('div#domiciliul').html(json.Residence);
        InregistrareaPrealabila_RemoveUI();
        setTimeout(function () {
            VerificaInregistrareaPrealabila(idnp);
        }, 3);

        if (json.PollingStation != null) {
            $('#stationNumber').html('№ ' + json.PollingStation.Number);
            $('div#locationDescription').html(json.PollingStation.LocationDescription);
            $('div#circumscription').html(json.PollingStation.Circumscription);
            $('div#addressFull').html(json.PollingStation.Address);
            $('div#phoneNumber').html(json.PollingStation.PhoneNumber);
        

            if (json.PollingStation.LatX == null || json.PollingStation.LongY == null)
                hideMap();
            else
                initGoogleMaps(json.PollingStation.LatX, json.PollingStation.LongY, json.PollingStation);
        }
        else {
            $('#stationNumber').html('Datele lipsesc');
        }
        if (json.VoterCertificate != false) {
            $('div#certificat').html('Este eliberat certificat cu drept de vot');
            document.getElementById("certificat").parentNode.parentNode.style.visibility = 'unset';
        }
        else {

            document.getElementById("certificat").parentNode.parentNode.style.visibility = 'collapse';
        }

        if (json.electionListNr != null) {
            $('div#electionListNr').html(json.electionListNr);
            document.getElementById("electionListNr").parentNode.parentNode.style.visibility = 'unset';
        }
        else {
            document.getElementById("electionListNr").parentNode.parentNode.style.visibility = 'collapse';
        }
        if (json.Circumscription != null) {
            $('div#electionCircNr').html(json.Circumscription);
            document.getElementById("electionCircNr").parentNode.parentNode.style.visibility = 'unset';
        }
        else {
            
            document.getElementById("electionCircNr").parentNode.parentNode.style.visibility = 'collapse';
        }
       
             
    }

    
    else {
        showInfoMessage(idnp, json.WarningMessage, json.ReCaptchaError);
    }
}

function showCriticalErrorMessage() {

    hideResultTables();
    showErrorByIdnp();

    $('#infoMessage').html(
        '<b>Confirmați ca nu sinteti robot!</b>');
    $('#helpPage').load('UI/CallCenter.html');
}

function showInfoMessage(idnp, warningMessage, reCaptchaError) {
    hideResultTables();
    showErrorByIdnp();

    $('#infoMessage').html(
        'Persoana cu IDNP <b>' + idnp + '</b> nu a fost găsită. ');

    if (warningMessage != null)
        if (reCaptchaError)
            $('#infoMessage').html(warningMessage);
        else {
            $('#infoMessage').html('<b>SERVER:</b> ' + warningMessage + '');
        }

    $('#helpPage').load('UI/CallCenter.html');
}

function showIncorrectIdnpMessage(idnp) {

    hideResultTables();
    showErrorByIdnp();

    $('#infoMessage').html(
        'Verificați corectitudinea IDNP-ului introdus - <b>' + idnp + '</b>');
    $('#helpPage').load('UI/CallCenter.html');
}

function checkForInputErrors(idnp) {

    if (idnp.length != 13) {
        showIncorrectIdnpMessage(idnp);
        return false;
    }

    var numberReg = new RegExp('^(09|20){1}[0-9]{11}$');

    if (idnp.match(numberReg) == null) {
        showIncorrectIdnpMessage(idnp);
        return false;
    }
}

function showOrHideIdnpHelpImage() {

    $('#idnpHelpImage').toggle();
}

function clearFields() {

    $('div#initialePersonal').empty();
    $('div#anulNasterii').empty();
    $('div#domiciliul').empty();
    $('#votersCount').empty();

    $('#stationNumber').empty();
    $('div#locationDescription').empty();
    $('div#addressFull').empty();
    $('div#phoneNumber').empty();
    $('div#electionListNr').empty();
    $('div#certificat').empty();
    $('div#map-2').empty();
}

function hideMap() {
    $('div#map-2').empty();
    $('div#map-2').hide();
}

function showResultTables() {
    $('#searchingProgress').hide();
    $('table#tableResult').show();
    $('div#map-2').show();
}

function hideResultTables() {
    $('table#tableResult').hide();
    $('div#map-2').hide();
}

function showErrorByIdnp() {
    $('#searchingProgress').hide();
    $('div#searchByIdnpResult').show();
}

function hideErrorByIdnp() {
    $('div#searchByIdnpResult').hide();
}

$.fn.enterKey = function (fnc) {
    return this.each(function () {
        $(this).keypress(function (ev) {
            var keycode = (ev.keyCode ? ev.keyCode : ev.which);
            if (keycode == '13') {
                fnc.call(this, ev);
            }
        });
    });
}

// JOHN. 18.05.2016. Verificarea inregistrarii prealabile peste hotarele tarii
//-------------------------------------------------------------------------------------------------------------------------
var CONST_IP_TITLE = 'Înregistrarea prealabilă în afara țării';
var CONST_IP_LINK = 'Pentru informații despre înregistrarea prealabilă în afara țării, accesați site-ul oficial: <a href="https://inregistrare.cec.md" target="_blank">inregistrare.cec.md</a>';

function VerificaInregistrareaPrealabila(idnp) {
    // 1. construim UI
    InregistrareaPrealabila_CreateUI();
    // 2. call service
    $.ajax({
        type: 'POST',
        crossDomain: true,
        dataType: 'json',
        url: "https://inregistrare.cec.md/Person/InregistrareInfo",
        data: { idnp: idnp }
    })
        .fail(function (jqXHR, textStatus, errorThrown) {
            // fail        
            InregistrareaPrealabila_Error(textStatus + '. ' + errorThrown)
        })
        .done(function (data, textStatus, jqXHR) {
            console.log(data);
            if (data.status == "success") {
                InregistrareaPrealabila_Response(data);
                return;
            }
            InregistrareaPrealabila_Error(data.message);
        })
        .always(function () { });
}

function InregistrareaPrealabila_RemoveUI() {
    $("#tableResult tr[inregistrareprealabila]").remove();
}

function InregistrareaPrealabila_CreateUI() {
    $("#tableResult")
        .append($('<tr inregistrareprealabila="1"><td colspan="2"><h4>' + CONST_IP_TITLE + '</h4></td></tr>'))
        .append($('<tr inregistrareprealabila="1"><td colspan="2" id="idInregistrareInfo"><div class="preloader">&nbsp;</div></td></tr>'))
        ;
}

function InregistrareaPrealabila_Error(error) {
    $("#idInregistrareInfo").empty().addClass("text-danger").text(error);
}

function InregistrareaPrealabila_Response(data) {
    InregistrareaPrealabila_RemoveUI();
    if (data.esteInregistrare) {
        $("#tableResult")
            .append($('<tr inregistrareprealabila="1"><td colspan="2"><h4 class="text-success">' + CONST_IP_TITLE + '</h4></td></tr>'))
            .append($('<tr inregistrareprealabila="1"><td colspan="2">' + data.info + '</td></tr>'))
            .append($('<tr inregistrareprealabila="1"><td>Localitatea</td><td><div class="info-container">' + data.localitate + '</div></td></tr>'))
            .append($('<tr inregistrareprealabila="1"><td>Țara</td><td><div class="info-container">' + data.tara + '</div></td></tr>'))
            .append($('<tr inregistrareprealabila="1"><td>Adresa deplină</td><td><div class="info-container">' + data.adresa + '</div></td></tr>'))
            .append($('<tr inregistrareprealabila="1"><td>Data înregistrării</td><td><div class="info-container">' + data.data + '</div></td></tr>'))
            .append($('<tr inregistrareprealabila="1"><td colspan="2">' + CONST_IP_LINK + '</td></tr>'))
            ;
        return;
    }
    $("#tableResult")
        .append($('<tr inregistrareprealabila="1"><td colspan="2"><h4 class="text-danger">' + CONST_IP_TITLE + '</h4></td></tr>'))
        .append($('<tr inregistrareprealabila="1"><td colspan="2">' + data.info + '</td></tr>'))
        .append($('<tr inregistrareprealabila="1"><td colspan="2">' + CONST_IP_LINK + '</td></tr>'))
        ;
}

//-------------------------------------------------------------------------------------------------------------------------
// end of JOHN
//-------------------------------------------------------------------------------------------------------------------------
