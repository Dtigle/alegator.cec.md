function BallotPaperVM() {
    const self = this;

    self.electionName = ko.observable();
    self.circumscriptionRegion = ko.observable();
    self.circumscriptionName = ko.observable();
    self.pollingStationRegion = ko.observable();
    self.pollingStationNumber = ko.observable();
    self.electionDate = ko.observable();
    self.documentHeaderElectionDetails = ko.observable();
    self.referendumQuestion = ko.observable();

    self.ReportParameterValues = ko.observableArray([]);
    self.CompetitorResults = ko.observableArray([]);

    self.DocumentId = ko.observable();

    self.wasUploadedWithSuccess = ko.observable();
    self.inCallMode = ko.observable(false);

    self.documentPrintPropertiesVM = new DocumentPrintPropertiesVM();

    self.circumscriptionNameFormatted = ko.computed(function () {
        if (self.circumscriptionName() && self.circumscriptionName().trim() !== '') {
            const circumscriptionNameParts = self.circumscriptionName().split(" ");

            return `${circumscriptionNameParts[0]} NR. ${circumscriptionNameParts[1]}`;
        }

        return "";
    });

    self.circumscriptionRegionFormatted = ko.computed(function () {
        if (self.circumscriptionRegion() && self.circumscriptionRegion().trim() !== '') {
            const circumscriptionRegionParts = self.circumscriptionRegion().split(" ");

            if (circumscriptionRegionParts.length > 0 && circumscriptionRegionParts[0] === "Consiliul") {
                circumscriptionRegionParts[0] = "Consiliului";
            }

            let formattedString = circumscriptionRegionParts.join(' ');

            return formattedString;
        }

        return "";
    });

    self.circumscriptionRegionType = ko.computed(function () {
        if (self.circumscriptionRegion() && self.circumscriptionRegion().trim() !== '') {
            const circumscriptionRegionParts = self.circumscriptionRegion().split(" ");
            const lastElementIndex = circumscriptionRegionParts.length - 1;

            return `${circumscriptionRegionParts[lastElementIndex]}`;
        }

        return "";
    });

    self.circumscriptionRegionWithoutType = ko.computed(function () {
        if (self.circumscriptionRegion() && self.circumscriptionRegion().trim() !== '') {
            const circumscriptionRegionParts = self.circumscriptionRegion().split(" ");

            return `${circumscriptionRegionParts.slice(0, -1).join(" ")}`;
        }

        return "";
    });

    self.formattedDate = ko.computed(function () {
        const months = [
            "ianuarie",
            "februarie",
            "martie",
            "aprilie",
            "mai",
            "iunie",
            "iulie",
            "august",
            "septembrie",
            "octombrie",
            "noiembrie",
            "decembrie",
        ];

        if (self.electionDate() && self.electionDate().trim() !== '') {
            const dateParts = self.electionDate().split(' ')[0].split('.');
            const day = parseInt(dateParts[0]);
            const monthIndex = parseInt(dateParts[1]) - 1;
            const year = parseInt(dateParts[2]);

            const formatted = day + ' ' + months[monthIndex] + ' ' + year;
            return formatted;
        } else {
            return "Invalid Date";
        }
    });

    const urlParams = new URLSearchParams(window.location.search);
    const templateNameId = urlParams.get('templateNameId');
    const electionId = urlParams.get('electionId');
    const pollingStationId = urlParams.get('pollingStationId');
    const circumscriptionId = urlParams.get('circumscriptionId');

    const delimitationData = {
        ElectionId: electionId,
        PollingStationId: pollingStationId,
        CircumscriptionId: circumscriptionId,
    };

    self.getHeaderParameters = async function () {
        await $.ajax({
            url: '/ElectionResults/GetBallotPaperHeaderData',
            method: 'GET',
            data: {
                ElectionId: delimitationData.ElectionId,
                PollingStationId: delimitationData.PollingStationId,
                circumscriptionId: circumscriptionId,
                templateNameId: templateNameId,
            },
            success: function (result) {
                self.electionDate(result.ElectionDate);
                self.electionName(result.ElectionName);
                self.circumscriptionRegion(result.CircumscriptionRegion);
                self.circumscriptionName(result.CircumscriptionName);
                self.pollingStationRegion(result.PollingStationRegion);
                self.pollingStationNumber(result.PollingStationNumber);
                self.documentHeaderElectionDetails(result.DocumentHeaderElectionDetails);
                self.referendumQuestion(result.ReferendumQuestion);
            },
        });
    }

    self.getReportParameterValues = async function () {
        await $.ajax({
            url: '/Documents/GetBallotPaperDocumentAsync',
            method: 'GET',
            dataType: 'text',
            data: {
                electionId: delimitationData.ElectionId,
                templateNameId: templateNameId,
                pollingStationId: delimitationData.PollingStationId,
                circumscriptionId: delimitationData.CircumscriptionId
            },
            success: function (result) {
                if (result) {
                    try {
                        var parsedResult = JSON.parse(result);
                        self.ReportParameterValues(parsedResult.ReportParameterValues.filter(resultItem => resultItem.ReportParameterName !== "DynamicParameter"));
                        self.CompetitorResults(
                            parsedResult.ReportParameterValues
                                .filter(resultItem => resultItem.ReportParameterName === "DynamicParameter" &&
                                    (resultItem.ElectionCompetitorName !== null || resultItem.ElectionCompetitorMemberName !== null)
                                )
                        );

                        self.ReportParameterValues().forEach(function (value) {
                            self.documentPrintPropertiesVM[value.ReportParameterName](value.ValueContent);
                        });

                        self.DocumentId(parsedResult.DocumentId);

                        const templateContainers = [
                            ".template-container",
                            ".template-container2",
                            ".template-container3",
                            ".template-container4",
                            ".template-container5",
                            ".template-container6",
                            ".template-container7"
                        ];

                        templateContainers.forEach(selector => {
                            const element = document.querySelector(selector);

                            if (element) {
                                //element.style.width = "";
                                //element.style.height = "";
                                element.style.paddingBottom = "0";
                                element.style.paddingTop = "118px";
                            }
                        });
                    } catch (e) {
                        console.log("Error parsing JSON:", e);
                    }
                } else {
                    console.log("Empty response received");
                }
            },
            error: function (xhr, status, error) {
                console.log("XHR status:", status);
                console.log("Error:", error);
            }
        });
    }

    function adjustContainerPaddings() {
        const containers = document.querySelectorAll(
            '.template-container2, .template-container3, .template-container4, .template-container5, .template-container6, .template-container7'
        );

        containers.forEach(container => {
            container.style.padding = '120px 90px 120px 90px';
            container.style.margin = '0 auto';
            container.style.setProperty('margin', '0 auto', 'important');
        });
    }

    self.SavePdf = async function () {
        self.inCallMode(true);

        const pdf = new jsPDF('', 'pt', 'a4');

        const templateContainers = [
            ".template-container",
            ".template-container2",
            ".template-container3",
            ".template-container4",
            ".template-container5",
            ".template-container6",
            ".template-container7"
        ];

        templateContainers.forEach((selector, index) => {
            const element = document.querySelector(selector);

            if (element) {
                element.style.width = "1240px";
                element.style.height = "1754px";
                element.style.lineHeight = "1.7";

                if (index === 0) {
                    element.style.paddingTop = "80px"
                }
            }
        });

        adjustContainerPaddings();

        const addImageToPdf = (selector) => {
            return new Promise((resolve, reject) => {
                html2canvas(document.querySelector(selector)).then((canvas) => {
                    const base64image = canvas.toDataURL('image/jpeg', 0.8);
                    
                    pdf.addImage(base64image, 'JPEG', 0, 0, 595.28, 592.28 / canvas.width * canvas.height, undefined, 'FAST' );

                    resolve();
                }).catch((error) => {
                    reject(error);
                });
            });
        };

        const generatePDF = async () => {
            window.scroll({
                top: document.body.scrollHeight,
                left: 100,
                behavior: "smooth",
            });

            async function addImagesToPdf(templateIds, pdfContainers) {
                for (let i = 0; i < templateIds.length; i++) {
                    const container = pdfContainers[i];
                    await addImageToPdf(container);

                    if (i < templateIds.length - 1) {
                        pdf.addPage();
                    }
                }
            }

            const templateMappings = {
                '6,7,8,9,24,25,26,27,28,29,30,31': { ids: [1, 2], containers: [".template-container", ".template-container2"] },
                '10,11,12,13,14,16,17,19,20,33,39': { ids: [1, 2, 3, 4], containers: [".template-container", ".template-container2", ".template-container3", ".template-container4"] },
                '15,18,21,32,35,36,38,41,42': { ids: [1, 2, 3, 4, 5], containers: [".template-container", ".template-container2", ".template-container3", ".template-container4", ".template-container5"] },
                '34,37,40,43': { ids: [1, 2, 3, 4, 5, 6], containers: [".template-container", ".template-container2", ".template-container3", ".template-container4", ".template-container5", ".template-container6"] }
            };

            const templateId = parseInt(templateNameId);
            const templateKey = Object.keys(templateMappings).find(key => key.split(',').includes(templateId.toString()));

            if (templateKey) {
                const { ids, containers } = templateMappings[templateKey];
                await addImagesToPdf(ids, containers);
            }
            return pdf.output('blob');
        };

        generatePDF().then((pdfBlob) => {
            const documentSaveModel = {
                DocumentId: self.DocumentId(),
                TemplateNameId: parseInt(templateNameId),
                ElectionId: parseInt(electionId),
                PollingStationId: parseInt(pollingStationId),
                CircumscriptionId: parseInt(circumscriptionId)
            };

            const blob = new Blob([pdfBlob], { type: 'application/pdf' });
            const formData = new FormData();

            formData.append('file', blob, 'webtylepress-receipt.pdf');
            formData.append('model', JSON.stringify(documentSaveModel));

            $.ajax({
                url: '/File/UploadPdf',
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                enctype: 'multipart/form-data',
                success: function (response) {
                    console.log('File uploaded successfully:', response);

                    templateContainers.forEach((selector, index) => {
                        const element = document.querySelector(selector);

                        if (element) {
                            element.style.width = "";
                            element.style.height = "";

                            if (index !== 0) {
                                element.style.paddingTop = "8px";
                            }

                            if (index !== templateContainers.length - 1) {
                                element.style.paddingBottom = "8px";
                            }
                        }
                    });

                    if (response.Success === true) {
                        self.wasUploadedWithSuccess(true);
                        self.inCallMode(false);
                    }

                    if (response.Success === false) {
                        self.wasUploadedWithSuccess(false);
                        self.inCallMode(false);
                    }
                },
                error: function (error) {
                    self.inCallMode(false);
                    console.error('Error uploading file:', error);
                }
            });
        }).catch(error => console.log("error", error));
    }

    self.PrintTemplate = function () {
        var printWindow = window.open('', '_blank');

        const newHTML = `
            <html>
                <head>
                    <title>Your Page Title</title>
                    <style>
                        ${customStyles}
                    </style>
                </head>
                <body>
                    ${document.querySelector("body").innerHTML}
                </body>
            </html>
        `;

        const printableContent = newHTML;

        printWindow.document.open();
        printWindow.document.write(printableContent);
        printWindow.document.close();

        const navbar = printWindow.document.querySelector(".navbar");
        const sidebarLeft = printWindow.document.querySelector("#sidebar-left");
        const printTemplateButton = printWindow.document.querySelector("input#printTemplate");
        const savePdfButton = printWindow.document.querySelector("input#savePdf");
        const pdfSavedWithSuccessMessage = printWindow.document.querySelector(".pdf-saved-with-success-message");
        const pdfSavingErrorMessage = printWindow.document.querySelector(".pdf-saving-error-message");
        const pdfSavingLoader = printWindow.document.querySelector(".pdf-saving-loader");
        const listItems = printWindow.document.querySelectorAll("li");

        listItems.forEach(function (li) {
            li.style.listStyle = "none";
        });

        navbar.remove();
        sidebarLeft.remove();
        printTemplateButton.remove();
        savePdfButton.remove();
        pdfSavedWithSuccessMessage.remove();
        pdfSavingErrorMessage.remove();
        pdfSavingLoader.remove();

        printWindow.print();
        printWindow.close();
    }
};

const ballotPaperVM = new BallotPaperVM();

ballotPaperVM.getHeaderParameters();
ballotPaperVM.getReportParameterValues();

ko.applyBindings(ballotPaperVM);