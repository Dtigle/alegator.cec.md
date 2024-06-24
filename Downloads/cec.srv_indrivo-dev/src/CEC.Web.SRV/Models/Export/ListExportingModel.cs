using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Models.Voters;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Export
{
	public class ListExportingModel
    {
		public long PrintSessionId { get; set; }

		[Display(Name = "ElectionInfo", ResourceType = typeof(MUI))]
		public ElectionModel ElectionInfo { get; set; }

        [Display(Name = "ElectionRound", ResourceType = typeof(MUI))]
        [Required(ErrorMessageResourceName = "ListExportingModel_FieldRequired", ErrorMessageResourceType = typeof(MUI))]
        [Select2RemoteConfig("", "GetElectionRounds", "Voters", "json", "electionRoundDataRequest", "electionRoundResults", PageLimit = 20)]
        [UIHint("Select2")]
        public long ElectionRoundId { get; set; }


        [Display(Name = "ListExportingModel_Circumscription", ResourceType = typeof(MUI))]
		[Required(ErrorMessageResourceName = "ListExportingModel_FieldRequired", ErrorMessageResourceType = typeof(MUI))]
		[Select2RemoteConfig("", "GetCircumscriptionsFromAdmin", "Reporting", "json", "circumscriptionsDataRequest", "circumscriptionsResults", PageLimit = 30)]
		[UIHint("Select2Multiple")]
		public long CircumscriptionId { get; set; }

		[Display(Name = "ListPrintingModel_PollingStation", ResourceType = typeof(MUI))]
		[Select2RemoteConfig("", "GetPollingStationbyCircumscriptions", "Reporting", "json", "pollingStationDataRequest", "pollingStationResults", PageLimit = 30)]
		[UIHint("Select2Multiple")]
		public long PollingStationId { get; set; }
		
		public bool IsProgress { get; set; }

		[Display(Name = "ListExportingModel_StartDate", ResourceType = typeof(MUI))]
		public string StartDate { get; set; }

		[Display(Name = "ListExportingModel_TotalPollingStationInPending", ResourceType = typeof(MUI))]
		public int? TotalPollingStationInPending { get; set; }

		[Display(Name = "ListExportingModel_TotalPollingStationForExporting", ResourceType = typeof(MUI))]
		public int? TotalPollingStationForExporting { get; set; }

		[Display(Name = "ListExportingModel_PollingStationFinished", ResourceType = typeof(MUI))]
		public int? PollingStationFinished { get; set; }

    }
}