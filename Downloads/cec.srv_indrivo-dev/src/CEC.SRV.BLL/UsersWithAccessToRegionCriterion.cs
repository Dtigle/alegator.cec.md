using NHibernate;
using NHibernate.Engine;

namespace CEC.SRV.BLL
{
    public class UsersWithAccessToRegionCriterion : AbstractUdfCriterion
    {
        private const string FunctionName = "[SRV].[fn_GetUsersWithAccessToRegion]";

        private readonly long _regionId;
        
        public UsersWithAccessToRegionCriterion(long regionId) : base(FunctionName)
        {
            _regionId = regionId;
        }

        protected override TypedValue[] GetTypedValues()
        {
            return new[] {new TypedValue(NHibernateUtil.Int64, _regionId, EntityMode.Poco)};
        }
    }
}