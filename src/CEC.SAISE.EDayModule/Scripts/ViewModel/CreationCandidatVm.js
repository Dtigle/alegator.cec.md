
var CreationCandidatVm = function () {
    var self = this;

    self.Order = ko.observable();

    self.Id = ko.observable();
    self.Idnp = ko.observable();

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
    self.ElectionId = ko.observable();
    self.PartyId = ko.observable();

    self.GetCandidat = function () {
        self.NameRo('FakeUser');
        self.LastNameRo('FakeUser');
        self.NameRu('FakeUser ru');
        self.LastNameRu('FakeUser ru');
        self.DateOfBirth((new Date()).toISOString().split('T')[0]);
    }

    self.SaveEditClick = function (eventObject) {
        //alert('Creation Candidat or party');
    }

}