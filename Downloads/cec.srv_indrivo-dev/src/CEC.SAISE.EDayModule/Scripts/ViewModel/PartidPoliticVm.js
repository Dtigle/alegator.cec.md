
var PartidPoliticVm = function (dataItem) {
    var self = this;

    self.UpdatedParty = ko.observable();

    self.Id = ko.observable(dataItem.Id);
    self.Code = ko.observable(dataItem.Code);
    self.NameRo = ko.observable(dataItem.NameRo);
    self.NameRu = ko.observable(dataItem.NameRu);
    self.DateOfRegistration = ko.observable(dataItem.DateOfRegistration);

    self.fileData = ko.observable({
        dataURL: ko.observable(),
        // base64String: ko.observable(),
    });

    self.onClear = function (fileData) {
        if (confirm('Are you sure?')) {
            fileData.clear && fileData.clear();
        }
    };

    self.Status = ko.observable(dataItem.Status);
    self.ElectionId = ko.observable();
    self.RegionId = ko.observable();

    self.SaveEditClick = function () {

        debugger;

        var data = new FormData();
       
        data.append("Id", self.Id());
        data.append("Code", self.Code());
        data.append("NameRo", self.NameRo());
        data.append("NameRu", self.NameRu());
        data.append("ElectionId", self.ElectionId());
        data.append("RegionId", self.RegionId());
        data.append("DateOfRegistration", moment(self.DateOfRegistration()).format('DD.MM.YYYY'));
        data.append("Status", self.Status());
        data.append("UploadedImage", self.fileData().file());

        // Make Ajax request with the contentType = false, and procesDate = false
        var ajaxRequest = $.ajax({
            type: "POST",
            url: "http://localhost/CEC.SAISE.EDayModule/RegisterPoliticalParty/SaveUpdatePoliticalParty",
            contentType: false,
            processData: false,
            data: data,
            success: function (newUpdatedParty) {
                debugger;

                self.UpdatedParty(new PartyVM(newUpdatedParty));
                $('#myModalForPartid').modal('hide');
            }
        });

        ajaxRequest.done(function (xhr, textStatus) {
            
        });

    }
}