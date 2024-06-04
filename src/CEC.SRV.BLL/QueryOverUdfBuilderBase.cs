using System;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Impl;

namespace CEC.SRV.BLL
{
    public class QueryOverUdfBuilderBase<TReturn, TRoot, TSubType> where TReturn : IQueryOver<TRoot, TSubType>
    {
        private readonly TReturn _root;
        private readonly AbstractUdfCriterion _udfCriterion;

        public QueryOverUdfBuilderBase(TReturn root, AbstractUdfCriterion udfCriterion)
        {
            _root = root;
            _udfCriterion = udfCriterion;
        }

        public TReturn HasPropertyIn(Expression<Func<TSubType, object>> expression)
        {
            var projection = ExpressionProcessor.FindMemberProjection(expression.Body);
            var criterion = projection.Create<ICriterion>(s => _udfCriterion.WithProperty(s),p => _udfCriterion.WithProjection(p));

            _root.And(criterion);

            return _root;
        }
    }
}