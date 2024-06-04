using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Account
{
    public class AllocateRegionModel
    {
        public AllocateRegionModel()
        {
            AllocatedRegions = new List<SelectListItem>();
        }

        public string UserId { get; set; }

        [UIHint("SelectMulti")]
        [HiddenInput(DisplayValue = false)]
        [Display(Name = "Users_AllocatedRegions", ResourceType = typeof(MUI))]
        public List<SelectListItem> AllocatedRegions { get; set; }
    }
}