using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.History
{
    public class HistoryGridRow : JqGridRow
    {
        [Display(Name = "Lookups_DataCreated", ResourceType = typeof(MUI))]
        public string DataCreated { get; set; }

        [Display(Name = "Lookups_DataModified", ResourceType = typeof(MUI))]
        public string DataModified { get; set; }

        [Display(Name = "Lookups_DataDeleted", ResourceType = typeof(MUI))]
        public string DataDeleted { get; set; }

        [Display(Name = "Lookups_CreatedById", ResourceType = typeof(MUI))]
        public string CreatedById { get; set; }

        [Display(Name = "Lookups_ModifiedById", ResourceType = typeof(MUI))]
        public string ModifiedById { get; set; }

        [Display(Name = "Lookups_DeletedById", ResourceType = typeof(MUI))]
        public string DeletedById { get; set; }
    }
}