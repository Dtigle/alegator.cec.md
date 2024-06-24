using System.Collections.Generic;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace CEC.Web.SRV.Infrastructure.Grids
{
    public interface IJqGridColumnCustomVisibilityRules
    {
        void Apply(List<JqGridColumnModel> colModels);
    }
}