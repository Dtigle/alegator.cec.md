using System.ComponentModel.DataAnnotations;

namespace CEC.Web.SRV.Models.Grid
{
    public abstract class JqGridRow
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }
    }
}