using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CEC.Web.SRV.Models.Reporting;

namespace CEC.Web.SRV.Models.Voters
{
    public class StayStatementPrintingModel : ReportingModel
    {
        public long StayStatementId { get; set; }
    }
}