using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Lookup
{
	public class UpdateDocumentTypesModel : UpdateLookupModel
	{
		[Display(Name = "DocumentTypeLookup_IsPrimary", ResourceType = typeof(MUI))]
		[UIHint("Checkbox")]
		public bool IsPrimary { get; set; }
    }
}