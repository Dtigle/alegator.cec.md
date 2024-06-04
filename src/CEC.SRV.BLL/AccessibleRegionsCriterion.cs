using NHibernate;
using NHibernate.Engine;

namespace CEC.SRV.BLL
{
    public class AccessibleRegionsCriterion : AbstractUdfCriterion
    {
        private const string FunctionName = "[SRV].[fn_GetAccessibleRegionsForUser]";
        private readonly string _userId;
        
        public AccessibleRegionsCriterion(string userId) : base(FunctionName)
        {
            _userId = userId;
        }

        protected override TypedValue[] GetTypedValues()
        {
            return new[]
            {
                new TypedValue(NHibernateUtil.String, _userId, EntityMode.Poco)
            };
        }
    }
}