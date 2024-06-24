using System;
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Voters
{
	public class StayStatementsGridModel : JqGridSoft
	{
		[Display(Name = "StayStatement_Id", ResourceType = typeof(MUI))]
		[JqGridColumnEditable(false)]
		[JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
		[SearchData(DbName = "Id", Type = typeof(long?))]
		public long StayStatementId { get; set; }

		[Display(Name = "StayStatement_Idnp", ResourceType = typeof(MUI))]
		[JqGridColumnEditable(true)]
		[JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
		[SearchData(DbName = "Person.Idnp", Type = typeof(string))]
		public string StayStatementIdnp { get; set; }

		[Display(Name = "StayStatement_PersonName", ResourceType = typeof(MUI))]
		[JqGridColumnEditable(true)]
		[JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
		[SearchData(DbName = "Person.FullName", Type = typeof(string))]
		public string PersonName { get; set; }

		[Display(Name = "StayStatement_PersonDateOfBirth", ResourceType = typeof(MUI))]
		[JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.NoTextOperators)]
		[SearchData(DbName = "Person.DateOfBirth", Type = typeof(DateTime))]
		public string PersonDateOfBirth { get; set; }

		[Display(Name = "AddressOfResidence", ResourceType = typeof(MUI))]
		[JqGridColumnEditable(true)]
		[JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
		public string BaseAddressInfo { get; set; }

		[Display(Name = "DeclaredAddress", ResourceType = typeof(MUI))]
		[JqGridColumnEditable(true)]
		[JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
		public string DeclaredStayAddressInfo { get; set; }

		[Display(Name = "ElectionInfo", ResourceType = typeof(MUI))]
		[JqGridColumnSortable(true)]
		[JqGridColumnEditable(true, "SelectElectionInstance", "Voters", EditType = JqGridColumnEditTypes.Select)]
		[JqGridColumnSearchable(true, "SelectElectionInstance", "Voters", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq)]
		[SearchData(DbName = "ElectionInstance.Id", Type = typeof(long?))]
		public string ElectionInfo { get; set; }
	}
}