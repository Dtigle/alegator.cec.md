using System.ComponentModel.DataAnnotations;
using CEC.SRV.Domain.Interop;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Interop
{
    public class TransactionGridModel : JqGridSoft
    {
        
        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [Display(Name = "Transaction_System", ResourceType = typeof(MUI))]
        [JqGridColumnSearchable(true, "SelectInteropSystems", "Interop", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.EqualOrNotEqual)]
        [SearchData(DbName = "InteropSystem.Id", Type = typeof(long?))]
        public string InteropSystem { get; set; }

        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [Display(Name = "Transaction_Institution", ResourceType = typeof(MUI))]
        [JqGridColumnSearchable(true, "SelectInstitutions", "Interop", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.EqualOrNotEqual)]
        [SearchData(DbName = "Institution.Id", Type = typeof(long?))]
        public string Institution { get; set; }

        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Transaction_IDNP", ResourceType = typeof(MUI))]
        public string Idnp { get; set; }

        
        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Transaction_LastName", ResourceType = typeof(MUI))]
        public string LastName { get; set; }

        
        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Transaction_FirstName", ResourceType = typeof(MUI))]
        public string FirstName { get; set; }

         
        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Transaction_Patronymic", ResourceType = typeof(MUI))]
        public string Patronymic { get; set; }


        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Transaction_Comments", ResourceType = typeof(MUI))]
        public string Comments { get; set; }


        [Display(Name = "Transaction_Status", ResourceType = typeof(MUI))]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, typeof(InteropHelper), "GetTransactionStatuses", SearchType = JqGridColumnSearchTypes.Select, SearchOperators = JqGridSearchOperators.EqualOrNotEqual)]
        [SearchData(DbName = "TransactionStatus", Type = typeof(TransactionStatus))]
        public string Status { get; set; }

        [JqGridColumnEditable(true)]
        [JqGridColumnSortable(true)]
        [JqGridColumnSearchable(true, SearchOperators = JqGridSearchOperators.TextOperators)]
        [Display(Name = "Transaction_StatusMessage", ResourceType = typeof(MUI))]
        public string StatusMessage { get; set; }
    }
}