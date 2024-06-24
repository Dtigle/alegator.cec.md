using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CEC.SRV.Domain.Interop;
using CEC.Web.SRV.Controllers;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Models.Lookup;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Interop
{
    public class InteropSystemGridModel : JqGridSoft
    {

        [Display(Name = "Lookups_Name", ResourceType = typeof(MUI))]
        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string Name { get; set; }

        [Display(Name = "Lookups_Description", ResourceType = typeof(MUI))]
        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        public string Description { get; set; }


        [Display(Name = "InteropSystem_TransactionProcessingMode", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(true)]
        [JqGridColumnSearchable(true, typeof(InteropHelper), "GetTransactionProcessingTypes", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.EqualOrNotEqual)]
        [SearchData(DbName = "TransactionProcessingType", Type = typeof(TransactionProcessingTypes))]
        public string TransactionProcessingType { get; set; }



        [Display(Name = "InteropSystem_PersonStatusConsignment", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        [JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
        public bool PersonStatusConsignment { get; set; }

        [Display(Name = "InteropSystem_PersonStatusType", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(true)]
        //[JqGridColumnSearchable(true, typeof(InteropHelper), "GetTransactionProcessingTypes", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.EqualOrNotEqual)]
        [SearchData(DbName = "PersonStatusType", Type = typeof(TransactionProcessingTypes))]
        public string PersonStatusType { get; set; }

        [Display(Name = "InteropSystem_TemporaryAddressConsignment", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnEditable(false)]
        [JqGridColumnFormatter(JqGridColumnPredefinedFormatters.CheckBox)]
        [JqGridColumnLayout(Alignment = JqGridAlignments.Center)]
        [JqGridColumnSearchable(true, typeof(SearchableColumnsHelper), "GetBooleanSelect", SearchType = JqGridColumnSearchTypes.Select)]
        public bool TemporaryAddressConsignment { get; set; }
    }
}