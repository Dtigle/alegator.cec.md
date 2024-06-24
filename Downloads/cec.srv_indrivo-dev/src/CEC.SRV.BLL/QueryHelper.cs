using NHibernate;

namespace CEC.SRV.BLL
{
    public static class QueryHelper
    {
        public static QueryOverUdfBuilder<TRoot, TSubType> WithUdf<TRoot, TSubType>(this IQueryOver<TRoot, TSubType> query, AbstractUdfCriterion udfCriterion)
        {
            return new QueryOverUdfBuilder<TRoot, TSubType>(query, udfCriterion);
        }
    }
}
