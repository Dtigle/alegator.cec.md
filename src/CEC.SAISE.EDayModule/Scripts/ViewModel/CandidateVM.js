

var CandidateVM = function (initData) {
    var self = this;

    self.Order = ko.observable(initData.CandidateOrder);
    self.Idnp = ko.observable(initData.Idnp);
    self.Id = ko.observable(initData.Id);
    self.LastNameRo = ko.observable(initData.LastNameRo);
    self.NameRo = ko.observable(initData.NameRo);
    self.LastNameRu = ko.observable(initData.LastNameRu);
    self.NameRu = ko.observable(initData.NameRu);
    self.DateOfBirth = ko.observable(initData.DateOfBirth);
    self.Occupation = ko.observable(initData.Occupation);
    self.Workplace = ko.observable(initData.Workplace);
    self.OccupationRu = ko.observable(initData.OccupationRu);
    self.WorkplaceRu = ko.observable(initData.WorkplaceRu);
    self.Status = ko.observable(initData.Status);

    self.ChangeCandidateStatus = function (obj, event) {

        $.ajax({
            url: '@Url.Action("UpdateCandidateStatus", "RegisterPoliticalParty")',
            data: { candidateId: self.Id(), statusId: self.Status() },
            type: "post",
            success: function (data) {
                //alert('Done ! No nuno shtoto pridumati sdeasi toje!');
            }
        });

    }
    //self.GetCandidat = function () {
    //    self.NameRo('FakeUser');
    //    self.LastNameRo('FakeUser');
    //    self.DateOfBirth('FakeUser');
    //}

    //self.SaveEditClick = function (eventObject) {

    //}
}