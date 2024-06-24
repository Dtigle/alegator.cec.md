using CEC.SAISE.Domain;
using CEC.SAISE.Domain.TemplateManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.Document
{
    public class DocumentDataModel
    {
        public List<ReportParameterValueModel> ReportParameterValues { get; set; }
        public DocumentDataModel()
        {
            ReportParameterValues = new List<ReportParameterValueModel>();
        }

        public long DocumentId { get; set; }
        public long TemplateId { get; set; }
        public long? AssignedCIrcumscriptionId { get; set; }
        public long? PollingStationId { get; set; }
        public long ElectionRoundId { get; set; }
        public DocumentStatus Status { get; set; }
        public DelimitationType EntryLevel { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public DateTime FillDate { get; set; }
        public string FileSize { get; set; }
        public int FileLength { get; set; }
        public byte[] FileContent { get; set; }
        public string FileExtension { get; set; }
        public DateTime EditDate { get; set; }

        public string EditUser { get; set; }

        public bool IsResultsConfirmed { get; set; }

        public long? ConfirmationUserId { get; set; }

        public DateTime? ConfirmationDate { get; set; }
        public bool AlreadySent { get; set; }
    }
}