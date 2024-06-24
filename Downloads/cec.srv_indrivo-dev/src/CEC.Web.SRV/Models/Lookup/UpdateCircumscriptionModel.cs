using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Lookup
{
	public class UpdateCircumscriptionModel
    {
        public long Id { get; set; }

		[Display(Name = "Circumscription_RegionName", ResourceType = typeof(MUI))]
		public string Name { get; set; }

		[RegularExpression(Const.OnlyFiveNumbers, ErrorMessageResourceName = "CircumscriptionEditeErrorNum_Formatter", ErrorMessageResourceType = typeof(MUI))]
		[Display(Name = "Circumscription_Number", ResourceType = typeof(MUI))]
		public int? CircumscriptionNumber { get; set; }
    }
}