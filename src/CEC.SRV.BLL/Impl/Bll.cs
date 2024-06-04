using System;
using System.Collections.Generic;
using System.Linq;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider.Utils;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.BLL.Extensions;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Resources;
using NHibernate.Criterion;

namespace CEC.SRV.BLL.Impl
{
    public class Bll : IBll
    {
        protected readonly ISRVRepository Repository;

        public Bll(ISRVRepository repository)
        {
            Repository = repository;
        }

        public T Get<T>(long id) where T : Entity
        {
            return Repository.Get<T>(id);
        }
        public IList<T> GetAll<T>() where T : Entity
        {
            return Repository.Query<T>().ToList();
        }


        /// <summary>
        /// GetAllLocalities, regardless of user; This is needed for conflictSharing
        /// </summary>
        public IList<Region> GetAllLocalities(int? nodeId)
        {
            var q = Repository.QueryOver<Region>();
            q = q.Where(x => x.Parent.Id == nodeId);
            return q.List();
        }

        public IList<Region> GetLocalities(int? nodeId)
        {
            var q = Repository.QueryOver<Region>();

            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                q = q.Where(x => x.Parent.Id == nodeId);
                return q.List();
            }

            q.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                .HasPropertyIn(x => x.Id);

            if (nodeId.HasValue)
            {
                q = q.Where(x => x.Parent.Id == nodeId);
                return q.List();
            }

            var data = q.List();
            var parentIds = data.Select(x => x.Parent.Id);
            var matches = data.Select(x => x.Id).Intersect(parentIds);
            var diffs = parentIds.Except(matches);
            data = data.Where(x => diffs.Contains(x.Parent.Id)).ToList();

            return data;
        }

        protected long[] GetCurrentUserRegions()
        {
            var userId = SecurityHelper.GetLoggedUserId();
            return Repository.Get<SRVIdentityUser, string>(userId)
                .Regions.Select(x => x.Id).ToArray();
        }

        public PageResponse<T> Get<T>(PageRequest pageRequest) where T : Entity
        {
            return Repository.Page<T>(pageRequest);
        }

        public PageResponse<T> SearchEntity<T>(PageRequest pageRequest) where T : Entity
        {
            var query = Repository.QueryOver<T>();
            if (!SecurityHelper.LoggedUserIsInRole("Administrator"))
            {
                query.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => x.Id);
            }
            return query.RootCriteria.CreatePage<T>(pageRequest);
        }

        public void Delete<T>(long id) where T : Entity
        {
            var entity = Repository.Get<T>(id);

            Repository.Delete(entity);
        }

        public void UnDelete<T>(long entityId) where T : SoftEntity<IdentityUser>
        {
            Repository.UnDelete<T>(entityId);
        }

        public void SaveOrUpdate<T>(T entity) where T : Entity
        {
            Repository.SaveOrUpdate(entity);
        }

        public IList<RegionType> GetRegionTypesByFilter(long regionId)
        {
            var region = Get<Region>(regionId);
            return Repository.Query<RegionType>().Where(x => x.Rank > region.RegionType.Rank).ToList();
        }

        public PublicAdministration GetPublicAdministration(long regionId)
        {
            var result = Repository.Query<PublicAdministration>().FirstOrDefault(x => x.Region.Id == regionId);
            return result;
        }

        public Street GetStreet(string streetsName, long regionId, long streetTypeId)
        {
            return Repository.Query<Street>().FirstOrDefault(x => x.Name == streetsName && x.Region.Id == regionId && x.StreetType.Id == streetTypeId);
        }

        public IList<Region> GetRegion(string name, long regionTypeId, long? parentId)
        {
            return Repository.Query<Region>().Where(x => x.Name == name && x.RegionType.Id == regionTypeId && x.Parent.Id == parentId.Value).ToList();
        }

        public IList<Address> GetAddress(long streetsId, long? houseNumber, string suffix)
        {
            return Repository.Query<Address>().Where(x => x.Street.Id == streetsId && x.HouseNumber == houseNumber && x.Suffix == suffix && x.Deleted == null).ToList();
        }

        public T GetByName<T>(string name) where T : Lookup, new()
        {
            return Repository.Query<T>().FirstOrDefault(x => x.Name == name);
        }

        public bool IsRegionAccessibleToCurrentUser(long currentRegionId)
        {
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return true;
            }

            var result = Repository.QueryOver<Region>()
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => x.Id)
                .Where(x => x.Id == currentRegionId)
                .Select(Projections.Count(Projections.Property<Region>(x => x.Id)))
                .Future<int>()
                .Single();

            return result > 0;
        }

        public void VerificationIsRegionDeleted(long regionId)
        {
            var region = Get<Region>(regionId);
            if (region.Deleted != null)
            {
                throw new SrvException("Error_RegionsIsDeleted", MUI.Error_RegionsIsDeleted);
            }
        }

        protected void CreateNotification(EventTypes eventType, INotificationEntity entity, long entityId, string message)
        {
            var adminRole = Repository.Query<IdentityRole>().FirstOrDefault(x => x.Name == Transactions.Administrator);
            var users = Repository.Query<SRVIdentityUser>()
                    .Where(x => x.Roles.Contains(adminRole));
            CreateNotification(eventType, entity, entityId, message, users);
        }

        protected void CreateNotification(EventTypes eventType, INotificationEntity entity, long entityId, string message,
            IEnumerable<SRVIdentityUser> users)
        {
            var op = eventType.GetEnumDescriptions();
            var localizedNotificationType = string.Format("{0} {1}", op,
                MUI.ResourceManager.GetString(entity.GetNotificationType()));
            var @event = new Event(eventType, localizedNotificationType, entityId);
            var notification = new Notification(@event, message.Truncate(255));
            notification.AddReceivers(users);
            Repository.SaveOrUpdate(notification);
        }

        public void VerificationIsDeletedLookup<T>(long id) where T : Lookup
        {
            var entity = Get<T>(id);
            if (entity.Deleted != null)
            {
                throw new SrvException("Error_EntityIsDeleted", MUI.Error_EntityIsDeleted);
            }
        }

        public void VerificationIsDeletedSrv<T>(long id) where T : SRVBaseEntity
        {
            var entity = Get<T>(id);
            if (entity.Deleted != null)
            {
                throw new SrvException("Error_EntityIsDeleted", MUI.Error_EntityIsDeleted);
            }
        }
    }


}