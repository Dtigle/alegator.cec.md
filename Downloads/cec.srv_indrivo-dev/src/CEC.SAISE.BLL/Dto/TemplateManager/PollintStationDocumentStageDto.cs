using Amdaris.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Dto.TemplateManager
{
    public class PollintStationDocumentStageDto : IEntity
    {
        public long? Id { get; set; }

        public string Circumscription { get; set; }

        public string Locality { get; set; }

        public string CircumscriptionNumber { get; set; }

        public string PollingStation { get; set; }

        public string TemplateName { get; set; }
        public int DocumentStatusId { get; set; }
        public string DocumentStatus { get; set; }
        public long? BallotPaperId { get; set; }
    }

}
