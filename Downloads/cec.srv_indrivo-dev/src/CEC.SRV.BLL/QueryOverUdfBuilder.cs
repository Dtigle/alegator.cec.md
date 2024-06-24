using NHibernate;

namespace CEC.SRV.BLL
{
    public class QueryOverUdfBuilder<TRoot, TSubType> : QueryOverUdfBuilderBase<IQueryOver<TRoot, TSubType>, TRoot, TSubType>
    {
        public QueryOverUdfBuilder(IQueryOver<TRoot, TSubType> root, AbstractUdfCriterion udfCriterion) : base(root, udfCriterion)
        {
        }
    }
}