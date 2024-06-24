using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider.Utils;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using CEC.SRV.Domain.Lookup;
using CEC.SRV.Domain.Print;
using CEC.Web.SRV.Resources;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace CEC.SRV.BLL.Impl
{
    public class PollingStationBll: Bll, IPollingStationBll
    {
        private readonly ISessionFactory _sessionFactory;
        
        public PollingStationBll(ISRVRepository repository, ISessionFactory sessionFactory) : base(repository)
        {
            _sessionFactory = sessionFactory;
          
        }

        public IEnumerable<Address> GetPollingStationAddresses(long regionId)
        {
            
            if (IsRegionAccessibleToCurrentUser(regionId))
            {
                return Repository.Query<Address>()
						.Where(x => x.Street.Region.Id == regionId)
                        .ToList();
            }
            return new List<Address>();
        }

        public void CreateUpdatePollingStation(long id, long regionId, string number, string location, long addressId, 
            string contactInfo, long? saiseId, PollingStationTypes pollingStationType)
        {
            var region = Repository.LoadProxy<Region>(regionId);

            var pollingStation = id == 0 ? new PollingStation(region) : Get<PollingStation>(id);

            pollingStation.Number = number;
            pollingStation.Location = location;
            pollingStation.PollingStationAddress = addressId != 0
                ? Repository.LoadProxy<Address>(addressId)
                : null;
            pollingStation.ContactInfo = contactInfo;
            pollingStation.SaiseId = saiseId;
            pollingStation.PollingStationType = pollingStationType;

            Repository.SaveOrUpdate(pollingStation);

            if (id == 0)
            {
                var users = Repository.QueryOver<SRVIdentityUser>()
                    .WithUdf(new UsersWithAccessToRegionCriterion(regionId))
                    .HasPropertyIn(x => x.Id)
                    .List();

                string notificationMessage = string.Format(MUI.Notification_PollingStation_Create, pollingStation.Number, pollingStation.Region.GetFullName());
                CreateNotification(EventTypes.New, pollingStation, pollingStation.Id, notificationMessage, users);
            }
        }

		public void DeletePollingStation(long pollingStationId)
		{
			var address = Repository.Query<Address>().FirstOrDefault(x => x.PollingStation.Id == pollingStationId && x.Deleted == null);
			if (address != null)
			{
				throw new SrvException("PollingStation_NotPermissionDeletePollingStation", MUI.PollingStation_NotPermissionDeletePollingStation);
			}
			var entity = Repository.Get<PollingStation>(pollingStationId);
			Repository.Delete(entity);

            var users = Repository.QueryOver<SRVIdentityUser>()
                    .WithUdf(new UsersWithAccessToRegionCriterion(entity.Region.Id))
                    .HasPropertyIn(x => x.Id)
                    .List();
            string notificationMessage = string.Format(MUI.Notification_PollingStation_Delete, entity.Number, entity.Region.GetFullName());
            CreateNotification(EventTypes.Delete, entity, entity.Id, notificationMessage, users);
		}

		public int? GetCircumscription(long regionId)
		{
			return Repository.QueryOver<Region>()
				.WithUdf(new ParentChildsFilterCriterion(regionId)).HasPropertyIn(x => x.Id) 
				.Where(x => x.Parent.Id == 1).Select(x => x.Circumscription).SingleOrDefault<int?>();
		}


		public PageResponse<PollingStationDto> GetPollingStation(PageRequest pageRequest, long regionId)
        {
            if (IsRegionAccessibleToCurrentUser(regionId))
            {
				PollingStationDto pollingStationDto = null;
				PollingStationWithFullAddress pollingStationGrid = null;
				PollingStation pollingStation = null;
				Region region = null;
				VotersListOrderType orderType = null;
				IdentityUser createdBy = null;
				IdentityUser modifiedBy = null;
				IdentityUser deletedBy = null;
               


				return Repository.QueryOver(() => pollingStationGrid)
					.JoinAlias(x => pollingStationGrid.PollingStation, () => pollingStation)
					.JoinAlias(x => pollingStationGrid.PollingStation.Region, () => region)
					.JoinAlias(x => pollingStationGrid.PollingStation.VotersListOrderType, () => orderType, JoinType.LeftOuterJoin)
					.JoinAlias(x => pollingStation.DeletedBy, () => deletedBy, JoinType.LeftOuterJoin)
					.JoinAlias(x => pollingStation.CreatedBy, () => createdBy, JoinType.LeftOuterJoin)
					.JoinAlias(x => pollingStation.ModifiedBy, () => modifiedBy, JoinType.LeftOuterJoin)
					.Where(() => region.Id == regionId)
					.Select(Projections.ProjectionList()
					.Add(Projections.Property<PollingStationWithFullAddress>(x => x.Id).WithAlias(() => pollingStationDto.Id))
					.Add(Projections.Property<PollingStationWithFullAddress>(x => pollingStation.OwingCircumscription).WithAlias(() => pollingStationDto.OwingCircumscription))
					.Add(Projections.Property<PollingStationWithFullAddress>(x => pollingStation.Number).WithAlias(() => pollingStationDto.Number))
                    .Add(Projections.Property<PollingStationWithFullAddress>(x => pollingStation.PollingStationType).WithAlias(() => pollingStationDto.PollingStationType))
					.Add(Projections.Property<PollingStationWithFullAddress>(x => pollingStation.Location).WithAlias(() => pollingStationDto.Location))
					.Add(Projections.Property<PollingStationWithFullAddress>(x => x.FullAddress).WithAlias(() => pollingStationDto.FullAddress))
					.Add(Projections.Property<PollingStationWithFullAddress>(x => pollingStation.ContactInfo).WithAlias(() => pollingStationDto.ContactInfo))
					.Add(Projections.Property<PollingStationWithFullAddress>(x => x.TotalAddress).WithAlias(() => pollingStationDto.TotalAddress))
					.Add(Projections.Property<PollingStationWithFullAddress>(x => pollingStation.SaiseId).WithAlias(() => pollingStationDto.SaiseId))
					.Add(Projections.Property<PollingStationWithFullAddress>(x => pollingStation.Created).WithAlias(() => pollingStationDto.Created))
					.Add(Projections.Property<PollingStationWithFullAddress>(x => pollingStation.Modified).WithAlias(() => pollingStationDto.Modified))
					.Add(Projections.Property<PollingStationWithFullAddress>(x => pollingStation.Deleted).WithAlias(() => pollingStationDto.Deleted))
				    .Add(Projections.Property<VotersListOrderType>(x => orderType.Description).WithAlias(() => pollingStationDto.OrderType))
                    .Add(Projections.Property<VotersListOrderType>(x => orderType.Description).WithAlias(() => pollingStationDto.VotersListOrderTypes))
					.Add(Projections.Property<PollingStationWithFullAddress>(x => x.name).WithAlias(() => pollingStationDto.VotersListOrderTypes))
					.Add(Projections.Property<IdentityUser>(x => createdBy.UserName).WithAlias(() => pollingStationDto.CreatedBy))
					.Add(Projections.Property<IdentityUser>(x => modifiedBy.UserName).WithAlias(() => pollingStationDto.ModifiedBy))
					.Add(Projections.Property<IdentityUser>(x => deletedBy.UserName).WithAlias(() => pollingStationDto.DeletedBy)))
					.TransformUsing(Transformers.AliasToBean<PollingStationDto>())
					.RootCriteria
					.CreatePage<PollingStationDto>(pageRequest);
            }
			return new PageResponse<PollingStationDto> { Items = new List<PollingStationDto>(), PageSize = pageRequest.PageSize, Total = 0 };
        }

        public IList<PollingStation> GetAccessiblePollingStations()
        {
            Region region = null;
            var q = Repository.QueryOver<PollingStation>()
                .JoinAlias(x => x.Region, () => region)
                .Where(x => x.Deleted == null);

            if (SecurityHelper.LoggedUserIsInRole(Transactions.LookupRegister))
            {
                q = q.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                        .HasPropertyIn(x => region.Id);
            }

            return q.List();
        }

		public PageResponse<PollingStation> GetPollingStationsByRegions(PageRequest pageRequest, long[] selectedRegion)
		{
			var circumscriptions = Repository.Query<Region>()
				.Where(x => selectedRegion.Contains(x.Id))
				.Select(x => x.Circumscription)
				.ToArray();

		    return Repository.QueryOver<PollingStation>()
		        .Where(x => x.Deleted == null)
		        .AndRestrictionOn(x => x.OwingCircumscription).IsIn(circumscriptions)
		        .RootCriteria.CreatePage<PollingStation>(pageRequest);
		}

		public IList<PollingStation> GetPollingStationsByRegion(long[] selectedRegions)
		{
			var circumscriptions = Repository.Query<Region>()
				.Where(x => selectedRegions.Contains(x.Id))
				.Select(x => x.Circumscription)
				.ToList();

			return Repository.Query<PollingStation>().Where(x => circumscriptions.Contains(x.OwingCircumscription)).ToList();
		}

		public void VerificationIfPollingStationHasReference(long pollingStationId)
		{
			var street = Repository.Query<ExportPollingStation>().FirstOrDefault(x => x.PollingStation.Id == pollingStationId && x.Deleted == null);
			if (street != null)
			{
				throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
				street.PollingStation.GetObjectType(), street.GetObjectType()));
			}

			var address = Repository.Query<Address>().FirstOrDefault(x => x.PollingStation.Id == pollingStationId && x.Deleted == null);
			if (address != null)
			{
				throw new SrvException(string.Empty, String.Format(MUI.HasReference_Error,
				address.PollingStation.GetObjectType(), address.GetObjectType()));
			}
		}

        public async Task DeleteElectionNumberList()
        {
            await Task.Run(() =>
                {
                    using (IStatelessSession session = _sessionFactory.OpenStatelessSession())
                    {
                        session.CreateSQLQuery("Execute dbo.People_DeleteElectionListNumber").ExecuteUpdate();
                    }
                }

            ).ConfigureAwait(false);



        }
        public async Task AdddElectionNumberList(ElectionNumberListOrderByDto model)
        {
            var list = model.SelectedAPSIds;
            
            int query = 0;
            switch (model.first)
            {
                case 1:
                {
                    query = 1;

                }
                    break;
                case 2:
                {
                    query = 2;
                }
                    break;

                default:
                    query = 1;
                    break;
            }

            var queryBuilder = Repository.CreateSQLQuery(
                    "Update SRV.PollingStations set votersListOrderTypeId =:sortTypeId where pollingStationId IN (:listPollingstation);")
                .SetParameter("sortTypeId", model.first)
                .SetParameterList("listPollingstation", model.SelectedAPSIds);

            queryBuilder.ExecuteUpdate();

            //foreach (var item in list)
            //{
            //    await Task.Run(() =>
            //    {
            //        using (IStatelessSession session = _sessionFactory.OpenStatelessSession())
            //        {
            //            session.CreateSQLQuery("Execute dbo.People_AddElectionListNumber @case= :newValue , @pollingId= :newValue1")
            //                .SetInt32("newValue", query)
            //                .SetInt64("newValue1",item)
            //                .ExecuteUpdate();
            //        }

            //    }).ConfigureAwait(false);
            //}
           



        }

        public PageResponse<VotersListOrderType> SearchOrderType(PageRequest pageRequest)
        {
            var query = Repository.QueryOver<VotersListOrderType>();
           

            return query.RootCriteria.CreatePage<VotersListOrderType>(pageRequest);
        }


    }
}