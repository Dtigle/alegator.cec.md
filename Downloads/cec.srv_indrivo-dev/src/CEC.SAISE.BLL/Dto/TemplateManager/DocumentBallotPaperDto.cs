using CEC.SAISE.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Dto.TemplateManager
{
    public class DocumentBallotPaperDto
    {
        public DocumentBallotPaperDto()
        {
            ReportParameterValues = new List<ReportParamValueResultsDto>();
        }
        public List<ReportParamValueResultsDto> ReportParameterValues { get; set; }
        public long DocumentId { get; set; }
        public long? AssignedCircumscriptionId { get; set; }
        public long TemplateId { get; set; }
        public long? PollingStationId { get; set; }
        public long ElectionRoundId { get; set; }
        public int Status { get; set; }
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
    }
}
