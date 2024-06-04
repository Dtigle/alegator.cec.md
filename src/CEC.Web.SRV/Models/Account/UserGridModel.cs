using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.Web.SRV.Models.Account
{
    public class UserGridModel : JqGridRow
    {
        [JqGridColumnEditable(false)]
		[Display(Name = "Users_Login_Name", ResourceType = typeof(MUI))]
        [JqGridColumnFormatter("$.userProfileFormatter")]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [SearchData(DbName = "UserName")]
        public string Login { get; set; }
        
		[Display(Name = "Users_Nume", ResourceType = typeof(MUI))]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [SearchData(DbName = "AdditionalInfo.FullName")]
        public string FullName { get; set; }
        
		[Display(Name = "Users_Rol", ResourceType = typeof(MUI))]
        [JqGridColumnSearchable(true, "SelectRoles", "Account", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq)]
        [SearchData(DbName = "Roles.Id")]
        public string Role { get; set; }

		[Display(Name = "Users_Statut", ResourceType = typeof(MUI))]
        [JqGridColumnSearchable(true, "AccountStatusesForSearch", "Account", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq)]
        [SearchData(DbName = "IsBlocked", Type = typeof(bool))]
        public string Status { get; set; }

        [JqGridColumnFormatter("$.regionsFormatter")]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Users_Regions", ResourceType = typeof(MUI))]
        [SearchData(DbName = "Regions.Name", Type = typeof(string))]
        public string Regions { get; set; }

		[Display(Name = "Users_Login", ResourceType = typeof(MUI))]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Eq | JqGridSearchOperators.Ne | JqGridSearchOperators.Le | JqGridSearchOperators.Ge)]
        //[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.JQueryUIDatepicker, SearchOperators = JqGridSearchOperators.Le, DatepickerDateFormat = "dd.mm.yy")]
        [SearchData(Type = typeof(DateTimeOffset?))]
        public string LastLogin { get; set; }

        [Display(Name = "Users_Comentarii", ResourceType = typeof(MUI))]
        [JqGridColumnEditable(true, EditType = JqGridColumnEditTypes.TextArea)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string Comments { get; set; }
        
        [Display(Name = "Users_IsOnline", ResourceType = typeof(MUI))]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
        public bool IsOnline { get; set; }
    }
}