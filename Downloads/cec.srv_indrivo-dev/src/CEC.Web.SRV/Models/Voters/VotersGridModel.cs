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
using NHibernate.Type;

namespace CEC.Web.SRV.Models.Voters
{
    public class VotersGridModel : JqGridSoft
    {
        [Display(Name = "Person_Idnp", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.Eq | JqGridSearchOperators.Cn | JqGridSearchOperators.Bw | JqGridSearchOperators.Ew | JqGridSearchOperators.Nu)]
        [JqGridColumnEditable(true)]
        [Required]
        public string Idnp { get; set; }

        [Display(Name = "Person_Surname", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Required]
        public string Surname { get; set; }

        [Display(Name = "Person_FirstName", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Person_MiddleName", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Required]
        public string MiddleName { get; set; }

        [Display(Name = "Person_DataOfBirth", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [SearchData(DbName = "BirthDate", Type = typeof(DateTime))]
        [JqGridColumnEditable(false)]
        public string DataOfBirth { get; set; }

        [Display(Name = "Person_Address", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string Address { get; set; }

        [JqGridColumnLayout(Alignment = JqGridAlignments.Right)]
        [Display(Name = "Person_ApNumber", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [SearchData(DbName = "ApNumber")]
        public virtual int ApNumber { get; set; }

        [Display(Name = "Person_ApSuffix", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [SearchData(DbName = "ApSuffix")]
        public virtual string ApSuffix { get; set; }

        [JqGridColumnLayout(Width = 120)]
        [Display(Name = "Person_DocumentNumber", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [SearchData(DbName = "DocumentNumber")]
        public string DocumentNumber { get; set; }



        [Display(Name = "Person_Status", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, "SelectVoterStatus", "Voters", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.EqualOrNotEqual, DefaultValue = "1")]
        [SearchData(DbName = "StatusId", Type = typeof(long?))]
        public string Status { get; set; }

        [Display(Name = "Person_AddressType", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, "SelectAddressType", "Voters", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq)]
        [SearchData(DbName = "AddressTypeId", Type = typeof(long?))]
        public string AddressType { get; set; }

        [Display(Name = "Person_Gender", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, "SelectVoterGender", "Voters", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.Eq)]
        [HiddenInput(DisplayValue = false)]
        //[SearchData(DbName = "Gender.Id", Type = typeof(long?))]
        public string Gender { get; set; }

        [Display(Name = "Person_Age", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [SearchData(DbName = "Age")]
        //[HiddenInput(DisplayValue = false)]
        public int Age { get; set; }

        [Display(Name = "Person_Comments", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.TextOperators)]
        [HiddenInput(DisplayValue = false)]
        public string Comments { get; set; }

        [Display(Name = "Region_HasStreets", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        [JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
        [HiddenInput(DisplayValue = false)]
        public bool RegionHasStreets { get; set; }

        [Display(Name = "PersonAddress_DateOfExpiration", ResourceType = typeof(MUI))]
        [HiddenInput(DisplayValue = false)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.Lt | JqGridSearchOperators.Ge | JqGridSearchOperators.NullOperators)]
        [SearchData(DbName = "AddressExpirationDate", Type = typeof(DateTime?))]
        public string AddressExpirationDate { get; set; }

        [JqGridColumnLayout(Alignment = JqGridAlignments.Right)]
        [Display(Name = "Election_List_Nr", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnSearchable(true, SearchType = JqGridColumnSearchTypes.Text, SearchOperators = JqGridSearchOperators.NoTextOperators)]
        [SearchData(DbName = "electionListNr")]
        public virtual long? electionListNr { get; set; }
    }
}