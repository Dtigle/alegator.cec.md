using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Models.Voters;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Export
{
	public class ExportRsaToSaiseModel
    {
		public long SaiseExporterId { get; set; }

		[Display(Name = "ElectionInfo", ResourceType = typeof(MUI))]
		public ElectionModel ElectionInfo { get; set; }

		[Display(Name = "ExportRsaToSaiseModel_ExportAll", ResourceType = typeof(MUI))]
		[UIHint("Checkbox")]
        public bool? ExportAll { get; set; }

		public bool IsProgress { get; set; }
	}
}