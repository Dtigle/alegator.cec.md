using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Dto.TemplateManager
{
    public class ReportParamValueResultsDto
    {
        public long ParameterValueId { get; set; }
        public long? DocumentId { get; set; }
        public long ReportParameterId { get; set; }
        public long? ElectionCompetitorMemberId { get; set; }
        public long? ElectionCompetitorId { get; set; }
        public string ReportParameterName { get; set; }
        public string ValueContent { get; set; }
        public virtual string ElectionCompetitorName { get; set; }
        public virtual string ElectionCompetitorMemberName { get; set; }
        public virtual int? Order { get; set; }
        public virtual long? BallotCount { get; set; }
    }
}
