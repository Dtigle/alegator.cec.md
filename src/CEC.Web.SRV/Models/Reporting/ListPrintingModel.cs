using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;
using DocumentFormat.OpenXml.Drawing;

namespace CEC.Web.SRV.Models.Reporting
{
    public class ListPrintingModel : ReportingModel
    {
        [Display(Name = "ListPrintingModel_Election", ResourceType = typeof(MUI))]
		[UIHint("Select2")]
		[Select2RemoteConfig("", "GetActiveElections", "Voters", "json", "electionsDataRequest", "electionsResults", "GetElectionsName", "Voters", PageLimit = 10)]
		[Required(ErrorMessageResourceName = "ListPrintingModel_FieldRequired", ErrorMessageResourceType = typeof(MUI))]
        public long ElectionId { get; set; }

        [Display(Name = "ListPrintingModel_PollingStation", ResourceType = typeof(MUI))]
		[UIHint("Select2")]
		[Select2RemoteConfig("", "GetPollingStations", "Voters", "json", "votersDataRequest", "votersResults", "GetPollingStationName", "Voters", PageLimit = 10)]
		[Required(ErrorMessageResourceName = "ListPrintingModel_FieldRequired", ErrorMessageResourceType = typeof(MUI))]
		public long PollingStationId { get; set; }

        public string ElectionTitleRO { get; set; }

        public string ElectionTitleRU { get; set; }

        public string PollingStationNr { get; set; }

        public DateTime? ElectionDate { get; set; }
        
        public string RegionName { get; set; }

        public string ManagerTypeName { get; set; }

        public string ManagerName { get; set; }

        public bool DataReadyForReport 
        {
            get { return ElectionDate.HasValue; }
        }
    }
}