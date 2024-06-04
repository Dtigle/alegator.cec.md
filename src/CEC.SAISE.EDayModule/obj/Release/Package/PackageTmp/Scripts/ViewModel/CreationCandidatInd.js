var CreationCandInd = function () {
    var self = this;

    self.Id = ko.observable();
    self.Code = ko.observable();

    self.LastNameRo = ko.observable();
    self.NameRo = ko.observable();

    self.LastNameRu = ko.observable();
    self.NameRu = ko.observable();

    self.DateOfBirth = ko.observable();
    self.DateOfRegistration = ko.observable(new Date());

    self.Occupation = ko.observable();
    self.Workplace = ko.observable();

    self.OccupationRu = ko.observable();
    self.WorkplaceRu = ko.observable();

    self.Status = ko.observable();
    self.PartyId = ko.observable();
    self.ElectionId = ko.observable();
    self.RegionId = ko.observable();
    self.isLocal = ko.observable(false);

    self.GetCandidat = function () {
        self.NameRo('FakeUser');
        self.LastNameRo('FakeUser');
        self.NameRu('FakeUser ru');
        self.LastNameRu('FakeUser ru');
        self.DateOfBirth('FakeUser');
    }

    self.SaveEditClick = function (eventObject) {

        var data = new FormData();

        var files = $("#fileToUpload").get(0).files;

        // Add the uploaded image content to the form data collection
        if (files.length > 0) {
            data.append("UploadedImage", files[0]);
        };

        data.append("Id", self.Id());
        data.append("Code", self.Code());

        data.append("PartyId", self.PartyId());
        data.append("ElectionId", self.ElectionId());

        data.append("NameRo", self.NameRo());
        data.append("LastNameRo", self.LastNameRo());
        data.append("NameRu", self.NameRu());

        data.append("LastNameRu", self.LastNameRu());
        data.append("DateOfBirth", self.DateOfBirth());

        data.append("Occupation", self.Occupation());
        data.append("OccupationRu", self.OccupationRu());

        data.append("Workplace", self.Workplace());
        data.append("WorkplaceRu", self.WorkplaceRu());


        data.append("DateOfRegistration", self.DateOfRegistration());
        data.append("Status", self.Status());
        data.append("IsLocal", self.isLocal());
        data.append("RegionId", self.RegionId());

        // Make Ajax request with the contentType = false, and procesDate = false
        var ajaxRequest = $.ajax({
            type: "POST",
            url: "http://localhost/CEC.SAISE.EDayModule/RegisterPoliticalParty/SaveUpdateCandidateIndependent",
            contentType: false,
            processData: false,
            data: data,
            succes: function () {
                alert('Done');
            }, error: function () {
                alert('Done, no ne sovsem');
            }
        });

        //ajaxRequest.done(function (xhr, textStatus) {
        //    // Do other operation

        //});
    }
}



