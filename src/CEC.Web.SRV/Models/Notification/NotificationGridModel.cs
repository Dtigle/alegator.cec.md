using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;


namespace CEC.Web.SRV.Models.Notification
{
    public class NotificationGridModel : JqGridSoft
    {
		[Display(Name = "Notification_Type", ResourceType = typeof(MUI))]
		[JqGridColumnSortable(true)]
		[JqGridColumnEditable(true, "SelectNotificationType", "Notification", EditType = JqGridColumnEditTypes.Select)]
		[JqGridColumnSearchable(true, "SelectNotificationType", "Notification", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq)]
		[SearchData(DbName = "Notification.Event.EntityType", Type = typeof(string))]
        [JqGridColumnLayout(Width = 110)]
        public string NotificationType { get; set; }

		[Display(Name = "Notification_UserName", ResourceType = typeof(MUI))]
		[JqGridColumnSortable(true)]
		[JqGridColumnEditable(true)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
		[SearchData(DbName = "User.UserName", Type = typeof(string))]
        [JqGridColumnLayout(Width = 110)]
        public string UserName { get; set; }

		[JqGridColumnFormatter("$.messageFormatter")]
		[Display(Name = "Notification_Message", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true,  SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
		[SearchData(DbName = "Notification.Message", Type = typeof(string))]
		public string Message { get; set; }

		[Display(Name = "Notification_CreateDate", ResourceType = typeof(MUI))]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
		[SearchData(DbName = "Notification.CreateDate", Type = typeof(DateTime))]
        [JqGridColumnLayout(Width = 80)]
		public string CreateDate { get; set; }

		[Display(Name = "Notification_IsRead", ResourceType = typeof(MUI))]
		[JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center, Width = 100)]
		[JqGridColumnEditable(true, EditType = JqGridColumnEditTypes.CheckBox, Value = "true:false")]
		[JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
        public bool NotificationIsRead { get; set; }
    }
}