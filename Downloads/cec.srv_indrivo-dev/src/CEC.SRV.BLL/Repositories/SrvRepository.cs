using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using Amdaris.NHibernateProvider;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using NHibernate;
using NHibernate.Envers;
using NHibernate.SqlCommand;

namespace CEC.SRV.BLL.Repositories
{
    public class SrvRepository : Repository, ISRVRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public SrvRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public IQueryOver<T,T> QueryOver<T>() where T : class, IEntity
        {
            return Session.QueryOver<T>();
        }

        public IQueryOver<T, T> QueryOver<T>(Expression<Func<T>> alias) where T : class, IEntity
        {
            return Session.QueryOver(alias);
        }

        public T UnDelete<T>(long id) where T : SoftEntity<IdentityUser>
        {
            if (id == 0)
                throw new ArgumentNullException("id");

            var softEntity = Session.Get<T>(id);

            if (softEntity == null)
                throw new SrvException("", string.Format("Entity of type ({0}) with id={1} not found", typeof(T), id));

            if (!softEntity.Deleted.HasValue || softEntity.DeletedBy == null)
            {
                throw new SrvException("", string.Format("Entity of type ({0}) with id={1} is not deleted", typeof(T), id));
            }

            if (!SecurityHelper.LoggedUserIsInRole(Transactions.Administrator) &&
                softEntity.DeletedBy.Id != SecurityHelper.GetLoggedUserId())
            {
                throw new SrvException("Error_DeleteData", "You are not owning this deleted data");
            }

            softEntity.DeletedBy = null;
            softEntity.Deleted = null;

            SaveOrUpdate(softEntity);

            return softEntity;
        }

        public override ISession Session
        {
            get
            {
                var session = _sessionFactory.GetCurrentSession();
                
                if (!SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
                {
                    session.EnableFilter("DeleteOwnFilter").SetParameter("deletedBy", SecurityHelper.GetLoggedUserId());    
                }

                return session;
            }
        }

        public IAuditReader GetAuditer()
        {
            return Session.Auditer();
        }

        /// <summary>
        /// Permits executing of RAW updates over the database
        /// </summary>
        /// <param name="sqlString">SQl UPDATE</param>
        /// <param name="_params">Parameters</param>
        /// <returns></returns>
        public int ExecuteUpdate(String sqlString, Dictionary<String, Object> _params)
        {
            if (sqlString == null) throw new ArgumentNullException(nameof(sqlString));
            using (IStatelessSession session = _sessionFactory.OpenStatelessSession())
            {

                ISQLQuery query = session.CreateSQLQuery(sqlString);

                if (_params != null)
                    foreach (var item in _params)
                    {
                        if (item.Value is String)
                        {
                            query.SetString(item.Key, (String)item.Value);
                        }
                        else if (item.Value is Int64)
                        {
                            query.SetInt64(item.Key, (Int64)item.Value);
                        }
                        else if (item.Value is int)
                        {
                            query.SetInt64(item.Key, (int)item.Value);
                        }
                    }
                return query.ExecuteUpdate();
            }
        }

        public bool TryToDeletePersistent<T>(long id)
        {
            var meta = _sessionFactory.GetClassMetadata(typeof(T)) as NHibernate.Persister.Entity.AbstractEntityPersister;
            ISession session = _sessionFactory.GetCurrentSession();
            IQuery query = session.CreateSQLQuery(@"
            BEGIN TRY
            delete from " + meta.TableName + @" where " + meta.KeyColumnNames.First() + @" = :id;
            SELECT 1;
            END TRY
            BEGIN CATCH
            SELECT 0;
            END CATCH;");
            query.SetParameter("id", id, NHibernateUtil.Int64);
            var result = query.UniqueResult();
            return result.ToString() == "0" ? false : true;
        }

        public ISQLQuery CreateSQLQuery(string queryString)
        {
            return Session.CreateSQLQuery(queryString);
        }       
    }
}