using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace CEC.SAISE.BLL.Helpers
{
    public class SqlStringBuilder
    {
        private readonly ISession _session;
        private readonly Action<string> _logging;
        readonly StringBuilder _builder = new StringBuilder();
        readonly ParameterBuilder _parameterBuilder;
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStringBuilder" /> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="logging"></param>
        public SqlStringBuilder(ISession session, string sql, Action<string> logging)
        {
            _session = session;
            _logging = logging;
            _builder.Append(sql);
            Parameters = new Dictionary<string, object>();
            ParameterList = new Dictionary<string, IEnumerable>();
            _parameterBuilder = new ParameterBuilder(this);
        }
        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public IDictionary<string, object> Parameters { get; set; }
        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public IDictionary<string, IEnumerable> ParameterList { get; set; }
        /// <summary>
        /// To the query.
        /// </summary>
        /// <returns>IQuery.</returns>
        public ISQLQuery ToSqlQuery()
        {
            string sql = _builder.ToString();
            if (_logging != null)
            {
                _logging(sql);
            }
            var query = _session.CreateSQLQuery(sql);
            foreach (var parameter in Parameters)
            {
                query.SetParameter(parameter.Key, parameter.Value);
            }
            foreach (var parameter in ParameterList)
            {
                query.SetParameterList(parameter.Key, parameter.Value);
            }
            return query;
        }
        /// <summary>
        /// To the query.
        /// </summary>
        /// <returns>IQuery.</returns>
        public IQuery ToQuery()
        {
            string sql = _builder.ToString();
            if (_logging != null)
            {
                _logging(sql);
            }
            var query = _session.CreateQuery(sql);
            foreach (var parameter in Parameters)
            {
                query.SetParameter(parameter.Key, parameter.Value);
            }
            foreach (var parameter in ParameterList)
            {
                query.SetParameterList(parameter.Key, parameter.Value);
            }
            return query;
        }
        /// <summary>
        /// Ifs the specified evaluation.
        /// </summary>
        /// <param name="evaluation">if set to <c>true</c> [evaluation].</param>
        /// <param name="sql">The SQL.</param>
        /// <returns>ParameterBuilder.</returns>
        public ParameterBuilder If(bool evaluation, string sql)
        {
            return If(evaluation, sql, null);
        }
        /// <summary>
        /// Conditionals the specified evaluation.
        /// </summary>
        /// <param name="evaluation">if set to <c>true</c> [evaluation].</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>SqlStringBuilder.</returns>
        public ParameterBuilder If(bool evaluation, string sql, Action<ParameterBuilder> parameters)
        {
            if (evaluation)
            {
                _builder.Append(string.Format(" {0} ", sql));
                if (parameters != null)
                {
                    parameters(_parameterBuilder);
                }
            }
            return _parameterBuilder;
        }
        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>ParameterBuilder.</returns>
        public ParameterBuilder SetParameter<T>(string key, T value)
        {
            _parameterBuilder.SetParameter(key, value);
            return _parameterBuilder;
        }
        /// <summary>
        /// Sets the parameter list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>ParameterBuilder.</returns>
        public ParameterBuilder SetParameterList<T>(string key, T value) where T : IEnumerable
        {
            _parameterBuilder.SetParameterList(key, value);
            return _parameterBuilder;
        }
        /// <summary>
        /// SQLs the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>IT2.DataAccess.SqlStringBuilder.</returns>
        public SqlStringBuilder Sql(string sql)
        {
            _builder.Append(string.Format(" {0} ", sql));
            return this;
        }
    }
    public class ParameterBuilder
    {
        private readonly SqlStringBuilder _builder;
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterBuilder" /> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public ParameterBuilder(SqlStringBuilder builder)
        {
            _builder = builder;
        }
        /// <summary>
        /// Parameters the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>ParameterBuilder.</returns>
        public ParameterBuilder SetParameter<T>(string key, T value)
        {
            _builder.Parameters.Add(key, value);
            return this;
        }
        /// <summary>
        /// Parameters the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>ParameterBuilder.</returns>
        public ParameterBuilder SetParameterList<T>(string key, T value) where T : IEnumerable
        {
            _builder.ParameterList.Add(key, value);
            return this;
        }
        /// <summary>
        /// Ifs the specified evaluation.
        /// </summary>
        /// <param name="evaluation">if set to <c>true</c> [evaluation].</param>
        /// <param name="sql">The SQL.</param>
        /// <returns>ParameterBuilder.</returns>
        public ParameterBuilder If(bool evaluation, string sql)
        {
            return _builder.If(evaluation, sql);
        }
        /// <summary>
        /// Conditions the specified evaluation.
        /// </summary>
        /// <param name="evaluation">if set to <c>true</c> [evaluation].</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>ParameterBuilder.</returns>
        public ParameterBuilder If(bool evaluation, string sql, Action<ParameterBuilder> parameters)
        {
            return _builder.If(evaluation, sql, parameters);
        }
        /// <summary>
        /// SQLs the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>SqlStringBuilder.</returns>
        public SqlStringBuilder Sql(string sql)
        {
            _builder.Sql(sql);
            return _builder;
        }
        /// <summary>
        /// To the query.
        /// </summary>
        /// <returns>ISQLQuery.</returns>
        public IQuery ToQuery()
        {
            return _builder.ToQuery();
        }
        /// <summary>
        /// To the query.
        /// </summary>
        /// <returns>ISQLQuery.</returns>
        public ISQLQuery ToSqlQuery()
        {
            return _builder.ToSqlQuery();
        }
    }
}
