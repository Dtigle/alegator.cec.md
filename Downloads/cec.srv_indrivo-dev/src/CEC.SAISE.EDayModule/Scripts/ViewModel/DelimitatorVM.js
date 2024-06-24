function DelimitatorVM() {
    const self = this;

    self.SelectedElectoralRegisteredId = ko.observable();
    self.SelectedElectionId = ko.observable();
    self.SelectedCircumscriptionId = ko.observable();
    self.SelectedRegionId = ko.observable();
    self.SelectedPollingStationId = ko.observable();
    self.ElectionIsLocal = ko.observable();
    self.IsMayorElection = ko.observable();
    self.onChanged = ko.observable();

    self.isCECEProcessReport = ko.pureComputed(function () {
        return [22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43].includes(self.SelectedElectoralRegisteredId());
    });

    self.isReady = ko.pureComputed(function () {
        return self.onChanged() != null && self.onChanged().isReady();
    });

    self.SelectedElectoralRegistered = function () {
        return $('.electoralRegisteredSelect').text().trim();
    };

    self.SelectedElection = function () {
        return $('.electionsSelect').text().trim();
    };

    self.SelectedCircumscription = function () {
        return $('.circumscriptionSelect').text().trim();
    };

    self.SelectedRegion = function () {
        return $('.regionSelect').text().trim();
    };

    self.SelectedPollingStation = function () {
        return $('.pollingStationSelect').text().trim();
    };

    function enableSelector(selector, placeholder, url, dataFunc) {
        const $select = $(selector);
        $select.select2("val", "");
        $select.unbind('change');

        $select.select2({
            placeholder: placeholder,
            ajax: {
                url: url,
                type: 'post',
                dataType: 'json',
                contentType: 'application/json',
                delay: 250,
                data: dataFunc,
                results: self._pageFunc,
                cache: true
            }
        }).on('change', function (e) {
            if (selector === '.electionsSelect') {
                self.selectionChanged(parseInt(e.val), null, null, null, null);
                self.enableElectoralRegisteredSelector();
            } else if (selector === '.electoralRegisteredSelect') {
                self.selectionChanged(self.SelectedElectionId(), parseInt(e.val), null, null, null);
                self.enableCircumscriptionsSelector();
            }  else if (selector === '.circumscriptionSelect') {
                self.selectionChanged(self.SelectedElectionId(), self.SelectedElectoralRegisteredId(), parseInt(e.val), null, null);
                self.enableRegionSelector();
            } else if (selector === '.regionSelect') {
                self.selectionChanged(self.SelectedElectionId(), self.SelectedElectoralRegisteredId(), self.SelectedCircumscriptionId(), parseInt(e.val), null);
                self.enablePollingStationSelector();
            } else if (selector === '.pollingStationSelect') {
                self.selectionChanged(self.SelectedElectionId(), self.SelectedElectoralRegisteredId(), self.SelectedCircumscriptionId(), self.SelectedRegionId(), parseInt(e.val));
            }
        });
    }

    self.enableElectionsSelector = function () {
        enableSelector('.electionsSelect', 'Selectați scrutinul...', 'Selectors/SelectElections', self._dataFuncForElectionsSelect);
    };

    self.enableElectoralRegisteredSelector = function () {
        enableSelector('.electoralRegisteredSelect', 'Selectați document electoral...', 'Selectors/SelectTemplateNames', self._dataFunc);
    }; 

    self.enableCircumscriptionsSelector = function () {
        enableSelector('.circumscriptionSelect', 'Selectați circumscripția...', 'Selectors/SelectCircumscription', function (term, page) {
            return {
                q: term,
                pageLimit: 10,
                page: page,
                electionId: self.SelectedElectionId()
            };
        });
    };

    self.enableRegionSelector = function () {
        enableSelector('.regionSelect', 'Selectați localitatea...', 'Selectors/SelectRegions', function (term, page) {
            return {
                q: term,
                pageLimit: 10,
                page: page,
                electionId: self.SelectedElectionId(),
                circumscriptionId: self.SelectedCircumscriptionId()
            };
        });
    };

    self.enablePollingStationSelector = function () {
        enableSelector('.pollingStationSelect', 'Selectați secţia de votare...', 'Selectors/SelectPollingStations', function (term, page) {
            return {
                q: term,
                pageLimit: 10,
                page: page,
                electionId: self.SelectedElectionId(),
                circumscriptionId: self.SelectedCircumscriptionId(),
                regionId: self.SelectedRegionId()
            };
        });
    };

    self._dataFunc = function (term, page) {
        return {
            q: term,
            pageLimit: 10,
            page: page,
            electionId: self.SelectedElectionId(),
        };
    };

    self._dataFuncForElectionsSelect = function (term, page) {
        return {
            q: term,
            pageLimit: 10,
            page: page,
            electionId: self.SelectedElectionId(),
        };
    };

    self._pageFunc = function (data, page) {
        const more = (page * 10) < data.Total;

        return { results: data.Items, more: more };
    };

    self.selectionChanged = function (electionId, electoralRegisteredId, circumscriptionId, regionId, pollingStationId) {
        self.SelectedElectoralRegisteredId(electoralRegisteredId);
        self.SelectedElectionId(electionId);
        self.SelectedCircumscriptionId(circumscriptionId);
        self.SelectedRegionId(regionId);
        self.SelectedPollingStationId(pollingStationId);

        const eventData = new DelimitatorChangedEvent(electoralRegisteredId, electionId, self.ElectionIsLocal(), self.IsMayorElection(), circumscriptionId, regionId, pollingStationId);

        self.onChanged(eventData);
    };

    self.getDelimitatorData = function () {
        return {
            ElectoralRegisteredId: self.SelectedElectoralRegisteredId(),
            ElectionId: self.SelectedElectionId(),
            ElectionIsLocal: self.ElectionIsLocal(),
            IsMayorElection: self.IsMayorElection(),
            CircumscriptionId: self.SelectedCircumscriptionId(),
            RegionId: self.SelectedRegionId(),
            PollingStationId: self.SelectedPollingStationId()
        };
    }
}

function DelimitatorChangedEvent(electoralRegisteredId, electionId, electionIsLocal, isMayorElection, circumscriptionId, regionId, pollingStationId) {
    const self = this;

    self.ElectoralRegisteredId = electoralRegisteredId;
    self.ElectionId = electionId;
    self.ElectionIsLocal = electionIsLocal;
    self.IsMayorElection = isMayorElection;
    self.CircumscriptionId = circumscriptionId;
    self.RegionId = regionId;
    self.PollingStationId = pollingStationId;

    if ([22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43].includes(electoralRegisteredId)) {
        self.isReady = function () {
            return self.ElectionId != null &&
                self.ElectoralRegisteredId != null &&
                self.CircumscriptionId != null;
        }

        return;
    }

    self.isReady = function () {
        return self.ElectionId != null &&
            self.ElectoralRegisteredId != null &&
            self.CircumscriptionId != null &&
            self.RegionId != null &&
            self.PollingStationId != null;
    }
}