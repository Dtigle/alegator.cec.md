using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain.TemplateManager
{
    public class Document : SaiseBaseEntity
    {
        public virtual IList<ReportParameterValue> ReportParameterValues { get; set; }
        public Document()
        {
            ReportParameterValues = new List<ReportParameterValue>();
        }

        public virtual int StatusId { get; set; }

        public virtual int EntryLevelId { get; set; }
        public virtual bool IsResultsConfirmed { get; set; }

        public virtual long? ConfirmationUserId { get; set; }

        public virtual DateTime? ConfirmationDate { get; set; }

        public virtual string DocumentName { get; set; }
        public virtual string DocumentPath { get; set; }
        public virtual string FileSize { get; set; }
        public virtual int? FileLength { get; set; }
        public virtual byte[] FileContent { get; set; }
        public virtual string FileExtension { get; set; }

        // Navigation properties
        public virtual Template Template { get; set; }
        public virtual BallotPaper BallotPaper { get; set; }
        public virtual PollingStation PollingStation { get; set; }
        public virtual ElectionRound ElectionRound { get; set; }
        public virtual AssignedCircumscription AssignedCircumscription { get; set; }
    }
}
