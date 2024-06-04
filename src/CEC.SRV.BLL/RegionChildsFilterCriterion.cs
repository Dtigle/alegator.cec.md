using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Engine;

namespace CEC.SRV.BLL
{
    public class RegionChildsFilterCriterion : AbstractUdfCriterion
    {
        private const string FunctionName = "[SRV].[fn_GetChildRegionsOfParent]";
        private readonly long _regionId;

        public RegionChildsFilterCriterion(long regionId)
            : base(FunctionName)
        {
            _regionId = regionId;
        }

        protected override TypedValue[] GetTypedValues()
        {
            return new[]
            {
                new TypedValue(NHibernateUtil.Int64, _regionId, EntityMode.Poco)
            };
        }
    }
}
