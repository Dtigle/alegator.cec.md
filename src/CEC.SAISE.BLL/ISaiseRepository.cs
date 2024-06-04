using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain;
using NHibernate;
using CEC.SAISE.BLL.Helpers;

namespace CEC.SAISE.BLL
{
    public interface ISaiseRepository : IRepository, IAsyncRepository
    {
        IQueryOver<T, T> QueryOver<T>() where T : class, IEntity;
        IQueryOver<T, T> QueryOver<T>(Expression<Func<T>> alias) where T : class, IEntity;

        ISQLQuery CreateSQLQuery(string queryString);
        SqlStringBuilder CreateSqlStringBuilder(string sql, Action<string> logging);
    }
}
