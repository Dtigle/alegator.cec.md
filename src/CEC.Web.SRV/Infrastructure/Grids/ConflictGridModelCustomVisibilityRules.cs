using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Infrastructure;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using CEC.Web.SRV.Models.Conflict;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace CEC.Web.SRV.Infrastructure.Grids
{
    public class ConflictGridModelCustomVisibilityRules : IJqGridColumnCustomVisibilityRules
    {
        public void Apply(List<JqGridColumnModel> colModels)
        {
            var streetCodeColumn = colModels.First(x => x.Name == Utils.GetPropName<ConflictGridModel>(y => y.Streetcode));
            var regionCodeColumn = colModels.First(x => x.Name == Utils.GetPropName<ConflictGridModel>(y => y.AdministrativeCode));

            streetCodeColumn.Hidden = SecurityHelper.LoggedUserIsInRole(Transactions.Registrator);
            regionCodeColumn.Hidden = SecurityHelper.LoggedUserIsInRole(Transactions.Registrator);
        }
    }
}