using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CEC.SRV.Domain.Importer;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;


namespace CEC.Web.SRV.Models.Conflict
{
	public class ConflictGridModel : JqGridRow
    {
        [Display(Name = "Conflict_Idnp", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
//        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.Eq)]
        [JqGridColumnEditable(false)]
        public string Idnp { get; set; }

        [Display(Name = "Conflict_Surname", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
//        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.Cn)]
        public string LastName { get; set; }

        [Display(Name = "Conflict_FirstName", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
//        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.Cn)]
        public string FirstName { get; set; }

        [Display(Name = "Conflict_MiddleName", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
//        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.Cn)]
        public string SecondName { get; set; }

        [Display(Name = "Conflict_DataOfBirth", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [SearchData(DbName = "Birthdate", Type = typeof(DateTime))]
        [JqGridColumnEditable(false)]
        public string DateOfBirth { get; set; }

        [Display(Name = "Conflict_Gender", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
//        [HiddenInput(DisplayValue = false)]
        [SearchData(DbName = "SexCode", Type = typeof(string))]
        [JqGridColumnSearchable(true, "SelectConflictGender", "Conflict", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.EqualOrNotEqual)]
        //[JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetSexCodeSelect", SearchType = JqGridColumnSearchTypes.Select, )]
        public string Gender { get; set; }

		[Display(Name = "Conflict_Dead", ResourceType = typeof(MUI))]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
		[JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        public bool Dead { get; set; }

        [Display(Name = "Conflict_CitizenRm", ResourceType = typeof(MUI))]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        public bool CitizenRm { get; set; }

        [Display(Name = "Conflict_DocSeria", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string Series { get; set; }

        [Display(Name = "Conflict_DocNumber", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string Number { get; set; }

        [Display(Name = "Conflict_DocType", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [HiddenInput(DisplayValue = false)]
        public string DocType { get; set; }

        [Display(Name = "Conflict_IssueDate", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [SearchData(DbName = "Issuedate", Type = typeof(DateTime))]
        [HiddenInput(DisplayValue = false)]
        public string IssueDate { get; set; }

        [Display(Name = "Conflict_ExpirationDate", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [SearchData(DbName = "Expirationdate", Type = typeof(DateTime))]
        [HiddenInput(DisplayValue = false)]
        public string ExpirationDate { get; set; }

        [Display(Name = "Conflict_Validity", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
        [HiddenInput(DisplayValue = false)]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        public bool Validity { get; set; }

        [Display(Name = "Conflict_Region", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
//		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.Cn)]
        public string Region { get; set; }

        [Display(Name = "Conflict_Locality", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
//		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.Cn)]
        public string Locality { get; set; }

        [Display(Name = "Conflict_AdministrativeCode", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [SearchData(DbName = "AdministrativeCode", Type = typeof(int))]
        public int AdministrativeCode { get; set; }

        [Display(Name = "Conflict_StreetName", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
//        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.Cn)]
        public string StreetName { get; set; }

        [Display(Name = "Conflict_StreetCode", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        public int Streetcode { get; set; }

        [Display(Name = "Conflict_HouseNumber", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        public int HouseNr { get; set; }

        [Display(Name = "Conflict_HouseSuffix", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string HouseSuffix { get; set; }

        [Display(Name = "Conflict_ApartmentNumber", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        public int ApNr { get; set; }

        [Display(Name = "Conflict_ApartmentSuffix", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string ApSuffix { get; set; }

        //[Display(Name = "Conflict_StatusConflict", ResourceType = typeof(MUI))]
        //[JqGridColumnSortable(true)]
        //[JqGridColumnEditable(false)]
        //public string StatusConflictCode { get; set; }

		[Display(Name = "Conflict_Source", ResourceType = typeof(MUI))]
		[HiddenInput(DisplayValue = false)]
		[JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, "SelectSource", "Conflict", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.EqualOrNotEqual)]
        [SearchData(DbName = "Source", Type = typeof(SourceEnum))]
        public string Source { get; set; }

        [Display(Name = "Conflict_Status", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, "SelectStatus", "Conflict", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.EqualOrNotEqual)]
        [SearchData(DbName = "Status", Type = typeof(RawDataStatus))]
        public string Status { get; set; }

        [Display(Name = "Conflict_StatusMessage", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [HiddenInput(DisplayValue = false)]
        public string StatusMessage { get; set; }

		[Display(Name = "Conflict_DataCreated", ResourceType = typeof(MUI))]
		[HiddenInput(DisplayValue = false)]
		[JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Eq | JqGridSearchOperators.Ne | JqGridSearchOperators.Le | JqGridSearchOperators.Ge)]
		[SearchData(DbName = "Created", Type = typeof(DateTimeOffset?))]
		public string DataCreated { get; set; }

		[Display(Name = "Conflict_StatusDate", ResourceType = typeof(MUI))]
		[HiddenInput(DisplayValue = false)]
		[JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Eq | JqGridSearchOperators.Ne | JqGridSearchOperators.Le | JqGridSearchOperators.Ge)]
        [SearchData(DbName = "StatusDate", Type = typeof(DateTimeOffset?))]
		public string StatusDate { get; set; }

		[Display(Name = "Conflict_Comments", ResourceType = typeof(MUI))]
		[HiddenInput(DisplayValue = false)]
		[JqGridColumnEditable(false)]
		[JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
		public string Comments { get; set; }

        [Display(Name = "Conflict_RegionId", ResourceType = typeof(MUI))]
        [HiddenInput(DisplayValue = false)]
        public virtual long RegionId { get; set; }
        
        [HiddenInput(DisplayValue = false)]
        public long RspModificationDataId { get; set; }
    }
}