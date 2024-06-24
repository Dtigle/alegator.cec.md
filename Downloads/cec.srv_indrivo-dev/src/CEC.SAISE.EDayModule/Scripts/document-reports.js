const BallotPaperStatus = {
    New: 0,
    WaitingForApproval: 1,
    Approved: 2,
    MissingInformation: 3,
    Rejected: 4,
    Inactive: 5
};

const PoliticalPartyStatus = {
    New: 0,
    WaitingForApproval: 1,
    Approved: 2,
    MissingInformation: 3,
    Rejected: 4,
    Inactive: 5,
    Withdrawn: 6
};

const parsedJSON = $.parseJSON(initialData);

//ko.validation.configure({
//    insertMessages: false,
//    errorsAsTitle: false,
//    decoratedElement: true,
//    allowHtmlMessages: true,
//    messagesOnModified: false,
//    errorElementClass: 'has-error',
//    errorMessageClass: 'help-block'
//});

const electionResultsVM = new ElectionResultsVM(parsedJSON);

function ReportTemplateViewModel() {
    const self = this;

    self.templateNameId = ko.observable(0);
}

function updateVisualizeButton(result) {
    const visualizeButton = document.querySelector('input[value="Vizualizare și Tipar"]');

    if (!result.AlreadySent || result.Status === 0) {
        visualizeButton?.classList.add("d-none");
    } else {
        visualizeButton?.classList.remove("d-none");
    }
}

function ElectionResultsVM(initialData) {
    const self = this;

    self.InCallMode = ko.observable();

    self.reportTemplateViewModel = ko.observable(new ReportTemplateViewModel());

    self.ModuleIsLockedOut = ko.observable();
    self.BallotPaper = ko.observable();
    self.BallotPaperNotFound = ko.observable(false);

    self.ReportParameterValues = ko.observableArray([]);
    self.UnconfirmedBallotPapers = ko.observableArray([]);
    self.UnconfirmedPollingStations = ko.observableArray([]);

    self.Delimitator = ko.observable(new DelimitatorVM());
    self.Delimitator().onChanged.subscribe(function (e) {
        self.BallotPaper(null);

        if (e.isReady()) {
            self.requestBallotPaper();
        }
    });

    self.ballotPaperConsolidationData = ko.observable();

    self.requestBallotPaper = function () {
        const self = this;

        self.BallotPaperNotFound(false);
        self.reportTemplateViewModel().templateNameId(self.Delimitator().SelectedElectoralRegisteredId() || 1);

        self.handleSuccess = function (result) {
            self.ModuleIsLockedOut(false);
            self.BallotPaper(new BallotPaper(result, self));

            if ([6, 7, 8, 9, 24, 25, 26, 27, 28, 29, 30, 31].includes(self.reportTemplateViewModel().templateNameId())) {
                self.BallotPaper().DocumentName("Procesul verbal");
            } else {
                self.BallotPaper().DocumentName("Raportul");
            }

            self.BallotPaper().ServerEditDate(self.BallotPaper().getFormatedDate(result.EditDate));
            self.BallotPaper().EditUser(result.EditUser);
            self.BallotPaper().ServerConfirmationDate(self.BallotPaper().getFormatedDate(result.ConfirmationDate));
            self.BallotPaper().ConfirmationUserId(result.ConfirmationUserId);

            updateVisualizeButton(result);
        };

        self.handleAjaxError = function (e) {
            self.ModuleIsLockedOut(false);

            if (e.status === 200 && e.responseText === '') {
                self.BallotPaperNotFound(true);
            } else if (e.status === 423) {
                self.ModuleIsLockedOut(true);
            } else {
                alert('error');
            }
        };

        function getUnconfirmedBallotPapers() {
            self.InCallMode(true);

            $.ajax({
                url: 'ElectionResults/GetUnconfirmedBallotPapers',
                type: 'get',
                dataType: 'json',
                contentType: 'application/json',
                data: self.Delimitator().getDelimitatorData(),
                success: function (result) {
                    self.UnconfirmedBallotPapers(result);
                    self.handleSuccess(result);

                    setTimeout(() => {
                        self.UnconfirmedBallotPapers([]);

                        getConsolidatedResults();
                    }, 2000);

                    if (result.length === 0) {
                        getConsolidatedResults();
                    }
                },
                error: self.handleAjaxError
            }).done(function (data) {
                self.InCallMode(false);
                $(".number").forceNumeric();
            });
        }

        function getConsolidatedResults() {
            $.ajax({
                url: 'ElectionResults/GetCircumscriptionResultsConsolidated',
                type: 'post',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify({ delimitationData: { ...electionResultsVM.Delimitator().getDelimitatorData(), TemplateNameId: electionResultsVM.reportTemplateViewModel().templateNameId() } }),
                success: function (result) {
                    if (result.AlreadySent) {
                        electionResultsVM.ballotPaperConsolidationData(result);
                        electionResultsVM.BallotPaper().RegisteredVoters(result.RegisteredVoters);
                        electionResultsVM.BallotPaper().Supplementary(result.Supplementary);
                        electionResultsVM.BallotPaper().BallotsSpoiled(result.BallotsSpoiled);
                        electionResultsVM.BallotPaper().BallotsValidVotes(result.BallotsValidVotes);
                        electionResultsVM.BallotPaper().BallotsCasted(result.BallotsCasted);
                        electionResultsVM.BallotPaper().BallotsIssued(result.BallotsIssued);
                        electionResultsVM.BallotPaper().DifferenceIssuedCasted(result.DifferenceIssuedCasted);
                        electionResultsVM.BallotPaper().BallotsReceived(result.BallotsReceived);
                        electionResultsVM.BallotPaper().BallotsUnusedSpoiled(result.BallotsUnusedSpoiled);

                        electionResultsVM.BallotPaper().CompetitorResults(result.CompetitorResults);
                    }

                    updateVisualizeButton(result);

                    self.BallotPaper().AlreadySent(result.AlreadySent);
                },
                error: function (error) {
                    console.log(error)
                }
            }).done(function (data) {
                electionResultsVM.InCallMode(false);
                $(".number").forceNumeric();
            });
        }

        function retrieveCECEDocumentData() {
            $.ajax({
                url: 'Documents/RetrieveCECEDocument',
                type: 'post',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify({
                    delimitationData: self.Delimitator().getDelimitatorData(),
                    templateNameId: self.reportTemplateViewModel().templateNameId()
                }),
                success: function (result) {
                    self.ReportParameterValues(result.ReportParameterValues);
                    self.handleSuccess(result);

                    result.ReportParameterValues.forEach(function (value) {
                        self.BallotPaper()[value.ReportParameterName](value.ValueContent);
                    });

                    self.BallotPaper().DocumentName("Raportul");
                    self.BallotPaper().ServerEditDate(self.BallotPaper().getFormatedDate(result.EditDate));
                    self.BallotPaper().EditUser(result.EditUser);
                    self.BallotPaper().ServerConfirmationDate(self.BallotPaper().getFormatedDate(result.ConfirmationDate));
                    self.BallotPaper().ConfirmationUserId(result.ConfirmationUserId);
                },
                error: self.handleAjaxError
            }).done(function (data) {
                $(".number").forceNumeric();
            });
        }

        function retrieveFinalReportData() {
            $.ajax({
                url: 'Documents/RetrieveFinalReportData',
                type: 'post',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify({
                    delimitationData: electionResultsVM.Delimitator().getDelimitatorData(),
                    templateNameId: electionResultsVM.reportTemplateViewModel().templateNameId()
                }),
                success: function (result) {
                    electionResultsVM.ReportParameterValues(result.ReportParameterValues);

                    if (result.AlreadySent) {
                        result.ReportParameterValues.forEach(function (value) {
                            electionResultsVM.BallotPaper()[value.ReportParameterName](value.ValueContent);
                        });
                    }

                    self.BallotPaper().AlreadySent(result.AlreadySent);

                    self.BallotPaper().DocumentName("Raportul");
                    self.BallotPaper().ServerEditDate(self.BallotPaper().getFormatedDate(result.EditDate));
                    self.BallotPaper().EditUser(result.EditUser);
                    self.BallotPaper().ServerConfirmationDate(self.BallotPaper().getFormatedDate(result.ConfirmationDate));
                    self.BallotPaper().ConfirmationUserId(result.ConfirmationUserId);

                    updateVisualizeButton(result);
                },
                error: function (error) {
                    console.log(error)
                }
            }).done(function (data) {
                electionResultsVM.InCallMode(false);
                $(".number").forceNumeric();
            });
        }

        function getUnconfirmedPollingStations() {
            self.InCallMode(true);

            $.ajax({
                url: 'Documents/GetUnconfirmedPollingStations',
                type: 'post',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify({
                    delimitationData: self.Delimitator().getDelimitatorData(),
                    templateNameId: self.reportTemplateViewModel().templateNameId()
                }),
                success: function (result) {
                    self.UnconfirmedPollingStations(result);
                    self.handleSuccess(result);

                    setTimeout(() => {
                        self.UnconfirmedPollingStations([]);

                        retrieveFinalReportData();
                    }, 2000);

                    if (self.UnconfirmedPollingStations().length === 0) {
                        retrieveFinalReportData();
                    }
                },
                error: self.handleAjaxError
            }).done(function (data) {
                self.InCallMode(false);
                $(".number").forceNumeric();
            });
        }

        if ([6, 7, 8, 9].includes(self.reportTemplateViewModel().templateNameId())) {
            $.ajax({
                url: 'ElectionResults/RetrieveBallotPaper',
                type: 'post',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify({ delimitationData: self.Delimitator().getDelimitatorData() }),
                success: self.handleSuccess,
                error: self.handleAjaxError
            }).done(function (data) {
                $(".number").forceNumeric();
            });

            return;
        }

        if ([22, 23].includes(self.reportTemplateViewModel().templateNameId())) {
            retrieveCECEDocumentData();

            return;
        }

        if ([24, 25, 26, 27, 28, 29, 30, 31].includes(self.reportTemplateViewModel().templateNameId())) {
            getUnconfirmedBallotPapers();

            return;
        }

        if ([32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43].includes(self.reportTemplateViewModel().templateNameId())) {
            getUnconfirmedPollingStations();

            return;
        }

        $.ajax({
            url: 'Documents/RetrieveDocument',
            type: 'post',
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify({
                delimitationData: self.Delimitator().getDelimitatorData(),
                templateNameId: self.reportTemplateViewModel().templateNameId()
            }),
            success: function (result) {
                self.ReportParameterValues(result.ReportParameterValues);
                self.handleSuccess(result);

                result.ReportParameterValues.forEach(function (value) {
                    self.BallotPaper()[value.ReportParameterName](value.ValueContent);
                });

                self.BallotPaper().DocumentName("Raportul");
                self.BallotPaper().ServerEditDate(self.BallotPaper().getFormatedDate(result.EditDate));
                self.BallotPaper().EditUser(result.EditUser);
                self.BallotPaper().ServerConfirmationDate(self.BallotPaper().getFormatedDate(result.ConfirmationDate));
                self.BallotPaper().ConfirmationUserId(result.ConfirmationUserId);

                const visualizeButton = document.querySelector('input[value="Vizualizare și Tipar"]');

                if (result.Status === 0) {
                    visualizeButton?.classList.add("d-none");
                } else {
                    visualizeButton?.classList.remove("d-none");
                }
            },
            error: self.handleAjaxError
        }).done(function (data) {
            $(".number").forceNumeric();
        });
            
    };

    self.CallSubmitBallotPaper = function (ballotPaperData, action) {
        const self = this;

        self.InCallMode(true);

        let controllerName = "Documents";

        const templateNameId = self.reportTemplateViewModel().templateNameId();
        if (templateNameId >= 6 && templateNameId <= 9) {
            controllerName = "ElectionResults";
        }

        const url = action === 2 ? `${controllerName}/ConfirmResults` : `${controllerName}/SubmitResults`;

        const requestData = {
            delimitationData: self.Delimitator().getDelimitatorData(),
            templateNameId: templateNameId,
            model: ballotPaperData,
        };

        if ([24, 25, 26, 27, 28, 29, 30, 31].includes(templateNameId)) {
            $.ajax({
                url: 'ElectionResults/SubmitCircumscriptionResults',
                type: 'post',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify({ ballotPaperConsolidationModel: self.ballotPaperConsolidationData(), templateNameId }),
                success: function (data) {
                    if (data.Success) {
                        self.BallotPaper().setUpdateResult(data);
                    } else {
                        self.requestBallotPaper();
                    }

                    self.BallotPaper().DocumentName("Procesul verbal");
                    self.BallotPaper().EditUser(data.UserName);
                    self.BallotPaper().ServerEditDate(self.BallotPaper().getFormatedDate(data.UpdatedAt));

                    const visualizeButton = document.querySelector('input[value="Vizualizare și Tipar"]');

                    visualizeButton?.classList.remove("d-none");
                },
                error: function (e) {
                    if (e.status === 423) {
                        BootstrapDialog.show({
                            draggable: true,
                            cssClass: 'info-dialog',
                            title: (typeof title !== 'undefined') ? title : 'Info',
                            message: 'Funcționalitatea dată a fost blocată.',
                            closable: false,
                            buttons: [
                                {
                                    label: 'Închide',
                                    action: function (dialogItself) {
                                        dialogItself.close();
                                        window.location = 'Documents/Index';
                                    }
                                }
                            ]
                        });
                    } else {
                        ShowAlert('A avut loc o eroare', 'Eroare', 'danger-dialog');
                    }
                }
            }).done(function (data) {
                self.InCallMode(false);
            });

            return;
        }

        if ([22, 23, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43].includes(templateNameId)) {
            $.ajax({
                url: 'Documents/SubmitFinalReportCECEResults',
                type: 'post',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(requestData),
                success: function (data) {
                    if (data.Success) {
                        self.BallotPaper().setUpdateResult(data);
                    } else {
                        self.requestBallotPaper();
                    }

                    self.BallotPaper().DocumentName("Raportul");
                    self.BallotPaper().EditUser(data.UserName);
                    self.BallotPaper().ServerEditDate(self.BallotPaper().getFormatedDate(data.UpdatedAt));

                    const visualizeButton = document.querySelector('input[value="Vizualizare și Tipar"]');

                    visualizeButton?.classList.remove("d-none");
                },
                error: function (e) {
                    if (e.status === 423) {
                        BootstrapDialog.show({
                            draggable: true,
                            cssClass: 'info-dialog',
                            title: (typeof title !== 'undefined') ? title : 'Info',
                            message: 'Funcționalitatea dată a fost blocată.',
                            closable: false,
                            buttons: [
                                {
                                    label: 'Închide',
                                    action: function (dialogItself) {
                                        dialogItself.close();
                                        window.location = 'Documents/Index';
                                    }
                                }
                            ]
                        });
                    } else {
                        ShowAlert('A avut loc o eroare', 'Eroare', 'danger-dialog');
                    }
                }
            }).done(function (data) {
                self.InCallMode(false);
            });

            return;
        }

        $.ajax({
            url: url,
            type: 'post',
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify(requestData),
            success: function (data) {
                if (data.Success) {
                    self.BallotPaper().setUpdateResult(data);
                } else {
                    self.requestBallotPaper();
                }

                if ([6, 7, 8, 9, 24, 25, 26, 27, 28, 29, 30, 31].includes(templateNameId)) {
                    self.BallotPaper().DocumentName("Procesul verbal");
                } else {
                    self.BallotPaper().DocumentName("Raportul");
                }

                self.BallotPaper().EditUser(data.UserName);
                self.BallotPaper().ServerEditDate(self.BallotPaper().getFormatedDate(data.UpdatedAt));

                const visualizeButton = document.querySelector('input[value="Vizualizare și Tipar"]');

                visualizeButton?.classList.remove("d-none");
            },
            error: function (e) {
                if (e.status === 423) {
                    BootstrapDialog.show({
                        draggable: true,
                        cssClass: 'info-dialog',
                        title: (typeof title !== 'undefined') ? title : 'Info',
                        message: 'Funcționalitatea dată a fost blocată.',
                        closable: false,
                        buttons: [
                            {
                                label: 'Închide',
                                action: function (dialogItself) {
                                    dialogItself.close();
                                    window.location = 'Documents/Index';
                                }
                            }
                        ]
                    });
                } else {
                    ShowAlert('A avut loc o eroare', 'Eroare', 'danger-dialog');
                }
            }
        }).done(function (data) {
            self.InCallMode(false);
        });
    };
};

function BallotPaper(BallotPaper, parent) {
    const self = this;
    self.parent = parent;
    self.BallotPaperId = BallotPaper.BallotPaperId;

    self.OpeningVotersCount = BallotPaper.OpeningVotersCount;

    self.CompetitorResults = ko.observableArray(ko.utils.arrayMap(BallotPaper.CompetitorResults, function (competitorResult) {
        return new CompetitorResult(competitorResult);
    }));

    self.validationEnabled = false;
    self.isValidationEnabled = function () {
        return self.validationEnabled;
    }

    function createValidatedObservable(observable, fieldName, max = null, validator = null, inputType = 'number') {
        const observableValue = ko.observable(observable).extend({
            required: { message: validationMsgs.isRequiredField(fieldName) },
            max: { params: max, message: "" }
        });

        if (validator) {
            observableValue.extend({
                validation: [{
                    validator: validator,
                    message: validationMsgs[fieldName + '_Rule_Failed'],
                    params: { bp: self }
                }]
            });
        }

        if (inputType === 'text') {
            observableValue.extend({
                custom: {
                    validation: function (value) {
                        const valid = /^[a-zA-Z0-9,]*(\s[a-zA-Z0-9,]+)*$/.test(value);

                        return valid || validationMsgs.invalidInput(fieldName);
                    }
                }
            });
        }


        return observableValue;
    }

    const templateId = electionResultsVM.reportTemplateViewModel().templateNameId();

    // BESV Report On SV Preparation
    if ([1].includes(templateId)) {
        self.ElectionOfficialsCount = createValidatedObservable(BallotPaper.ElectionOfficialsCount, 'a', 3500);
        self.OtherAuthorizedPersonsCount = createValidatedObservable(BallotPaper.OtherAuthorizedPersonsCount, 'b', 3500);
        self.StationaryBalltoBoxesCount = createValidatedObservable(BallotPaper.StationaryBalltoBoxesCount, 'a', 3500);
        self.MobileBallotBoxesCount = createValidatedObservable(BallotPaper.MobileBallotBoxesCount, 'b', 3500);
        self.PlasticSealsCount_Rule_Succeeded = ko.validatedObservable(true);
        self.PlasticSealsCount = createValidatedObservable(BallotPaper.PlasticSealsCount, 'c', null, validationRules.C_GT_4A_B_Rule);
        self.VotingRequestsNumber = createValidatedObservable(BallotPaper.VotingRequestsNumber, '3', 3500);
        self.CertsFromCECECount_Rule_Succeeded = ko.validatedObservable(true);
        self.CertsFromCECECount = createValidatedObservable(BallotPaper.CertsFromCECECount, 'a', null, validationRules.A_eq_B_D_Rule);
        self.CertsHandedToVoters = createValidatedObservable(BallotPaper.CertsHandedToVoters, 'b');
        self.CertsHandedToVotersSeries = createValidatedObservable(BallotPaper.CertsHandedToVotersSeries, 'c');
        self.CertsUnusedCancelled = createValidatedObservable(BallotPaper.CertsUnusedCancelled, 'd');
        self.CertsUnusedCancelledSeries = createValidatedObservable(BallotPaper.CertsUnusedCancelledSeries, 'e');
    }

    // BESV Report On SV Preparation 2nd day
    if ([2].includes(templateId)) {
        self.ElectionOfficialsCount = createValidatedObservable(BallotPaper.ElectionOfficialsCount, 'a', 3500);
        self.OtherAuthorizedPersonsCount = createValidatedObservable(BallotPaper.OtherAuthorizedPersonsCount, 'b', 3500);
        self.StationaryBalltoBoxesCount = createValidatedObservable(BallotPaper.StationaryBalltoBoxesCount, 'a', 3500);
        self.MobileBallotBoxesCount = createValidatedObservable(BallotPaper.MobileBallotBoxesCount, 'b', 3500);
        self.PlasticSealsUsedCount_Rule_Succeeded = ko.validatedObservable(true);
        self.PlasticSealsUsedCount = createValidatedObservable(BallotPaper.PlasticSealsUsedCount, 'c', 3500, validationRules.C_GT_4A_B_Rule_Second_Day);
    }

    // Intermediate BESV Report 14:00 (elections, referendum)
    if ([3, 4].includes(templateId)) {
        self.IssuesContestationsReceived = createValidatedObservable(BallotPaper.IssuesContestationsReceived, '1', 3500);
        self.PublicOrderViolationsCount = createValidatedObservable(BallotPaper.PublicOrderViolationsCount, '2', 3500);
        self.VotingRequestsNumber = createValidatedObservable(BallotPaper.VotingRequestsNumber, '3', 3500);
        self.ElectionObserversCount = createValidatedObservable(BallotPaper.ElectionObserversCount, '4', 3500);
        self.ElectionCompetitorsRepresCount = createValidatedObservable(BallotPaper.ElectionCompetitorsRepresCount, '5', 3500);
    }

    // Intermediate BESV Report 21:00
    if ([5].includes(templateId)) {
        self.SignaturesOnBasicElectoralLists = createValidatedObservable(BallotPaper.SignaturesOnBasicElectoralLists, 'a', 3500);
        self.SignaturesOnAdditionalElectionLists = createValidatedObservable(BallotPaper.SignaturesOnAdditionalElectionLists, 'b', 3500);
        self.SignaturesOnElectionListsResidenceLocation = createValidatedObservable(BallotPaper.SignaturesOnElectionListsResidenceLocation, 'c', 3500);
        self.BallotPapersUnused = createValidatedObservable(BallotPaper.BallotPapersUnused, '2', 3500);
        self.PublicOrderViolationsFirstDay = createValidatedObservable(BallotPaper.PublicOrderViolationsFirstDay, '3', 3500);
    }

    // Proces verbal BESV privind rezultatele numărării voturilor (alegeri)
    if ([6, 7, 8, 9].includes(templateId)) {
        self.RegisteredVoters = ko.observable(BallotPaper.RegisteredVoters).extend({
            required: { message: validationMsgs.isRequiredField('a') },
            digit: { message: validationMsgs.digitsExpected },
        });

        self.Supplementary = ko.observable(BallotPaper.Supplementary).extend({
            required: { message: validationMsgs.isRequiredField('b') },
            digit: { message: validationMsgs.digitsExpected },

        });

        self.BallotsSpoiled = ko.observable(BallotPaper.BallotsSpoiled).extend({
            required: { message: validationMsgs.isRequiredField('f') },
            digit: { message: validationMsgs.digitsExpected },
            validation: {
                validator: validationRules.F_eq_D_minus_H_Rule,
                params: { bp: self },
                onlyIf: self.isValidationEnabled
            }
        });

        self.validate = function (observable) {
            if (ko.validation.utils.isValidatable(observable))
                ko.validation.validateObservable(observable);
        };
        self.validateBallotsSpoiled = function () {
            self.validationEnabled = true;
            self.validate(self.BallotsSpoiled);
        };

        self.BallotsValidVotes = ko.observable(BallotPaper.BallotsValidVotes).extend({
            required: { message: validationMsgs.isRequiredField('h') },
            digit: { message: validationMsgs.digitsExpected },
            validation: {
                validator: validationRules.H_eq_Sum_of_g_Rule,
                message: validationMsgs.H_eq_Sum_of_g_Rule_Failed,
                params: { competitorsArray: self.CompetitorResults }
            }
        });

        self.BallotsCasted = ko.observable(BallotPaper.BallotsCasted).extend({
            required: { message: validationMsgs.isRequiredField('d') },
            digit: { message: validationMsgs.digitsExpected },
            validation: {
                validator: validationRules.D_eq_F_H_Rule,
                message: validationMsgs.D_eq_F_H_Rule_Failed,
                params: { f: self.BallotsSpoiled, h: self.BallotsValidVotes }
            }
        });

        self.BallotsIssued_Rule1_Succeeded = ko.validatedObservable(true);
        self.BallotsIssued_Rule2_Succeeded = ko.validatedObservable(true);
        self.BallotsIssued = ko.observable(BallotPaper.BallotsIssued).extend({
            required: { message: validationMsgs.isRequiredField('c') },
            digit: { message: validationMsgs.digitsExpected },
            validation: [{
                validator: validationRules.C_GT_A_B_Rule,
                message: validationMsgs.C_GT_A_B_Rule_Failed,
                params: { bp: self }
            }, {
                validator: validationRules.C_LT_D_Rule,
                message: validationMsgs.C_LT_D_Rule_Failed,
                params: { bp: self }
            }]
        });

        self.DifferenceIssuedCasted = ko.observable(BallotPaper.DifferenceIssuedCasted).extend({
            required: { message: validationMsgs.isRequiredField('e') },
            digit: { message: validationMsgs.digitsExpected },
            validation: {
                validator: validationRules.E_NE_C_D_Rule,
                message: validationMsgs.E_NE_C_D_Rule_Failed,
                params: { bp: self }
            }
        });

        self.BallotsReceived = ko.observable(BallotPaper.BallotsReceived);

        self.BallotsUnusedSpoiled = ko.observable(BallotPaper.BallotsUnusedSpoiled).extend({
            required: { message: validationMsgs.isRequired },
            digit: { message: validationMsgs.digitsExpected },
            validation: {
                validator: validationRules.J_eq_I_diff_C_Rule,
                message: validationMsgs.J_eq_I_diff_C_Rule,
                params: { bp: self }
            }
        });

        self.BallotsReceived = ko.observable(BallotPaper.BallotsReceived).extend({
            required: { message: validationMsgs.isRequiredField('i') },
            digit: { message: validationMsgs.digitsExpected },
            validation: [{
                validator: validationRules.I_eq_C_J_Rule,
                message: validationMsgs.I_eq_C_J_Rule,
                params: { c: self.BallotsIssued, j: self.BallotsUnusedSpoiled }
            }, {
                validator: validationRules.X_GT_Zero_Rule,
                message: validationMsgs.filedMustBe_GT_Zero('i')
            }]
        });
    }

    if ([24, 25, 26, 27, 28, 29, 30, 31].includes(templateId)) {
        self.RegisteredVoters = ko.observable();
        self.Supplementary = ko.observable();
        self.BallotsSpoiled = ko.observable();
        self.BallotsValidVotes = ko.observable();
        self.BallotsCasted = ko.observable();
        self.BallotsIssued = ko.observable();
        self.DifferenceIssuedCasted = ko.observable();
        self.BallotsReceived = ko.observable();
        self.BallotsUnusedSpoiled = ko.observable();
    }

    // Final BESV Report (elections, 1 day, round I)
    if ([10, 16].includes(templateId)) {
        self.ElectoralBoardMembersCountOpening = createValidatedObservable(BallotPaper.ElectoralBoardMembersCountOpening, 'a', 3500);
        self.SIASOperatorsCountOpening = createValidatedObservable(BallotPaper.SIASOperatorsCountOpening, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesCountOpening = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesCountOpening, 'c', 3500);
        self.InternationalObserversCountOpening = createValidatedObservable(BallotPaper.InternationalObserversCountOpening, 'd', 3500);
        self.NationalObserversFromAssociationsCountOpening = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsCountOpening, 'e', 3500);
        self.MediaRepresentativesCountOpening = createValidatedObservable(BallotPaper.MediaRepresentativesCountOpening, 'f', 3500);
        self.StationaryVotingBoxes80L = createValidatedObservable(BallotPaper.StationaryVotingBoxes80L, 'a', 10);
        self.StationaryVotingBoxes45L = createValidatedObservable(BallotPaper.StationaryVotingBoxes45L, 'b', 10);
        self.MobileVotingBoxes27L = createValidatedObservable(BallotPaper.MobileVotingBoxes27L, 'c', 5);
        self.PlasticSealsUsedForVotingBoxesCount_Rule_Succeeded = ko.validatedObservable(true);
        self.PlasticSealsUsedForVotingBoxesCount = createValidatedObservable(BallotPaper.PlasticSealsUsedForVotingBoxesCount, 'd', null, validationRules.D_eq_or_GT_4A_4B_C_Rule);
        self.PlasticSealCodesOnVotingBoxes = createValidatedObservable(BallotPaper.PlasticSealCodesOnVotingBoxes, 'e', 3500, null, 'text');
        self.SecretVotingBoothsCount = createValidatedObservable(BallotPaper.SecretVotingBoothsCount, 'a', 10);
        self.SpecialNeedsVotingBoothsCount = createValidatedObservable(BallotPaper.SpecialNeedsVotingBoothsCount, 'b', 5);
        self.StampedVotedCount = createValidatedObservable(BallotPaper.StampedVotedCount, 'a', 3500);
        self.StampedWithdrawnCount = createValidatedObservable(BallotPaper.StampedWithdrawnCount, 'b', 3500);
        self.ElectoralOfficeMembersCompositionCount = createValidatedObservable(BallotPaper.ElectoralOfficeMembersCompositionCount, '5', 15);
        self.OrganizedMeetingsCount = createValidatedObservable(BallotPaper.OrganizedMeetingsCount, '6', 3500);
        self.DecisionsAdoptedByElectoralOfficeCount = createValidatedObservable(BallotPaper.DecisionsAdoptedByElectoralOfficeCount, '7', 3500);
        self.ContestationsUntilEDay_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsUntilEDay = createValidatedObservable(BallotPaper.ContestationsUntilEDay, 'a', null, validationRules.A_eq_9A_9E_9F_9G_Final_BESV_Rule);
        self.ContestationsOnEDay_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsOnEDay = createValidatedObservable(BallotPaper.ContestationsOnEDay, 'b', null, validationRules.A_eq_10A_10E_10F_10G_Final_BESV_Rule);
        self.TotalDecisionsOnObjectionsCountBefore_Rule_Succeeded = ko.validatedObservable(true);
        self.TotalDecisionsOnObjectionsCountBefore = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsCountBefore, 'a', null, validationRules.A_eq_B_C_D_Rule_Before);
        self.FullyAdmittedObjectionsCountBefore = createValidatedObservable(BallotPaper.FullyAdmittedObjectionsCountBefore, 'b', 3500);
        self.PartiallyAdmittedObjectionsCountBefore = createValidatedObservable(BallotPaper.PartiallyAdmittedObjectionsCountBefore, 'c', 3500);
        self.UnfoundedObjectionsCountBefore = createValidatedObservable(BallotPaper.UnfoundedObjectionsCountBefore, 'd', 3500);
        self.WrittenResponsesToObjectionsCountBefore = createValidatedObservable(BallotPaper.WrittenResponsesToObjectionsCountBefore, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesCountBefore = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesCountBefore, 'f', 3500);
        self.ReturnedObjectionsCountBefore = createValidatedObservable(BallotPaper.ReturnedObjectionsCountBefore, 'g', 3500);
        self.TotalDecisionsOnObjectionsCount_Rule_Succeeded = ko.validatedObservable(true);
        self.TotalDecisionsOnObjectionsCount = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsCount, 'a', null, validationRules.A_eq_B_C_D_Rule);
        self.FullyAdmittedObjectionsCount = createValidatedObservable(BallotPaper.FullyAdmittedObjectionsCount, 'b', 3500);
        self.PartiallyAdmittedObjectionsCount = createValidatedObservable(BallotPaper.PartiallyAdmittedObjectionsCount, 'c', 3500);
        self.UnfoundedObjectionsCount = createValidatedObservable(BallotPaper.UnfoundedObjectionsCount, 'd', 3500);
        self.WrittenResponsesToObjectionsCount = createValidatedObservable(BallotPaper.WrittenResponsesToObjectionsCount, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesCount = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesCount, 'f', 3500);
        self.ReturnedObjectionsCount = createValidatedObservable(BallotPaper.ReturnedObjectionsCount, 'g', 3500);
        self.PublicOrderViolationsOnVotingDayCount = createValidatedObservable(BallotPaper.PublicOrderViolationsOnVotingDayCount, '11', 3500);
        self.BallotAssistanceCount = createValidatedObservable(BallotPaper.BallotAssistanceCount, '12', 3500);
        self.RegisteredVotersCountBeforeVotingDay = createValidatedObservable(BallotPaper.RegisteredVotersCountBeforeVotingDay, '13', 3500);
        self.VoterRequestsCount_Rule_Succeeded = ko.validatedObservable(true);
        self.VoterRequestsCount = createValidatedObservable(BallotPaper.VoterRequestsCount, 'a', null, validationRules.A_B_C_eq_D_E_F_eq_G_H_Rule);
        self.ElectoralCompetitorRequestsCount = createValidatedObservable(BallotPaper.ElectoralCompetitorRequestsCount, 'b', 3500);
        self.ObserverRequestsCount = createValidatedObservable(BallotPaper.ObserverRequestsCount, 'c', 3500);
        self.InclusionRequestsCount = createValidatedObservable(BallotPaper.InclusionRequestsCount, 'd', 3500);
        self.ExclusionRequestsCount = createValidatedObservable(BallotPaper.ExclusionRequestsCount, 'e', 3500);
        self.DataCorrectionRequestsCount = createValidatedObservable(BallotPaper.DataCorrectionRequestsCount, 'f', 3500);
        self.AdmittedRequestsCount = createValidatedObservable(BallotPaper.AdmittedRequestsCount, 'g', 3500);
        self.RejectedRequestsCount = createValidatedObservable(BallotPaper.RejectedRequestsCount, 'h', 3500);
        self.RegisteredVotersCountAfterUpdates = createValidatedObservable(BallotPaper.RegisteredVotersCountAfterUpdates, '15', 3500);
        self.CECEReceivedCertificatesCount_Rule_Succeeded = ko.observable(true);
        self.CECEReceivedCertificatesCount = createValidatedObservable(BallotPaper.CECEReceivedCertificatesCount, 'a', null, validationRules.A_eq_B_D_BESV_Final_Rule);
        self.VotersIssuedCertificatesCount = createValidatedObservable(BallotPaper.VotersIssuedCertificatesCount, 'b', 3500);
        self.VotersCertificateSeries = createValidatedObservable(BallotPaper.VotersCertificateSeries, 'c', 3500, null, 'text');
        self.UnusedOrCancelledCertificatesCount = createValidatedObservable(BallotPaper.UnusedOrCancelledCertificatesCount, 'd', 3500);
        self.UnusedOrCancelledCertificateSeries = createValidatedObservable(BallotPaper.UnusedOrCancelledCertificateSeries, 'e', 3500, null, 'text');
        self.TotalVotingLocationRequestsCount = createValidatedObservable(BallotPaper.TotalVotingLocationRequestsCount, '17', 3500);
        self.BallotsIssuedByMobileTeamCount = createValidatedObservable(BallotPaper.BallotsIssuedByMobileTeamCount, '18', 3500);
        self.VotesBasedOnVotingCertificatesCount = createValidatedObservable(BallotPaper.VotesBasedOnVotingCertificatesCount, 'a', 3500);
        self.VotesBasedOnProvisionalIDCount = createValidatedObservable(BallotPaper.VotesBasedOnProvisionalIDCount, 'b', 3500);
        self.VotesBasedOnPassportCount = createValidatedObservable(BallotPaper.VotesBasedOnPassportCount, 'c', 3500);
        self.VotesBasedOnExpiredPassportCount_Rule_Succeeded = ko.validatedObservable(true);
        self.VotesBasedOnExpiredPassportCount = createValidatedObservable(BallotPaper.VotesBasedOnExpiredPassportCount, 'd', null, validationRules.D_equal_or_less_than_C_Rule);
        self.VotesAtLocationUsingMobileBoxCount = createValidatedObservable(BallotPaper.VotesAtLocationUsingMobileBoxCount, 'e', 3500);
        self.VotesByIncarceratedPersonsCount = createValidatedObservable(BallotPaper.VotesByIncarceratedPersonsCount, 'f', 3500);
        self.VotesByPersonsInMedicalFacilitiesCount = createValidatedObservable(BallotPaper.VotesByPersonsInMedicalFacilitiesCount, 'g', 3500);
        self.VotesByMilitaryPersonnelCount = createValidatedObservable(BallotPaper.VotesByMilitaryPersonnelCount, 'h', 3500);
        self.VotesByPersonsWithMobilityDisabilitiesCount = createValidatedObservable(BallotPaper.VotesByPersonsWithMobilityDisabilitiesCount, 'k', 3500);
        self.VotesByVisuallyImpairedPersonsCount = createValidatedObservable(BallotPaper.VotesByVisuallyImpairedPersonsCount, 'l', 3500);
        self.VotesByHearingImpairedPersonsCount = createValidatedObservable(BallotPaper.VotesByHearingImpairedPersonsCount, 'm', 3500);
        self.VotesBasedOnNewResidenceDeclarationCount = createValidatedObservable(BallotPaper.VotesBasedOnNewResidenceDeclarationCount, 'n', 3500);
        self.VotesByStudentCardHoldersCount = createValidatedObservable(BallotPaper.VotesByStudentCardHoldersCount, 'o', 3500);
        self.VotesByElectoralOfficersFromOtherStationsCount = createValidatedObservable(BallotPaper.VotesByElectoralOfficersFromOtherStationsCount, 'p', 3500);
        self.SignaturesOnMainElectoralListsCount = createValidatedObservable(BallotPaper.SignaturesOnMainElectoralListsCount, 'a', 3500);
        self.SignaturesOnSupplementaryElectoralListsCount = createValidatedObservable(BallotPaper.SignaturesOnSupplementaryElectoralListsCount, 'b', 3500);
        self.SignaturesOnLocationVotingElectoralListsCount = createValidatedObservable(BallotPaper.SignaturesOnLocationVotingElectoralListsCount, 'c', 3500);
        self.AbsentElectoralBoardMembersOnVotingDayCount_Rule_Succeeded = ko.validatedObservable(true);
        self.AbsentElectoralBoardMembersOnVotingDayCount = createValidatedObservable(BallotPaper.AbsentElectoralBoardMembersOnVotingDayCount, '21', null, validationRules.equal_or_less_than_point_5_Rule);
        self.RepresentativesAppointedByPoliticalPartiesCount = createValidatedObservable(BallotPaper.RepresentativesAppointedByPoliticalPartiesCount, 'a', 3500);
        self.RepresentativesAppointedByElectoralBlocksCount = createValidatedObservable(BallotPaper.RepresentativesAppointedByElectoralBlocksCount, 'b', 3500);
        self.RepresentativesAppointedByIndependentCandidatesCount = createValidatedObservable(BallotPaper.RepresentativesAppointedByIndependentCandidatesCount, 'c', 3500);
        self.ElectoralBoardMembersCount_Rule_Succeeded = ko.validatedObservable(true);
        self.ElectoralBoardMembersCount = createValidatedObservable(BallotPaper.ElectoralBoardMembersCount, 'a', null, validationRules.A_equal_point5_minus_point21_Rule);
        self.SIASOperatorsCount = createValidatedObservable(BallotPaper.SIASOperatorsCount, 'b', 3500);
        self.ElectoralCompetitorRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralCompetitorRepresentativesCount, 'c', 3500);
        self.InternationalObserversCount = createValidatedObservable(BallotPaper.InternationalObserversCount, 'd', 3500);
        self.NationalObserversFromAssociationsCount = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsCount, 'e', 3500);
        self.MediaRepresentativesCount = createValidatedObservable(BallotPaper.MediaRepresentativesCount, 'f', 3500);
        self.VotingResultsReportCountByBESV = createValidatedObservable(BallotPaper.VotingResultsReportCountByBESV, '24', 3500);
    }

    // Final BESV Report (elections, 1 day, round II)
    if ([11, 17].includes(templateId)) {
        self.ElectoralBoardMembersOnOpening = createValidatedObservable(BallotPaper.ElectoralBoardMembersOnOpening, 'a', 3500);
        self.SIASOperatorsOnOpening = createValidatedObservable(BallotPaper.SIASOperatorsOnOpening, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesOnOpening = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesOnOpening, 'c', 3500);
        self.InternationalObserversOnOpening = createValidatedObservable(BallotPaper.InternationalObserversOnOpening, 'd', 3500);
        self.NationalObserversFromAssociationsOnOpening = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsOnOpening, 'e', 3500);
        self.MediaRepresentativesOnOpening = createValidatedObservable(BallotPaper.MediaRepresentativesOnOpening, 'f', 3500);
        self.StationaryVotingBox80Liters = createValidatedObservable(BallotPaper.StationaryVotingBox80Liters, 'a', 10);
        self.StationaryVotingBox45Liters = createValidatedObservable(BallotPaper.StationaryVotingBox45Liters, 'b', 10);
        self.MobileVotingBox27Liters = createValidatedObservable(BallotPaper.MobileVotingBox27Liters, 'c', 5);
        self.PlasticSealsForVotingBoxCount_Rule_Succeeded = ko.observable(true);
        self.PlasticSealsForVotingBoxCount = createValidatedObservable(BallotPaper.PlasticSealsForVotingBoxCount, 'd', null, validationRules.D_eq_or_GT_4A_4B_C_Rule_Second_Round);
        self.PlasticSealCodesOnVotingBox = createValidatedObservable(BallotPaper.PlasticSealCodesOnVotingBox, 'e', 3500);
        self.VotingBoothsCount = createValidatedObservable(BallotPaper.VotingBoothsCount, 'a', 10);
        self.SpecialNeedsVotingBoothsCount = createValidatedObservable(BallotPaper.SpecialNeedsVotingBoothsCount, 'b', 5);
        self.StampedVotedCount = createValidatedObservable(BallotPaper.StampedVotedCount, 'a', 3500);
        self.StampedWithdrawnCount = createValidatedObservable(BallotPaper.StampedWithdrawnCount, 'b', 3500);
        self.ElectoralBoardMembersNumericComposition = createValidatedObservable(BallotPaper.ElectoralBoardMembersNumericComposition, '5', 15);
        self.MeetingsCountBetweenRounds = createValidatedObservable(BallotPaper.MeetingsCountBetweenRounds, '6', 3500);
        self.DecisionsCountBetweenRounds = createValidatedObservable(BallotPaper.DecisionsCountBetweenRounds, '7', 3500);
        self.ContestationsUntilEDay_Rule_Succeeded = ko.observable(true);
        self.ContestationsUntilEDay = createValidatedObservable(BallotPaper.ContestationsUntilEDay, 'a', null, validationRules.A_eq_9A_9E_9F_9G_Final_BESV_elections_2nd_round_Rule);
        self.ContestationsOnEDay_Rule_Succeeded = ko.observable(true);
        self.ContestationsOnEDay = createValidatedObservable(BallotPaper.ContestationsOnEDay, 'b', null, validationRules.A_eq_10A_10E_10F_10G_Final_BESV_2nd_round_Rule);
        self.TotalDecisionsOnObjectionsBefore_Rule_Succeeded = ko.observable(true);
        self.TotalDecisionsOnObjectionsBefore = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsBefore, 'a', null, validationRules.A_eq_B_C_D_2nd_round_before_Rule);
        self.DecisionsFullyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsBefore, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsBefore, 'c', 3500);
        self.DecisionsRejectedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsBefore, 'd', 3500);
        self.ObjectionResponsesByLetterBefore = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterBefore, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesBefore = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesBefore, 'f', 3500);
        self.ReturnedObjectionsBefore = createValidatedObservable(BallotPaper.ReturnedObjectionsBefore, 'g', 3500);
        self.TotalDecisionsOnObjectionsCount_Rule_Succeeded = ko.observable(true);
        self.TotalDecisionsOnObjectionsCount = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsCount, 'a', null, validationRules.A_eq_B_C_D_2nd_round_Rule);
        self.FullyAdmittedObjectionsCount = createValidatedObservable(BallotPaper.FullyAdmittedObjectionsCount, 'b', 3500);
        self.PartiallyAdmittedObjectionsCount = createValidatedObservable(BallotPaper.PartiallyAdmittedObjectionsCount, 'c', 3500);
        self.UnfoundedObjectionsCount = createValidatedObservable(BallotPaper.UnfoundedObjectionsCount, 'd', 3500);
        self.WrittenResponsesToObjectionsCount = createValidatedObservable(BallotPaper.WrittenResponsesToObjectionsCount, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesCount = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesCount, 'f', 3500);
        self.ReturnedObjectionsCount = createValidatedObservable(BallotPaper.ReturnedObjectionsCount, 'g', 3500);
        self.PublicOrderViolationsOnVotingDay = createValidatedObservable(BallotPaper.PublicOrderViolationsOnVotingDay, '11', 3500);
        self.BallotAssistanceCount = createValidatedObservable(BallotPaper.BallotAssistanceCount, '12', 3500);
        self.CECEReceivedCertificates_Rule_Succeeded = ko.observable(true);
        self.CECEReceivedCertificates = createValidatedObservable(BallotPaper.CECEReceivedCertificates, 'a', null, validationRules.A_eq_B_D_Final_BESV_2nd_round_Rule);
        self.VotersIssuedCertificates = createValidatedObservable(BallotPaper.VotersIssuedCertificates, 'b', 3500);
        self.SeriesOfIssuedCertificates = createValidatedObservable(BallotPaper.SeriesOfIssuedCertificates, 'c', 3500);
        self.UnusedOrCancelledCertificates = createValidatedObservable(BallotPaper.UnusedOrCancelledCertificates, 'd', 3500);
        self.SeriesOfUnusedOrCancelledCertificates = createValidatedObservable(BallotPaper.SeriesOfUnusedOrCancelledCertificates, 'e', 3500);
        self.TotalVotingRequestsAtLocation = createValidatedObservable(BallotPaper.TotalVotingRequestsAtLocation, '14', 3500);
        self.BallotsIssuedByMobileTeam = createValidatedObservable(BallotPaper.BallotsIssuedByMobileTeam, '15', 3500);
        self.VotersBasedOnVotingCertificates = createValidatedObservable(BallotPaper.VotersBasedOnVotingCertificates, 'a', 3500);
        self.VotersWithTemporaryID = createValidatedObservable(BallotPaper.VotersWithTemporaryID, 'b', 3500);
        self.VotersWithPassport = createValidatedObservable(BallotPaper.VotersWithPassport, 'c', 3500);
        self.VotersWithExpiredPassport_Rule_Succeeded = ko.observable(true);
        self.VotersWithExpiredPassport = createValidatedObservable(BallotPaper.VotersWithExpiredPassport, 'd', null, validationRules.D_equal_or_less_than_C_2nd_round_Rule);
        self.VotersAtLocationWithMobileBox = createValidatedObservable(BallotPaper.VotersAtLocationWithMobileBox, 'e', 3500);
        self.VotersInPenitentiaryInstitutions = createValidatedObservable(BallotPaper.VotersInPenitentiaryInstitutions, 'f', 3500);
        self.VotersInSanatoriumsOrHospitals = createValidatedObservable(BallotPaper.VotersInSanatoriumsOrHospitals, 'g', 3500);
        self.MilitaryVotersInMilitaryUnits = createValidatedObservable(BallotPaper.MilitaryVotersInMilitaryUnits, 'h', 3500);
        self.VotersWithLocomotorDisabilities = createValidatedObservable(BallotPaper.VotersWithLocomotorDisabilities, 'k', 3500);
        self.VotersWithVisualImpairments = createValidatedObservable(BallotPaper.VotersWithVisualImpairments, 'l', 3500);
        self.VotersWithHearingImpairments = createValidatedObservable(BallotPaper.VotersWithHearingImpairments, 'm', 3500);
        self.VotersBasedOnNewResidenceDeclaration = createValidatedObservable(BallotPaper.VotersBasedOnNewResidenceDeclaration, 'n', 3500);
        self.VotersWithStudentCard = createValidatedObservable(BallotPaper.VotersWithStudentCard, 'o', 3500);
        self.ElectoralOfficialsFromOtherSV = createValidatedObservable(BallotPaper.ElectoralOfficialsFromOtherSV, 'p', 3500);
        self.SignaturesOnBasicElectoralLists = createValidatedObservable(BallotPaper.SignaturesOnBasicElectoralLists, 'a', 3500);
        self.SignaturesOnSupplementaryElectoralLists = createValidatedObservable(BallotPaper.SignaturesOnSupplementaryElectoralLists, 'b', 3500);
        self.SignaturesOnVotingLocationElectoralLists = createValidatedObservable(BallotPaper.SignaturesOnVotingLocationElectoralLists, 'c', 3500);
        self.AbsentElectoralBoardMembersOnVotingDay_Rule_Succeeded = ko.observable(true);
        self.AbsentElectoralBoardMembersOnVotingDay = createValidatedObservable(BallotPaper.AbsentElectoralBoardMembersOnVotingDay, '18', null, validationRules.equal_or_less_than_point_6_final_BESV_1day_2nd_round_Rule);
        self.PartyDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.PartyDesignatedRepresentativesCount, 'a', 3500);
        self.ElectoralBlocDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralBlocDesignatedRepresentativesCount, 'b', 3500);
        self.IndependentCandidateDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.IndependentCandidateDesignatedRepresentativesCount, 'c', 3500);
        self.ElectoralBoardMembersCount_Rule_Succeeded = ko.observable(true);
        self.ElectoralBoardMembersCount = createValidatedObservable(BallotPaper.ElectoralBoardMembersCount, 'a', null, validationRules.A_equal_point5_minus_point18_Rule);
        self.SIASOperatorsCount = createValidatedObservable(BallotPaper.SIASOperatorsCount, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesCount, 'c', 3500);
        self.InternationalObserversCount = createValidatedObservable(BallotPaper.InternationalObserversCount, 'd', 3500);
        self.NationalObserversFromAssociationsCount = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsCount, 'e', 3500);
        self.MediaRepresentativesCount = createValidatedObservable(BallotPaper.MediaRepresentativesCount, 'f', 3500);
        self.VotingResultsReportCountByBESV = createValidatedObservable(BallotPaper.VotingResultsReportCountByBESV, '21', 3500);
    }

    // Final BESV Report (elections, 2 days, round I)
    if ([12, 18].includes(templateId)) {
        self.ElectoralBoardMembersOnOpening = createValidatedObservable(BallotPaper.ElectoralBoardMembersOnOpening, 'a', 3500);
        self.SIASOperatorsOnOpening = createValidatedObservable(BallotPaper.SIASOperatorsOnOpening, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesOnOpening = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesOnOpening, 'c', 3500);
        self.InternationalObserversOnOpening = createValidatedObservable(BallotPaper.InternationalObserversOnOpening, 'd', 3500);
        self.NationalObserversFromAssociationsOnOpening = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsOnOpening, 'e', 3500);
        self.MediaRepresentativesOnOpening = createValidatedObservable(BallotPaper.MediaRepresentativesOnOpening, 'f', 3500);
        self.ElectoralBoardMembersSecondDay = createValidatedObservable(BallotPaper.ElectoralBoardMembersSecondDay, 'a', 3500);
        self.SIASOperatorsSecondDay = createValidatedObservable(BallotPaper.SIASOperatorsSecondDay, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesSecondDay = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesSecondDay, 'c', 3500);
        self.InternationalObserversSecondDay = createValidatedObservable(BallotPaper.InternationalObserversSecondDay, 'd', 3500);
        self.NationalObserversFromAssociationsSecondDay = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsSecondDay, 'e', 3500);
        self.MediaRepresentativesSecondDay = createValidatedObservable(BallotPaper.MediaRepresentativesSecondDay, 'f', 3500);
        self.StationaryVotingBox80Liters = createValidatedObservable(BallotPaper.StationaryVotingBox80Liters, 'a', 10);
        self.StationaryVotingBox45Liters = createValidatedObservable(BallotPaper.StationaryVotingBox45Liters, 'b', 10);
        self.MobileVotingBox27Liters = createValidatedObservable(BallotPaper.MobileVotingBox27Liters, 'c', 5);
        self.PlasticSealsForVotingBoxCount_Rule_Succeeded = ko.validatedObservable(true);
        self.PlasticSealsForVotingBoxCount = createValidatedObservable(BallotPaper.PlasticSealsForVotingBoxCount, 'd', null, validationRules.D_eq_or_GT_4A_4B_C_2days_Rule);
        self.PlasticSealCodesOnVotingBox = createValidatedObservable(BallotPaper.PlasticSealCodesOnVotingBox, 'e', 3500, null, 'text');
        self.PlasticSealCodesOnVotingBoxSlits_Rule_Succeeded = ko.validatedObservable(true);
        self.PlasticSealCodesOnVotingBoxSlits = createValidatedObservable(BallotPaper.PlasticSealCodesOnVotingBoxSlits, 'f', 3500, null, 'text');
        self.VotingBoothsCount = createValidatedObservable(BallotPaper.VotingBoothsCount, 'a', 10);
        self.SpecialNeedsVotingBoothsCount = createValidatedObservable(BallotPaper.SpecialNeedsVotingBoothsCount, 'b', 5);
        self.StampedVotedCount = createValidatedObservable(BallotPaper.StampedVotedCount, 'a', 3500);
        self.StampedWithdrawnCount = createValidatedObservable(BallotPaper.StampedWithdrawnCount, 'b', 3500);
        self.ElectoralBoardMembersNumericComposition = createValidatedObservable(BallotPaper.ElectoralBoardMembersNumericComposition, '6', 15);
        self.MeetingsCountBetweenRounds = createValidatedObservable(BallotPaper.MeetingsCountBetweenRounds, '7', 3500);
        self.DecisionsCountBetweenRounds = createValidatedObservable(BallotPaper.DecisionsCountBetweenRounds, '8', 3500);
        self.ContestationsUntilEDay_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsUntilEDay = createValidatedObservable(BallotPaper.ContestationsUntilEDay, 'a', null, validationRules.A_eq_10A_10E_10F_10G_2days_Rule);
        self.ContestationsOnEDay_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsOnEDay = createValidatedObservable(BallotPaper.ContestationsOnEDay, 'b', null, validationRules.A_eq_11A_11E_11F_11G_2days_Rule);
        self.SubmittedOnVotingDayCountSecondDay_Rule_Succeeded = ko.validatedObservable(true);
        self.SubmittedOnVotingDayCountSecondDay = createValidatedObservable(BallotPaper.SubmittedOnVotingDayCountSecondDay, 'c', null, validationRules.A_eq_12A_12E_12F_12G_2days_Rule);
        self.TotalDecisionsOnObjectionsBefore_Rule_Succeeded = ko.validatedObservable(true);
        self.TotalDecisionsOnObjectionsBefore = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsBefore, 'a', null, validationRules.A_eq_B_C_D_2days_Rule_Before);
        self.DecisionsFullyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsBefore, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsBefore, 'c', 3500);
        self.DecisionsRejectedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsBefore, 'd', 3500);
        self.ObjectionResponsesByLetterBefore = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterBefore, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesBefore = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesBefore, 'f', 3500);
        self.ReturnedObjectionsBefore = createValidatedObservable(BallotPaper.ReturnedObjectionsBefore, 'g', 3500);
        self.TotalDecisionsOnObjectionsFirstDay_Rule_Succeeded = ko.validatedObservable(true);
        self.TotalDecisionsOnObjectionsFirstDay = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsFirstDay, 'a', null, validationRules.A_eq_B_C_D_2days_1stDay_Rule_Before);
        self.DecisionsFullyAcceptedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsFirstDay, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsFirstDay, 'c', 3500);
        self.DecisionsRejectedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsFirstDay, 'd', 3500);
        self.ObjectionResponsesByLetterFirstDay = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterFirstDay, 'e', 3500);
        self.ObjectionsForwardedToOtherBodiesFirstDay = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherBodiesFirstDay, 'f', 3500);
        self.ReturnedObjectionsFirstDay = createValidatedObservable(BallotPaper.ReturnedObjectionsFirstDay, 'g', 3500);
        self.TotalDecisionsOnObjectionsSecondDay_Rule_Succeeded = ko.validatedObservable(true);
        self.TotalDecisionsOnObjectionsSecondDay = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsSecondDay, 'a', null, validationRules.A_eq_B_C_D_2days_2ndDay_Rule_Before);
        self.DecisionsFullyAcceptedObjectionsSecondDay = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsSecondDay, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsSecondDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsSecondDay, 'c', 3500);
        self.DecisionsRejectedObjectionsSecondDay = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsSecondDay, 'd', 3500);
        self.ObjectionResponsesByLetterSecondDay = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterSecondDay, 'e', 3500);
        self.ObjectionsForwardedToOtherBodiesSecondDay = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherBodiesSecondDay, 'f', 3500);
        self.ReturnedObjectionsSecondDay = createValidatedObservable(BallotPaper.ReturnedObjectionsSecondDay, 'g', 3500);
        self.PublicOrderViolationsOnVotingDay = createValidatedObservable(BallotPaper.PublicOrderViolationsOnVotingDay, '13', 3500);
        self.BallotAssistanceCount = createValidatedObservable(BallotPaper.BallotAssistanceCount, '14', 3500);
        self.NumberOfVotersInElectoralListByMunicipality = createValidatedObservable(BallotPaper.NumberOfVotersInElectoralListByMunicipality, '15', 3500);
        self.NumberOfRequestsByVoters_Rule_Succeeded = ko.validatedObservable(true);
        self.NumberOfRequestsByVoters = createValidatedObservable(BallotPaper.NumberOfRequestsByVoters, 'a', null, validationRules.A_B_C_eq_D_E_F_eq_G_H_Final_BESV_2days_Rule);
        self.NumberOfRequestsByElectoralRepresentatives = createValidatedObservable(BallotPaper.NumberOfRequestsByElectoralRepresentatives, 'b', 3500);
        self.NumberOfRequestsByObservers = createValidatedObservable(BallotPaper.NumberOfRequestsByObservers, 'c', 3500);
        self.NumberOfInclusionRequests = createValidatedObservable(BallotPaper.NumberOfInclusionRequests, 'd', 3500);
        self.NumberOfExclusionRequests = createValidatedObservable(BallotPaper.NumberOfExclusionRequests, 'e', 3500);
        self.NumberOfDataCorrectionRequests = createValidatedObservable(BallotPaper.NumberOfDataCorrectionRequests, 'f', 3500);
        self.NumberOfAdmittedRequests = createValidatedObservable(BallotPaper.NumberOfAdmittedRequests, 'h', 3500);
        self.NumberOfRejectedRequests = createValidatedObservable(BallotPaper.NumberOfRejectedRequests, 'g', 3500);
        self.NumberOfVotersInUpdatedElectoralList = createValidatedObservable(BallotPaper.NumberOfVotersInUpdatedElectoralList, '17', 3500);
        self.CertificatesReceivedFromCECE_Rule_Succeeded = ko.observable(true);
        self.CertificatesReceivedFromCECE = createValidatedObservable(BallotPaper.CertificatesReceivedFromCECE, 'a', null, validationRules.A_eq_B_D_BESV_Final_2days_Rule);
        self.CertificatesIssuedToVoters = createValidatedObservable(BallotPaper.CertificatesIssuedToVoters, 'b', 3500);
        self.SeriesOfCertificatesIssuedToVoters = createValidatedObservable(BallotPaper.SeriesOfCertificatesIssuedToVoters, 'c', 3500, null, 'text');
        self.UnusedOrCancelledCertificates = createValidatedObservable(BallotPaper.UnusedOrCancelledCertificates, 'd', 3500);
        self.SeriesOfUnusedOrCancelledCertificates = createValidatedObservable(BallotPaper.SeriesOfUnusedOrCancelledCertificates, 'e', 3500, null, 'text');
        self.TotalVotingRequestsAtLocation = createValidatedObservable(BallotPaper.TotalVotingRequestsAtLocation, '19', 3500);
        self.BallotsIssuedByMobileTeam = createValidatedObservable(BallotPaper.BallotsIssuedByMobileTeam, '20', 3500);
        self.BallotsIssuedByMobileTeamOnSecondVotingDay = createValidatedObservable(BallotPaper.BallotsIssuedByMobileTeamOnSecondVotingDay, '21', 3500);
        self.VotersBasedOnVotingCertificates = createValidatedObservable(BallotPaper.VotersBasedOnVotingCertificates, 'a', 3500);
        self.VotersWithTemporaryID = createValidatedObservable(BallotPaper.VotersWithTemporaryID, 'b', 3500);
        self.VotersWithPassport = createValidatedObservable(BallotPaper.VotersWithPassport, 'c', 3500);
        self.VotersWithExpiredPassport_Rule_Succeeded = ko.validatedObservable(true);
        self.VotersWithExpiredPassport = createValidatedObservable(BallotPaper.VotersWithExpiredPassport, 'd', null, validationRules.D_equal_or_less_than_C_2days_Rule);
        self.VotersAtLocationWithMobileBox = createValidatedObservable(BallotPaper.VotersAtLocationWithMobileBox, 'e', 3500);
        self.VotersInPenitentiaryInstitutions = createValidatedObservable(BallotPaper.VotersInPenitentiaryInstitutions, 'f', 3500);
        self.VotersInSanatoriumsOrHospitals = createValidatedObservable(BallotPaper.VotersInSanatoriumsOrHospitals, 'g', 3500);
        self.MilitaryVotersInMilitaryUnits = createValidatedObservable(BallotPaper.MilitaryVotersInMilitaryUnits, 'h', 3500);
        self.VotersWithLocomotorDisabilities = createValidatedObservable(BallotPaper.VotersWithLocomotorDisabilities, 'k', 3500);
        self.VotersWithVisualImpairments = createValidatedObservable(BallotPaper.VotersWithVisualImpairments, 'l', 3500);
        self.VotersWithHearingImpairments = createValidatedObservable(BallotPaper.VotersWithHearingImpairments, 'm', 3500);
        self.VotersBasedOnNewResidenceDeclaration = createValidatedObservable(BallotPaper.VotersBasedOnNewResidenceDeclaration, 'n', 3500);
        self.VotersWithStudentCard = createValidatedObservable(BallotPaper.VotersWithStudentCard, 'o', 3500);
        self.ElectoralOfficialsFromOtherSV = createValidatedObservable(BallotPaper.ElectoralOfficialsFromOtherSV, 'p', 3500);
        self.SignaturesOnBasicListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnBasicListsFirstDay, 'a', 3500);
        self.SignaturesOnSupplementaryListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnSupplementaryListsFirstDay, 'b', 3500);
        self.SignaturesOnVotingLocationListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnVotingLocationListsFirstDay, 'c', 3500);
        self.SignaturesOnBasicListSecondDay_Rule_Succeeded = ko.observable(true);
        self.SignaturesOnBasicListSecondDay = createValidatedObservable(BallotPaper.SignaturesOnBasicListSecondDay, 'a', null, validationRules.A_equal_or_more_than_point23A);
        self.SignaturesOnSupplementaryListSecondDay_Rule_Succeeded = ko.observable(true);
        self.SignaturesOnSupplementaryListSecondDay = createValidatedObservable(BallotPaper.SignaturesOnSupplementaryListSecondDay, 'b', null, validationRules.B_equal_or_more_than_point23B);
        self.SignaturesOnVotingLocationListSecondDay_Rule_Succeeded = ko.observable(true);
        self.SignaturesOnVotingLocationListSecondDay = createValidatedObservable(BallotPaper.SignaturesOnVotingLocationListSecondDay, 'c', null, validationRules.C_equal_or_more_than_point23C);
        self.AbsentElectoralBoardMembersOnVotingDay_Rule_Succeeded = ko.observable(true);
        self.AbsentElectoralBoardMembersOnVotingDay = createValidatedObservable(BallotPaper.AbsentElectoralBoardMembersOnVotingDay, '25', null, validationRules.equal_or_less_than_point_6_2days_Rule);
        self.AbsentElectoralBoardMembersOnSecondDay_Rule_Succeeded = ko.observable(true);
        self.AbsentElectoralBoardMembersOnSecondDay = createValidatedObservable(BallotPaper.AbsentElectoralBoardMembersOnSecondDay, '26', null, validationRules.equal_or_less_than_point_6_2days_2nd_day_Rule);
        self.PartyDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.PartyDesignatedRepresentativesCount, 'a', 3500);
        self.ElectoralBlocDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralBlocDesignatedRepresentativesCount, 'b', 3500);
        self.IndependentCandidateDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.IndependentCandidateDesignatedRepresentativesCount, 'c', 3500);
        self.ElectoralBoardMembersCount_Rule_Succeeded = ko.validatedObservable(true);
        self.ElectoralBoardMembersCount = createValidatedObservable(BallotPaper.ElectoralBoardMembersCount, 'a', null, validationRules.A_equal_point6_minus_point25_Rule);
        self.SIASOperatorsCount = createValidatedObservable(BallotPaper.SIASOperatorsCount, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesCount, 'c', 3500);
        self.InternationalObserversCount = createValidatedObservable(BallotPaper.InternationalObserversCount, 'd', 3500);
        self.NationalObserversFromAssociationsCount = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsCount, 'e', 3500);
        self.MediaRepresentativesCount = createValidatedObservable(BallotPaper.MediaRepresentativesCount, 'f', 3500);
        self.VotingResultsReportCountByBESV = createValidatedObservable(BallotPaper.VotingResultsReportCountByBESV, '29', 3500);
        self.TotalDecisionsOnObjectionsBefore_Rule_Succeeded = ko.validatedObservable(true);
        self.TotalDecisionsOnObjectionsBefore = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsBefore, 'a', null, validationRules.A_eq_B_C_D_2days_Rule_Before);
        self.DecisionsFullyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsBefore, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsBefore, 'c', 3500);
        self.DecisionsRejectedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsBefore, 'd', 3500);
        self.ObjectionResponsesByLetterBefore = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterBefore, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesBefore = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesBefore, 'f', 3500);
        self.ReturnedObjectionsBefore = createValidatedObservable(BallotPaper.ReturnedObjectionsBefore, 'g', 3500);
    }

    // Final BESV Report (elections, 2 days, round II)
    if ([13, 19].includes(templateId)) {
        self.ElectoralBoardMembersOnOpening = createValidatedObservable(BallotPaper.ElectoralBoardMembersOnOpening, 'a', 3500);
        self.SIASOperatorsOnOpening = createValidatedObservable(BallotPaper.SIASOperatorsOnOpening, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesOnOpening = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesOnOpening, 'c', 3500);
        self.InternationalObserversOnOpening = createValidatedObservable(BallotPaper.InternationalObserversOnOpening, 'd', 3500);
        self.NationalObserversFromAssociationsOnOpening = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsOnOpening, 'e', 3500);
        self.MediaRepresentativesOnOpening = createValidatedObservable(BallotPaper.MediaRepresentativesOnOpening, 'f', 3500);
        self.ElectoralBoardMembersSecondDay = createValidatedObservable(BallotPaper.ElectoralBoardMembersSecondDay, 'a', 3500);
        self.SIASOperatorsSecondDay = createValidatedObservable(BallotPaper.SIASOperatorsSecondDay, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesSecondDay = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesSecondDay, 'c', 3500);
        self.InternationalObserversSecondDay = createValidatedObservable(BallotPaper.InternationalObserversSecondDay, 'd', 3500);
        self.NationalObserversFromAssociationsSecondDay = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsSecondDay, 'e', 3500);
        self.MediaRepresentativesSecondDay = createValidatedObservable(BallotPaper.MediaRepresentativesSecondDay, 'f', 3500);
        self.StationaryVotingBox80Liters = createValidatedObservable(BallotPaper.StationaryVotingBox80Liters, 'a', 10);
        self.StationaryVotingBox45Liters = createValidatedObservable(BallotPaper.StationaryVotingBox45Liters, 'b', 10);
        self.MobileVotingBox27Liters = createValidatedObservable(BallotPaper.MobileVotingBox27Liters, 'c', 5);
        self.PlasticSealsForVotingBoxCount_Rule_Succeeded = ko.observable(true);
        self.PlasticSealsForVotingBoxCount = createValidatedObservable(BallotPaper.PlasticSealsForVotingBoxCount, 'd', null, validationRules.D_eq_or_GT_4A_4B_C_2days_Rule);
        self.PlasticSealCodesOnVotingBox = createValidatedObservable(BallotPaper.PlasticSealCodesOnVotingBox, 'e', 3500);
        self.PlasticSealCodesOnVotingBoxSlits = createValidatedObservable(BallotPaper.PlasticSealCodesOnVotingBoxSlits, 'f', 3500);
        self.VotingBoothsCount = createValidatedObservable(BallotPaper.VotingBoothsCount, 'a', 10);
        self.SpecialNeedsVotingBoothsCount = createValidatedObservable(BallotPaper.SpecialNeedsVotingBoothsCount, 'b', 5);
        self.StampedVotedCount = createValidatedObservable(BallotPaper.StampedVotedCount, 'a', 3500);
        self.StampedWithdrawnCount = createValidatedObservable(BallotPaper.StampedWithdrawnCount, 'b', 3500);
        self.ElectoralBoardMembersNumericComposition = createValidatedObservable(BallotPaper.ElectoralBoardMembersNumericComposition, '6', 15);
        self.MeetingsCountBetweenRounds = createValidatedObservable(BallotPaper.MeetingsCountBetweenRounds, '7', 3500);
        self.DecisionsCountBetweenRounds = createValidatedObservable(BallotPaper.DecisionsCountBetweenRounds, '8', 3500);
        self.ContestationsUntilEDay_Rule_Succeeded = ko.observable(true);
        self.ContestationsUntilEDay = createValidatedObservable(BallotPaper.ContestationsUntilEDay, 'a', null, validationRules.A_eq_10A_10E_10F_10G_2days_Rule);
        self.ContestationsOnEDay_Rule_Succeeded = ko.observable(true);
        self.ContestationsOnEDay = createValidatedObservable(BallotPaper.ContestationsOnEDay, 'b', null, validationRules.A_eq_11A_11E_11F_11G_2days_Rule);
        self.SubmittedOnVotingDayCountSecondDay_Rule_Succeeded = ko.observable(true);
        self.SubmittedOnVotingDayCountSecondDay = createValidatedObservable(BallotPaper.SubmittedOnVotingDayCountSecondDay, 'c', null, validationRules.A_eq_12A_12E_12F_12G_2days_Rule);
        self.TotalDecisionsOnObjectionsBefore_Rule_Succeeded = ko.observable(true);
        self.TotalDecisionsOnObjectionsBefore = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsBefore, 'a', null, validationRules.A_eq_B_C_D_2nd_round_before_Rule);
        self.DecisionsFullyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsBefore, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsBefore, 'c', 3500);
        self.DecisionsRejectedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsBefore, 'd', 3500);
        self.ObjectionResponsesByLetterBefore = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterBefore, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesBefore = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesBefore, 'f', 3500);
        self.ReturnedObjectionsBefore = createValidatedObservable(BallotPaper.ReturnedObjectionsBefore, 'g', 3500);
        self.TotalDecisionsOnObjectionsFirstDay_Rule_Succeeded = ko.observable(true);
        self.TotalDecisionsOnObjectionsFirstDay = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsFirstDay, 'a', null, validationRules.A_eq_B_C_D_2days_1stDay_Rule_Before);
        self.DecisionsFullyAcceptedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsFirstDay, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsFirstDay, 'c', 3500);
        self.DecisionsRejectedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsFirstDay, 'd', 3500);
        self.ObjectionResponsesByLetterFirstDay = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterFirstDay, 'e', 3500);
        self.ObjectionsForwardedToOtherBodiesFirstDay = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherBodiesFirstDay, 'f', 3500);
        self.ReturnedObjectionsFirstDay = createValidatedObservable(BallotPaper.ReturnedObjectionsFirstDay, 'g', 3500);
        self.TotalDecisionsOnObjectionsSecondDay_Rule_Succeeded = ko.observable(true);
        self.TotalDecisionsOnObjectionsSecondDay = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsSecondDay, 'a', null, validationRules.A_eq_B_C_D_2days_2ndDay_Rule_Before);
        self.DecisionsFullyAcceptedObjectionsSecondDay = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsSecondDay, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsSecondDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsSecondDay, 'c', 3500);
        self.DecisionsRejectedObjectionsSecondDay = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsSecondDay, 'd', 3500);
        self.ObjectionResponsesByLetterSecondDay = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterSecondDay, 'e', 3500);
        self.ObjectionsForwardedToOtherBodiesSecondDay = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherBodiesSecondDay, 'f', 3500);
        self.ReturnedObjectionsSecondDay = createValidatedObservable(BallotPaper.ReturnedObjectionsSecondDay, 'g', 3500);
        self.PublicOrderViolationsOnVotingDay = createValidatedObservable(BallotPaper.PublicOrderViolationsOnVotingDay, '13', 3500);
        self.BallotAssistanceCount = createValidatedObservable(BallotPaper.BallotAssistanceCount, '14', 3500);
        self.CertificatesReceivedFromCECE_Rule_Succeeded = ko.observable(true);
        self.CertificatesReceivedFromCECE = createValidatedObservable(BallotPaper.CertificatesReceivedFromCECE, 'a', null, validationRules.A_eq_B_D_BESV_Final_2days_Rule);
        self.CertificatesIssuedToVoters = createValidatedObservable(BallotPaper.CertificatesIssuedToVoters, 'b', 3500);
        self.SeriesOfCertificatesIssuedToVoters = createValidatedObservable(BallotPaper.SeriesOfCertificatesIssuedToVoters, 'c', 3500);
        self.UnusedOrCancelledCertificates = createValidatedObservable(BallotPaper.UnusedOrCancelledCertificates, 'd', 3500);
        self.SeriesOfUnusedOrCancelledCertificates = createValidatedObservable(BallotPaper.SeriesOfUnusedOrCancelledCertificates, 'e', 3500);
        self.TotalVotingRequestsAtLocation = createValidatedObservable(BallotPaper.TotalVotingRequestsAtLocation, '16', 3500);
        self.BallotsIssuedByMobileTeam = createValidatedObservable(BallotPaper.BallotsIssuedByMobileTeam, '17', 3500);
        self.BallotsIssuedByMobileTeamOnSecondVotingDay = createValidatedObservable(BallotPaper.BallotsIssuedByMobileTeamOnSecondVotingDay, '18', 3500);
        self.VotersBasedOnVotingCertificates = createValidatedObservable(BallotPaper.VotersBasedOnVotingCertificates, 'a', 3500);
        self.VotersWithTemporaryID = createValidatedObservable(BallotPaper.VotersWithTemporaryID, 'b', 3500);
        self.VotersWithPassport = createValidatedObservable(BallotPaper.VotersWithPassport, 'c', 3500);
        self.VotersWithExpiredPassport_Rule_Succeeded = ko.observable(true);
        self.VotersWithExpiredPassport = createValidatedObservable(BallotPaper.VotersWithExpiredPassport, 'd', null, validationRules.D_equal_or_less_than_C_2nd_round_Rule);
        self.VotersAtLocationWithMobileBox = createValidatedObservable(BallotPaper.VotersAtLocationWithMobileBox, 'e', 3500);
        self.VotersInPenitentiaryInstitutions = createValidatedObservable(BallotPaper.VotersInPenitentiaryInstitutions, 'f', 3500);
        self.VotersInSanatoriumsOrHospitals = createValidatedObservable(BallotPaper.VotersInSanatoriumsOrHospitals, 'g', 3500);
        self.MilitaryVotersInMilitaryUnits = createValidatedObservable(BallotPaper.MilitaryVotersInMilitaryUnits, 'h', 3500);
        self.VotersWithLocomotorDisabilities = createValidatedObservable(BallotPaper.VotersWithLocomotorDisabilities, 'k', 3500);
        self.VotersWithVisualImpairments = createValidatedObservable(BallotPaper.VotersWithVisualImpairments, 'l', 3500);
        self.VotersWithHearingImpairments = createValidatedObservable(BallotPaper.VotersWithHearingImpairments, 'm', 3500);
        self.VotersBasedOnNewResidenceDeclaration = createValidatedObservable(BallotPaper.VotersBasedOnNewResidenceDeclaration, 'n', 3500);
        self.VotersWithStudentCard = createValidatedObservable(BallotPaper.VotersWithStudentCard, 'o', 3500);
        self.ElectoralOfficialsFromOtherSV = createValidatedObservable(BallotPaper.ElectoralOfficialsFromOtherSV, 'p', 3500);
        self.SignaturesOnBasicListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnBasicListsFirstDay, 'a', 3500);
        self.SignaturesOnSupplementaryListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnSupplementaryListsFirstDay, 'b', 3500);
        self.SignaturesOnVotingLocationListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnVotingLocationListsFirstDay, 'c', 3500);
        self.SignaturesOnBasicListSecondDay_Rule_Succeeded = ko.observable(true);
        self.SignaturesOnBasicListSecondDay = createValidatedObservable(BallotPaper.SignaturesOnBasicListSecondDay, 'a', null, validationRules.A_equal_or_more_than_point23A);
        self.SignaturesOnSupplementaryListSecondDay_Rule_Succeeded = ko.observable(true);
        self.SignaturesOnSupplementaryListSecondDay = createValidatedObservable(BallotPaper.SignaturesOnSupplementaryListsSecondDay, 'b', null, validationRules.B_equal_or_more_than_point23B);
        self.SignaturesOnVotingLocationListSecondDay_Rule_Succeeded = ko.observable(true);
        self.SignaturesOnVotingLocationListSecondDay = createValidatedObservable(BallotPaper.SignaturesOnVotingLocationListsSecondDay, 'c', null, validationRules.C_equal_or_more_than_point23C);
        self.AbsentElectoralBoardMembersOnVotingDay_Rule_Succeeded = ko.observable(true);
        self.AbsentElectoralBoardMembersOnVotingDay = createValidatedObservable(BallotPaper.AbsentElectoralBoardMembersOnVotingDay, '22', null, validationRules.equal_or_less_than_point_6_Final_BESV_2days_2nd_day_Rule);
        self.AbsentElectoralBoardMembersOnSecondDay_Rule_Succeeded = ko.observable(true);
        self.AbsentElectoralBoardMembersOnSecondDay = createValidatedObservable(BallotPaper.AbsentElectoralBoardMembersOnSecondDay, '23', null, validationRules.equal_or_less_than_point_6_Final_BESV_2nd_round_Rule);
        self.PartyDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.PartyDesignatedRepresentativesCount, 'a', 3500);
        self.ElectoralBlocDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralBlocDesignatedRepresentativesCount, 'b', 3500);
        self.IndependentCandidateDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.IndependentCandidateDesignatedRepresentativesCount, 'c', 3500);
        self.ElectoralBoardMembersCount_Rule_Succeeded = ko.observable(true);
        self.ElectoralBoardMembersCount = createValidatedObservable(BallotPaper.ElectoralBoardMembersCount, 'a', null, validationRules.A_equal_point6_minus_point25_Rule);
        self.SIASOperatorsCount = createValidatedObservable(BallotPaper.SIASOperatorsCount, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesCount, 'c', 3500);
        self.InternationalObserversCount = createValidatedObservable(BallotPaper.InternationalObserversCount, 'd', 3500);
        self.NationalObserversFromAssociationsCount = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsCount, 'e', 3500);
        self.MediaRepresentativesCount = createValidatedObservable(BallotPaper.MediaRepresentativesCount, 'f', 3500);
        self.VotingResultsReportCountByBESV = createValidatedObservable(BallotPaper.VotingResultsReportCountByBESV, '26', 3500);
    }

    // Final BESV Report (referendum, 1 day)
    if ([14, 20].includes(templateId)) {
        self.ElectoralBoardMembersOnOpening = createValidatedObservable(BallotPaper.ElectoralBoardMembersOnOpening, 'a', 3500);
        self.SIASOperatorsOnOpening = createValidatedObservable(BallotPaper.SIASOperatorsOnOpening, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesOnOpening = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesOnOpening, 'c', 3500);
        self.InternationalObserversOnOpening = createValidatedObservable(BallotPaper.InternationalObserversOnOpening, 'd', 3500);
        self.NationalObserversFromAssociationsOnOpening = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsOnOpening, 'e', 3500);
        self.MediaRepresentativesOnOpening = createValidatedObservable(BallotPaper.MediaRepresentativesOnOpening, 'f', 3500);
        self.StationaryVotingBox80Liters = createValidatedObservable(BallotPaper.StationaryVotingBox80Liters, 'a', 10);
        self.StationaryVotingBox45Liters = createValidatedObservable(BallotPaper.StationaryVotingBox45Liters, 'b', 10);
        self.MobileVotingBox27Liters = createValidatedObservable(BallotPaper.MobileVotingBox27Liters, 'c', 5);
        self.PlasticSealsForVotingBoxCount_Rule_Succeeded = ko.observable(true);
        self.PlasticSealsForVotingBoxCount = createValidatedObservable(BallotPaper.PlasticSealsForVotingBoxCount, 'd', null, validationRules.D_eq_or_GT_4A_4B_C_2days_Rule);
        self.PlasticSealCodesOnVotingBox = createValidatedObservable(BallotPaper.PlasticSealCodesOnVotingBox, 'e', 3500);
        self.VotingBoothsCount = createValidatedObservable(BallotPaper.VotingBoothsCount, 'a', 10);
        self.SpecialNeedsVotingBoothsCount = createValidatedObservable(BallotPaper.SpecialNeedsVotingBoothsCount, 'b', 5);
        self.StampedVotedCount = createValidatedObservable(BallotPaper.StampedVotedCount, 'a', 3500);
        self.StampedWithdrawnCount = createValidatedObservable(BallotPaper.StampedWithdrawnCount, 'b', 3500);
        self.ElectoralBoardMembersNumericComposition = createValidatedObservable(BallotPaper.ElectoralBoardMembersNumericComposition, '5', 15);
        self.MeetingsCountBetweenRounds = createValidatedObservable(BallotPaper.MeetingsCountBetweenRounds, '6', 3500);
        self.DecisionsCountBetweenRounds = createValidatedObservable(BallotPaper.DecisionsCountBetweenRounds, '7', 3500);
        self.ContestationsUntilEDay_Rule_Succeeded = ko.observable(true);
        self.ContestationsUntilEDay = createValidatedObservable(BallotPaper.ContestationsUntilEDay, 'a', null, validationRules.A_eq_9A_9E_9F_9G_Final_BESV_Referendum_1day_Rule);
        self.ContestationsOnEDay_Rule_Succeeded = ko.observable(true);
        self.ContestationsOnEDay = createValidatedObservable(BallotPaper.ContestationsOnEDay, 'b', null, validationRules.A_eq_10A_10E_10F_10G_Final_BESV_Referendum_1day_Rule);
        self.TotalDecisionsOnObjectionsBefore_Rule_Succeeded = ko.observable(true);
        self.TotalDecisionsOnObjectionsBefore = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsBefore, 'a', null, validationRules.A_eq_B_C_D_Final_BESV_Referendum_1day_Rule_Before);
        self.DecisionsFullyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsBefore, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsBefore, 'c', 3500);
        self.DecisionsRejectedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsBefore, 'd', 3500);
        self.ObjectionResponsesByLetterBefore = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterBefore, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesBefore = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesBefore, 'f', 3500);
        self.ReturnedObjectionsBefore = createValidatedObservable(BallotPaper.ReturnedObjectionsBefore, 'g', 3500);
        self.TotalDecisionsOnObjectionsFirstDay_Rule_Succeeded = ko.observable(true);
        self.TotalDecisionsOnObjectionsFirstDay = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsFirstDay, 'a', null, validationRules.A_eq_B_C_D_Final_BESV_Referendum_1day_Rule_1stDay);
        self.DecisionsFullyAcceptedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsFirstDay, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsFirstDay, 'c', 3500);
        self.DecisionsRejectedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsFirstDay, 'd', 3500);
        self.ObjectionResponsesByLetterFirstDay = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterFirstDay, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesFirstDay = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesFirstDay, 'f', 3500);
        self.ReturnedObjectionsFirstDay = createValidatedObservable(BallotPaper.ReturnedObjectionsFirstDay, 'g', 3500);
        self.PublicOrderViolationsOnVotingDay = createValidatedObservable(BallotPaper.PublicOrderViolationsOnVotingDay, '11', 3500);
        self.BallotAssistanceCount = createValidatedObservable(BallotPaper.BallotAssistanceCount, '12', 3500);
        self.NumberOfVotersInElectoralListByMunicipality = createValidatedObservable(BallotPaper.NumberOfVotersInElectoralListByMunicipality, '13', 3500);
        self.NumberOfRequestsByVoters_Rule_Succeeded = ko.validatedObservable(true);
        self.NumberOfRequestsByVoters = createValidatedObservable(BallotPaper.NumberOfRequestsByVoters, 'a', null, validationRules.A_B_C_eq_D_E_F_eq_G_H_2days_Rule);
        self.NumberOfRequestsByElectoralRepresentatives = createValidatedObservable(BallotPaper.NumberOfRequestsByElectoralRepresentatives, 'b', 3500);
        self.NumberOfRequestsByObservers = createValidatedObservable(BallotPaper.NumberOfRequestsByObservers, 'c', 3500);
        self.NumberOfInclusionRequests = createValidatedObservable(BallotPaper.NumberOfInclusionRequests, 'd', 3500);
        self.NumberOfExclusionRequests = createValidatedObservable(BallotPaper.NumberOfExclusionRequests, 'e', 3500);
        self.NumberOfDataCorrectionRequests = createValidatedObservable(BallotPaper.NumberOfDataCorrectionRequests, 'f', 3500);
        self.NumberOfAdmittedRequests = createValidatedObservable(BallotPaper.NumberOfAdmittedRequests, 'h', 3500);
        self.NumberOfRejectedRequests = createValidatedObservable(BallotPaper.NumberOfRejectedRequests, 'g', 3500);
        self.NumberOfVotersInUpdatedElectoralList = createValidatedObservable(BallotPaper.NumberOfVotersInUpdatedElectoralList, '15', 3500);
        self.CertificatesReceivedFromCECE_Rule_Succeeded = ko.observable(true);
        self.CertificatesReceivedFromCECE = createValidatedObservable(BallotPaper.CertificatesReceivedFromCECE, 'a', null, validationRules.A_eq_B_D_Final_BESV_Referendum_Rule);
        self.CertificatesIssuedToVoters = createValidatedObservable(BallotPaper.CertificatesIssuedToVoters, 'b', 3500);
        self.SeriesOfCertificatesIssuedToVoters = createValidatedObservable(BallotPaper.SeriesOfCertificatesIssuedToVoters, 'c', 3500, null, 'text');
        self.UnusedOrCancelledCertificates = createValidatedObservable(BallotPaper.UnusedOrCancelledCertificates, 'd', 3500);
        self.SeriesOfUnusedOrCancelledCertificates = createValidatedObservable(BallotPaper.SeriesOfUnusedOrCancelledCertificates, 'e', 3500, null, 'text');
        self.VotingRequestsAtLocation = createValidatedObservable(BallotPaper.VotingRequestsAtLocation, '17', 3500);
        self.BallotsIssuedByMobileTeam = createValidatedObservable(BallotPaper.BallotsIssuedByMobileTeam, '18', 3500);
        self.VotersBasedOnVotingCertificates = createValidatedObservable(BallotPaper.VotersBasedOnVotingCertificates, 'a', 3500);
        self.VotersWithTemporaryID = createValidatedObservable(BallotPaper.VotersWithTemporaryID, 'b', 3500);
        self.VotersWithPassport = createValidatedObservable(BallotPaper.VotersWithPassport, 'c', 3500);
        self.VotersWithExpiredPassport_Rule_Succeeded = ko.observable(true);
        self.VotersWithExpiredPassport = createValidatedObservable(BallotPaper.VotersWithExpiredPassport, 'd', null, validationRules.D_equal_or_less_than_C_2nd_round_Rule);
        self.VotersAtLocationWithMobileBox = createValidatedObservable(BallotPaper.VotersAtLocationWithMobileBox, 'e', 3500);
        self.VotersInPenitentiaryInstitutions = createValidatedObservable(BallotPaper.VotersInPenitentiaryInstitutions, 'f', 3500);
        self.VotersInSanatoriumsOrHospitals = createValidatedObservable(BallotPaper.VotersInSanatoriumsOrHospitals, 'g', 3500);
        self.MilitaryVotersInMilitaryUnits = createValidatedObservable(BallotPaper.MilitaryVotersInMilitaryUnits, 'h', 3500);
        self.VotersWithLocomotorDisabilities = createValidatedObservable(BallotPaper.VotersWithLocomotorDisabilities, 'k', 3500);
        self.VotersWithVisualImpairments = createValidatedObservable(BallotPaper.VotersWithVisualImpairments, 'l', 3500);
        self.VotersWithHearingImpairments = createValidatedObservable(BallotPaper.VotersWithHearingImpairments, 'm', 3500);
        self.VotersBasedOnNewResidenceDeclaration = createValidatedObservable(BallotPaper.VotersBasedOnNewResidenceDeclaration, 'n', 3500);
        self.VotersWithStudentCard = createValidatedObservable(BallotPaper.VotersWithStudentCard, 'o', 3500);
        self.ElectoralOfficialsFromOtherSV = createValidatedObservable(BallotPaper.ElectoralOfficialsFromOtherSV, 'p', 3500);
        self.SignaturesOnBasicListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnBasicListsFirstDay, 'a', 3500);
        self.SignaturesOnSupplementaryListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnSupplementaryListsFirstDay, 'b', 3500);
        self.SignaturesOnVotingLocationListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnVotingLocationListsFirstDay, 'c', 3500);
        self.AbsentElectoralBoardMembersOnVotingDay_Rule_Succeeded = ko.validatedObservable(true);
        self.AbsentElectoralBoardMembersOnVotingDay = createValidatedObservable(BallotPaper.AbsentElectoralBoardMembersOnVotingDay, '21', null, validationRules.equal_or_less_than_point_5_referendum_1day_Rule);
        self.PartyDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.PartyDesignatedRepresentativesCount, 'a', 3500);
        self.ElectoralBlocDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralBlocDesignatedRepresentativesCount, 'b', 3500);
        self.IndependentCandidateDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.IndependentCandidateDesignatedRepresentativesCount, 'c', 3500);
        self.ElectoralBoardMembersCount_Rule_Succeeded = ko.validatedObservable(true);
        self.ElectoralBoardMembersCount = createValidatedObservable(BallotPaper.ElectoralBoardMembersCount, 'a', null, validationRules.A_equal_point5_minus_point21_referendum_Rule);
        self.SIASOperatorsCount = createValidatedObservable(BallotPaper.SIASOperatorsCount, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesCount, 'c', 3500);
        self.InternationalObserversCount = createValidatedObservable(BallotPaper.InternationalObserversCount, 'd', 3500);
        self.NationalObserversFromAssociationsCount = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsCount, 'e', 3500);
        self.MediaRepresentativesCount = createValidatedObservable(BallotPaper.MediaRepresentativesCount, 'f', 3500);
        self.VotingResultsReportCountByBESV = createValidatedObservable(BallotPaper.VotingResultsReportCountByBESV, '24', 3500);
    }

    // Final BESV Report (referendum, 2 days)
    if ([15, 21].includes(templateId)) {
        self.ElectoralBoardMembersOnOpening = createValidatedObservable(BallotPaper.ElectoralBoardMembersOnOpening, 'a', 3500);
        self.SIASOperatorsOnOpening = createValidatedObservable(BallotPaper.SIASOperatorsOnOpening, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesOnOpening = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesOnOpening, 'c', 3500);
        self.InternationalObserversOnOpening = createValidatedObservable(BallotPaper.InternationalObserversOnOpening, 'd', 3500);
        self.NationalObserversFromAssociationsOnOpening = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsOnOpening, 'e', 3500);
        self.MediaRepresentativesOnOpening = createValidatedObservable(BallotPaper.MediaRepresentativesOnOpening, 'f', 3500);
        self.ElectoralBoardMembersSecondDay = createValidatedObservable(BallotPaper.ElectoralBoardMembersSecondDay, 'a', 3500);
        self.SIASOperatorsSecondDay = createValidatedObservable(BallotPaper.SIASOperatorsSecondDay, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesSecondDay = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesSecondDay, 'c', 3500);
        self.InternationalObserversSecondDay = createValidatedObservable(BallotPaper.InternationalObserversSecondDay, 'd', 3500);
        self.NationalObserversFromAssociationsSecondDay = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsSecondDay, 'e', 3500);
        self.MediaRepresentativesSecondDay = createValidatedObservable(BallotPaper.MediaRepresentativesSecondDay, 'f', 3500);
        self.StationaryVotingBox80Liters = createValidatedObservable(BallotPaper.StationaryVotingBox80Liters, 'a', 10);
        self.StationaryVotingBox45Liters = createValidatedObservable(BallotPaper.StationaryVotingBox45Liters, 'b', 10);
        self.MobileVotingBox27Liters = createValidatedObservable(BallotPaper.MobileVotingBox27Liters, 'c', 5);
        self.PlasticSealsForVotingBoxCount_Rule_Succeeded = ko.validatedObservable(true);
        self.PlasticSealsForVotingBoxCount = createValidatedObservable(BallotPaper.PlasticSealsForVotingBoxCount, 'd', null, validationRules.D_eq_or_GT_4A_4B_C_2days_Rule);
        self.PlasticSealCodesOnVotingBox = createValidatedObservable(BallotPaper.PlasticSealCodesOnVotingBox, 'e', 3500, null, 'text');
        self.PlasticSealCodesOnVotingBoxSlits_Rule_Succeeded = ko.validatedObservable(true);
        self.PlasticSealCodesOnVotingBoxSlits = createValidatedObservable(BallotPaper.PlasticSealCodesOnVotingBoxSlits, 'f', 3500, null, 'text');
        self.VotingBoothsCount = createValidatedObservable(BallotPaper.VotingBoothsCount, 'a', 10);
        self.SpecialNeedsVotingBoothsCount = createValidatedObservable(BallotPaper.SpecialNeedsVotingBoothsCount, 'b', 5);
        self.StampedVotedCount = createValidatedObservable(BallotPaper.StampedVotedCount, 'a', 3500);
        self.StampedWithdrawnCount = createValidatedObservable(BallotPaper.StampedWithdrawnCount, 'b', 3500);
        self.ElectoralBoardMembersNumericComposition = createValidatedObservable(BallotPaper.ElectoralBoardMembersNumericComposition, '6', 15);
        self.MeetingsCountBetweenRounds = createValidatedObservable(BallotPaper.MeetingsCountBetweenRounds, '7', 3500);
        self.DecisionsCountBetweenRounds = createValidatedObservable(BallotPaper.DecisionsCountBetweenRounds, '8', 3500);
        self.ContestationsUntilEDay_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsUntilEDay = createValidatedObservable(BallotPaper.ContestationsUntilEDay, 'a', null, validationRules.A_eq_10A_10E_10F_10G_2days_Rule);
        self.ContestationsOnEDay_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsOnEDay = createValidatedObservable(BallotPaper.ContestationsOnEDay, 'b', null, validationRules.A_eq_11A_11E_11F_11G_2days_Rule);
        self.SubmittedOnVotingDayCountSecondDay_Rule_Succeeded = ko.validatedObservable(true);
        self.SubmittedOnVotingDayCountSecondDay = createValidatedObservable(BallotPaper.SubmittedOnVotingDayCountSecondDay, 'c', null, validationRules.A_eq_12A_12E_12F_12G_2days_Rule);
        self.TotalDecisionsOnObjectionsBefore_Rule_Succeeded = ko.validatedObservable(true);
        self.TotalDecisionsOnObjectionsBefore = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsBefore, 'a', null, validationRules.A_eq_B_C_D_2days_Rule_Before);
        self.DecisionsFullyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsBefore, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsBefore, 'c', 3500);
        self.DecisionsRejectedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsBefore, 'd', 3500);
        self.ObjectionResponsesByLetterBefore = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterBefore, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesBefore = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesBefore, 'f', 3500);
        self.ReturnedObjectionsBefore = createValidatedObservable(BallotPaper.ReturnedObjectionsBefore, 'g', 3500);
        self.TotalDecisionsOnObjectionsFirstDay_Rule_Succeeded = ko.validatedObservable(true);
        self.TotalDecisionsOnObjectionsFirstDay = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsFirstDay, 'a', null, validationRules.A_eq_B_C_D_2days_1stDay_Rule_Before);
        self.DecisionsFullyAcceptedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsFirstDay, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsFirstDay, 'c', 3500);
        self.DecisionsRejectedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsFirstDay, 'd', 3500);
        self.ObjectionResponsesByLetterFirstDay = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterFirstDay, 'e', 3500);
        self.ObjectionsForwardedToOtherBodiesFirstDay = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherBodiesFirstDay, 'f', 3500);
        self.ReturnedObjectionsFirstDay = createValidatedObservable(BallotPaper.ReturnedObjectionsFirstDay, 'g', 3500);
        self.TotalDecisionsOnObjectionsSecondDay_Rule_Succeeded = ko.validatedObservable(true);
        self.TotalDecisionsOnObjectionsSecondDay = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsSecondDay, 'a', null, validationRules.A_eq_B_C_D_2days_2ndDay_Rule_Before);
        self.DecisionsFullyAcceptedObjectionsSecondDay = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsSecondDay, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsSecondDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsSecondDay, 'c', 3500);
        self.DecisionsRejectedObjectionsSecondDay = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsSecondDay, 'd', 3500);
        self.ObjectionResponsesByLetterSecondDay = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterSecondDay, 'e', 3500);
        self.ObjectionsForwardedToOtherBodiesSecondDay = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherBodiesSecondDay, 'f', 3500);
        self.ReturnedObjectionsSecondDay = createValidatedObservable(BallotPaper.ReturnedObjectionsSecondDay, 'g', 3500);
        self.PublicOrderViolationsOnVotingDay = createValidatedObservable(BallotPaper.PublicOrderViolationsOnVotingDay, '13', 3500);
        self.BallotAssistanceCount = createValidatedObservable(BallotPaper.BallotAssistanceCount, '14', 3500);
        self.NumberOfVotersInElectoralListByMunicipality = createValidatedObservable(BallotPaper.NumberOfVotersInElectoralListByMunicipality, '15', 3500);
        self.NumberOfRequestsByVoters_Rule_Succeeded = ko.validatedObservable(true);
        self.NumberOfRequestsByVoters = createValidatedObservable(BallotPaper.NumberOfRequestsByVoters, 'a', null, validationRules.A_B_C_eq_D_E_F_eq_G_H_2days_Rule);
        self.NumberOfRequestsByElectoralRepresentatives = createValidatedObservable(BallotPaper.NumberOfRequestsByElectoralRepresentatives, 'b', 3500);
        self.NumberOfRequestsByObservers = createValidatedObservable(BallotPaper.NumberOfRequestsByObservers, 'c', 3500);
        self.NumberOfInclusionRequests = createValidatedObservable(BallotPaper.NumberOfInclusionRequests, 'd', 3500);
        self.NumberOfExclusionRequests = createValidatedObservable(BallotPaper.NumberOfExclusionRequests, 'e', 3500);
        self.NumberOfDataCorrectionRequests = createValidatedObservable(BallotPaper.NumberOfDataCorrectionRequests, 'f', 3500);
        self.NumberOfAdmittedRequests = createValidatedObservable(BallotPaper.NumberOfAdmittedRequests, 'h', 3500);
        self.NumberOfRejectedRequests = createValidatedObservable(BallotPaper.NumberOfRejectedRequests, 'g', 3500);
        self.NumberOfVotersInUpdatedElectoralList = createValidatedObservable(BallotPaper.NumberOfVotersInUpdatedElectoralList, '17', 3500);
        self.CertificatesReceivedFromCECE_Rule_Succeeded = ko.observable(true);
        self.CertificatesReceivedFromCECE = createValidatedObservable(BallotPaper.CertificatesReceivedFromCECE, 'a', null, validationRules.A_eq_B_D_Final_BESV_Referendum_2days_Rule);
        self.CertificatesIssuedToVoters = createValidatedObservable(BallotPaper.CertificatesIssuedToVoters, 'b', 3500);
        self.SeriesOfCertificatesIssuedToVoters = createValidatedObservable(BallotPaper.SeriesOfCertificatesIssuedToVoters, 'c', 3500, null, 'text');
        self.UnusedOrCancelledCertificates = createValidatedObservable(BallotPaper.UnusedOrCancelledCertificates, 'd', 3500);
        self.SeriesOfUnusedOrCancelledCertificates = createValidatedObservable(BallotPaper.SeriesOfUnusedOrCancelledCertificates, 'e', 3500, null, 'text');
        self.TotalVotingRequestsAtLocation = createValidatedObservable(BallotPaper.TotalVotingRequestsAtLocation, '19', 3500);
        self.BallotsIssuedByMobileTeam = createValidatedObservable(BallotPaper.BallotsIssuedByMobileTeam, '20', 3500);
        self.BallotsIssuedByMobileTeamOnSecondVotingDay = createValidatedObservable(BallotPaper.BallotsIssuedByMobileTeamOnSecondVotingDay, '21', 3500);
        self.VotersBasedOnVotingCertificates = createValidatedObservable(BallotPaper.VotersBasedOnVotingCertificates, 'a', 3500);
        self.VotersWithTemporaryID = createValidatedObservable(BallotPaper.VotersWithTemporaryID, 'b', 3500);
        self.VotersWithPassport = createValidatedObservable(BallotPaper.VotersWithPassport, 'c', 3500);
        self.VotersWithExpiredPassport_Rule_Succeeded = ko.validatedObservable(true);
        self.VotersWithExpiredPassport = createValidatedObservable(BallotPaper.VotersWithExpiredPassport, 'd', null, validationRules.D_equal_or_less_than_C_2days_Rule);
        self.VotersAtLocationWithMobileBox = createValidatedObservable(BallotPaper.VotersAtLocationWithMobileBox, 'e', 3500);
        self.VotersInPenitentiaryInstitutions = createValidatedObservable(BallotPaper.VotersInPenitentiaryInstitutions, 'f', 3500);
        self.VotersInSanatoriumsOrHospitals = createValidatedObservable(BallotPaper.VotersInSanatoriumsOrHospitals, 'g', 3500);
        self.MilitaryVotersInMilitaryUnits = createValidatedObservable(BallotPaper.MilitaryVotersInMilitaryUnits, 'h', 3500);
        self.VotersWithLocomotorDisabilities = createValidatedObservable(BallotPaper.VotersWithLocomotorDisabilities, 'k', 3500);
        self.VotersWithVisualImpairments = createValidatedObservable(BallotPaper.VotersWithVisualImpairments, 'l', 3500);
        self.VotersWithHearingImpairments = createValidatedObservable(BallotPaper.VotersWithHearingImpairments, 'm', 3500);
        self.VotersBasedOnNewResidenceDeclaration = createValidatedObservable(BallotPaper.VotersBasedOnNewResidenceDeclaration, 'n', 3500);
        self.VotersWithStudentCard = createValidatedObservable(BallotPaper.VotersWithStudentCard, 'o', 3500);
        self.ElectoralOfficialsFromOtherSV = createValidatedObservable(BallotPaper.ElectoralOfficialsFromOtherSV, 'p', 3500);
        self.SignaturesOnBasicListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnBasicListsFirstDay, 'a', 3500);
        self.SignaturesOnSupplementaryListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnSupplementaryListsFirstDay, 'b', 3500);
        self.SignaturesOnVotingLocationListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnVotingLocationListsFirstDay, 'c', 3500);
        self.SignaturesOnBasicListSecondDay_Rule_Succeeded = ko.observable(true);
        self.SignaturesOnBasicListSecondDay = createValidatedObservable(BallotPaper.SignaturesOnBasicListSecondDay, 'a', null, validationRules.A_equal_or_more_than_point23A);
        self.SignaturesOnSupplementaryListSecondDay_Rule_Succeeded = ko.observable(true);
        self.SignaturesOnSupplementaryListSecondDay = createValidatedObservable(BallotPaper.SignaturesOnSupplementaryListSecondDay, 'b', null, validationRules.B_equal_or_more_than_point23B);
        self.SignaturesOnVotingLocationListSecondDay_Rule_Succeeded = ko.observable(true);
        self.SignaturesOnVotingLocationListSecondDay = createValidatedObservable(BallotPaper.SignaturesOnVotingLocationListSecondDay, 'c', null, validationRules.C_equal_or_more_than_point23C);
        self.AbsentElectoralBoardMembersOnVotingDay_Rule_Succeeded = ko.observable(true);
        self.AbsentElectoralBoardMembersOnVotingDay = createValidatedObservable(BallotPaper.AbsentElectoralBoardMembersOnVotingDay, '25', null, validationRules.equal_or_less_than_point_6_2days_Rule);
        self.AbsentElectoralBoardMembersOnSecondDay_Rule_Succeeded = ko.observable(true);
        self.AbsentElectoralBoardMembersOnSecondDay = createValidatedObservable(BallotPaper.AbsentElectoralBoardMembersOnSecondDay, '26', null, validationRules.equal_or_less_than_point_6_2days_2nd_day_Rule);
        self.PartyDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.PartyDesignatedRepresentativesCount, 'a', 3500);
        self.ElectoralBlocDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralBlocDesignatedRepresentativesCount, 'b', 3500);
        self.IndependentCandidateDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.IndependentCandidateDesignatedRepresentativesCount, 'c', 3500);
        self.ElectoralBoardMembersCount_Rule_Succeeded = ko.validatedObservable(true);
        self.ElectoralBoardMembersCount = createValidatedObservable(BallotPaper.ElectoralBoardMembersCount, 'a', null, validationRules.A_equal_point6_minus_point25_Rule);
        self.SIASOperatorsCount = createValidatedObservable(BallotPaper.SIASOperatorsCount, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesCount, 'c', 3500);
        self.InternationalObserversCount = createValidatedObservable(BallotPaper.InternationalObserversCount, 'd', 3500);
        self.NationalObserversFromAssociationsCount = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsCount, 'e', 3500);
        self.MediaRepresentativesCount = createValidatedObservable(BallotPaper.MediaRepresentativesCount, 'f', 3500);
        self.VotingResultsReportCountByBESV = createValidatedObservable(BallotPaper.VotingResultsReportCountByBESV, '29', 3500);
    }

    // Intermediate CECE report 14.00, elections
    if ([22].includes(templateId)) {
        self.ObjectionsFiled = createValidatedObservable(BallotPaper.ObjectionsFiled, '1', 3500);
        self.PublicOrderViolationsAtElectoralCouncil = createValidatedObservable(BallotPaper.PublicOrderViolationsAtElectoralCouncil, '2', 3500);
        self.ObserversParticipating = createValidatedObservable(BallotPaper.ObserversParticipating, '3', 3500);
        self.ElectoralCompetitorsRepresentativesParticipating = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesParticipating, '4', 3500);
    }

    // Intermediate CECE report 14.00, referendum
    if ([23].includes(templateId)) {
        self.ObjectionsFiled = createValidatedObservable(BallotPaper.ObjectionsFiled, '1', 3500);
        self.PublicOrderViolationsAtElectoralCouncil = createValidatedObservable(BallotPaper.PublicOrderViolationsAtElectoralCouncil, '2', 3500);
        self.ObserversParticipating = createValidatedObservable(BallotPaper.ObserversParticipating, '3', 3500);
        self.ReferendumRepresentativesParticipating = createValidatedObservable(BallotPaper.ReferendumRepresentativesParticipating, '4', 3500);
    }

    // Final CECE report, elections, 1 day, 1st round
    if ([32, 38].includes(templateId)) {
        self.CouncilMembers = createValidatedObservable(BallotPaper.CouncilMembers, '1', 11);
        self.MeetingSessions = createValidatedObservable(BallotPaper.MeetingSessions, '2', 3500);
        self.DecisionsAdoptedByElectoralOffice = createValidatedObservable(BallotPaper.DecisionsAdoptedByElectoralOffice, '3', 3500);
        self.ContestationsUntilEDayCount_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsUntilEDayCount = createValidatedObservable(BallotPaper.ContestationsUntilEDay, 'a', null, validationRules.A_eq_5A_5E_5F_5G_Rule);
        self.ContestationsOnEDayCount_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsOnEDayCount = createValidatedObservable(BallotPaper.ContestationsOnEDay, 'b', null, validationRules.A_eq_6A_6E_6F_6G_Final_CECE_1st_round_Rule);
        self.TotalDecisionsOnContestationsUntilEDay_Rule_Succeeded = ko.validatedObservable(true);
        self.TotalDecisionsOnContestationsUntilEDay = createValidatedObservable(BallotPaper.TotalDecisionsOnContestationsUntilEDay, 'a', null, validationRules.A_eq_B_C_D_CECE_1_day_1st_round_Rule_Before);
        self.DecisionsFullyAdmittingContestationsUntilEDay = createValidatedObservable(BallotPaper.DecisionsFullyAdmittingContestationsUntilEDay, 'b', 3500);
        self.DecisionsPartiallyAdmittingContestationsUntilEDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAdmittingContestationsUntilEDay, 'c', 3500);
        self.ContestationsRejectedUntilEDay = createValidatedObservable(BallotPaper.ContestationsRejectedUntilEDay, 'd', 3500);
        self.ResponsesToContestationsByLetter = createValidatedObservable(BallotPaper.ResponsesToContestationsByLetter, 'e', 3500);
        self.ContestationsRemittedToOtherAuthorities = createValidatedObservable(BallotPaper.ContestationsRemittedToOtherAuthorities, 'f', 3500);
        self.ContestationsReturned = createValidatedObservable(BallotPaper.ContestationsReturned, 'g', 3500);
        self.ContestationsOnEDayDecisions_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsOnEDayDecisions = createValidatedObservable(BallotPaper.ContestationsOnEDayDecisions, 'a', null, validationRules.A_eq_B_C_D_CECE_1_day_1st_round_Rule);
        self.DecisionsFullyAdmittingContestationsOnEDay = createValidatedObservable(BallotPaper.DecisionsFullyAdmittingContestationsOnEDay, 'b', 3500);
        self.DecisionsPartiallyAdmittingContestationsOnEDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAdmittingContestationsOnEDay, 'c', 3500);
        self.ContestationsRejectedOnEDay = createValidatedObservable(BallotPaper.ContestationsRejectedOnEDay, 'd', 3500);
        self.ResponsesToContestationsOnEDayByLetter = createValidatedObservable(BallotPaper.ResponsesToContestationsOnEDayByLetter, 'e', 3500);
        self.ContestationsOnEDayRemittedToOtherAuthorities = createValidatedObservable(BallotPaper.ContestationsOnEDayRemittedToOtherAuthorities, 'f', 3500);
        self.ContestationsOnEDayReturned = createValidatedObservable(BallotPaper.ContestationsOnEDayReturned, 'g', 3500);
        self.ElectoralCompetitorsRepresentatives = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentatives, 'a', 3500);
        self.ElectoralBlocRepresentatives = createValidatedObservable(BallotPaper.ElectoralBlocRepresentatives, 'b', 3500);
        self.IndependentCandidatesRepresentatives = createValidatedObservable(BallotPaper.IndependentCandidatesRepresentatives, 'c', 3500);
        self.CouncilOperationsParticipantsCompetitors = createValidatedObservable(BallotPaper.CouncilOperationsParticipantsCompetitors, 'a', 3500);
        self.CouncilOperationsParticipantsInternationalObservers = createValidatedObservable(BallotPaper.CouncilOperationsParticipantsInternationalObservers, 'b', 3500);
        self.CouncilOperationsParticipantsNationalObservers = createValidatedObservable(BallotPaper.CouncilOperationsParticipantsNationalObservers, 'c', 3500);
        self.CouncilOperationsParticipantsMediaRepresentatives = createValidatedObservable(BallotPaper.CouncilOperationsParticipantsMediaRepresentatives, 'd', 3500);
        self.PublicOrderViolations = createValidatedObservable(BallotPaper.PublicOrderViolations, '9', 3500);
        self.AbsentCouncilMembers_Rule_Succeeded = ko.observable(true);
        self.AbsentCouncilMembers = createValidatedObservable(BallotPaper.AbsentCouncilMembers, '10', null, validationRules.qual_or_less_than_point_1_Rule);
        self.CECEResultSummaryReports = createValidatedObservable(BallotPaper.CECEResultSummaryReports, '11', 3500);
        self.ElectoralBoardMembersCountOpening = createValidatedObservable(BallotPaper.ElectoralBoardMembersCountOpening, 'a', 3500);
        self.SIASOperatorsCountOpening = createValidatedObservable(BallotPaper.SIASOperatorsCountOpening, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesCountOpening = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesCountOpening, 'c', 3500);
        self.InternationalObserversCountOpening = createValidatedObservable(BallotPaper.InternationalObserversCountOpening, 'd', 3500);
        self.NationalObserversFromAssociationsCountOpening = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsCountOpening, 'e', 3500);
        self.MediaRepresentativesCountOpening = createValidatedObservable(BallotPaper.MediaRepresentativesCountOpening, 'f', 3500);
        self.StationaryVotingBoxes80L = createValidatedObservable(BallotPaper.StationaryVotingBoxes80L, 'a', 3500);
        self.StationaryVotingBoxes45L = createValidatedObservable(BallotPaper.StationaryVotingBoxes45L, 'b', 3500);
        self.MobileVotingBoxes27L = createValidatedObservable(BallotPaper.MobileVotingBoxes27L, 'c', 3500);
        self.PlasticSealsUsedForVotingBoxesCount_Rule_Succeeded = ko.validatedObservable(true);
        self.PlasticSealsUsedForVotingBoxesCount = createValidatedObservable(BallotPaper.PlasticSealsUsedForVotingBoxesCount, 'd', null, validationRules.D_eq_or_GT_4A_4B_C_Rule);
        self.SecretVotingBoothsCount = createValidatedObservable(BallotPaper.SecretVotingBoothsCount, 'a', 3500);
        self.SpecialNeedsVotingBoothsCount = createValidatedObservable(BallotPaper.SpecialNeedsVotingBoothsCount, 'b', 3500);
        self.StampedVotedCount = createValidatedObservable(BallotPaper.StampedVotedCount, 'a', 3500);
        self.StampedWithdrawnCount = createValidatedObservable(BallotPaper.StampedWithdrawnCount, 'b', 3500);
        self.ContestationsUntilEDay = createValidatedObservable(BallotPaper.ContestationsUntilEDay, 'a', 3500);
        self.ContestationsOnEDay = createValidatedObservable(BallotPaper.ContestationsOnEDay, 'b', 3500);
        self.TotalDecisionsOnObjectionsCountBefore_Rule_Succeeded = ko.validatedObservable(true);
        self.TotalDecisionsOnObjectionsCountBefore = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsCountBefore, 'a', null, validationRules.A_eq_B_C_D_Rule_Before);
        self.FullyAdmittedObjectionsCountBefore = createValidatedObservable(BallotPaper.FullyAdmittedObjectionsCountBefore, 'b', 3500);
        self.PartiallyAdmittedObjectionsCountBefore = createValidatedObservable(BallotPaper.PartiallyAdmittedObjectionsCountBefore, 'c', 3500);
        self.UnfoundedObjectionsCountBefore = createValidatedObservable(BallotPaper.UnfoundedObjectionsCountBefore, 'd', 3500);
        self.WrittenResponsesToObjectionsCountBefore = createValidatedObservable(BallotPaper.WrittenResponsesToObjectionsCountBefore, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesCountBefore = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesCountBefore, 'f', 3500);
        self.ReturnedObjectionsCountBefore = createValidatedObservable(BallotPaper.ReturnedObjectionsCountBefore, 'g', 3500);
        self.TotalDecisionsOnObjectionsCount_Rule_Succeeded = ko.validatedObservable(true);
        self.TotalDecisionsOnObjectionsCount = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsCount, 'a', null, validationRules.A_eq_B_C_D_Rule);
        self.FullyAdmittedObjectionsCount = createValidatedObservable(BallotPaper.FullyAdmittedObjectionsCount, 'b', 3500);
        self.PartiallyAdmittedObjectionsCount = createValidatedObservable(BallotPaper.PartiallyAdmittedObjectionsCount, 'c', 3500);
        self.UnfoundedObjectionsCount = createValidatedObservable(BallotPaper.UnfoundedObjectionsCount, 'd', 3500);
        self.WrittenResponsesToObjectionsCount = createValidatedObservable(BallotPaper.WrittenResponsesToObjectionsCount, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesCount = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesCount, 'f', 3500);
        self.ReturnedObjectionsCount = createValidatedObservable(BallotPaper.ReturnedObjectionsCount, 'g', 3500);
        self.PublicOrderViolationsOnVotingDayCount = createValidatedObservable(BallotPaper.PublicOrderViolationsOnVotingDayCount, '12.8', 3500);
        self.BallotAssistanceCount = createValidatedObservable(BallotPaper.BallotAssistanceCount, '12.9', 3500);
        self.VoterRequestsCount_Rule_Succeeded = ko.validatedObservable(true);
        self.VoterRequestsCount = createValidatedObservable(BallotPaper.VoterRequestsCount, 'a', null, validationRules.A_B_C_eq_D_E_F_eq_G_H_Rule);
        self.ElectoralCompetitorRequestsCount = createValidatedObservable(BallotPaper.ElectoralCompetitorRequestsCount, 'b', 3500);
        self.ObserverRequestsCount = createValidatedObservable(BallotPaper.ObserverRequestsCount, 'c', 3500);
        self.InclusionRequestsCount = createValidatedObservable(BallotPaper.InclusionRequestsCount, 'd', 3500);
        self.ExclusionRequestsCount = createValidatedObservable(BallotPaper.ExclusionRequestsCount, 'e', 3500);
        self.DataCorrectionRequestsCount = createValidatedObservable(BallotPaper.DataCorrectionRequestsCount, 'f', 3500);
        self.AdmittedRequestsCount = createValidatedObservable(BallotPaper.AdmittedRequestsCount, 'g', 3500);
        self.RejectedRequestsCount = createValidatedObservable(BallotPaper.RejectedRequestsCount, 'h', 3500);
        self.CertificatesReceivedFromCEC = createValidatedObservable(BallotPaper.CertificatesReceivedFromCEC, 'a', 3500);
        self.CECEReceivedCertificatesCount = createValidatedObservable(BallotPaper.CECEReceivedCertificatesCount, 'b', 3500);
        self.VotersIssuedCertificatesCount = createValidatedObservable(BallotPaper.VotersIssuedCertificatesCount, 'c', 3500);
        self.UnusedOrCancelledCertificatesCount = createValidatedObservable(BallotPaper.UnusedOrCancelledCertificatesCount, 'd', 3500);
        self.TotalVotingLocationRequestsCount = createValidatedObservable(BallotPaper.TotalVotingLocationRequestsCount, '12.12', 3500);
        self.BallotsIssuedByMobileTeamCount = createValidatedObservable(BallotPaper.BallotsIssuedByMobileTeamCount, '12.13', 3500);
        self.VotesBasedOnVotingCertificatesCount = createValidatedObservable(BallotPaper.VotesBasedOnVotingCertificatesCount, 'a', 3500);
        self.VotesBasedOnProvisionalIDCount = createValidatedObservable(BallotPaper.VotesBasedOnProvisionalIDCount, 'b', 3500);
        self.VotesBasedOnPassportCount = createValidatedObservable(BallotPaper.VotesBasedOnPassportCount, 'c', 3500);
        self.VotesBasedOnExpiredPassportCount_Rule_Succeeded = ko.validatedObservable(true);
        self.VotesBasedOnExpiredPassportCount = createValidatedObservable(BallotPaper.VotesBasedOnExpiredPassportCount, 'd', null, validationRules.D_equal_or_less_than_C_Rule);
        self.VotesAtLocationUsingMobileBoxCount = createValidatedObservable(BallotPaper.VotesAtLocationUsingMobileBoxCount, 'e', 3500);
        self.VotesByIncarceratedPersonsCount = createValidatedObservable(BallotPaper.VotesByIncarceratedPersonsCount, 'f', 3500);
        self.VotesByPersonsInMedicalFacilitiesCount = createValidatedObservable(BallotPaper.VotesByPersonsInMedicalFacilitiesCount, 'g', 3500);
        self.VotesByMilitaryPersonnelCount = createValidatedObservable(BallotPaper.VotesByMilitaryPersonnelCount, 'h', 3500);
        self.VotesByPersonsWithMobilityDisabilitiesCount = createValidatedObservable(BallotPaper.VotesByPersonsWithMobilityDisabilitiesCount, 'k', 3500);
        self.VotesByVisuallyImpairedPersonsCount = createValidatedObservable(BallotPaper.VotesByVisuallyImpairedPersonsCount, 'l', 3500);
        self.VotesByHearingImpairedPersonsCount = createValidatedObservable(BallotPaper.VotesByHearingImpairedPersonsCount, 'm', 3500);
        self.VotesBasedOnNewResidenceDeclarationCount = createValidatedObservable(BallotPaper.VotesBasedOnNewResidenceDeclarationCount, 'n', 3500);
        self.VotesByStudentCardHoldersCount = createValidatedObservable(BallotPaper.VotesByStudentCardHoldersCount, 'o', 3500);
        self.VotesByElectoralOfficersFromOtherStationsCount = createValidatedObservable(BallotPaper.VotesByElectoralOfficersFromOtherStationsCount, 'p', 3500);
        self.SignaturesOnMainElectoralListsCount = createValidatedObservable(BallotPaper.SignaturesOnMainElectoralListsCount, 'a', 3500);
        self.SignaturesOnSupplementaryElectoralListsCount = createValidatedObservable(BallotPaper.SignaturesOnSupplementaryElectoralListsCount, 'b', 3500);
        self.SignaturesOnLocationVotingElectoralListsCount = createValidatedObservable(BallotPaper.SignaturesOnLocationVotingElectoralListsCount, 'c', 3500);
        self.RepresentativesAppointedByPoliticalPartiesCount = createValidatedObservable(BallotPaper.RepresentativesAppointedByPoliticalPartiesCount, 'a', 3500);
        self.RepresentativesAppointedByElectoralBlocksCount = createValidatedObservable(BallotPaper.RepresentativesAppointedByElectoralBlocksCount, 'b', 3500);
        self.RepresentativesAppointedByIndependentCandidatesCount = createValidatedObservable(BallotPaper.RepresentativesAppointedByIndependentCandidatesCount, 'c', 3500);
        self.ElectoralBoardMembersCount = createValidatedObservable(BallotPaper.ElectoralBoardMembersCount, 'a', 3500);
        self.SIASOperatorsCount = createValidatedObservable(BallotPaper.SIASOperatorsCount, 'b', 3500);
        self.ElectoralCompetitorRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralCompetitorRepresentativesCount, 'c', 3500);
        self.InternationalObserversCount = createValidatedObservable(BallotPaper.InternationalObserversCount, 'd', 3500);
        self.NationalObserversFromAssociationsCount = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsCount, 'e', 3500);
        self.MediaRepresentativesCount = createValidatedObservable(BallotPaper.MediaRepresentativesCount, 'f', 3500);
        self.VotingResultsReportCountByBESV = createValidatedObservable(BallotPaper.VotingResultsReportCountByBESV, '12.18', 3500);
    }

    // Final CECE report, elections, 1 day, 2nd round
    if ([33, 39].includes(templateId)) {
        self.CouncilMembers = createValidatedObservable(BallotPaper.CouncilMembers, '1', 11);
        self.MeetingSessions = createValidatedObservable(BallotPaper.MeetingSessions, '2', 3500);
        self.DecisionsAdoptedByElectoralOffice = createValidatedObservable(BallotPaper.DecisionsAdoptedByElectoralOffice, '3', 3500);
        self.ContestationsSubmittedToCouncilBefore_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsSubmittedToCouncilBefore = createValidatedObservable(BallotPaper.ContestationsSubmittedToCouncilBefore, 'a', null, validationRules.A_eq_5A_5E_5F_5G_CECE_Rule);
        self.ContestationsSubmittedToCouncilOnEDay_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsSubmittedToCouncilOnEDay = createValidatedObservable(BallotPaper.ContestationsSubmittedToCouncilOnEDay, 'b', null, validationRules.A_eq_6A_6E_6F_6G_Rule);
        self.TotalDecisionsOnContestationsUntilEDay_Rule_Succeeded = ko.validatedObservable(true);
        self.TotalDecisionsOnContestationsUntilEDay = createValidatedObservable(BallotPaper.TotalDecisionsOnContestationsUntilEDay, 'a', null, validationRules.A_eq_B_C_D_CECE_1_day_1st_round_Rule_Before);
        self.DecisionsFullyAdmittingContestationsUntilEDay = createValidatedObservable(BallotPaper.DecisionsFullyAdmittingContestationsUntilEDay, 'b', 3500);
        self.DecisionsPartiallyAdmittingContestationsUntilEDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAdmittingContestationsUntilEDay, 'c', 3500);
        self.ContestationsRejectedUntilEDay = createValidatedObservable(BallotPaper.ContestationsRejectedUntilEDay, 'd', 3500);
        self.ResponsesToContestationsByLetter = createValidatedObservable(BallotPaper.ResponsesToContestationsByLetter, 'e', 3500);
        self.ContestationsRemittedToOtherAuthorities = createValidatedObservable(BallotPaper.ContestationsRemittedToOtherAuthorities, 'f', 3500);
        self.ContestationsReturned = createValidatedObservable(BallotPaper.ContestationsReturned, 'g', 3500);
        self.ContestationsOnEDayDecisions_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsOnEDayDecisions = createValidatedObservable(BallotPaper.ContestationsOnEDayDecisions, 'a', null, validationRules.A_eq_B_C_D_CECE_1_day_1st_round_Rule);
        self.DecisionsFullyAdmittingContestationsOnEDay = createValidatedObservable(BallotPaper.DecisionsFullyAdmittingContestationsOnEDay, 'b', 3500);
        self.DecisionsPartiallyAdmittingContestationsOnEDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAdmittingContestationsOnEDay, 'c', 3500);
        self.ContestationsRejectedOnEDay = createValidatedObservable(BallotPaper.ContestationsRejectedOnEDay, 'd', 3500);
        self.ResponsesToContestationsOnEDayByLetter = createValidatedObservable(BallotPaper.ResponsesToContestationsOnEDayByLetter, 'e', 3500);
        self.ContestationsOnEDayRemittedToOtherAuthorities = createValidatedObservable(BallotPaper.ContestationsOnEDayRemittedToOtherAuthorities, 'f', 3500);
        self.ContestationsOnEDayReturned = createValidatedObservable(BallotPaper.ContestationsOnEDayReturned, 'g', 3500);
        self.ElectoralCompetitorsRepresentatives = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentatives, 'a', 3500);
        self.ElectoralBlocRepresentatives = createValidatedObservable(BallotPaper.ElectoralBlocRepresentatives, 'b', 3500);
        self.IndependentCandidatesRepresentatives = createValidatedObservable(BallotPaper.IndependentCandidatesRepresentatives, 'c', 3500);
        self.CouncilOperationsParticipantsCompetitors = createValidatedObservable(BallotPaper.CouncilOperationsParticipantsCompetitors, 'a', 3500);
        self.CouncilOperationsParticipantsInternationalObservers = createValidatedObservable(BallotPaper.CouncilOperationsParticipantsInternationalObservers, 'b', 3500);
        self.CouncilOperationsParticipantsNationalObservers = createValidatedObservable(BallotPaper.CouncilOperationsParticipantsNationalObservers, 'c', 3500);
        self.CouncilOperationsParticipantsMediaRepresentatives = createValidatedObservable(BallotPaper.CouncilOperationsParticipantsMediaRepresentatives, 'd', 3500);
        self.PublicOrderViolations = createValidatedObservable(BallotPaper.PublicOrderViolations, '9', 3500);
        self.AbsentCouncilMembers_Rule_Succeeded = ko.observable(true);
        self.AbsentCouncilMembers = createValidatedObservable(BallotPaper.AbsentCouncilMembers, '10', null, validationRules.qual_or_less_than_point_1_Rule);
        self.CECEResultSummaryReports = createValidatedObservable(BallotPaper.CECEResultSummaryReports, '11', 3500);
        self.ElectoralBoardMembersOnOpening = createValidatedObservable(BallotPaper.ElectoralBoardMembersOnOpening, 'a', 3500);
        self.SIASOperatorsOnOpening = createValidatedObservable(BallotPaper.SIASOperatorsOnOpening, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesOnOpening = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesOnOpening, 'c', 3500);
        self.InternationalObserversOnOpening = createValidatedObservable(BallotPaper.InternationalObserversOnOpening, 'd', 3500);
        self.NationalObserversFromAssociationsOnOpening = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsOnOpening, 'e', 3500);
        self.MediaRepresentativesOnOpening = createValidatedObservable(BallotPaper.MediaRepresentativesOnOpening, 'f', 3500);
        self.StationaryVotingBox80Liters = createValidatedObservable(BallotPaper.StationaryVotingBox80Liters, 'a', 3500);
        self.StationaryVotingBox45Liters = createValidatedObservable(BallotPaper.StationaryVotingBox45Liters, 'b', 3500);
        self.MobileVotingBox27Liters = createValidatedObservable(BallotPaper.MobileVotingBox27Liters, 'c', 3500);
        self.PlasticSealsForVotingBoxCount = createValidatedObservable(BallotPaper.PlasticSealsForVotingBoxCount, 'd', 3500);
        self.VotingBoothsCount = createValidatedObservable(BallotPaper.VotingBoothsCount, 'a', 3500);
        self.SpecialNeedsVotingBoothsCount = createValidatedObservable(BallotPaper.SpecialNeedsVotingBoothsCount, 'b', 3500);
        self.StampedVotedCount = createValidatedObservable(BallotPaper.StampedVotedCount, 'a', 3500);
        self.StampedWithdrawnCount = createValidatedObservable(BallotPaper.StampedWithdrawnCount, 'b', 3500);
        self.ContestationsUntilEDay = createValidatedObservable(BallotPaper.ContestationsUntilEDay, 'a', 3500);
        self.ContestationsOnEDay = createValidatedObservable(BallotPaper.ContestationsOnEDay, 'b', 3500);
        self.TotalDecisionsOnObjectionsBefore = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsBefore, 'a', 3500);
        self.DecisionsFullyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsBefore, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsBefore, 'c', 3500);
        self.DecisionsRejectedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsBefore, 'd', 3500);
        self.ObjectionResponsesByLetterBefore = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterBefore, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesBefore = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesBefore, 'f', 3500);
        self.ReturnedObjectionsBefore = createValidatedObservable(BallotPaper.ReturnedObjectionsBefore, 'g', 3500);
        self.TotalDecisionsOnObjectionsCount_Rule_Succeeded = ko.validatedObservable(true);
        self.TotalDecisionsOnObjectionsCount = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsCount, 'a', null, validationRules.A_eq_B_C_D_Rule);
        self.FullyAdmittedObjectionsCount = createValidatedObservable(BallotPaper.FullyAdmittedObjectionsCount, 'b', 3500);
        self.PartiallyAdmittedObjectionsCount = createValidatedObservable(BallotPaper.PartiallyAdmittedObjectionsCount, 'c', 3500);
        self.UnfoundedObjectionsCount = createValidatedObservable(BallotPaper.UnfoundedObjectionsCount, 'd', 3500);
        self.WrittenResponsesToObjectionsCount = createValidatedObservable(BallotPaper.WrittenResponsesToObjectionsCount, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesCount = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesCount, 'f', 3500);
        self.ReturnedObjectionsCount = createValidatedObservable(BallotPaper.ReturnedObjectionsCount, 'g', 3500);
        self.PublicOrderViolationsOnVotingDay = createValidatedObservable(BallotPaper.PublicOrderViolationsOnVotingDay, '12.8', 3500);
        self.BallotAssistanceCount = createValidatedObservable(BallotPaper.BallotAssistanceCount, '12.9', 3500);
        self.CECReceivedCertificates = createValidatedObservable(BallotPaper.CECReceivedCertificates, 'a', 3500);
        self.CECEReceivedCertificates = createValidatedObservable(BallotPaper.CECEReceivedCertificates, 'b', 3500);
        self.VotersIssuedCertificates = createValidatedObservable(BallotPaper.VotersIssuedCertificates, 'c', 3500);
        self.UnusedOrCancelledCertificates = createValidatedObservable(BallotPaper.UnusedOrCancelledCertificates, 'd', 3500);
        self.TotalVotingRequestsAtLocation = createValidatedObservable(BallotPaper.TotalVotingRequestsAtLocation, '12.11', 3500);
        self.BallotsIssuedByMobileTeam = createValidatedObservable(BallotPaper.BallotsIssuedByMobileTeam, '12.12', 3500);
        self.VotersBasedOnVotingCertificates = createValidatedObservable(BallotPaper.VotersBasedOnVotingCertificates, 'a', 3500);
        self.VotersWithTemporaryID = createValidatedObservable(BallotPaper.VotersWithTemporaryID, 'b', 3500);
        self.VotersWithPassport = createValidatedObservable(BallotPaper.VotersWithPassport, 'c', 3500);
        self.VotersWithExpiredPassport = createValidatedObservable(BallotPaper.VotersWithExpiredPassport, 'd', 3500);
        self.VotersAtLocationWithMobileBox = createValidatedObservable(BallotPaper.VotersAtLocationWithMobileBox, 'e', 3500);
        self.VotersInPenitentiaryInstitutions = createValidatedObservable(BallotPaper.VotersInPenitentiaryInstitutions, 'f', 3500);
        self.VotersInSanatoriumsOrHospitals = createValidatedObservable(BallotPaper.VotersInSanatoriumsOrHospitals, 'g', 3500);
        self.MilitaryVotersInMilitaryUnits = createValidatedObservable(BallotPaper.MilitaryVotersInMilitaryUnits, 'h', 3500);
        self.VotersWithLocomotorDisabilities = createValidatedObservable(BallotPaper.VotersWithLocomotorDisabilities, 'k', 3500);
        self.VotersWithVisualImpairments = createValidatedObservable(BallotPaper.VotersWithVisualImpairments, 'l', 3500);
        self.VotersWithHearingImpairments = createValidatedObservable(BallotPaper.VotersWithHearingImpairments, 'm', 3500);
        self.VotersBasedOnNewResidenceDeclaration = createValidatedObservable(BallotPaper.VotersBasedOnNewResidenceDeclaration, 'n', 3500);
        self.VotersWithStudentCard = createValidatedObservable(BallotPaper.VotersWithStudentCard, 'o', 3500);
        self.ElectoralOfficialsFromOtherSV = createValidatedObservable(BallotPaper.ElectoralOfficialsFromOtherSV, 'p', 3500);
        self.SignaturesOnBasicElectoralLists = createValidatedObservable(BallotPaper.SignaturesOnBasicElectoralLists, 'a', 3500);
        self.SignaturesOnSupplementaryElectoralLists = createValidatedObservable(BallotPaper.SignaturesOnSupplementaryElectoralLists, 'b', 3500);
        self.SignaturesOnVotingLocationElectoralLists = createValidatedObservable(BallotPaper.SignaturesOnVotingLocationElectoralLists, 'c', 3500);
        self.PartyDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.PartyDesignatedRepresentativesCount, 'a', 3500);
        self.ElectoralBlocDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralBlocDesignatedRepresentativesCount, 'b', 3500);
        self.IndependentCandidateDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.IndependentCandidateDesignatedRepresentativesCount, 'c', 3500);
        self.ElectoralBoardMembersCount = createValidatedObservable(BallotPaper.ElectoralBoardMembersCount, 'a', 3500);
        self.SIASOperatorsCount = createValidatedObservable(BallotPaper.SIASOperatorsCount, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesCount, 'c', 3500);
        self.InternationalObserversCount = createValidatedObservable(BallotPaper.InternationalObserversCount, 'd', 3500);
        self.NationalObserversFromAssociationsCount = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsCount, 'e', 3500);
        self.MediaRepresentativesCount = createValidatedObservable(BallotPaper.MediaRepresentativesCount, 'f', 3500);
        self.VotingResultsReportCountByBESV = createValidatedObservable(BallotPaper.VotingResultsReportCountByBESV, '12.17', 3500);
    }

    // Final CECE report, elections, 2 days, 1st round
    if ([34, 40].includes(templateId)) {
        self.CouncilMembers = createValidatedObservable(BallotPaper.CouncilMembers, '1', 11);
        self.MeetingsDuringElectionPeriod = createValidatedObservable(BallotPaper.MeetingsDuringElectionPeriod, '2', 3500);
        self.DecisionsDuringElectionPeriod = createValidatedObservable(BallotPaper.DecisionsDuringElectionPeriod, '3', 3500);
        self.ContestationsSubmittedToCouncil_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsSubmittedToCouncil = createValidatedObservable(BallotPaper.ContestationsSubmittedToCouncil, 'a', null, validationRules.A_eq_5A_5E_5F_5G_CECE_2days_Rule);
        self.ContestationsSubmittedToCouncilOnEDay_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsSubmittedToCouncilOnEDay = createValidatedObservable(BallotPaper.ContestationsSubmittedToCouncilOnEDay, 'b', null, validationRules.A_eq_6A_6E_6F_6G_CECE_2days_Rule);
        self.ContestationsSubmittedToCouncilOnDay2OfVoting_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsSubmittedToCouncilOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsSubmittedToCouncilOnDay2OfVoting, 'c', null, validationRules.A_eq_7A_7E_7F_7G_CECE_2days_Rule);
        self.DecisionsOnContestationsMade_Rule_Succeeded = ko.validatedObservable(true);
        self.DecisionsOnContestationsMade = createValidatedObservable(BallotPaper.DecisionsOnContestationsMade, 'a', null, validationRules.A_eq_B_C_D_CECE_2days_Rule_Before);
        self.ContestationsAcceptedInFull = createValidatedObservable(BallotPaper.ContestationsAcceptedInFull, 'b', 3500);
        self.ContestationsAcceptedPartially = createValidatedObservable(BallotPaper.ContestationsAcceptedPartially, 'c', 3500);
        self.ContestationsRejected = createValidatedObservable(BallotPaper.ContestationsRejected, 'd', 3500);
        self.ResponsesToContestationsByLetter = createValidatedObservable(BallotPaper.ResponsesToContestationsByLetter, 'e', 3500);
        self.ContestationsReferredToOtherAuthorities = createValidatedObservable(BallotPaper.ContestationsReferredToOtherAuthorities, 'f', 3500);
        self.ContestationsReturned = createValidatedObservable(BallotPaper.ContestationsReturned, 'g', 3500);
        self.DecisionsOnContestationsMadeOnEDay_Rule_Succeeded = ko.validatedObservable(true);
        self.DecisionsOnContestationsMadeOnEDay = createValidatedObservable(BallotPaper.DecisionsOnContestationsMadeOnEDay, 'a', null, validationRules.A_eq_B_C_D_CECE_2days_Rule_1st_day);
        self.ContestationsAcceptedInFullOnEDay = createValidatedObservable(BallotPaper.ContestationsAcceptedInFullOnEDay, 'b', 3500);
        self.ContestationsAcceptedPartiallyOnEDay = createValidatedObservable(BallotPaper.ContestationsAcceptedPartiallyOnEDay, 'c', 3500);
        self.ContestationsRejectedOnEDay = createValidatedObservable(BallotPaper.ContestationsRejectedOnEDay, 'd', 3500);
        self.ResponsesToContestationsByLetterOnEDay = createValidatedObservable(BallotPaper.ResponsesToContestationsByLetterOnEDay, 'e', 3500);
        self.ContestationsReferredToOtherAuthoritiesOnEDay = createValidatedObservable(BallotPaper.ContestationsReferredToOtherAuthoritiesOnEDay, 'f', 3500);
        self.ContestationsReturnedOnEDay = createValidatedObservable(BallotPaper.ContestationsReturnedOnEDay, 'g', 3500);
        self.DecisionsOnContestationsMadeOnDay2OfVoting_Rule_Succeeded = ko.observable(true);
        self.DecisionsOnContestationsMadeOnDay2OfVoting = createValidatedObservable(BallotPaper.DecisionsOnContestationsMadeOnDay2OfVoting, 'a', null, validationRules.A_eq_B_C_D_CECE_2days_Rule_2nd_day);
        self.ContestationsAcceptedInFullOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsAcceptedInFullOnDay2OfVoting, 'b', 3500);
        self.ContestationsAcceptedPartiallyOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsAcceptedPartiallyOnDay2OfVoting, 'c', 3500);
        self.ContestationsRejectedOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsRejectedOnDay2OfVoting, 'd', 3500);
        self.ResponsesToContestationsByLetterOnDay2OfVoting = createValidatedObservable(BallotPaper.ResponsesToContestationsByLetterOnDay2OfVoting, 'e', 3500);
        self.ContestationsReferredToOtherAuthoritiesOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsReferredToOtherAuthoritiesOnDay2OfVoting, 'f', 3500);
        self.ContestationsReturnedOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsReturnedOnDay2OfVoting, 'g', 3500);
        self.ConfirmedCandidateRepresentatives = createValidatedObservable(BallotPaper.ConfirmedCandidateRepresentatives, 'a', 3500);
        self.ConfirmedBlocRepresentatives = createValidatedObservable(BallotPaper.ConfirmedBlocRepresentatives, 'b', 3500);
        self.ConfirmedIndependentCandidateRepresentatives = createValidatedObservable(BallotPaper.ConfirmedIndependentCandidateRepresentatives, 'c', 3500);
        self.ParticipantsInCouncilActivitiesOnEDays = createValidatedObservable(BallotPaper.ParticipantsInCouncilActivitiesOnEDays, 'a', 3500);
        self.InternationalObserversInCouncilActivitiesOnEDays = createValidatedObservable(BallotPaper.InternationalObserversInCouncilActivitiesOnEDays, 'b', 3500);
        self.NationalObserversFromCivilSocietyOrganizationsInCouncilActivitiesOnEDays = createValidatedObservable(BallotPaper.NationalObserversFromCivilSocietyOrganizationsInCouncilActivitiesOnEDays, 'c', 3500);
        self.MediaRepresentativesInCouncilActivitiesOnEDays = createValidatedObservable(BallotPaper.MediaRepresentativesInCouncilActivitiesOnEDays, 'd', 3500);
        self.PublicOrderViolationsAtCouncil = createValidatedObservable(BallotPaper.PublicOrderViolationsAtCouncil, '10', 3500);
        self.CouncilMembersAbsentOnEDay_Rule_Succeeded = ko.observable(true);
        self.CouncilMembersAbsentOnEDay = createValidatedObservable(BallotPaper.CouncilMembersAbsentOnEDay, '11', null, validationRules.equal_or_less_than_point_1_CECE_2days_Rule);
        self.CouncilMembersAbsentOnDay2OfVoting_Rule_Succeeded = ko.observable(true);
        self.CouncilMembersAbsentOnDay2OfVoting = createValidatedObservable(BallotPaper.CouncilMembersAbsentOnDay2OfVoting, '12', null, validationRules.equal_or_less_than_point_1_CECE_2days_Rule_2nd_day);
        self.MinutesOfVotingResultsCentralizedByCECE = createValidatedObservable(BallotPaper.MinutesOfVotingResultsCentralizedByCECE, '13', 3500);

        self.ElectoralBoardMembersOnOpening = ko.observable();
        self.SIASOperatorsOnOpening = ko.observable();
        self.ElectoralCompetitorsRepresentativesOnOpening = ko.observable();
        self.InternationalObserversOnOpening = ko.observable();
        self.NationalObserversFromAssociationsOnOpening = ko.observable();
        self.MediaRepresentativesOnOpening = ko.observable();
        self.ElectoralBoardMembersSecondDay = ko.observable();
        self.SIASOperatorsSecondDay = ko.observable();
        self.ElectoralCompetitorsRepresentativesSecondDay = ko.observable();
        self.InternationalObserversSecondDay = ko.observable();
        self.NationalObserversFromAssociationsSecondDay = ko.observable();
        self.MediaRepresentativesSecondDay = ko.observable();
        self.StationaryVotingBox80Liters = ko.observable();
        self.StationaryVotingBox45Liters = ko.observable();
        self.MobileVotingBox27Liters = ko.observable();
        self.PlasticSealsForVotingBoxCount = ko.observable();
        self.VotingBoothsCount = ko.observable();
        self.SpecialNeedsVotingBoothsCount = ko.observable();
        self.StampedVotedCount = ko.observable();
        self.StampedWithdrawnCount = ko.observable();
        self.ContestationsUntilEDay = ko.observable();
        self.ContestationsOnEDay = ko.observable();
        self.SubmittedOnVotingDayCountSecondDay = ko.observable();
        self.TotalDecisionsOnObjectionsBefore = ko.observable();
        self.DecisionsFullyAcceptedObjectionsBefore = ko.observable();
        self.DecisionsPartiallyAcceptedObjectionsBefore = ko.observable();
        self.DecisionsRejectedObjectionsBefore = ko.observable();
        self.ObjectionResponsesByLetterBefore = ko.observable();
        self.ObjectionsForwardedToOtherAuthoritiesBefore = ko.observable();
        self.ReturnedObjectionsBefore = ko.observable();
        self.TotalDecisionsOnObjectionsFirstDay = ko.observable();
        self.DecisionsFullyAcceptedObjectionsFirstDay = ko.observable();
        self.DecisionsPartiallyAcceptedObjectionsFirstDay = ko.observable();
        self.DecisionsRejectedObjectionsFirstDay = ko.observable();
        self.ObjectionResponsesByLetterFirstDay = ko.observable();
        self.ObjectionsForwardedToOtherBodiesFirstDay = ko.observable();
        self.ReturnedObjectionsFirstDay = ko.observable();
        self.TotalDecisionsOnObjectionsSecondDay = ko.observable();
        self.DecisionsFullyAcceptedObjectionsSecondDay = ko.observable();
        self.DecisionsPartiallyAcceptedObjectionsSecondDay = ko.observable();
        self.DecisionsRejectedObjectionsSecondDay = ko.observable();
        self.ObjectionResponsesByLetterSecondDay = ko.observable();
        self.ObjectionsForwardedToOtherBodiesSecondDay = ko.observable();
        self.ReturnedObjectionsSecondDay = ko.observable();
        self.PublicOrderViolationsOnVotingDay = ko.observable();
        self.BallotAssistanceCount = ko.observable();
        self.NumberOfRequestsByVoters = ko.observable();
        self.NumberOfRequestsByElectoralRepresentatives = ko.observable();
        self.NumberOfRequestsByObservers = ko.observable();
        self.NumberOfInclusionRequests = ko.observable();
        self.NumberOfExclusionRequests = ko.observable();
        self.NumberOfDataCorrectionRequests = ko.observable();
        self.NumberOfAdmittedRequests = ko.observable();
        self.NumberOfRejectedRequests = ko.observable();

        self.CertificatesReceivedFromCEC = createValidatedObservable(BallotPaper.CertificatesReceivedFromCEC, 'a', 3500);

        self.CertificatesReceivedFromCECE = ko.observable();
        self.CertificatesIssuedToVoters = ko.observable();
        self.UnusedOrCancelledCertificates = ko.observable();
        self.TotalVotingRequestsAtLocation = ko.observable();
        self.BallotsIssuedByMobileTeam = ko.observable();
        self.VotersBasedOnVotingCertificates = ko.observable();
        self.VotersWithTemporaryID = ko.observable();
        self.VotersWithPassport = ko.observable();
        self.VotersWithExpiredPassport = ko.observable();
        self.VotersAtLocationWithMobileBox = ko.observable();
        self.VotersInPenitentiaryInstitutions = ko.observable();
        self.VotersInSanatoriumsOrHospitals = ko.observable();
        self.MilitaryVotersInMilitaryUnits = ko.observable();
        self.VotersWithLocomotorDisabilities = ko.observable();
        self.VotersWithVisualImpairments = ko.observable();
        self.VotersWithHearingImpairments = ko.observable();
        self.VotersBasedOnNewResidenceDeclaration = ko.observable();
        self.VotersWithStudentCard = ko.observable();
        self.ElectoralOfficialsFromOtherSV = ko.observable();
        self.SignaturesOnBasicListsFirstDay = ko.observable();
        self.SignaturesOnSupplementaryListsFirstDay = ko.observable();
        self.SignaturesOnVotingLocationListsFirstDay = ko.observable();
        self.ObserversConfirmedByPoliticalParties = ko.observable();
        self.ObserversConfirmedByElectoralBlocs = ko.observable();
        self.ObserversConfirmedByIndependentCandidates = ko.observable();
        self.PartyDesignatedRepresentativesCount = ko.observable();
        self.ElectoralBlocDesignatedRepresentativesCount = ko.observable();
        self.IndependentCandidateDesignatedRepresentativesCount = ko.observable();
        self.ElectoralBoardMembersCount = ko.observable();
        self.SIASOperatorsCount = ko.observable();
        self.ElectoralCompetitorsRepresentativesCount = ko.observable();
        self.InternationalObserversCount = ko.observable();
        self.NationalObserversFromAssociationsCount = ko.observable();
        self.MediaRepresentativesCount = ko.observable();
        self.VotingResultsReportCountByBESV = ko.observable();
    }

    // Final CECE report, elections, 2 days, 2nd round
    if ([35, 41].includes(templateId)) {
        self.CouncilMembers = createValidatedObservable(BallotPaper.CouncilMembers, '1', 11);
        self.MeetingsDuringElectionPeriod = createValidatedObservable(BallotPaper.MeetingsDuringElectionPeriod, '2', 3500);
        self.DecisionsDuringElectionPeriod = createValidatedObservable(BallotPaper.DecisionsDuringElectionPeriod, '3', 3500);
        self.ContestationsSubmittedToCouncil_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsSubmittedToCouncil = createValidatedObservable(BallotPaper.ContestationsSubmittedToCouncil, 'a', null, validationRules.A_eq_5A_5E_5F_5G_CECE_2days_Rule);
        self.ContestationsSubmittedToCouncilOnEDay_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsSubmittedToCouncilOnEDay = createValidatedObservable(BallotPaper.ContestationsSubmittedToCouncilOnEDay, 'b', null, validationRules.A_eq_6A_6E_6F_6G_CECE_2days_Rule);
        self.ContestationsSubmittedToCouncilOnDay2OfVoting_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsSubmittedToCouncilOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsSubmittedToCouncilOnDay2OfVoting, 'c', null, validationRules.A_eq_7A_7E_7F_7G_CECE_2days_Rule);
        self.DecisionsOnContestationsMade_Rule_Succeeded = ko.validatedObservable(true);
        self.DecisionsOnContestationsMade = createValidatedObservable(BallotPaper.DecisionsOnContestationsMade, 'a', null, validationRules.A_eq_B_C_D_CECE_2days_Rule_Before);
        self.ContestationsAcceptedInFull = createValidatedObservable(BallotPaper.ContestationsAcceptedInFull, 'b', 3500);
        self.ContestationsAcceptedPartially = createValidatedObservable(BallotPaper.ContestationsAcceptedPartially, 'c', 3500);
        self.ContestationsRejected = createValidatedObservable(BallotPaper.ContestationsRejected, 'd', 3500);
        self.ResponsesToContestationsByLetter = createValidatedObservable(BallotPaper.ResponsesToContestationsByLetter, 'e', 3500);
        self.ContestationsReferredToOtherAuthorities = createValidatedObservable(BallotPaper.ContestationsReferredToOtherAuthorities, 'f', 3500);
        self.ContestationsReturned = createValidatedObservable(BallotPaper.ContestationsReturned, 'g', 3500);
        self.DecisionsOnContestationsMadeOnEDay_Rule_Succeeded = ko.validatedObservable(true);
        self.DecisionsOnContestationsMadeOnEDay = createValidatedObservable(BallotPaper.DecisionsOnContestationsMadeOnEDay, 'a', null, validationRules.A_eq_B_C_D_CECE_2days_Rule_1st_day);
        self.ContestationsAcceptedInFullOnEDay = createValidatedObservable(BallotPaper.ContestationsAcceptedInFullOnEDay, 'b', 3500);
        self.ContestationsAcceptedPartiallyOnEDay = createValidatedObservable(BallotPaper.ContestationsAcceptedPartiallyOnEDay, 'c', 3500);
        self.ContestationsRejectedOnEDay = createValidatedObservable(BallotPaper.ContestationsRejectedOnEDay, 'd', 3500);
        self.ResponsesToContestationsByLetterOnEDay = createValidatedObservable(BallotPaper.ResponsesToContestationsByLetterOnEDay, 'e', 3500);
        self.ContestationsReferredToOtherAuthoritiesOnEDay = createValidatedObservable(BallotPaper.ContestationsReferredToOtherAuthoritiesOnEDay, 'f', 3500);
        self.ContestationsReturnedOnEDay = createValidatedObservable(BallotPaper.ContestationsReturnedOnEDay, 'g', 3500);
        self.DecisionsOnContestationsMadeOnDay2OfVoting_Rule_Succeeded = ko.observable(true);
        self.DecisionsOnContestationsMadeOnDay2OfVoting = createValidatedObservable(BallotPaper.DecisionsOnContestationsMadeOnDay2OfVoting, 'a', null, validationRules.A_eq_B_C_D_CECE_2days_Rule_2nd_day);
        self.ContestationsAcceptedInFullOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsAcceptedInFullOnDay2OfVoting, 'b', 3500);
        self.ContestationsAcceptedPartiallyOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsAcceptedPartiallyOnDay2OfVoting, 'c', 3500);
        self.ContestationsRejectedOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsRejectedOnDay2OfVoting, 'd', 3500);
        self.ResponsesToContestationsByLetterOnDay2OfVoting = createValidatedObservable(BallotPaper.ResponsesToContestationsByLetterOnDay2OfVoting, 'e', 3500);
        self.ContestationsReferredToOtherAuthoritiesOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsReferredToOtherAuthoritiesOnDay2OfVoting, 'f', 3500);
        self.ContestationsReturnedOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsReturnedOnDay2OfVoting, 'g', 3500);
        self.ConfirmedCandidateRepresentatives = createValidatedObservable(BallotPaper.ConfirmedCandidateRepresentatives, 'a', 3500);
        self.ConfirmedBlocRepresentatives = createValidatedObservable(BallotPaper.ConfirmedBlocRepresentatives, 'b', 3500);
        self.ConfirmedIndependentCandidateRepresentatives = createValidatedObservable(BallotPaper.ConfirmedIndependentCandidateRepresentatives, 'c', 3500);
        self.ParticipantsInCouncilActivitiesOnEDays = createValidatedObservable(BallotPaper.ParticipantsInCouncilActivitiesOnEDays, 'a', 3500);
        self.InternationalObserversInCouncilActivitiesOnEDays = createValidatedObservable(BallotPaper.InternationalObserversInCouncilActivitiesOnEDays, 'b', 3500);
        self.NationalObserversFromCivilSocietyOrganizationsInCouncilActivitiesOnEDays = createValidatedObservable(BallotPaper.NationalObserversFromCivilSocietyOrganizationsInCouncilActivitiesOnEDays, 'c', 3500);
        self.MediaRepresentativesInCouncilActivitiesOnEDays = createValidatedObservable(BallotPaper.MediaRepresentativesInCouncilActivitiesOnEDays, 'd', 3500);
        self.PublicOrderViolationsAtCouncil = createValidatedObservable(BallotPaper.PublicOrderViolationsAtCouncil, '10', 3500);
        self.CouncilMembersAbsentOnEDay_Rule_Succeeded = ko.observable(true);
        self.CouncilMembersAbsentOnEDay = createValidatedObservable(BallotPaper.CouncilMembersAbsentOnEDay, '11', null, validationRules.equal_or_less_than_point_1_CECE_2days_Rule);
        self.CouncilMembersAbsentOnDay2OfVoting_Rule_Succeeded = ko.observable(true);
        self.CouncilMembersAbsentOnDay2OfVoting = createValidatedObservable(BallotPaper.CouncilMembersAbsentOnDay2OfVoting, '12', null, validationRules.equal_or_less_than_point_1_CECE_2days_Rule_2nd_day);
        self.MinutesOfVotingResultsCentralizedByCECE = createValidatedObservable(BallotPaper.MinutesOfVotingResultsCentralizedByCECE, '13', 3500);
        self.ElectoralBoardMembersOnOpening = createValidatedObservable(BallotPaper.ElectoralBoardMembersOnOpening, 'a', 3500);
        self.SIASOperatorsOnOpening = createValidatedObservable(BallotPaper.SIASOperatorsOnOpening, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesOnOpening = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesOnOpening, 'c', 3500);
        self.InternationalObserversOnOpening = createValidatedObservable(BallotPaper.InternationalObserversOnOpening, 'd', 3500);
        self.NationalObserversFromAssociationsOnOpening = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsOnOpening, 'e', 3500);
        self.MediaRepresentativesOnOpening = createValidatedObservable(BallotPaper.MediaRepresentativesOnOpening, 'f', 3500);
        self.ElectoralBoardMembersSecondDay = createValidatedObservable(BallotPaper.ElectoralBoardMembersSecondDay, 'a', 3500);
        self.SIASOperatorsSecondDay = createValidatedObservable(BallotPaper.SIASOperatorsSecondDay, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesSecondDay = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesSecondDay, 'c', 3500);
        self.InternationalObserversSecondDay = createValidatedObservable(BallotPaper.InternationalObserversSecondDay, 'd', 3500);
        self.NationalObserversFromAssociationsSecondDay = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsSecondDay, 'e', 3500);
        self.MediaRepresentativesSecondDay = createValidatedObservable(BallotPaper.MediaRepresentativesSecondDay, 'f', 3500);
        self.StationaryVotingBox80Liters = createValidatedObservable(BallotPaper.StationaryVotingBox80Liters, 'a', 3500);
        self.StationaryVotingBox45Liters = createValidatedObservable(BallotPaper.StationaryVotingBox45Liters, 'b', 3500);
        self.MobileVotingBox27Liters = createValidatedObservable(BallotPaper.MobileVotingBox27Liters, 'c', 3500);
        self.PlasticSealsForVotingBoxCount = createValidatedObservable(BallotPaper.PlasticSealsForVotingBoxCount, 'd', 3500);
        self.VotingBoothsCount = createValidatedObservable(BallotPaper.VotingBoothsCount, 'a', 3500);
        self.SpecialNeedsVotingBoothsCount = createValidatedObservable(BallotPaper.SpecialNeedsVotingBoothsCount, 'b', 3500);
        self.StampedVotedCount = createValidatedObservable(BallotPaper.StampedVotedCount, 'a', 3500);
        self.StampedWithdrawnCount = createValidatedObservable(BallotPaper.StampedWithdrawnCount, 'b', 3500);
        self.ContestationsUntilEDay = createValidatedObservable(BallotPaper.ContestationsUntilEDay, 'a', 3500);
        self.ContestationsOnEDay = createValidatedObservable(BallotPaper.ContestationsOnEDay, 'b', 3500);
        self.SubmittedOnVotingDayCountSecondDay = createValidatedObservable(BallotPaper.SubmittedOnVotingDayCountSecondDay, 'c', 3500);
        self.TotalDecisionsOnObjectionsBefore = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsBefore, 'a', 3500);
        self.DecisionsFullyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsBefore, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsBefore, 'c', 3500);
        self.DecisionsRejectedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsBefore, 'd', 3500);
        self.ObjectionResponsesByLetterBefore = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterBefore, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesBefore = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesBefore, 'f', 3500);
        self.ReturnedObjectionsBefore = createValidatedObservable(BallotPaper.ReturnedObjectionsBefore, 'g', 3500);
        self.TotalDecisionsOnObjectionsFirstDay = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsFirstDay, 'a', 3500);
        self.DecisionsFullyAcceptedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsFirstDay, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsFirstDay, 'c', 3500);
        self.DecisionsRejectedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsFirstDay, 'd', 3500);
        self.ObjectionResponsesByLetterFirstDay = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterFirstDay, 'e', 3500);
        self.ObjectionsForwardedToOtherBodiesFirstDay = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherBodiesFirstDay, 'f', 3500);
        self.ReturnedObjectionsFirstDay = createValidatedObservable(BallotPaper.ReturnedObjectionsFirstDay, 'g', 3500);
        self.TotalDecisionsOnObjectionsSecondDay = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsSecondDay, 'a', 3500);
        self.DecisionsFullyAcceptedObjectionsSecondDay = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsSecondDay, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsSecondDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsSecondDay, 'c', 3500);
        self.DecisionsRejectedObjectionsSecondDay = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsSecondDay, 'd', 3500);
        self.ObjectionResponsesByLetterSecondDay = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterSecondDay, 'e', 3500);
        self.ObjectionsForwardedToOtherBodiesSecondDay = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherBodiesSecondDay, 'f', 3500);
        self.ReturnedObjectionsSecondDay = createValidatedObservable(BallotPaper.ReturnedObjectionsSecondDay, 'g', 3500);
        self.PublicOrderViolationsOnVotingDay = createValidatedObservable(BallotPaper.PublicOrderViolationsOnVotingDay, '14.10', 3500);
        self.BallotAssistanceCount = createValidatedObservable(BallotPaper.BallotAssistanceCount, '14.11', 3500);
        self.CertificatesReceivedFromCEC = createValidatedObservable(BallotPaper.CertificatesReceivedFromCEC, 'a', 3500);
        self.CertificatesReceivedFromCECE = createValidatedObservable(BallotPaper.CertificatesReceivedFromCECE, 'b', 3500);
        self.CertificatesIssuedToVoters = createValidatedObservable(BallotPaper.CertificatesIssuedToVoters, 'c', 3500);
        self.UnusedOrCancelledCertificates = createValidatedObservable(BallotPaper.UnusedOrCancelledCertificates, 'd', 3500);
        self.TotalVotingRequestsAtLocation = createValidatedObservable(BallotPaper.TotalVotingRequestsAtLocation, '14.13', 3500);
        self.BallotsIssuedByMobileTeam = createValidatedObservable(BallotPaper.BallotsIssuedByMobileTeam, '14.14', 3500);
        self.VotersBasedOnVotingCertificates = createValidatedObservable(BallotPaper.VotersBasedOnVotingCertificates, 'a', 3500);
        self.VotersWithTemporaryID = createValidatedObservable(BallotPaper.VotersWithTemporaryID, 'b', 3500);
        self.VotersWithPassport = createValidatedObservable(BallotPaper.VotersWithPassport, 'c', 3500);
        self.VotersWithExpiredPassport = createValidatedObservable(BallotPaper.VotersWithExpiredPassport, 'd', 3500);
        self.VotersAtLocationWithMobileBox = createValidatedObservable(BallotPaper.VotersAtLocationWithMobileBox, 'e', 3500);
        self.VotersInPenitentiaryInstitutions = createValidatedObservable(BallotPaper.VotersInPenitentiaryInstitutions, 'f', 3500);
        self.VotersInSanatoriumsOrHospitals = createValidatedObservable(BallotPaper.VotersInSanatoriumsOrHospitals, 'g', 3500);
        self.MilitaryVotersInMilitaryUnits = createValidatedObservable(BallotPaper.MilitaryVotersInMilitaryUnits, 'h', 3500);
        self.VotersWithLocomotorDisabilities = createValidatedObservable(BallotPaper.VotersWithLocomotorDisabilities, 'k', 3500);
        self.VotersWithVisualImpairments = createValidatedObservable(BallotPaper.VotersWithVisualImpairments, 'l', 3500);
        self.VotersWithHearingImpairments = createValidatedObservable(BallotPaper.VotersWithHearingImpairments, 'm', 3500);
        self.VotersBasedOnNewResidenceDeclaration = createValidatedObservable(BallotPaper.VotersBasedOnNewResidenceDeclaration, 'n', 3500);
        self.VotersWithStudentCard = createValidatedObservable(BallotPaper.VotersWithStudentCard, 'o', 3500);
        self.ElectoralOfficialsFromOtherSV = createValidatedObservable(BallotPaper.ElectoralOfficialsFromOtherSV, 'p', 3500);
        self.SignaturesOnBasicListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnBasicListsFirstDay, 'a', 3500);
        self.SignaturesOnSupplementaryListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnSupplementaryListsFirstDay, 'b', 3500);
        self.SignaturesOnVotingLocationListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnVotingLocationListsFirstDay, 'c', 3500);
        self.PartyDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.PartyDesignatedRepresentativesCount, 'a', 3500);
        self.ElectoralBlocDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralBlocDesignatedRepresentativesCount, 'b', 3500);
        self.IndependentCandidateDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.IndependentCandidateDesignatedRepresentativesCount, 'c', 3500);
        self.ElectoralBoardMembersCount = createValidatedObservable(BallotPaper.ElectoralBoardMembersCount, 'a', 3500);
        self.SIASOperatorsCount = createValidatedObservable(BallotPaper.SIASOperatorsCount, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesCount, 'c', 3500);
        self.InternationalObserversCount = createValidatedObservable(BallotPaper.InternationalObserversCount, 'd', 3500);
        self.NationalObserversFromAssociationsCount = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsCount, 'e', 3500);
        self.MediaRepresentativesCount = createValidatedObservable(BallotPaper.MediaRepresentativesCount, 'f', 3500);
        self.VotingResultsReportCountByBESV = createValidatedObservable(BallotPaper.VotingResultsReportCountByBESV, '14.19', 3500);
    }

    // Final CECE report, referendum, 1 day
    if ([36, 42].includes(templateId)) {
        self.CouncilMembers = createValidatedObservable(BallotPaper.CouncilMembers, '1', 11);
        self.MeetingSessions = createValidatedObservable(BallotPaper.MeetingSessions, '2', 3500);
        self.DecisionsAdoptedByElectoralOffice = createValidatedObservable(BallotPaper.DecisionsAdoptedByElectoralOffice, '3', 3500);
        self.ContestationsUntilEDayCount_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsUntilEDayCount = createValidatedObservable(BallotPaper.ContestationsUntilEDayCount, 'a', null, validationRules.A_eq_5A_5E_5F_5G_CECE_referendum_Rule);
        self.ContestationsOnEDayCount_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsOnEDayCount = createValidatedObservable(BallotPaper.ContestationsOnEDayCount, 'b', null, validationRules.A_eq_6A_6E_6F_6G_CECE_referendum_Rule);
        self.TotalDecisionsOnContestationsUntilEDay_Rule_Succeeded = ko.validatedObservable(true);
        self.TotalDecisionsOnContestationsUntilEDay = createValidatedObservable(BallotPaper.TotalDecisionsOnContestationsUntilEDay, 'a', null, validationRules.A_eq_B_C_D_CECE_referendum_Rule_Before);
        self.DecisionsFullyAdmittingContestationsUntilEDay = createValidatedObservable(BallotPaper.DecisionsFullyAdmittingContestationsUntilEDay, 'b', 3500);
        self.DecisionsPartiallyAdmittingContestationsUntilEDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAdmittingContestationsUntilEDay, 'c', 3500);
        self.ContestationsRejectedUntilEDay = createValidatedObservable(BallotPaper.ContestationsRejectedUntilEDay, 'd', 3500);
        self.ResponsesToContestationsByLetter = createValidatedObservable(BallotPaper.ResponsesToContestationsByLetter, 'e', 3500);
        self.ContestationsRemittedToOtherAuthorities = createValidatedObservable(BallotPaper.ContestationsRemittedToOtherAuthorities, 'f', 3500);
        self.ContestationsReturned = createValidatedObservable(BallotPaper.ContestationsReturned, 'g', 3500);
        self.ContestationsOnEDayDecisions_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsOnEDayDecisions = createValidatedObservable(BallotPaper.ContestationsOnEDayDecisions, 'a', null, validationRules.A_eq_B_C_D_CECE_referendum_Rule);
        self.DecisionsFullyAdmittingContestationsOnEDay = createValidatedObservable(BallotPaper.DecisionsFullyAdmittingContestationsOnEDay, 'b', 3500);
        self.DecisionsPartiallyAdmittingContestationsOnEDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAdmittingContestationsOnEDay, 'c', 3500);
        self.ContestationsRejectedOnEDay = createValidatedObservable(BallotPaper.ContestationsRejectedOnEDay, 'd', 3500);
        self.ResponsesToContestationsOnEDayByLetter = createValidatedObservable(BallotPaper.ResponsesToContestationsOnEDayByLetter, 'e', 3500);
        self.ContestationsOnEDayRemittedToOtherAuthorities = createValidatedObservable(BallotPaper.ContestationsOnEDayRemittedToOtherAuthorities, 'f', 3500);
        self.ContestationsOnEDayReturned = createValidatedObservable(BallotPaper.ContestationsOnEDayReturned, 'g', 3500);
        self.ElectoralCompetitorsRepresentatives = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentatives, 'a', 3500);
        self.ElectoralBlocRepresentatives = createValidatedObservable(BallotPaper.ElectoralBlocRepresentatives, 'b', 3500);
        self.IndependentCandidatesRepresentatives = createValidatedObservable(BallotPaper.IndependentCandidatesRepresentatives, 'c', 3500);
        self.CouncilOperationsParticipantsCompetitors = createValidatedObservable(BallotPaper.CouncilOperationsParticipantsCompetitors, 'a', 3500);
        self.CouncilOperationsParticipantsInternationalObservers = createValidatedObservable(BallotPaper.CouncilOperationsParticipantsInternationalObservers, 'b', 3500);
        self.CouncilOperationsParticipantsNationalObservers = createValidatedObservable(BallotPaper.CouncilOperationsParticipantsNationalObservers, 'c', 3500);
        self.CouncilOperationsParticipantsMediaRepresentatives = createValidatedObservable(BallotPaper.CouncilOperationsParticipantsMediaRepresentatives, 'd', 3500);
        self.PublicOrderViolations = createValidatedObservable(BallotPaper.PublicOrderViolations, '9', 3500);
        self.AbsentCouncilMembers_Rule_Succeeded = ko.observable(true);
        self.AbsentCouncilMembers = createValidatedObservable(BallotPaper.AbsentCouncilMembers, '10', null, validationRules.qual_or_less_than_point_1_Rule);
        self.CECEResultSummaryReports = createValidatedObservable(BallotPaper.CECEResultSummaryReports, '11', 3500);
        self.ElectoralBoardMembersOnOpening = createValidatedObservable(BallotPaper.ElectoralBoardMembersOnOpening, 'a', 3500);
        self.SIASOperatorsOnOpening = createValidatedObservable(BallotPaper.SIASOperatorsOnOpening, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesOnOpening = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesOnOpening, 'c', 3500);
        self.InternationalObserversOnOpening = createValidatedObservable(BallotPaper.InternationalObserversOnOpening, 'd', 3500);
        self.NationalObserversFromAssociationsOnOpening = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsOnOpening, 'e', 3500);
        self.MediaRepresentativesOnOpening = createValidatedObservable(BallotPaper.MediaRepresentativesOnOpening, 'f', 3500);
        self.StationaryVotingBox80Liters = createValidatedObservable(BallotPaper.StationaryVotingBox80Liters, 'a', 3500);
        self.StationaryVotingBox45Liters = createValidatedObservable(BallotPaper.StationaryVotingBox45Liters, 'b', 3500);
        self.MobileVotingBox27Liters = createValidatedObservable(BallotPaper.MobileVotingBox27Liters, 'c', 3500);
        self.PlasticSealsForVotingBoxCount = createValidatedObservable(BallotPaper.PlasticSealsForVotingBoxCount, 'd', 3500);
        self.VotingBoothsCount = createValidatedObservable(BallotPaper.VotingBoothsCount, 'a', 3500);
        self.SpecialNeedsVotingBoothsCount = createValidatedObservable(BallotPaper.SpecialNeedsVotingBoothsCount, 'b', 3500);
        self.StampedVotedCount = createValidatedObservable(BallotPaper.StampedVotedCount, 'a', 3500);
        self.StampedWithdrawnCount = createValidatedObservable(BallotPaper.StampedWithdrawnCount, 'b', 3500);
        self.ContestationsUntilEDay = createValidatedObservable(BallotPaper.ContestationsUntilEDay, 'a', 3500);
        self.ContestationsOnEDay = createValidatedObservable(BallotPaper.ContestationsOnEDay, 'b', 3500);
        self.TotalDecisionsOnObjectionsBefore = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsBefore, 'a', 3500);
        self.DecisionsFullyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsBefore, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsBefore, 'c', 3500);
        self.DecisionsRejectedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsBefore, 'd', 3500);
        self.ObjectionResponsesByLetterBefore = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterBefore, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesBefore = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesBefore, 'f', 3500);
        self.ReturnedObjectionsBefore = createValidatedObservable(BallotPaper.ReturnedObjectionsBefore, 'g', 3500);
        self.TotalDecisionsOnObjectionsFirstDay = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsFirstDay, 'a', 3500);
        self.DecisionsFullyAcceptedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsFirstDay, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsFirstDay, 'c', 3500);
        self.DecisionsRejectedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsFirstDay, 'd', 3500);
        self.ObjectionResponsesByLetterFirstDay = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterFirstDay, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesFirstDay = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesFirstDay, 'f', 3500);
        self.ReturnedObjectionsFirstDay = createValidatedObservable(BallotPaper.ReturnedObjectionsFirstDay, 'g', 3500);
        self.PublicOrderViolationsOnVotingDay = createValidatedObservable(BallotPaper.PublicOrderViolationsOnVotingDay, '12.8', 3500);
        self.BallotAssistanceCount = createValidatedObservable(BallotPaper.BallotAssistanceCount, '12.9', 3500);
        self.NumberOfRequestsByVoters = createValidatedObservable(BallotPaper.NumberOfRequestsByVoters, 'a', 3500);
        self.NumberOfRequestsByElectoralRepresentatives = createValidatedObservable(BallotPaper.NumberOfRequestsByElectoralRepresentatives, 'b', 3500);
        self.NumberOfRequestsByObservers = createValidatedObservable(BallotPaper.NumberOfRequestsByObservers, 'c', 3500);
        self.NumberOfInclusionRequests = createValidatedObservable(BallotPaper.NumberOfInclusionRequests, 'd', 3500);
        self.NumberOfExclusionRequests = createValidatedObservable(BallotPaper.NumberOfExclusionRequests, 'e', 3500);
        self.NumberOfDataCorrectionRequests = createValidatedObservable(BallotPaper.NumberOfDataCorrectionRequests, 'f', 3500);
        self.NumberOfAdmittedRequests = createValidatedObservable(BallotPaper.NumberOfAdmittedRequests, 'g', 3500);
        self.NumberOfRejectedRequests = createValidatedObservable(BallotPaper.NumberOfRejectedRequests, 'h', 3500);
        self.CertificatesReceivedFromCEC = createValidatedObservable(BallotPaper.CertificatesReceivedFromCEC, 'a', 3500);
        self.CertificatesReceivedFromCECE = createValidatedObservable(BallotPaper.CertificatesReceivedFromCECE, 'b', 3500);
        self.CertificatesIssuedToVoters = createValidatedObservable(BallotPaper.CertificatesIssuedToVoters, 'c', 3500);
        self.UnusedOrCancelledCertificates = createValidatedObservable(BallotPaper.UnusedOrCancelledCertificates, 'd', 3500);
        self.VotingRequestsAtLocation = createValidatedObservable(BallotPaper.VotingRequestsAtLocation, '12.12', 3500);
        self.BallotsIssuedByMobileTeam = createValidatedObservable(BallotPaper.BallotsIssuedByMobileTeam, '12.13', 3500);
        self.VotersBasedOnVotingCertificates = createValidatedObservable(BallotPaper.VotersBasedOnVotingCertificates, 'a', 3500);
        self.VotersWithTemporaryID = createValidatedObservable(BallotPaper.VotersWithTemporaryID, 'b', 3500);
        self.VotersWithPassport = createValidatedObservable(BallotPaper.VotersWithPassport, 'c', 3500);
        self.VotersWithExpiredPassport = createValidatedObservable(BallotPaper.VotersWithExpiredPassport, 'd', 3500);
        self.VotersAtLocationWithMobileBox = createValidatedObservable(BallotPaper.VotersAtLocationWithMobileBox, 'e', 3500);
        self.VotersInPenitentiaryInstitutions = createValidatedObservable(BallotPaper.VotersInPenitentiaryInstitutions, 'f', 3500);
        self.VotersInSanatoriumsOrHospitals = createValidatedObservable(BallotPaper.VotersInSanatoriumsOrHospitals, 'g', 3500);
        self.MilitaryVotersInMilitaryUnits = createValidatedObservable(BallotPaper.MilitaryVotersInMilitaryUnits, 'h', 3500);
        self.VotersWithLocomotorDisabilities = createValidatedObservable(BallotPaper.VotersWithLocomotorDisabilities, 'k', 3500);
        self.VotersWithVisualImpairments = createValidatedObservable(BallotPaper.VotersWithVisualImpairments, 'l', 3500);
        self.VotersWithHearingImpairments = createValidatedObservable(BallotPaper.VotersWithHearingImpairments, 'm', 3500);
        self.VotersBasedOnNewResidenceDeclaration = createValidatedObservable(BallotPaper.VotersBasedOnNewResidenceDeclaration, 'n', 3500);
        self.VotersWithStudentCard = createValidatedObservable(BallotPaper.VotersWithStudentCard, 'o', 3500);
        self.ElectoralOfficialsFromOtherSV = createValidatedObservable(BallotPaper.ElectoralOfficialsFromOtherSV, 'p', 3500);
        self.SignaturesOnBasicListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnBasicListsFirstDay, 'a', 3500);
        self.SignaturesOnSupplementaryListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnSupplementaryListsFirstDay, 'b', 3500);
        self.SignaturesOnVotingLocationListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnVotingLocationListsFirstDay, 'c', 3500);
        self.PartyDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.PartyDesignatedRepresentativesCount, 'a', 3500);
        self.ElectoralBlocDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralBlocDesignatedRepresentativesCount, 'b', 3500);
        self.IndependentCandidateDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.IndependentCandidateDesignatedRepresentativesCount, 'c', 3500);
        self.ElectoralBoardMembersCount = createValidatedObservable(BallotPaper.ElectoralBoardMembersCount, 'a', 3500);
        self.SIASOperatorsCount = createValidatedObservable(BallotPaper.SIASOperatorsCount, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesCount, 'c', 3500);
        self.InternationalObserversCount = createValidatedObservable(BallotPaper.InternationalObserversCount, 'd', 3500);
        self.NationalObserversFromAssociationsCount = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsCount, 'e', 3500);
        self.MediaRepresentativesCount = createValidatedObservable(BallotPaper.MediaRepresentativesCount, 'f', 3500);
        self.VotingResultsReportCountByBESV = createValidatedObservable(BallotPaper.VotingResultsReportCountByBESV, '12.18', 3500);
    }

    // Final CECE report, referendum, 2 days
    if ([37, 43].includes(templateId)) {
        self.CouncilMembers = createValidatedObservable(BallotPaper.CouncilMembers, '1', 11);
        self.MeetingsDuringElectionPeriod = createValidatedObservable(BallotPaper.MeetingsDuringElectionPeriod, '2', 3500);
        self.DecisionsDuringElectionPeriod = createValidatedObservable(BallotPaper.DecisionsDuringElectionPeriod, '3', 3500);
        self.ContestationsSubmittedToCouncil_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsSubmittedToCouncil = createValidatedObservable(BallotPaper.ContestationsSubmittedToCouncil, 'a', null, validationRules.A_eq_5A_5E_5F_5G_CECE_2days_Rule);
        self.ContestationsSubmittedToCouncilOnEDay_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsSubmittedToCouncilOnEDay = createValidatedObservable(BallotPaper.ContestationsSubmittedToCouncilOnEDay, 'b', null, validationRules.A_eq_6A_6E_6F_6G_CECE_2days_Rule);
        self.ContestationsSubmittedToCouncilOnDay2OfVoting_Rule_Succeeded = ko.validatedObservable(true);
        self.ContestationsSubmittedToCouncilOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsSubmittedToCouncilOnDay2OfVoting, 'c', null, validationRules.A_eq_7A_7E_7F_7G_CECE_2days_Rule);
        self.DecisionsOnContestationsMade_Rule_Succeeded = ko.validatedObservable(true);
        self.DecisionsOnContestationsMade = createValidatedObservable(BallotPaper.DecisionsOnContestationsMade, 'a', null, validationRules.A_eq_B_C_D_CECE_2days_Rule_Before);
        self.ContestationsAcceptedInFull = createValidatedObservable(BallotPaper.ContestationsAcceptedInFull, 'b', 3500);
        self.ContestationsAcceptedPartially = createValidatedObservable(BallotPaper.ContestationsAcceptedPartially, 'c', 3500);
        self.ContestationsRejected = createValidatedObservable(BallotPaper.ContestationsRejected, 'd', 3500);
        self.ResponsesToContestationsByLetter = createValidatedObservable(BallotPaper.ResponsesToContestationsByLetter, 'e', 3500);
        self.ContestationsReferredToOtherAuthorities = createValidatedObservable(BallotPaper.ContestationsReferredToOtherAuthorities, 'f', 3500);
        self.ContestationsReturned = createValidatedObservable(BallotPaper.ContestationsReturned, 'g', 3500);
        self.DecisionsOnContestationsMadeOnEDay_Rule_Succeeded = ko.validatedObservable(true);
        self.DecisionsOnContestationsMadeOnEDay = createValidatedObservable(BallotPaper.DecisionsOnContestationsMadeOnEDay, 'a', null, validationRules.A_eq_B_C_D_CECE_2days_Rule_1st_day);
        self.ContestationsAcceptedInFullOnEDay = createValidatedObservable(BallotPaper.ContestationsAcceptedInFullOnEDay, 'b', 3500);
        self.ContestationsAcceptedPartiallyOnEDay = createValidatedObservable(BallotPaper.ContestationsAcceptedPartiallyOnEDay, 'c', 3500);
        self.ContestationsRejectedOnEDay = createValidatedObservable(BallotPaper.ContestationsRejectedOnEDay, 'd', 3500);
        self.ResponsesToContestationsByLetterOnEDay = createValidatedObservable(BallotPaper.ResponsesToContestationsByLetterOnEDay, 'e', 3500);
        self.ContestationsReferredToOtherAuthoritiesOnEDay = createValidatedObservable(BallotPaper.ContestationsReferredToOtherAuthoritiesOnEDay, 'f', 3500);
        self.ContestationsReturnedOnEDay = createValidatedObservable(BallotPaper.ContestationsReturnedOnEDay, 'g', 3500);
        self.DecisionsOnContestationsMadeOnDay2OfVoting_Rule_Succeeded = ko.observable(true);
        self.DecisionsOnContestationsMadeOnDay2OfVoting = createValidatedObservable(BallotPaper.DecisionsOnContestationsMadeOnDay2OfVoting, 'a', null, validationRules.A_eq_B_C_D_CECE_2days_Rule_2nd_day);
        self.ContestationsAcceptedInFullOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsAcceptedInFullOnDay2OfVoting, 'b', 3500);
        self.ContestationsAcceptedPartiallyOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsAcceptedPartiallyOnDay2OfVoting, 'c', 3500);
        self.ContestationsRejectedOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsRejectedOnDay2OfVoting, 'd', 3500);
        self.ResponsesToContestationsByLetterOnDay2OfVoting = createValidatedObservable(BallotPaper.ResponsesToContestationsByLetterOnDay2OfVoting, 'e', 3500);
        self.ContestationsReferredToOtherAuthoritiesOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsReferredToOtherAuthoritiesOnDay2OfVoting, 'f', 3500);
        self.ContestationsReturnedOnDay2OfVoting = createValidatedObservable(BallotPaper.ContestationsReturnedOnDay2OfVoting, 'g', 3500);
        self.ConfirmedCandidateRepresentatives = createValidatedObservable(BallotPaper.ConfirmedCandidateRepresentatives, 'a', 3500);
        self.ConfirmedBlocRepresentatives = createValidatedObservable(BallotPaper.ConfirmedBlocRepresentatives, 'b', 3500);
        self.ConfirmedIndependentCandidateRepresentatives = createValidatedObservable(BallotPaper.ConfirmedIndependentCandidateRepresentatives, 'c', 3500);
        self.ParticipantsInCouncilActivitiesOnEDays = createValidatedObservable(BallotPaper.ParticipantsInCouncilActivitiesOnEDays, 'a', 3500);
        self.InternationalObserversInCouncilActivitiesOnEDays = createValidatedObservable(BallotPaper.InternationalObserversInCouncilActivitiesOnEDays, 'b', 3500);
        self.NationalObserversFromCivilSocietyOrganizationsInCouncilActivitiesOnEDays = createValidatedObservable(BallotPaper.NationalObserversFromCivilSocietyOrganizationsInCouncilActivitiesOnEDays, 'c', 3500);
        self.MediaRepresentativesInCouncilActivitiesOnEDays = createValidatedObservable(BallotPaper.MediaRepresentativesInCouncilActivitiesOnEDays, 'd', 3500);
        self.PublicOrderViolationsAtCouncil = createValidatedObservable(BallotPaper.PublicOrderViolationsAtCouncil, '10', 3500);
        self.CouncilMembersAbsentOnEDay_Rule_Succeeded = ko.observable(true);
        self.CouncilMembersAbsentOnEDay = createValidatedObservable(BallotPaper.CouncilMembersAbsentOnEDay, '11', null, validationRules.equal_or_less_than_point_1_CECE_2days_Rule);
        self.CouncilMembersAbsentOnDay2OfVoting_Rule_Succeeded = ko.observable(true);
        self.CouncilMembersAbsentOnDay2OfVoting = createValidatedObservable(BallotPaper.CouncilMembersAbsentOnDay2OfVoting, '12', null, validationRules.equal_or_less_than_point_1_CECE_2days_Rule_2nd_day);
        self.MinutesOfVotingResultsCentralizedByCECE = createValidatedObservable(BallotPaper.MinutesOfVotingResultsCentralizedByCECE, '13', 3500);
        self.ElectoralBoardMembersOnOpening = createValidatedObservable(BallotPaper.ElectoralBoardMembersOnOpening, 'a', 3500);
        self.SIASOperatorsOnOpening = createValidatedObservable(BallotPaper.SIASOperatorsOnOpening, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesOnOpening = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesOnOpening, 'c', 3500);
        self.InternationalObserversOnOpening = createValidatedObservable(BallotPaper.InternationalObserversOnOpening, 'd', 3500);
        self.NationalObserversFromAssociationsOnOpening = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsOnOpening, 'e', 3500);
        self.MediaRepresentativesOnOpening = createValidatedObservable(BallotPaper.MediaRepresentativesOnOpening, 'f', 3500);
        self.ElectoralBoardMembersSecondDay = createValidatedObservable(BallotPaper.ElectoralBoardMembersSecondDay, 'a', 3500);
        self.SIASOperatorsSecondDay = createValidatedObservable(BallotPaper.SIASOperatorsSecondDay, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesSecondDay = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesSecondDay, 'c', 3500);
        self.InternationalObserversSecondDay = createValidatedObservable(BallotPaper.InternationalObserversSecondDay, 'd', 3500);
        self.NationalObserversFromAssociationsSecondDay = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsSecondDay, 'e', 3500);
        self.MediaRepresentativesSecondDay = createValidatedObservable(BallotPaper.MediaRepresentativesSecondDay, 'f', 3500);
        self.StationaryVotingBox80Liters = createValidatedObservable(BallotPaper.StationaryVotingBox80Liters, 'a', 3500);
        self.StationaryVotingBox45Liters = createValidatedObservable(BallotPaper.StationaryVotingBox45Liters, 'b', 3500);
        self.MobileVotingBox27Liters = createValidatedObservable(BallotPaper.MobileVotingBox27Liters, 'c', 3500);
        self.PlasticSealsForVotingBoxCount = createValidatedObservable(BallotPaper.PlasticSealsForVotingBoxCount, 'd', 3500);
        self.VotingBoothsCount = createValidatedObservable(BallotPaper.VotingBoothsCount, 'a', 3500);
        self.SpecialNeedsVotingBoothsCount = createValidatedObservable(BallotPaper.SpecialNeedsVotingBoothsCount, 'b', 3500);
        self.StampedVotedCount = createValidatedObservable(BallotPaper.StampedVotedCount, 'a', 3500);
        self.StampedWithdrawnCount = createValidatedObservable(BallotPaper.StampedWithdrawnCount, 'b', 3500);
        self.ContestationsUntilEDay = createValidatedObservable(BallotPaper.ContestationsUntilEDay, 'a', 3500);
        self.ContestationsOnEDay = createValidatedObservable(BallotPaper.ContestationsOnEDay, 'b', 3500);
        self.SubmittedOnVotingDayCountSecondDay = createValidatedObservable(BallotPaper.SubmittedOnVotingDayCountSecondDay, 'c', 3500);
        self.TotalDecisionsOnObjectionsBefore = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsBefore, 'a', 3500);
        self.DecisionsFullyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsBefore, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsBefore, 'c', 3500);
        self.DecisionsRejectedObjectionsBefore = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsBefore, 'd', 3500);
        self.ObjectionResponsesByLetterBefore = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterBefore, 'e', 3500);
        self.ObjectionsForwardedToOtherAuthoritiesBefore = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherAuthoritiesBefore, 'f', 3500);
        self.ReturnedObjectionsBefore = createValidatedObservable(BallotPaper.ReturnedObjectionsBefore, 'g', 3500);
        self.TotalDecisionsOnObjectionsFirstDay = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsFirstDay, 'a', 3500);
        self.DecisionsFullyAcceptedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsFirstDay, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsFirstDay, 'c', 3500);
        self.DecisionsRejectedObjectionsFirstDay = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsFirstDay, 'd', 3500);
        self.ObjectionResponsesByLetterFirstDay = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterFirstDay, 'e', 3500);
        self.ObjectionsForwardedToOtherBodiesFirstDay = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherBodiesFirstDay, 'f', 3500);
        self.ReturnedObjectionsFirstDay = createValidatedObservable(BallotPaper.ReturnedObjectionsFirstDay, 'g', 3500);
        self.TotalDecisionsOnObjectionsSecondDay = createValidatedObservable(BallotPaper.TotalDecisionsOnObjectionsSecondDay, 'a', 3500);
        self.DecisionsFullyAcceptedObjectionsSecondDay = createValidatedObservable(BallotPaper.DecisionsFullyAcceptedObjectionsSecondDay, 'b', 3500);
        self.DecisionsPartiallyAcceptedObjectionsSecondDay = createValidatedObservable(BallotPaper.DecisionsPartiallyAcceptedObjectionsSecondDay, 'c', 3500);
        self.DecisionsRejectedObjectionsSecondDay = createValidatedObservable(BallotPaper.DecisionsRejectedObjectionsSecondDay, 'd', 3500);
        self.ObjectionResponsesByLetterSecondDay = createValidatedObservable(BallotPaper.ObjectionResponsesByLetterSecondDay, 'e', 3500);
        self.ObjectionsForwardedToOtherBodiesSecondDay = createValidatedObservable(BallotPaper.ObjectionsForwardedToOtherBodiesSecondDay, 'f', 3500);
        self.ReturnedObjectionsSecondDay = createValidatedObservable(BallotPaper.ReturnedObjectionsSecondDay, 'g', 3500);
        self.PublicOrderViolationsOnVotingDay = createValidatedObservable(BallotPaper.PublicOrderViolationsOnVotingDay, '14.10', 3500);
        self.BallotAssistanceCount = createValidatedObservable(BallotPaper.BallotAssistanceCount, '14.11', 3500);
        self.NumberOfRequestsByVoters = createValidatedObservable(BallotPaper.NumberOfRequestsByVoters, 'a', 3500);
        self.NumberOfRequestsByElectoralRepresentatives = createValidatedObservable(BallotPaper.NumberOfRequestsByElectoralRepresentatives, 'b', 3500);
        self.NumberOfRequestsByObservers = createValidatedObservable(BallotPaper.NumberOfRequestsByObservers, 'c', 3500);
        self.NumberOfInclusionRequests = createValidatedObservable(BallotPaper.NumberOfInclusionRequests, 'd', 3500);
        self.NumberOfExclusionRequests = createValidatedObservable(BallotPaper.NumberOfExclusionRequests, 'e', 3500);
        self.NumberOfDataCorrectionRequests = createValidatedObservable(BallotPaper.NumberOfDataCorrectionRequests, 'f', 3500);
        self.NumberOfAdmittedRequests = createValidatedObservable(BallotPaper.NumberOfAdmittedRequests, 'h', 3500);
        self.NumberOfRejectedRequests = createValidatedObservable(BallotPaper.NumberOfRejectedRequests, 'g', 3500);
        self.CertificatesReceivedFromCEC = createValidatedObservable(BallotPaper.CertificatesReceivedFromCEC, 'a', 3500);
        self.CertificatesReceivedFromCECE = createValidatedObservable(BallotPaper.CertificatesReceivedFromCECE, 'b', 3500);
        self.CertificatesIssuedToVoters = createValidatedObservable(BallotPaper.CertificatesIssuedToVoters, 'c', 3500);
        self.UnusedOrCancelledCertificates = createValidatedObservable(BallotPaper.UnusedOrCancelledCertificates, 'd', 3500);
        self.TotalVotingRequestsAtLocation = createValidatedObservable(BallotPaper.TotalVotingRequestsAtLocation, '14.14', 3500);
        self.BallotsIssuedByMobileTeam = createValidatedObservable(BallotPaper.BallotsIssuedByMobileTeam, '14.15', 3500);
        self.VotersBasedOnVotingCertificates = createValidatedObservable(BallotPaper.VotersBasedOnVotingCertificates, 'a', 3500);
        self.VotersWithTemporaryID = createValidatedObservable(BallotPaper.VotersWithTemporaryID, 'b', 3500);
        self.VotersWithPassport = createValidatedObservable(BallotPaper.VotersWithPassport, 'c', 3500);
        self.VotersWithExpiredPassport = createValidatedObservable(BallotPaper.VotersWithExpiredPassport, 'd', 3500);
        self.VotersAtLocationWithMobileBox = createValidatedObservable(BallotPaper.VotersAtLocationWithMobileBox, 'e', 3500);
        self.VotersInPenitentiaryInstitutions = createValidatedObservable(BallotPaper.VotersInPenitentiaryInstitutions, 'f', 3500);
        self.VotersInSanatoriumsOrHospitals = createValidatedObservable(BallotPaper.VotersInSanatoriumsOrHospitals, 'g', 3500);
        self.MilitaryVotersInMilitaryUnits = createValidatedObservable(BallotPaper.MilitaryVotersInMilitaryUnits, 'h', 3500);
        self.VotersWithLocomotorDisabilities = createValidatedObservable(BallotPaper.VotersWithLocomotorDisabilities, 'k', 3500);
        self.VotersWithVisualImpairments = createValidatedObservable(BallotPaper.VotersWithVisualImpairments, 'l', 3500);
        self.VotersWithHearingImpairments = createValidatedObservable(BallotPaper.VotersWithHearingImpairments, 'm', 3500);
        self.VotersBasedOnNewResidenceDeclaration = createValidatedObservable(BallotPaper.VotersBasedOnNewResidenceDeclaration, 'n', 3500);
        self.VotersWithStudentCard = createValidatedObservable(BallotPaper.VotersWithStudentCard, 'o', 3500);
        self.ElectoralOfficialsFromOtherSV = createValidatedObservable(BallotPaper.ElectoralOfficialsFromOtherSV, 'p', 3500);
        self.SignaturesOnBasicListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnBasicListsFirstDay, 'a', 3500);
        self.SignaturesOnSupplementaryListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnSupplementaryListsFirstDay, 'b', 3500);
        self.SignaturesOnVotingLocationListsFirstDay = createValidatedObservable(BallotPaper.SignaturesOnVotingLocationListsFirstDay, 'c', 3500);
        self.PartyDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.PartyDesignatedRepresentativesCount, 'a', 3500);
        self.ElectoralBlocDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralBlocDesignatedRepresentativesCount, 'b', 3500);
        self.IndependentCandidateDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.IndependentCandidateDesignatedRepresentativesCount, 'c', 3500);
        self.PartyDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.PartyDesignatedRepresentativesCount, 'a', 3500);
        self.ElectoralBlocDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralBlocDesignatedRepresentativesCount, 'b', 3500);
        self.IndependentCandidateDesignatedRepresentativesCount = createValidatedObservable(BallotPaper.IndependentCandidateDesignatedRepresentativesCount, 'c', 3500);
        self.ElectoralBoardMembersCount = createValidatedObservable(BallotPaper.ElectoralBoardMembersCount, 'a', 3500);
        self.SIASOperatorsCount = createValidatedObservable(BallotPaper.SIASOperatorsCount, 'b', 3500);
        self.ElectoralCompetitorsRepresentativesCount = createValidatedObservable(BallotPaper.ElectoralCompetitorsRepresentativesCount, 'c', 3500);
        self.InternationalObserversCount = createValidatedObservable(BallotPaper.InternationalObserversCount, 'd', 3500);
        self.NationalObserversFromAssociationsCount = createValidatedObservable(BallotPaper.NationalObserversFromAssociationsCount, 'e', 3500);
        self.MediaRepresentativesCount = createValidatedObservable(BallotPaper.MediaRepresentativesCount, 'f', 3500);
        self.VotingResultsReportCountByBESV = createValidatedObservable(BallotPaper.VotingResultsReportCountByBESV, '14.20', 3500);
    }

    self.EditDate = ko.observable(BallotPaper.EditDate);
    self.EditUser = ko.observable(BallotPaper.EditUser);
    self.ServerEditDate = ko.observable(BallotPaper.ServerEditDate);
    self.IsResultsConfirmed = ko.observable(BallotPaper.IsResultsConfirmed);
    self.ConfirmationUserId = ko.observable(BallotPaper.ConfirmationUserId);
    self.ConfirmationDate = ko.observable(BallotPaper.ConfirmationDate);
    self.ServerConfirmationDate = ko.observable(BallotPaper.ServerConfirmationDate);
    self.Status = ko.observable(BallotPaper.Status);
    self.ElectionType = BallotPaper.ElectionType;
    self.DocumentName = ko.observable();

    self.ShowValidationErrors = ko.observable(false);

    self.AlreadySent = ko.observable(BallotPaper.AlreadySent);
    self.AllowSubmitResults = ko.observable(BallotPaper.AllowSubmitResults);
    self.AllowSubmitResults = ko.observable(true);
    self.AllowSubmitConfirmation = ko.observable(BallotPaper.AllowSubmitConfirmation);

    self.isCompletedAutomatically = ko.observable(false);

    self.Validate = function () {
        self.ShowValidationErrors(true);
    }

    self.resolveTemplate = function (isIndependent) {
        if (self.ElectionType === 3) {
            $('#titleParty').html(BallotPaper.CompetitorResults[0].PoliticalPartyName);
            return 'referendumElectionTemplate';
        }

        if (isIndependent) {
            return 'independentCandidateTemplate';
        }

        if (self.ElectionType === 1) {
            return 'uninominalTemplate';
        }

        return 'normalPartyTemplate';
    };

    self.hasBeenSent = ko.pureComputed(function () {
        return self.Status > BallotPaperStatus.New && self.ConfirmationUserId > 0;
    });

    self.getFormatedDate = function (date) {
        return moment(date).format('DD.MM.YYYY HH:mm:ss');
    };

    self.getStatusBarTemplate = function () {
        if (self.IsResultsConfirmed() || self.AllowSubmitConfirmation() || self.AlreadySent()) {
            return 'alreadySubmitedStatusBarTemplate';
        }

        if (self.updateSuccess()) {
            return 'updateSuccessStatusBarTemplate';
        }

        if (!self.AllowSubmitResults()) {
            return 'submitNotAllowedStatusBarTemplate';
        }

        if (self.AllowSubmitResults() && self.isValid()) {
            return 'submitAllowedStatusBarTemplate';
        }

        return 'defaultStatusBarTemplate';
    };

    self.AllowEdit = ko.pureComputed(function () {
        return ((!self.AlreadySent() && self.AllowSubmitResults()) || self.AllowSubmitConfirmation()) && !self.parent.InCallMode();
    });

    self.errors = ko.validation.group(self, { deep: true, live: true });
    self.updateSuccess = ko.observable(null);

    self.__callSubmitBallotPaper = function (action) {
        const self = this;

        if (self.isValid()) {
            var bp = self.getBallotPaperData();

            self.parent.CallSubmitBallotPaper(bp, action);
        } else {
            console.log("error");
            self.ShowValidationErrors(true);
        }
    };

    self.SubmitResults = function () {
        self.__callSubmitBallotPaper(1);
    };

    self.ConfirmResults = function () {
        self.__callSubmitBallotPaper(2);
    };

    self.setUpdateResult = function (data) {
        if (data.Success) {
            self.AlreadySent(true);
            self.AllowSubmitResults(false);
            self.updateSuccess(true);
        }
    };

    self.reportTemplateViewModel = ko.observable(new ReportTemplateViewModel());

    self.templateNameId = ko.observable(electionResultsVM.reportTemplateViewModel().templateNameId());

    self.getBallotPaperData = function () {
        if ([6, 7, 8, 9].includes(self.templateNameId())) {
            return {
                BallotPaperId: self.BallotPaperId,
                RegisteredVoters: self.RegisteredVoters(),
                Supplementary: self.Supplementary(),
                BallotsIssued: self.BallotsIssued(),
                BallotsCasted: self.BallotsCasted(),
                DifferenceIssuedCasted: self.DifferenceIssuedCasted(),
                BallotsSpoiled: self.BallotsSpoiled(),
                BallotsValidVotes: self.BallotsValidVotes(),
                BallotsReceived: self.BallotsReceived(),
                BallotsUnusedSpoiled: self.BallotsUnusedSpoiled(),
                CompetitorResults: self.CompetitorResults().map(x => ({ ElectionResultId: x.ElectionResultId, BallotCount: x.BallotCount() }))
            };
        }

        const parameterNames = [];

        const resultValues = electionResultsVM.ReportParameterValues().map(value => {
            parameterNames.push(value.ReportParameterName);

            return {
                ...value,
                ValueContent: self[value.ReportParameterName]()
            };
        });

        return { ReportParameterValues: resultValues };
    };

    self.PrintDocument = function (data) {
        if (self.templateNameId()) {
            const { ElectionId, PollingStationId, CircumscriptionId } = electionResultsVM.Delimitator().getDelimitatorData();

            window.location.href = `/Documents/Print?templateNameId=${self.templateNameId()}&electionId=${ElectionId}&pollingStationId=${PollingStationId}&circumscriptionId=${CircumscriptionId}`;
        } else {
            console.error("TemplateNameId is not set or invalid");
        }
    }


    self.CompleteAutomatically = function () {
        electionResultsVM.InCallMode(true);

        if ([24, 25, 26, 27, 28, 29, 30, 31].includes(electionResultsVM.reportTemplateViewModel().templateNameId())) {
            $.ajax({
                url: 'ElectionResults/GetCircumscriptionResultsConsolidated',
                type: 'post',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify({ delimitationData: { ...electionResultsVM.Delimitator().getDelimitatorData(), TemplateNameId: electionResultsVM.reportTemplateViewModel().templateNameId() } }),
                success: function (result) {
                    electionResultsVM.ballotPaperConsolidationData(result);
                    electionResultsVM.BallotPaper().RegisteredVoters(result.RegisteredVoters);
                    electionResultsVM.BallotPaper().Supplementary(result.Supplementary);
                    electionResultsVM.BallotPaper().BallotsSpoiled(result.BallotsSpoiled);
                    electionResultsVM.BallotPaper().BallotsValidVotes(result.BallotsValidVotes);
                    electionResultsVM.BallotPaper().BallotsCasted(result.BallotsCasted);
                    electionResultsVM.BallotPaper().BallotsIssued(result.BallotsIssued);
                    electionResultsVM.BallotPaper().DifferenceIssuedCasted(result.DifferenceIssuedCasted);
                    electionResultsVM.BallotPaper().BallotsReceived(result.BallotsReceived);
                    electionResultsVM.BallotPaper().BallotsUnusedSpoiled(result.BallotsUnusedSpoiled);

                    electionResultsVM.BallotPaper().CompetitorResults(result.CompetitorResults);

                    self.isCompletedAutomatically(true);
                    self.AlreadySent(result.AlreadySent);
                },
                error: function (error) {
                    console.log(error)
                }
            }).done(function (data) {
                electionResultsVM.InCallMode(false);
                $(".number").forceNumeric();
            });
        }

        if ([32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43].includes(electionResultsVM.reportTemplateViewModel().templateNameId())) {
            $.ajax({
                url: 'Documents/RetrieveFinalReportData',
                type: 'post',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify({
                    delimitationData: electionResultsVM.Delimitator().getDelimitatorData(),
                    templateNameId: electionResultsVM.reportTemplateViewModel().templateNameId()
                }),
                success: function (result) {
                    electionResultsVM.ReportParameterValues(result.ReportParameterValues);

                    result.ReportParameterValues.forEach(function (value) {
                        if (value.ValueContent !== null) {
                            electionResultsVM.BallotPaper()[value.ReportParameterName](value.ValueContent);
                        }
                    });

                    self.isCompletedAutomatically(true);
                    self.AlreadySent(result.AlreadySent);
                },
                error: function (error) {
                    console.log(error)
                }
            }).done(function (data) {
                electionResultsVM.InCallMode(false);
                $(".number").forceNumeric();
            });
        }
    }
}

ko.components.register('valid-submission-notification', {
    viewModel: function (params) {
        this.isValid = params.isValid;
        this.electionName = params.electionName;
    },
    template: `
        <div class="col-xs-12 text-center">
            <p data-bind="visible: isValid()">
                Înscrisul electoral pentru scrutinul <b><span data-bind="text: electionName"></span></b> este corect întocmit.<br>
                Acum puteți expedia înscrisul electoral.<br>
                Înscrisul electoral expediat nu va mai fi disponibil pentru modificare.
            </p>
        </div>
    `
});


function CompetitorResult(CompetitorResult) {
    const self = this;

    self.ElectionResultId = CompetitorResult.ElectionResultId;
    self.BallotOrder = CompetitorResult.BallotOrder;
    self.PoliticalPartyCode = CompetitorResult.PoliticalPartyCode;
    self.PoliticalPartyName = CompetitorResult.PoliticalPartyName;
    self.CandidateName = CompetitorResult.CandidateName;
    self.BallotCount = ko.observable(CompetitorResult.BallotCount).extend({
        required: { message: validationMsgs.isRequired },
        number: { message: validationMsgs.digitsExpected }
    });
    self.PartyStatus = CompetitorResult.PartyStatus;
    self.IsIndependent = CompetitorResult.IsIndependent;

    self.isWithdrawn = ko.pureComputed(function () {
        return self.PartyStatus === PoliticalPartyStatus.Withdrawn;
    });
}

// forceNumeric() plug-in implementation
jQuery.fn.forceNumeric = function () {
    return this.each(function () {
        $(this).keydown(function (e) {
            var key = e.which || e.keyCode;

            if (!e.shiftKey && !e.altKey && !e.ctrlKey &&
                // numbers
                key >= 48 && key <= 57 ||
                // Numeric keypad
                key >= 96 && key <= 105 ||
                // comma, period and minus, . on keypad
                //key == 190 || key == 188 || key == 109 || key == 110 ||
                // Backspace and Tab and Enter
                key == 8 || key == 9 || key == 13 ||
                // Home and End
                key == 35 || key == 36 ||
                // left and right arrows
                key == 37 || key == 39 ||
                // Del and Ins
                key == 46 || key == 45)
                return true;

            return false;
        });
    });
};

ko.validation.configure({
    insertMessages: false,
    errorsAsTitle: false,
    decoratedElement: true,
    allowHtmlMessages: true,
    messagesOnModified: false,
    errorElementClass: 'has-error',
    errorMessageClass: 'help-block'
});

ko.applyBindingsWithValidation(electionResultsVM);

(function ($) {
    electionResultsVM.Delimitator().enableElectionsSelector();

    $('body')
        .on('focus', 'tr > td > input', function () {
            $(this).closest('tr').addClass('alert-info');
        })
        .on('blur', 'tr > td > input', function () {
            $(this).closest('tr').removeClass('alert-info');
        })
        .on('click', '.checkActivation', function () {
            if (electionResultsVM.Delimitator().isReady()) {
                electionResultsVM.requestBallotPaper();
            }
        });
})(jQuery);