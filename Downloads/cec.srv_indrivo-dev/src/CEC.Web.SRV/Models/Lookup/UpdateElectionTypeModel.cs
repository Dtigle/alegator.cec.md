using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Resources;
using System.ComponentModel.DataAnnotations;

namespace CEC.Web.SRV.Models.Lookup
{
    public class UpdateElectionTypeModel
    {
        public long? Id { get; set; }

        [Display(Name = "ElectionTypes_Code", ResourceType = typeof(MUI))]
        [Required(ErrorMessageResourceName = "ElectionTypes_Code_Required", ErrorMessageResourceType = typeof(MUI))]
        public int Code { get; set; }

        [Display(Name = "ElectionTypes_Name", ResourceType = typeof(MUI))]
        [Required(ErrorMessageResourceName = "ElectionTypes_Name_Required", ErrorMessageResourceType = typeof(MUI))]
        [StringLength(50, ErrorMessageResourceName = "ElectionTypes_Name_MaxLength", ErrorMessageResourceType = typeof(MUI))]
        public string Name { get; set; }

        [Display(Name = "ElectionTypes_Description", ResourceType = typeof(MUI))]
        [Required(ErrorMessageResourceName = "ElectionTypes_Description_Required", ErrorMessageResourceType = typeof(MUI))]
        [StringLength(255, ErrorMessageResourceName = "ElectionTypes_Description_MaxLength", ErrorMessageResourceType = typeof(MUI))]
        [UIHint("MultilineText")]
        public string Description { get; set; }

        /*participanti eligibili*/
        [Display(Name = "ElectionTypes_ElectionCompetitorType", ResourceType = typeof(MUI))]
        [Required(ErrorMessageResourceName = "ElectionTypes_ElectionCompetitorType_Required", ErrorMessageResourceType = typeof(MUI))]
        [UIHint("SelectList")]
        public ElectionCompetitorType ElectionCompetitorType { get; set; }

        [Display(Name = "ElectionTypes_ElectionRoundsNumber", ResourceType = typeof(MUI))]
        [Required(ErrorMessageResourceName = "ElectionTypes_ElectionRoundsNo_Required", ErrorMessageResourceType = typeof(MUI))]
        [Range(0,255, ErrorMessageResourceName =  "ElectionTypes_ElectionRoundsNo_Limit", ErrorMessageResourceType = typeof(MUI))]
        [UIHint("SelectList")]
        public int ElectionRoundsNo { get; set; }

        [Display(Name = "ElectionTypes_ElectionArea", ResourceType = typeof(MUI))]
        [Required(ErrorMessageResourceName = "ElectionTypes_ElectionArea_Required", ErrorMessageResourceType = typeof(MUI))]
        [UIHint("SelectList")]
        public ElectionArea ElectionArea{ get; set; }

        [Display(Name = "ElectionTypes_AcceptResidenceDoc", ResourceType = typeof(MUI))]
        public bool AcceptResidenceDoc { get; set; }

        [Display(Name = "ElectionTypes_AcceptVotingCert", ResourceType = typeof(MUI))]
        public bool AcceptVotingCert { get; set; }

        [Display(Name = "ElectionTypes_AcceptAbroadDeclaration", ResourceType = typeof(MUI))]
        public bool AcceptAbroadDeclaration { get; set; }

        [Display(Name = "ElectionTypes_CircumscriptionList", ResourceType = typeof(MUI))]
        [Required(ErrorMessageResourceName = "ElectionTypes_CircumscriptionList_Required", ErrorMessageResourceType = typeof(MUI))]
        [UIHint("SelectList")]
        public long CircumscriptionListId { get; set; }
    }
}