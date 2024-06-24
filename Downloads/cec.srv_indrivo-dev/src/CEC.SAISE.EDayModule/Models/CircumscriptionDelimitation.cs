using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Models
{
    public class CircumscriptionDelimitation
    {
        public long ElectionId { get; set; }

        public long CircumscriptionId { get; set; }

        public long TemplateNameId { get; set; }
    }
}