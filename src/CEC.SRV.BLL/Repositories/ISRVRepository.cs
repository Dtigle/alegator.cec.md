using System;
using System.Linq.Expressions;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using NHibernate;
using NHibernate.SqlCommand;

namespace CEC.SRV.BLL.Repositories
{
    public interface ISRVRepository : IRepository
    {
        IQueryOver<T, T> QueryOver<T>() where T : class, IEntity;
        IQueryOver<T, T> QueryOver<T>(Expression<Func<T>> alias) where T : class, IEntity;
        ISQLQuery CreateSQLQuery(string queryString);
        
        T UnDelete<T>(long id) where T : SoftEntity<IdentityUser>;

        bool TryToDeletePersistent<T>(long id);
    }
}