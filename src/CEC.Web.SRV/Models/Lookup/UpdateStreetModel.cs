using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Lookup
{
    public class UpdateStreetModel
    {
        public long Id { get; set; }

        [Required(ErrorMessageResourceName = "StreetAddEditeErrorRequired_Name", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name= "StreetName", ResourceType = typeof(MUI))]
        [RegularExpression(Const.CharacterValidationExpression, ErrorMessageResourceName = "FieldError_CharacterValidation", ErrorMessageResourceType = typeof(MUI))]
        public string Name { get; set; }

        [Display(Name = "StreetDescription", ResourceType = typeof(MUI))]
        [RegularExpression(Const.CharacterValidationExpression, ErrorMessageResourceName = "FieldError_CharacterValidation", ErrorMessageResourceType = typeof(MUI))]
        public string Description { get; set; }

        public long RegionId { get; set; }

        [Required(ErrorMessageResourceName = "StreetAddEditeErrorRequired_StreetTypes", ErrorMessageResourceType = typeof(MUI))]
        [UIHint("SelectList")]
        [Display(Name= "StreetTypes", ResourceType = typeof(MUI))]
        public long StreetTypeId { get; set; }

        [RegularExpression(Const.OnlyNumbers, ErrorMessageResourceName = "StreetAddEditeErrorNum_RopSaise", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name= "StreetRopId", ResourceType = typeof(MUI))]
        public long? RopId { get; set; }

        [RegularExpression(Const.OnlyNumbers, ErrorMessageResourceName = "StreetAddEditeErrorNum_RopSaise", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "StreetSaiseId", ResourceType = typeof(MUI))]
        public long? SaiseId { get; set; }
    }
}