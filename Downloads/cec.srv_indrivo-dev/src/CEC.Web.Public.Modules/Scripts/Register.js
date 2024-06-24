$(document).ready(function () {
    //initCheckRegisterTheList();
    //setTimeout(function () {
    //    initialize();
    //}, 500);

    //LoadBootstrapValidatorScript(DemoFormValidator);
});

var autocomplete;

function initialize() {
    
    var input = document.getElementById('txtLocation');
    autocomplete = new google.maps.places.Autocomplete(input, { types: ['(cities)'] });
    google.maps.event.addListener(autocomplete, 'place_changed', function () {
        $('#emailAddress').show();
        $('#marketplace').show();

        var place = autocomplete.getPlace();

        for (var i = 0; i < place.address_components.length; i++){
            var addressType = place.address_components[i].types[0];
            if (addressType == 'country') 
                $('#locationCountry').val(place.address_components[i]['long_name']);
        }

        $("#locationid").val(place.id);
        $("#locationLat").val(place.geometry.location.lat());
        $("#locationLong").val(place.geometry.location.lng());
        
        $('#MainContent_lblStrainatate').html(place.formatted_address);
    });
}

function LoadBootstrapValidatorScript(callback) {
    if (!$.fn.bootstrapValidator) {
        $.getScript('Scripts/bootstrapValidator.js', callback);
    }
    else {
        if (callback && typeof (callback) === "function") {
            callback();
        }
    }
}

function DemoFormValidator() {
    $('#emailAddress').bootstrapValidator({
        message: 'This value is not valid',
        fields: {
            email: {
                validators: {
                    notEmpty: {
                        message: 'Adresa de e-mail este obligatorie'
                    },
                    emailAddress: {
                        message: 'Adresa de e-mail nu este validă'
                    }
                }
            }
        },
        submitHandler: function (validator, form, submitButton) {
            
        }

    })
    .on('keyup', '[name="email"]', function () {
        $('#emailAddress').bootstrapValidator('validateField', 'email');

    })
    .on('success.field.bv', function (e) {
        $('#MainContent_ConfirmCheckBox').removeAttr("disabled");
    })
    .on('error.field.bv', function (e) {
        $('#MainContent_ConfirmCheckBox').prop("checked", false);
        $('#MainContent_ConfirmCheckBox').prop("disabled", true);
    });
}

function initCheckRegisterTheList() {
	
	verificationIsSetAbroadDeclaration();

    addValidationToIdnp();

    $('#goSearchByIdnp').click(getPersonFullInfoByIdnp);
    $('#txtIdnpPersonal').enterKey(getPersonFullInfoByIdnp);

    $("#MainContent_ConfirmCheckBox").change(function () {
        $("#btnSubmit").prop("disabled", !this.checked);
    });

    
    $("#btnSubmit").click(saveAbroadRegistration);

    initializeElectionName();

    $('#marketplace').hide();
    $('#locationSearch').hide();
    $('#emailAddress').hide();

    hideResultTables();
    hideErrorByIdnp();
}

function verificationIsSetAbroadDeclaration() {
    $.ajax({
    	url: globalServerRootUrl + "PollingStation/GetAbroadDeclaration",
        type: "GET",

        success: function (data) {
        	if (data) {
        		$('#infoMessageAbroadDeclaration').hide();
        		$('#electionContent').show();
        	} else {
        		$('#electionContent').hide();
        		$('#infoMessageAbroadDeclaration').show();
        	}
        }
    });
}

function initializeElectionName() {
	$.ajax({
		url: globalServerRootUrl + "PollingStation/Election",
		type: "GET",
		dataType: "json",

		success: function (json) {
			getCurrentElection(json);
		},

		error: function (xhr, status, errorThrown) {
			showCriticalErrorMessageOfVoter();
		}

	});
}

function getCurrentElection(json)
{
    if (json != null && json.Name != null && json !== undefined)
    {
        $("#electionName").html(json.Name);
        $("#electionDate").html(json.Data);
        $("#electionId").val(json.Id);
    } else 
        $('#electionSection').hide();
}

function saveAbroadRegistration() {
    
    var message = {
        IDNP: $("#idnp").val(),
        AbroadAddress: $("#MainContent_lblStrainatate").html(),
        AbroadAddressid: $("#locationid").val(),
        AbroadAddressLat: $("#locationLat").val(),
        AbroadAddresCountry : $('#locationCountry').val(),
        AbroadAddressLong: $("#locationLong").val(),
        ResidenceAddress: $('#txtAddress').val(),
        Election: $("#electionId").val(),
        Email: $("#email").val(),
        Election: $("#electionName").html() + " din " + $("#electionDate").html()
    };


    var dialClose = function (dialogItself) {
        dialogItself.close();
        window.location.reload();
    }

    var dialogus = new BootstrapDialog({
        title: ' ',
        message: " <br/> <div id='progressbar' class='progress progress-striped active'><div class='progress-bar progress-bar-info active' style='width: 100%;' aria-valuemax='100' aria-valuemin='0' aria-valuenow='100' role='progressbar'><span>In progres</span></div></div> <br/>",
        closable: false,
        buttons: [{
            id: 'closeButton',
            label: 'Inchide',
            action: dialClose
        }]
    });

    dialogus.realize();
    dialogus.getButton('closeButton').hide();;
    dialogus.getModalHeader().hide();
    dialogus.getModalHeader().hide();

    dialogus.getModalFooter().hide();
    dialogus.getModalFooter().hide();
    dialogus.open();

        $.ajax({
            url: globalServerRootUrl + "PollingStation/SaveReport/",
            type: "POST",
            data: message,
            dataType: "json",

            success: function (json) {
                IsRunning = false;
                dialogus.setMessage("Cererea Dvs. a fost procesată");
                dialogus.getModalFooter().show();
                dialogus.getModalHeader().show();
                dialogus.getButton('closeButton').show();

            },

            error: function (xhr, status, errorThrown) {
                IsRunning = false;
                dialogus.close();
                checkForInputErrorsofVoters();
            }
        });
    
}

function addValidationToIdnp(e) {
    $.mask.definitions['~'] = '[02]';
    $("#txtIdnpPersonal").mask("~999999999999");
}

function getPersonFullInfoByIdnp() {
    var idnp1 = $('#txtIdnpPersonal').val();
    $('#searchingProgress').show();

    if (checkForInputErrorsofVoters(idnp1) == false) {
        return;
    }

    $.ajax({
        url: globalServerRootUrl + "PollingStation/AssignedForRegistration/" + idnp1,
        type: "GET",
        dataType: "json",

        success: function (json) {
            getPersonFullInfoPollingStation(json, idnp1);
            $('#btnSubmit').show();
            $('#MainContent_Panel2').show();
        },

        error: function (xhr, status, errorThrown) {
            checkForInputErrorsofVoters();
        }

    });
}

function getPersonFullInfoPollingStation(json, idnp) {
      

    clearFields();

    if (json != null && json.Name != null && json !== undefined && json.WarningMessage != null) {

        $('#locationSearch').show();

        showResultTables();
        $('#searchingProgress').hide();

        hideErrorByIdnp();

        $('#nume').html(json.Name);
        $('#prenume').html(json.Surname);
        $('#idnp').html(idnp);
        $('#anulNasterii').html(json.DOB);
        $('#MainContent_lblDomiciliu').html(json.Residence);

        $('#txtName').val(json.Name);
        $('#txtSurname').val(json.Surname);
        $('#idnp').val(idnp);
        $('#txtDob').val(json.DOB);
        $('#txtAddress').val(json.Residence);

        
    }
    else {
        $('#MainContent_Panel2').hide();
        showInfoMessage(idnp, json.WarningMessage);
    }
}

function showCriticalErrorMessageOfVoter() {

    $('#MainContent_Panel2').hide();
    $('#btnSubmit').hide();

    showErrorByIdnp();

    $('#infoMessage').html(
        '<b>Eroare de server. Accesați mai târziu!</b>');
    $('#helpPage').load('UI/CallCenter.html');
}

function showIncorrectIdnpMessageOfVoter(idnp) {

    hideResultTables();
    showErrorByIdnp();

    $('#btnSubmit').hide();
    $('#MainContent_Panel2').hide();

    $('#infoMessage').html(
        'Verificați corectitudinea IDNP-ului introdus - <b>' + idnp + '</b>');
    $('#helpPage').load('UI/CallCenter.html');
}

function checkForInputErrorsofVoters(idnp) {

    if (idnp.length != 13) {
        showIncorrectIdnpMessageOfVoter(idnp);
        return false;
    }

    var numberReg = new RegExp('^(09|20){1}[0-9]{11}$');

    if (idnp.match(numberReg) == null) {
        showIncorrectIdnpMessageOfVoter(idnp);
        return false;
    }
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