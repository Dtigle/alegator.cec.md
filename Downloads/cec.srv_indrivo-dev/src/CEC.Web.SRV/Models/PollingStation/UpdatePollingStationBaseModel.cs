using System.ComponentModel.DataAnnotations;
using CEC.SRV.Domain;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.PollingStation
{
    public class UpdatePollingStationBaseModel
    {
        public long Id { get; set; }

        public long RegionId { get; set; }
		
		[Display(Name = "Circumscription_Number", ResourceType = typeof(MUI))]
		public int? CircumscriptionNumber { get; set; }

        [Required(ErrorMessageResourceName = "PollingStationAddEditeErrorRequired_Number", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "PollingStationNumber", ResourceType = typeof(MUI))]
		[RegularExpression(Const.NumberPollingStationValidationExpression, ErrorMessageResourceName = "FieldError_FormatterValidation", ErrorMessageResourceType = typeof(MUI))]
        public string Number { get; set; }

        [Required(ErrorMessageResourceName = "PollingStationAddEditeErrorRequired_Location", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "PollingStationLocation", ResourceType = typeof(MUI))]
        [RegularExpression(Const.CharacterValidationExpression, ErrorMessageResourceName = "FieldError_CharacterValidation", ErrorMessageResourceType = typeof(MUI))]
        public string Location { get; set; }
        

        [Display(Name = "PollingStationContactInfo", ResourceType = typeof(MUI))]
        [RegularExpression(Const.CharacterValidationExpression, ErrorMessageResourceName = "FieldError_CharacterValidation", ErrorMessageResourceType = typeof(MUI))]
        public string ContactInfo { get; set; }

        [Display(Name = "PollingStationSaiseId", ResourceType = typeof(MUI))]
        [RegularExpression(Const.OnlyNumbers, ErrorMessageResourceName = "PollingStationAddEditeErrorNum_SaiseId", ErrorMessageResourceType = typeof(MUI))]
        public long? SaiseId { get; set; }

        [Display(Name = "PSType", ResourceType = typeof(MUI))]
        [UIHint("SelectList")]
        public PollingStationTypes PollingStationType { get; set; }

        //[Required(ErrorMessageResourceName = "PollingStationVotersListOrderType_Required", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "PollingStationVotersListOrderType", ResourceType = typeof(MUI))]
        [UIHint("SelectList")]
        public long? VotersListOrderType { get; set; }
    }
}