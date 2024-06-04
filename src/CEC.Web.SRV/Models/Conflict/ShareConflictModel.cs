using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CEC.Web.SRV.Resources;


namespace CEC.Web.SRV.Models.Conflict
{
    public class ShareConflictModel
    {
        public ShareConflictModel()
        {
            AllocatedRegions = new List<SelectListItem>();
            OriginalRegions = new List<SelectListItem>();
        }

        public long ConflictId {get; set;}

        public List<SelectListItem> OriginalRegions { get; set; }

        [Required(ErrorMessageResourceName = "ShareConflict_RequiredReason", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "ShareConflict_ReasonLabel", ResourceType = typeof(MUI))]
        [UIHint("SelectList")]
        public long ReasonId { get; set; }

        [Required(ErrorMessageResourceName = "ShareConflict_RequiredNote", ErrorMessageResourceType = typeof(MUI))]
        [Display(Name = "ShareConflict_Notes", ResourceType = typeof(MUI))]
        [DataType(DataType.MultilineText)]
        public string ConflictShareNote { get; set; }


        [UIHint("SelectMulti")]
        [HiddenInput(DisplayValue = false)]
        [Display(Name = "Users_AllocatedRegions", ResourceType = typeof(MUI))]
        public List<SelectListItem> AllocatedRegions { get; set; }
    }
}