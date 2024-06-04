using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Lookup
{
    public class UpdatePublicAdministrationModel
    {
        public long Id { get; set; }

        [Required(ErrorMessageResourceName = "PublicAdministrationErrorRequired_Name", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "PublicAdministrationName", ResourceType = typeof(MUI))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "PublicAdministrationErrorRequired_Surname", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "PublicAdministrationSurname", ResourceType = typeof(MUI))]
        public string Surname { get; set; }

        public long RegionId { get; set; }

        [Required(ErrorMessageResourceName = "PublicAdministrationErrorRequired_ManagerTypes", ErrorMessageResourceType = typeof(MUI))]
        [UIHint("SelectList")]
        [Display(Name= "ManagerTypes", ResourceType = typeof(MUI))]
        public long ManagerTypeId { get; set; }

    }
}