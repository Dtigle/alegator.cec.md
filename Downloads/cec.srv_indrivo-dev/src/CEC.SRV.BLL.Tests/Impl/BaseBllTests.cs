using System;
using System.Linq;
using CEC.SRV.BLL.Extensions;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.SRV.Domain.Print;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using CEC.Web.SRV.Resources;
using System.Linq.Expressions;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using NHibernate;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class BaseBllTests : BaseTests<Bll, ISRVRepository>
    {
        protected void VerificationIsRegionDeletedTrueCaseTest(long regionId)
        {
            SafeExec(Bll.VerificationIsRegionDeleted, regionId, true, false, "Error_RegionsIsDeleted", MUI.Error_RegionsIsDeleted);
        }

        protected void VerificationIsRegionDeletedFalseCaseTest(long regionId)
        {
            SafeExec(Bll.VerificationIsRegionDeleted, regionId);
        }

        #region Interaction with db

        protected List<T> GetAllObjectsFromDbTableWithUdfPropertyIn<T>(Expression<Func<T, bool>> condition, Expression<Func<T, object>> property, AbstractUdfCriterion udfCriterion) where T : class, IEntity
        {
            return GetAllObjectsFromDbTableWithUdfPropertyInQuery(condition, property, udfCriterion).List().ToList();
        }

        protected List<T> GetAllObjectsFromDbTableWithUdfPropertyIn<T>(Expression<Func<T, object>> property, AbstractUdfCriterion udfCriterion) where T : class, IEntity
        {
            return GetAllObjectsFromDbTableWithUdfPropertyInQuery(property, udfCriterion).List().ToList();
        }

        protected List<long> GetAllIdsFromDbTableWithUdfPropertyIn<T>(Expression<Func<T, bool>> condition, Expression<Func<T, object>> property, AbstractUdfCriterion udfCriterion) where T : Entity
        {
            return GetAllObjectsFromDbTableWithUdfPropertyInQuery(condition, property, udfCriterion).List().Select(x => x.Id).ToList();
        }

        protected List<long> GetAllIdsFromDbTableWithUdfPropertyIn<T>(Expression<Func<T, object>> property, AbstractUdfCriterion udfCriterion) where T : Entity
        {
            return GetAllObjectsFromDbTableWithUdfPropertyInQuery(property, udfCriterion).List().Select(x => x.Id).ToList();
        }

        protected IQueryOver<T, T> GetAllObjectsFromDbTableWithUdfPropertyInQuery<T>(Expression<Func<T, bool>> condition, Expression<Func<T, object>> property, AbstractUdfCriterion udfCriterion) where T : class, IEntity
        {
            return Repository.QueryOver<T>().Where(condition).WithUdf(udfCriterion).HasPropertyIn(property);
        }

        protected IQueryOver<T, T> GetAllObjectsFromDbTableWithUdfPropertyInQuery<T>(Expression<Func<T, object>> property, AbstractUdfCriterion udfCriterion) where T : class, IEntity
        {
            return Repository.QueryOver<T>().WithUdf(udfCriterion).HasPropertyIn(property);
        }

        protected AbstractUdfCriterion UdfRegionsCriterion()
        {
            return new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId());
        }

        #endregion Interaction with db
    }
}
