using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace CEC.SRV.BLL
{
    public abstract class AbstractUdfCriterion : AbstractCriterion
    {
        private readonly string _functionName;
        private IProjection _projection;

        private string _propertyName;

        protected AbstractUdfCriterion(string functionName)
        {
            _functionName = functionName;
        }


        public AbstractUdfCriterion WithProjection(IProjection projection)
        {
            _projection = projection;
            return this;
        }

        public AbstractUdfCriterion WithProperty(string property)
        {
            _propertyName = property;
            return this;
        }

        public override string ToString()
        {
            string @params = string.Join(", ", GetTypedValues().Select(x => ReferenceEquals(x.Type, NHibernateUtil.String) ? "'" + x.Value + "'" : x.Value));

            return string.Format("{0} in ( SELECT * FROM [SRV].[fn_GetAccessibleRegionsForUser]({1}) )", _propertyName,
                @params);
        }

        public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
        {
            var sqlBuilder = new SqlStringBuilder();

            SqlString[] columnNames = CriterionUtil.GetColumnNames(_propertyName, _projection, criteriaQuery, criteria,
                enabledFilters);

            if (columnNames.Length != 1)
                throw new HibernateException("UdfCriterion may only be used with single-column properties / projections.");


            sqlBuilder
                .Add(columnNames[0])
                .Add(string.Format(" IN ( SELECT * FROM {0}(", _functionName));

            var _typedValues = GetTypedValues();

            foreach (var typedValue in _typedValues)
            {
                sqlBuilder.Add(criteriaQuery.NewQueryParameter(typedValue).Single());
            }

            sqlBuilder.Add("))");

            return sqlBuilder.ToSqlString();
        }

        public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
        {
            return GetTypedValues();
        }

        public override IProjection[] GetProjections()
        {
            if (_projection != null)
            {
                return new[] { _projection };
            }

            return null;
        }


        protected abstract TypedValue[] GetTypedValues();
    }
}