using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models.File
{
    public class DocumentSaveModel
    {
        public long DocumentId { get; set; }
        public long TemplateNameId { get; set; }
        public long ElectionId { get; set; }
        public long? PollingStationId { get; set; }
        public long? CircumscriptionId { get; set; }

    }
}