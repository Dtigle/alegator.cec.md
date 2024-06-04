
var PartyVM = function (initData) {
    var self = this;

    self.Id = ko.observable(initData.Id);
    self.Code = ko.observable(initData.Code);
    self.NameRo = ko.observable(initData.NameRo);
    self.NameRu = ko.observable(initData.NameRu);
    self.Status = ko.observable(initData.Status);
    self.BallotOrder = ko.observable(initData.BallotOrder);
    self.CandidateCount = ko.observable(initData.CandidateCount);
    self.Status = ko.observable(initData.Status);
    self.DateOfRegistration = ko.observable(initData.DateOfRegistration);

    self.IsIndependent = ko.observable(initData.IsIndependent);
    self.CandidateData = ko.observable(initData.CandidateData);

    self.FullName = ko.pureComputed(function () {
        if (self.IsIndependent()) {
            return self.NameRo() + ' ( a.n.' + moment(self.CandidateData().DateOfBirth).format('YYYY') +
                ',' + self.CandidateData().Occupation +
                ',' + self.CandidateData().Workplace +
                ')';
        }
        return '';

    });

    self.FullNameRu = ko.pureComputed(function () {
        if (self.IsIndependent()) {
            return self.NameRu() + ' ( г.р.' + moment(self.CandidateData().DateOfBirth).format('YYYY') +
                ',' + self.CandidateData().OccupationRu +
                ',' + self.CandidateData().WorkplaceRu +
                ')';
        }
        return '';
    });

    
    self.templateUsed = function () {
        
        if (self.IsIndependent())
            return 'IndependentTemplate';
        return 'PartyTemplate';
    }

    self.ChangePartyStatus = function (obj, event) {

        $.ajax({
            url: '@Url.Action("UpdatePartyStatus", "RegisterPoliticalParty")',
            data: { partyId: self.Id(), statusId: self.Status() },
            type: "post",
            success: function (data) {
                //alert('Done ! No nuno shtoto pridumati sdeasi!');
            }
        });
    }
}