//var urlPath = window.location.pathname;

//$(function() {
//    ko.applyBindings(SearchVoterVM);
//});

//var SearchVoterVM = {
//    Idnp: ko.observable(),
//    Results: ko.observableArray([]),

//    btnSearch: function() {
//        var self = this;
//        self.Results([]);

//        $.ajax({
//            url: urlPath + "/SearchVoter",
//            type: 'post',
//            dataType: 'json',
//            contentType: 'application/json',
//            data: ko.toJSON(self),
//            success: function(data) {
//                self.Results(data.Results);
//            }
//        });
//    }
//};

//function Results(Results) {
//    this.Idnp = ko.observable(Results.Idnp);
//    this.FirstName = ko.observable(Results.FirstName);
//    this.LastName = ko.observable(Results.LastName);
//    this.Patronymic = ko.observable(Results.Patronymic);
//    this.DateOfBirth = ko.observable(Results.DateOfBirth);
//    this.DocumentNumber = ko.observable(Results.DocumentNumber);
//    this.Address = ko.observable(Results.Address);
//    this.VoterId = ko.observable(Results.VoterId);
//}

//function UserData(UserData) {
//    this.SelectedElection = ko.observable(UserData.SelectedElection);
//    this.SelectedDistrict = ko.observable(UserData.SelectedDistrict);
//    this.SelectedVillage = ko.observable(UserData.SelectedVillage);
//    this.SelectedPollingStation = ko.observable(UserData.SelectedPollingStation);
//    this.IsAdmin = ko.observable(UserData.IsAdmin);
//}
