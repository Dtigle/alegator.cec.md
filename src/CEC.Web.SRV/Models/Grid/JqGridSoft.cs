using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Grid
{
    public abstract class JqGridSoft: JqGridRow
    {
		[Display(Name = "Lookups_DataCreated", ResourceType = typeof(MUI))]
        [HiddenInput(DisplayValue = false)]
        [JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Lt | JqGridSearchOperators.Ge | JqGridSearchOperators.NullOperators)]
        [SearchData(DbName = "Created", Type = typeof(DateTimeOffset?))]
		public string DataCreated { get; set; }

		[Display(Name = "Lookups_DataModified", ResourceType = typeof(MUI))]
        [HiddenInput(DisplayValue = false)]
        [JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Lt | JqGridSearchOperators.Ge | JqGridSearchOperators.NullOperators)]
        [SearchData(DbName = "Modified", Type = typeof(DateTimeOffset?))]
        public string DataModified { get; set; }

		[Display(Name = "Lookups_DataDeleted", ResourceType = typeof(MUI))]
        [HiddenInput(DisplayValue = false)]
        [JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Lt | JqGridSearchOperators.Ge | JqGridSearchOperators.NullOperators)]
        [SearchData(DbName = "Deleted", Type = typeof(DateTimeOffset?))]
        public string DataDeleted { get; set; }

		[Display(Name = "Lookups_CreatedById", ResourceType = typeof(MUI))]
        [HiddenInput(DisplayValue = false)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [SearchData(DbName = "CreatedBy.UserName", Type = typeof(string))]
        public string CreatedById { get; set; }

		[Display(Name = "Lookups_ModifiedById", ResourceType = typeof(MUI))]
        [HiddenInput(DisplayValue = false)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [SearchData(DbName = "ModifiedBy.UserName", Type = typeof(string))]
        public string ModifiedById { get; set; }

		[Display(Name = "Lookups_DeletedById", ResourceType = typeof(MUI))]
        [HiddenInput(DisplayValue = false)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [SearchData(DbName = "DeletedBy.UserName", Type = typeof(string))]
        public string DeletedById { get; set; }
    }
}