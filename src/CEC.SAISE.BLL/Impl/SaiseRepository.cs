using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain;
using Amdaris.NHibernateProvider;
using NHibernate;
using CEC.SAISE.BLL.Helpers;

namespace CEC.SAISE.BLL.Impl
{
    public class SaiseRepository : Repository, ISaiseRepository
    {
        public readonly ISessionFactory SessionFactory;

        public SaiseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            SessionFactory = sessionFactory;
        }

        public IQueryOver<T, T> QueryOver<T>() where T : class, IEntity
        {
            return Session.QueryOver<T>();
        }

        public IQueryOver<T, T> QueryOver<T>(Expression<Func<T>> alias) where T : class, IEntity
        {
            return Session.QueryOver(alias);
        }

        public ISQLQuery CreateSQLQuery(string queryString)
        {
            return Session.CreateSQLQuery(queryString);
        }

        public SqlStringBuilder CreateSqlStringBuilder(string sql, Action<string> logging)
        {
            return new SqlStringBuilder(Session, sql, logging);
        }
    }
}
