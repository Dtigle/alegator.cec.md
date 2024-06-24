using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.Grid;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Voters
{
    public class SearchVotersGridModel : JqGridSoft
    {
        [Display(Name = "Person_Idnp", ResourceType = typeof(MUI))]
        [JqGridColumnLayout(Width = 50)]
        public string Idnp { get; set; }

        [Display(Name = "Person_FullName", ResourceType = typeof(MUI))]
        [JqGridColumnLayout(Width = 100)]
        [SearchData(DbName = "FullName", Type = typeof(string))]
        public string FullName { get; set; }

        [Display(Name = "Person_DataOfBirth", ResourceType = typeof(MUI))]
        [JqGridColumnLayout(Width = 50)]
        public string DataOfBirth { get; set; }

        [Display(Name = "Person_Address", ResourceType = typeof(MUI))]
        public string Address { get; set; }

        [Display(Name = "Person_AddressType", ResourceType = typeof(MUI))]
        [JqGridColumnLayout(Width = 50)]
        public string AddressType { get; set; }
    
        [Display(Name = "Person_DocumentSeries", ResourceType = typeof(MUI))]
        [JqGridColumnLayout(Width = 50)]
        //[SearchData(DbName = "Documents.DocumentNumber", Type = typeof(string))]
        public string Document { get; set; }

        [Display(Name = "Person_Status", ResourceType = typeof(MUI))]
        [JqGridColumnLayout(Width = 100)]
        public string Status { get; set; }
    }
}