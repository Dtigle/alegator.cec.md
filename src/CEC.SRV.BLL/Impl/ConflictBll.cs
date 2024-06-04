using System;
using System.Collections.Generic;
using System.Linq;
using Amdaris.Domain.Identity;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider;
using Amdaris.NHibernateProvider.Utils;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Lookup;
using CEC.SRV.Domain.ViewItem;
using CEC.Web.SRV.Resources;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;

namespace CEC.SRV.BLL.Impl
{
    public class ConflictBll : Bll, IConflictBll
    {

		public ConflictBll(ISRVRepository repository)
			: base(repository)
		{
            
        }

        #region Conflict sharing

        /// <summary>
        /// Shares the conflict identified by the <code>conflictDataId</code> with the region identified by <code>regionId</code>
        /// </summary>
        /// <param name="conflictDataId">The conflict</param>
        /// <param name="regionId">The region to with the conflict is shared to</param>
        /// <param name="conflictReasonId">The reason for which the conflict is shared</param>
        /// <param name="note">user notes regarding conflict sharing</param>
        /// <returns>True if the conflict was shared or if it was already shared</returns>
        public BllResult ShareConflict(long conflictDataId, long regionId, long conflictReasonId, string note)
        {
            var conflict = Repository.QueryOver<RspConflictData>().Where(x => x.Id == conflictDataId).SingleOrDefault();
            var region = Repository.QueryOver<Region>().Where(x => x.Id == regionId).SingleOrDefault();
            var reason = Repository.QueryOver<ConflictShareReasonTypes>().Where(x => x.Id == conflictReasonId).SingleOrDefault();

            if (conflict == null || region == null || reason == null)
                return new BllResult(-1, "Unable to share conflict. Cannot identify beans for all parameters <conflict, region, reason>");

            if (string.IsNullOrEmpty(note))
            {
                return new BllResult(-1, MUI.ShareConflict_RequiredReason);
            }

            if (conflict.SrvRegion.Id == regionId)
            {
                return new BllResult(-1, MUI.ShareConflict_DestinationIsOwnRegion_ErrorMessage);
            }

            var share = Repository.QueryOver<ConflictShare>()
                .Where(x => x.Conflict.Id == conflictDataId)
                .And(x => x.Destination.Id == regionId)
                .SingleOrDefault();

            bool isUpdate = share != null;

            if (share == null)
                share = new ConflictShare
                {
                    Conflict = conflict,
                    Source = conflict.SrvRegion,
                    Destination = region,
                    Reason = reason,
                    Note = note,
                    Created = new DateTimeOffset(),
                };
            else //share already exists; we are updating it
            {
                share.Note = note;
                share.Modified = DateTimeOffset.Now;
            }

            SaveOrUpdate(share);

            return new BllResult(share.Id, share.Id <= 0? "Sharing with region "+ region.Name+" failed!": "Conflict was shared with region "+ region.Name + (isUpdate?"[Updated]":""));
        }

        /// <summary>
        /// Cancel sharing with the indicated region 
        /// </summary>
        /// <param name="conflictDataId"></param>
        /// <param name="regionId"></param>
        /// <returns>BllResult with <code>statusCode</code> = -1 or the id of the conflict which was canceled </returns>
        public BllResult CancelConflictShare(long conflictDataId, long regionId)
        {
            var share = Repository.QueryOver<ConflictShare>()
                .Where(x => x.Conflict.Id == conflictDataId)
                .And(x => x.Destination.Id == regionId).SingleOrDefault();

            var region = Repository.Get<Region>(regionId);
            
            if (share == null)
            {
                return new BllResult(-1, string.Format(MUI.ConflictShare_NotFound, region.GetFullName(), conflictDataId));
            }

            // Authorisation. User from author's region can remove 
            if (!GetCurrentUserRegions().Contains(share.Source.Id))
            {
                return new BllResult(-1, string.Format(MUI.ConflictShare_DeleteNotPermited_DueToRegion, share.Source.GetFullName(), region.GetFullName()));
            }

            share.Deleted = DateTime.Now;
            SaveOrUpdate(share);

            //check if succeded
            share = Get<ConflictShare>(share.Id);
            if(share != null && share.Deleted != null)
                return new BllResult(share.Id, "Conflict sharing was canceled");
            return new BllResult(-1, "Conflict failed!");
        }

        /// <summary>
        /// Calcel all conflict sharings for the provided conflict id
        /// </summary>
        /// <param name="conflictDataId"></param>
        /// <returns></returns>
        public BllResult CancelAllConflictShares(long conflictDataId)
        {
            var shares = Repository.QueryOver<ConflictShare>()
                .Where(x => x.Conflict.Id == conflictDataId)
                .And(Restrictions.On<ConflictShare>(x => x.Deleted).IsNull)
                .List();

            var size = shares.Count;

            //actualy delete
            foreach (var conflictShare in shares)
                Delete<ConflictShare>(conflictShare.Id);

            //check al were canceled
            shares = Repository.QueryOver<ConflictShare>()
                .Where(x => x.Conflict.Id == conflictDataId)
                .And(x => x.Deleted != null)
                .List();

            if(shares.Count > 0)
                return new BllResult(-1, size != shares.Count ? "No Conflict sharing were canceled!":"Conflict canceling failed!");
            return new BllResult(1, "Confict sharings were canceled");
        }

        public IList<ConflictShare> GetAllConflictShares(long conflictDataId)
        {
            return Repository.QueryOver<ConflictShare>()
                .Where(x => x.Conflict.Id == conflictDataId)
                .And(Restrictions.On<ConflictShare>(x => x.Deleted).IsNull)
                .List();
        }

        /// <summary>
        /// Get conflicts shared by other regions with my region
        /// </summary>
        /// <param name="pageRequest"></param>
        /// <param name="conflictCode"></param>
        /// <returns></returns>
        public PageResponse<ConflictViewItem> GetConflictSharedWithMyRegionsList(PageRequest pageRequest, ConflictStatusCode[] conflictCodes)
        {
            ConflictViewItem rspConflict = null;
            var query = Repository.QueryOver(() => rspConflict)
                        .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => x.RegionId)
                        //get conflicts with share
                        .Where(() => rspConflict.SourceRegionId != null);



            if (conflictCodes != null && conflictCodes.Length > 0)
            {
                query = query.And(Restrictions.On(() => rspConflict.StatusConflictCode).IsIn(conflictCodes));
            }

            //not resolved
            return query
                .TransformUsing(Transformers.DistinctRootEntity)
                .RootCriteria.CreatePage<ConflictViewItem>(pageRequest);
        }

        public PageResponse<RspConflictData> GetConflictListSharedWithMyRegionsListForLinkedRegions(PageRequest pageRequest, ConflictStatusCode[] conflictCodes)
        {
            Region region = null;
            Region region1 = null;
            LinkedRegion linkedRegion = null;
            var linkedRegions = Repository.QueryOver(() => linkedRegion)
                .JoinAlias(() => linkedRegion.Regions, () => region)
                .Where(() => linkedRegion.Deleted == null)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                .HasPropertyIn(x => region.Id)
                .Select(Projections.Distinct(Projections.Id()))
                .Future<long>()
                .ToList();

            var regionIds = Repository.QueryOver<LinkedRegion>()
                .JoinAlias(x => x.Regions, () => region1)
                .Where(Restrictions.In(Projections.Property<LinkedRegion>(x => x.Id), linkedRegions))
                .Select(Projections.ProjectionList()
                    .Add(Projections.Property<Region>(x => region1.Id)))
                .Future<long>()
                .ToList();

            //
            RspConflictData rspConflict = null;
            ConflictShare share = null;
            var query = Repository.QueryOver(() => rspConflict)
                .JoinQueryOver(() => rspConflict.Shares, () => share, JoinType.RightOuterJoin)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                .HasPropertyIn(x => share.Destination.Id)
                .Where(() => share.Deleted == null)
                .And(Restrictions.In(Projections.Property<RspConflictData>(x => rspConflict.SrvRegion.Id), regionIds));
            //get conflicts with share

            query = query.And(Restrictions.On(() => rspConflict.StatusConflictCode).IsIn(conflictCodes));

            //not resolved
            return query
                .And(() => rspConflict.AcceptConflictCode != rspConflict.StatusConflictCode)
                //not rejected
                .And(() => rspConflict.RejectConflictCode == 0)
                .TransformUsing(Transformers.DistinctRootEntity)
                .RootCriteria.CreatePage<RspConflictData>(pageRequest);
        }

        /// <summary>
        /// Returns: <br/>
        ///    1) All conflicts from my region <br/>
        ///    2) All conflicts from other regions but shared with my region <br/>
        ///    3) Excludes conflicts from my region but shared with other regions <br/>
        /// <br/>
        /// 
        /// 
        ///  
        /** Query mockup is:
            
            select a.* 
            from RspConflictData a
            where (
		            a.region = "myregion"
			            or 
		            a.id in (
			            select id 
			            from ConflictShare 
			            where destination = "myregion"
		            )
	            ) and a.id not in (
		            select id 
		            from ConflictShare 
		            where source = "myregion"
	            ) 
            
            */
        /// </summary>
        /// <param name="pageRequest"></param>
        /// <returns></returns>
        public PageResponse<RspConflictData> GetAllConflicts(PageRequest pageRequest, ConflictStatusCode[] conflictCodes)
        {
            // my regions
            long[] userRegionIds = GetCurrentUserRegions();

            // aliases
            RspConflictData rspConflict = null;
            ConflictShare share = null;

            // Conflict IDs shared by others for me
            var queryConflictsSharedWithMe = QueryOver.Of<ConflictShare>()
                .Select(x => x.Conflict.Id)
                .Where(x => x.Destination.Id.IsIn(userRegionIds));

            // Conflict IDs shared by me for others
            var queryConflictsSharedByMe = QueryOver.Of<ConflictShare>()
                .Select(x => x.Conflict.Id)
                .Where(x => x.Source.Id.IsIn(userRegionIds));
            

            // final query
            var query = Repository.QueryOver(() => rspConflict);

            //filter by conflictCodes
            if (conflictCodes != null && conflictCodes.Length > 0)
            {
                query = query.And(Restrictions.On(() => rspConflict.StatusConflictCode).IsIn(conflictCodes));
            }

            return query
                //not resolved
                .And(() => rspConflict.AcceptConflictCode != rspConflict.StatusConflictCode)
                //not rejected
                .And(() => rspConflict.RejectConflictCode == 0)
                //one of this region filters ...
                .And(Restrictions.Or(
                    //include conflicts from my regions
                    Restrictions.Where(
                        () => rspConflict.SrvRegion.Id.IsIn(userRegionIds)
                    ),
                    //include conflicts shared with my regions
                    Subqueries.WhereProperty<RspModificationData>(x => x.Id).In(queryConflictsSharedWithMe)
                ))
                //exclude conflicts shared by my regions 
                .WithSubquery.WhereProperty(x => x.Id).NotIn(queryConflictsSharedByMe)
                .TransformUsing(Transformers.DistinctRootEntity)
                .RootCriteria.CreatePage<RspConflictData>(pageRequest);
        }

        public PageResponse<ConflictViewItem> GetAllConflicts2(PageRequest pageRequest, ConflictStatusCode[] conflictCodes)
        {
            ConflictViewItem rspConflict = null;

            var query = Repository.QueryOver(() => rspConflict);

            var containedStatuses = Expression.Disjunction();

            foreach (var conflictCode in conflictCodes)
            {
                containedStatuses
                    .Add(Expression.Conjunction()
                            .Add(ContainStatusConflict(conflictCode))
                            .Add(NotContainAcceptConflict(conflictCode))
                            .Add(NotContainRejectConflict(conflictCode)));
            }

            return query.Where(containedStatuses)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                    .HasPropertyIn(x => x.RegionId)
                .RootCriteria.CreatePage<ConflictViewItem>(pageRequest);
        }


        /// <summary>
        /// Uses same logic as GetAllConflicts function
        /// </summary>
        /// <param name="conflictCodes"></param>
        /// <returns></returns>
        public long GetAllConflictCount(ConflictStatusCode[] conflictCodes)
        {
            var query = Repository.QueryOver<ConflictCountRegistrator>()
                .Select(Projections.Sum<ConflictCountRegistrator>(t => t.ConflictCount))
                ;
            
            return query
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                    .HasPropertyIn(x => x.RegionId)
                .SingleOrDefault<int>();
        }

        public long GetAllConflictCountForAdmin(ConflictStatusCode[] conflictCodes)
        {
            var query = Repository.QueryOver<ConflictCountAdmin>()
                .Select( Projections.Sum<ConflictCountAdmin>(t => t.ConflictCount));
            return query.SingleOrDefault<int>();
        }


        public PageResponse<RspConflictData> GetConflictListSharedByMyRegionsListForLinkedRegions(PageRequest pageRequest, ConflictStatusCode[] conflictCodes)
        {
            Region region = null;
            Region region1 = null;
            LinkedRegion linkedRegion = null;
            var linkedRegions = Repository.QueryOver(() => linkedRegion)
                .JoinAlias(() => linkedRegion.Regions, () => region)
                .Where(() => linkedRegion.Deleted == null)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                .HasPropertyIn(x => region.Id)
                .Select(Projections.Distinct(Projections.Id()))
                .Future<long>()
                .ToList();

            var regionIds = Repository.QueryOver<LinkedRegion>()
                .JoinAlias(x => x.Regions, () => region1)
                .Where(Restrictions.In(Projections.Property<LinkedRegion>(x => x.Id), linkedRegions))
                .Select(Projections.ProjectionList()
                    .Add(Projections.Property<Region>(x => region1.Id)))
                .Future<long>()
                .ToList();

            //
            RspConflictData rspConflict = null;
            ConflictShare share = null;
            var query = Repository.QueryOver(() => rspConflict)
                .JoinQueryOver(() => rspConflict.Shares, () => share, JoinType.RightOuterJoin)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                .HasPropertyIn(x => share.Source.Id)
                .Where(() => share.Deleted == null)
                .And(Restrictions.In(Projections.Property<RspConflictData>(x => rspConflict.SrvRegion.Id), regionIds));
            //get conflicts with share

            query = query.And(Restrictions.On(() => rspConflict.StatusConflictCode).IsIn(conflictCodes));

            //not resolved
            return query
                .And(() => rspConflict.AcceptConflictCode != rspConflict.StatusConflictCode)
                //not rejected
                .And(() => rspConflict.RejectConflictCode == 0)
                .TransformUsing(Transformers.DistinctRootEntity)
                .RootCriteria.CreatePage<RspConflictData>(pageRequest);
        }



        public PageResponse<ConflictViewItem> GetConflictSharedByMyRegionsList(PageRequest pageRequest, ConflictStatusCode[] conflictCodes)
        {

            ConflictViewItem rspConflict = null;
            var query = Repository.QueryOver(() => rspConflict)
                        .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => x.SourceRegionId)
                        ;

            if (conflictCodes != null && conflictCodes.Length > 0)
            {
                query = query.And(Restrictions.On(() => rspConflict.StatusConflictCode).IsIn(conflictCodes));
            }

            //not resolved
            return query
                .TransformUsing(Transformers.DistinctRootEntity)
                .RootCriteria.CreatePage<ConflictViewItem>(pageRequest);
        }


        #endregion


        /// <summary>
        /// Returns the entire conflict History of the voter
        /// </summary>
        /// <param name="pageRequest"></param>
        /// <param name="idnp"></param>
        /// <returns></returns>
        public PageResponse<RspModificationData> GetConflictHistory(PageRequest pageRequest, string idnp)
        {
            return Repository.QueryOver<RspModificationData>()
                .Where(x => x.Idnp == idnp)
                .And(x => x.StatusConflictCode > 0)
                .OrderBy(Projections.Property("Id")).Desc()
                .RootCriteria.CreatePage<RspModificationData>(pageRequest);
        }


        public PageResponse<RspConflictData> GetConflictList(PageRequest pageRequest, ConflictStatusCode conflictCode)
        {
            RspConflictData rspConflict = null;

            var query = Repository.QueryOver(() => rspConflict)
                .Where(ContainStatusConflict(conflictCode))
                .And(NotContainAcceptConflict(conflictCode))
                .And(NotContainRejectConflict(conflictCode))
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                    .HasPropertyIn(x => x.SrvRegion.Id)
                .RootCriteria.CreatePage<RspConflictData>(pageRequest);
            
            return query;
        }

        public PageResponse<ConflictViewItem> GetConflictList2(PageRequest pageRequest, ConflictStatusCode conflictCode)
        {
            ConflictViewItem rspConflict = null;

            var query = Repository.QueryOver(() => rspConflict)
                .Where(ContainStatusConflict(conflictCode))
                .And(NotContainAcceptConflict(conflictCode))
                .And(NotContainRejectConflict(conflictCode))
                .And(x => x.SourceRegionId == null)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                    .HasPropertyIn(x => x.RegionId)
                .RootCriteria.CreatePage<ConflictViewItem>(pageRequest);
            
            return query;
        }

        public PageResponse<RspConflictData> GetConflictList(PageRequest pageRequest, ConflictStatusCode[] conflictCodes)
        {
            RspConflictData rspConflict = null;

            var query = Repository.QueryOver(() => rspConflict);

            var containedStatuses = Expression.Disjunction();

            foreach (var conflictCode in conflictCodes)
            {
                containedStatuses
                    .Add(Expression.Conjunction()
                            .Add(ContainStatusConflict(conflictCode))
                            .Add(NotContainAcceptConflict(conflictCode))
                            .Add(NotContainRejectConflict(conflictCode)));
            }

            return query.Where(containedStatuses)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                    .HasPropertyIn(x => x.SrvRegion.Id)
                .RootCriteria.CreatePage<RspConflictData>(pageRequest);
        }
        public PageResponse<ConflictViewItem> GetConflictList2(PageRequest pageRequest, ConflictStatusCode[] conflictCodes)
        {
            ConflictViewItem rspConflict = null;

            var query = Repository.QueryOver(() => rspConflict);

            var containedStatuses = Expression.Disjunction();

            foreach (var conflictCode in conflictCodes)
            {
                containedStatuses
                    .Add(Expression.Conjunction()
                            .Add(ContainStatusConflict(conflictCode))
                            .Add(NotContainAcceptConflict(conflictCode))
                            .Add(NotContainRejectConflict(conflictCode)));
            }
            
            return query.Where(containedStatuses)
                .And(x => x.SourceRegionId == null)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                    .HasPropertyIn(x => x.RegionId)
                .RootCriteria.CreatePage<ConflictViewItem>(pageRequest);
        }

        public PageResponse<RspConflictData> GetConflictListForLinkedRegions(PageRequest pageRequest, ConflictStatusCode conflictCode)
        {
            Region region = null;
            Region region1 = null;
            LinkedRegion linkedRegion = null;
            var linkedRegions = Repository.QueryOver(() => linkedRegion)
                .JoinAlias(() => linkedRegion.Regions, () => region)
                .Where(() => linkedRegion.Deleted == null)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                    .HasPropertyIn(x => region.Id)
                .Select(Projections.Distinct(Projections.Id()))
                .Future<long>()
                .ToList();

            var regionIds = Repository.QueryOver<LinkedRegion>()
                .JoinAlias(x => x.Regions, () => region1)
                .Where(Restrictions.In(Projections.Property<LinkedRegion>(x => x.Id), linkedRegions))
                .Select(Projections.ProjectionList()
                    .Add(Projections.Property<Region>(x => region1.Id)))
                .Future<long>()
                .ToList();

            RspConflictData rspConflict = null;

            var query = Repository.QueryOver(() => rspConflict)
                .Where(ContainStatusConflict(conflictCode))
                .And(NotContainAcceptConflict(conflictCode))
                .And(NotContainRejectConflict(conflictCode))
                .Where(Restrictions.In(Projections.Property<RspConflictData>(x => rspConflict.SrvRegion.Id), regionIds))
                .RootCriteria.CreatePage<RspConflictData>(pageRequest);
            
            return query;
        }

        public PageResponse<RspConflictDataAdmin> GetConflictListForAdmin(PageRequest pageRequest, ConflictStatusCode conflictCode)
        {
            RspConflictDataAdmin rspConflict = null;

            var query = Repository.QueryOver(() => rspConflict)
                .Where(ContainStatusConflict(conflictCode))
                .And(NotContainAcceptConflict(conflictCode))
                .RootCriteria.CreatePage<RspConflictDataAdmin>(pageRequest);

            return query;
        }

        public PageResponse<ConflictViewItem> GetConflictListForAdmin2(PageRequest pageRequest, ConflictStatusCode conflictCode)
        {
            ConflictViewItem rspConflict = null;

            var query = Repository.QueryOver(() => rspConflict)
                .Where(ContainStatusConflict(conflictCode))
                .And(NotContainAcceptConflict(conflictCode))
                .RootCriteria.CreatePage<ConflictViewItem>(pageRequest);

            return query;
        }


        public PageResponse<RspConflictDataAdmin> GetConflictListForAdmin(PageRequest pageRequest, ConflictStatusCode[] conflictCodes)
        {
            RspConflictDataAdmin rspConflict = null;

            var query = Repository.QueryOver(() => rspConflict);

            var containedStatuses = Expression.Disjunction();

            foreach (var conflictCode in conflictCodes)
            {
                containedStatuses
                    .Add(Expression.Conjunction()
                            .Add(ContainStatusConflict(conflictCode))
                            .Add(NotContainAcceptConflict(conflictCode)));
            }

            query.Where(containedStatuses);
            
            return query.RootCriteria.CreatePage<RspConflictDataAdmin>(pageRequest);
        }
        public PageResponse<ConflictViewItem> GetConflictListForAdmin2(PageRequest pageRequest, ConflictStatusCode[] conflictCodes)
        {
            ConflictViewItem rspConflict = null;

            var query = Repository.QueryOver(() => rspConflict);

            var containedStatuses = Expression.Disjunction();

            foreach (var conflictCode in conflictCodes)
            {
                containedStatuses
                    .Add(Expression.Conjunction()
                            .Add(ContainStatusConflict(conflictCode))
                            .Add(NotContainAcceptConflict(conflictCode)));
            }

            query.Where(containedStatuses);
            
            return query.RootCriteria.CreatePage<ConflictViewItem>(pageRequest);
        }

        public PageResponse<AddressBaseDto> GetAddresses(PageRequest pageRequest, long? regionId)
        {
            Region region = null;
            Street street = null;
            StreetType streetType = null;
            Address address = null;
            AddressBaseDto addressDto = null;
            PollingStation pollingStation = null;
            var query = Repository.QueryOver(() => address)
                .JoinAlias(x => x.Street, () => street)
                .JoinAlias(() => street.Region, () => region)
                .JoinAlias(() => street.StreetType, () => streetType)
                .JoinAlias(() => address.PollingStation, () => pollingStation, JoinType.LeftOuterJoin);
                //.Where(() => region.HasStreets);
            if (regionId.HasValue)
            {
                query.Where(() => region.Id == regionId);
            }

            if (SecurityHelper.LoggedUserIsInRole(Transactions.Registrator))
            {
                query.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                    .HasPropertyIn(x => region.Id);
            }

            query.Select(Projections.ProjectionList()
                .Add(Projections.Property<Address>(x => x.Id).WithAlias(() => addressDto.Id))
                .Add(Projections.Property<Address>(x => region.Id).WithAlias(() => addressDto.RegionId))
                .Add(Projections.Property<Address>(x => region.Name).WithAlias(() => addressDto.RegionName))
                .Add(Projections.Property<Address>(x => street.Name).WithAlias(() => addressDto.StreetName))
                .Add(Projections.Property<Address>(x => address.HouseNumber).WithAlias(() => addressDto.HouseNumber))
                .Add(Projections.Property<Address>(x => address.Suffix).WithAlias(() => addressDto.Suffix))
                .Add(Projections.Property<PollingStation>(x => pollingStation.FullNumber).WithAlias(() => addressDto.PollingStation))
                .Add(Projections.Property<Address>(x => address.Created).WithAlias(() => addressDto.Created))
                .Add(Projections.Property<Address>(x => address.Modified).WithAlias(() => addressDto.Modified))
                .Add(Projections.Property<Address>(x => address.Deleted).WithAlias(() => addressDto.Deleted))
                .Add(Projections.Property<Address>(x => address.CreatedBy).WithAlias(() => addressDto.CreatedBy))
                .Add(Projections.Property<Address>(x => address.ModifiedBy).WithAlias(() => addressDto.ModifiedBy))
                .Add(Projections.Property<Address>(x => address.DeletedBy).WithAlias(() => addressDto.DeletedBy))
                .Add(Projections.Property<StreetType>(x => streetType.Name).WithAlias(() => addressDto.StreetTypeName))
                ).TransformUsing(Transformers.AliasToBean<AddressBaseDto>());
            return query.RootCriteria.CreatePage<AddressBaseDto>(pageRequest);

        }

        /// <summary>
        /// Get Conflicts with same address excluding conflict passed as parameter
        /// </summary>
        /// <param name="pageRequest"></param>
        /// <param name="conflictId"> </param>
        /// <returns></returns>

        public PageResponse<RspConflictData> GetConflictListByConflictAddress(PageRequest pageRequest, long conflictId)
        {

            RspRegistrationData conflictRegistrationData = Repository.Get<RspRegistrationData>(conflictId);
            if (conflictRegistrationData == null)
            {
                throw new ArgumentException("Conflict not found, id=" + conflictId);
            }
            // my regions
            //long[] userRegionIds = GetCurrentUserRegions();

            RspConflictData rspConflict = null;
            var query = Repository.QueryOver(() => rspConflict);


            //if (userRegionIds.Length > 0)
            //{
            //    //user's region filter
            //    query = query.Where(x => x.SrvRegion.Id.IsIn(userRegionIds));
            //}

            //
            // Build address conflicts filter
            //
            var containedStatuses = Expression.Disjunction();
            foreach (var conflictCode in new [] { ConflictStatusCode.AddressConflict, ConflictStatusCode.StreetConflict, ConflictStatusCode.AddressFatalConflict})
            {
                containedStatuses
                    .Add(Expression.Conjunction()
                            .Add(ContainStatusConflict(conflictCode))
                            .Add(NotContainAcceptConflict(conflictCode))
                            .Add(NotContainRejectConflict(conflictCode)));
            }

            return query
                //with same address, condition is inspired from ImportBll.ResolveByMappingAddress function
                .Where(x => x.Id != conflictId &&
                                x.Administrativecode == conflictRegistrationData.Administrativecode &&
                                x.Streetcode == conflictRegistrationData.StreetCode &&
                                x.HouseNr == conflictRegistrationData.HouseNumber &&
                                x.HouseSuffix == conflictRegistrationData.HouseSuffix &&
                                x.IsInConflict == true)
                 //address conflicts 
                .And(containedStatuses)
                //not resolved 
                .And(() => rspConflict.AcceptConflictCode != rspConflict.StatusConflictCode && rspConflict.AcceptConflictCode != ConflictStatusCode.ManualSolved)
                //not rejected
                .And(() => rspConflict.RejectConflictCode == 0)
                .TransformUsing(Transformers.DistinctRootEntity)
                .RootCriteria.CreatePage<RspConflictData>(pageRequest);
        }

        public VoterConflictDataDto GetVoter(string idnp)
	    {
	        Gender gender = null;
	        //PersonFullAddress personFullAddress = null;
	        Person person= null;
	        DocumentType docType = null;
	        PersonStatus personStatus = null;
	        PersonStatusType personStatusType = null;
	        VoterConflictDataDto voter = null;

            var personDto = Repository.QueryOver(() => person)
                //.JoinAlias(()=> person.Addresses, () => personAddress, JoinType.LeftOuterJoin)
                .JoinAlias(()=> person.Gender, () => gender, JoinType.InnerJoin)
                .JoinAlias(() => person.PersonStatuses, () => personStatus, JoinType.LeftOuterJoin)
                .JoinAlias(() => personStatus.StatusType, () => personStatusType, JoinType.LeftOuterJoin)
                .JoinAlias(() => person.Document.Type, () => docType, JoinType.LeftOuterJoin)
                .Where(() => person.Idnp == idnp && person.Deleted == null && personStatus.IsCurrent)
                .Select(Projections.ProjectionList()
                .Add(Projections.Property<Person>(x => person.Id).WithAlias(() => voter.Id))
                .Add(Projections.Property<Person>(x => person.Idnp).WithAlias(() => voter.Idnp))
                .Add(Projections.Property<Person>(x => person.FirstName).WithAlias(() => voter.FirstName))
                .Add(Projections.Property<Person>(x => person.Surname).WithAlias(() => voter.Surname))
                .Add(Projections.Property<Person>(x => person.MiddleName).WithAlias(() => voter.MiddleName))
                .Add(Projections.Property<Person>(x => person.DateOfBirth).WithAlias(() => voter.DateOfBirth))
                .Add(Projections.Property<Gender>(x => gender.Name).WithAlias(() => voter.Gender))
                .Add(Projections.Property<Person>(x => person.Document.Seria).WithAlias(() => voter.DocSeria))
                .Add(Projections.Property<Person>(x => person.Document.Number).WithAlias(() => voter.DocNumber))
                .Add(Projections.Property<Person>(x => person.Document.IssuedBy).WithAlias(() => voter.DocIssueBy))
                .Add(Projections.Property<Person>(x => person.Document.IssuedDate).WithAlias(() => voter.DocIssueDate))
                .Add(Projections.Property<Person>(x => person.Document.ValidBy).WithAlias(() => voter.DocValidBy))
                .Add(Projections.Property<DocumentType>(x => docType.Name).WithAlias(() => voter.DocType))
                .Add(Projections.Property<PersonStatusType>(x => personStatusType.Name).WithAlias(() => voter.PersonStatus))
                //.Add(Projections.Property<PersonFullAddress>(x => personAddress.PersonFullAddress.FullAddress).WithAlias(() => voter.Address))
                .Add(Projections.Property<Person>(x => person.Comments).WithAlias(() => voter.Comments))
                )
                .TransformUsing(Transformers.AliasToBean<VoterConflictDataDto>());


            VoterConflictDataDto result = personDto.SingleOrDefault<VoterConflictDataDto>();

            if (result != null)
            {
                PersonAddress personAddress = null;
                PersonAddress pfa = Repository.QueryOver(() => personAddress).Where(x => x.Person.Id.IsIn(new [] { result.Id }) && x.IsEligible).SingleOrDefault();
                if (pfa != null)
                {
                    result.Address = pfa.GetFullPersonAddress();
                }
            }

            return result;
	    }

        public Person GetPerson(string idnp)
        {
            return Repository.QueryOver<Person>().Where(x => x.Idnp == idnp).SingleOrDefault();
        }

        public Region GetRegionByAdministrativeCode(long cuatmCode)
	    {
	        return Repository.Query<Region>().FirstOrDefault(region => region.StatisticIdentifier == cuatmCode);
	    }

	    public Street GetStreetByRopId(long streetCodeId)
	    {
	        return Repository.Query<Street>()
                .FirstOrDefault(x => x.RopId == streetCodeId && x.Deleted == null);
	    }

        public Street GetStreetByRopId(long streetCodeId, long regionId)
        {
            return Repository.Query<Street>()
                .SingleOrDefault(x => x.RopId == streetCodeId && x.Region.Id == regionId && x.Deleted == null);
        }

        public Street GetStreetNameAndRegionId(string streetName, long regionId)
        {
            return Repository.Query<Street>()
                .SingleOrDefault(x => x.Name == streetName && x.Region.Id == regionId && x.Deleted == null);
        }

        public long CreateStreet(string name, string description, long regionId, long streetTypeId, long? ropId, long? saiseId)
        {
            var streetType = Repository.LoadProxy<StreetType>(streetTypeId);
            var region = Repository.LoadProxy<Region>(regionId);

            var street = new Street(region, streetType, name) ;
            street.Name = name;
            street.StreetType = streetType;
            street.Description = description;
            street.RopId = ropId;
            street.SaiseId = saiseId;
            Repository.SaveOrUpdate(street);
            return street.Id;
        }
        
	    public List<long> GetPersonIdbyRspIds(List<long> conflictIds)
	    {
            var personIds = new List<long>();
	        foreach (var id in conflictIds)
	        {
	            var person = GetPersonByConflict(id);
                if (person == null) 
                    throw new SrvException("Conflict_NoPersonExistInRSV_ErrorMessage", MUI.Conflict_NoPersonExistInRSV_ErrorMessage);
                personIds.Add(person.Id);
	        }
	        return personIds;
	    }
 
	    public long GetPersonIdByConflictId(long conflictId)
	    {
            var person = GetPersonByConflict(conflictId);
	        if (person == null)
                throw new SrvException("Conflict_NoPersonExistInRSV_ErrorMessage", MUI.Conflict_NoPersonExistInRSV_ErrorMessage);    

	        return person.Id;
	    }

        private Person GetPersonByConflict(long conflictId)
        {
            var conflictRegistration = Get<RspRegistrationData>(conflictId);
            return Repository.Query<Person>()
                .SingleOrDefault(x => x.Idnp == conflictRegistration.RspModificationData.Idnp);
        }

        public void WriteNotification(ConflictStatusCode conflictStatus, RspModificationData conflictData, string notificationMessage, long regionId = -1, long regionId2 = -1)
        {
            var users = new List<SRVIdentityUser>(GetUsersToBeNotified());

            if (regionId > 0)
            {
                users.AddRange( Repository.QueryOver<SRVIdentityUser>()
                .WithUdf(new UsersWithAccessToRegionCriterion(regionId))
                .HasPropertyIn(x => x.Id)
                .List());
            }
            if (regionId2 > 0)
            {
                users.AddRange( Repository.QueryOver<SRVIdentityUser>()
                .WithUdf(new UsersWithAccessToRegionCriterion(regionId2))
                .HasPropertyIn(x => x.Id)
                .List());
            }
            
            CreateNotification(EventTypes.Update, conflictData, conflictData.Id, notificationMessage, users.Distinct());
        }

        public Address SaveAddress(long streetId, int? houseNumber, string suffix, BuildingTypes buildingType, long? pollingStationId)
        {
            var street = Repository.LoadProxy<Street>(streetId);
            var pollingStation = pollingStationId != null ? Repository.LoadProxy<PollingStation>((long)pollingStationId) : null;
            var verificationByUnique = GetAddress(streetId, houseNumber, suffix);

            if (verificationByUnique.Any() && verificationByUnique.Any())
            {
                throw new SrvException("CreateAddress_AlreadyExist_Message", MUI.CreateAddress_AlreadyExist_Message);
            }

            var address = new Address
            {
                Street = street,
                HouseNumber = houseNumber,
                Suffix = suffix,
                BuildingType = buildingType,
                PollingStation = pollingStation
            };

            Repository.SaveOrUpdate(address);

            return address;
        }

        public void UpdateStatusToRetry(long conflictId, string message, ConflictStatusCode conflictCode)
        {
            var registrationData = Get<RspRegistrationData>(conflictId);

            RspModificationData conflictData = null;
            RspRegistrationData conflictAddress = null;
            var conflictsData = Repository.QueryOver(() => conflictData)
                .JoinAlias(x => x.Registrations, () => conflictAddress)
                .Where(ContainStatusConflict(conflictCode))
                .And(x => conflictAddress.Administrativecode == registrationData.Administrativecode)
                .List<RspModificationData>();

            foreach (var conflict in conflictsData)
            {
                conflict.SetRetry(message);
                Repository.SaveOrUpdate(conflict);
                
            }
        }

        public RspConflictData GetConflictFromRspConflictData(long id)
        {
            var conflict = Repository.Query<RspConflictData>()
                .FirstOrDefault(x => x.Id == id);
            return conflict;
        }

        public RspModificationData GetConflict(long id)
        {
            var conflict = Repository.Query<RspModificationData>()
                .FirstOrDefault(x => x.Id == id);
            return conflict;
        }

        public string GetUserName()
        {
            var user = Repository.Get<SRVIdentityUser, string>(SecurityHelper.GetLoggedUserId());
            return user.UserName;
        }

        public PageResponse<Region> GetUserRegions(PageRequest pageRequest)
        {
            var regions = Repository.QueryOver<Region>();
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Registrator))
            {
                regions = regions.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                        .HasPropertyIn(x => x.Id);
            }
            regions = regions.Where(x => x.Deleted == null);
                    
            return regions.RootCriteria.CreatePage<Region>(pageRequest);
        }

        public void ChangeAddress(long addressId, long personId)
        {
            var address = Get<Address>(addressId);
            var person = Repository.Get<Person>(personId);
            person.EligibleAddress.Address = address;
            Repository.SaveOrUpdate(person);
        }

        public RspRegistrationData GetConflictAddress(long conflictId)
        {
            var registrationDatas = Repository.Query<RspRegistrationData>()
                .Where(x => x.RspModificationData.Id == conflictId).ToList();
            var result = registrationDatas;
            if (result.FirstOrDefault(x => x.IsInConflict) == null)
            {
                return registrationDatas.FirstOrDefault();
            }

            return registrationDatas.FirstOrDefault(x => x.IsInConflict);
        }

        public PageResponse<AddressWithoutPollingStation> GetAddressesWithoutPollingStation(PageRequest pageRequest)
        {
            var query = Repository.QueryOver<AddressWithoutPollingStation>();
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Registrator))
            {
                query = query.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                        .HasPropertyIn(x => x.RegionId);
            }

            return query.RootCriteria.CreatePage<AddressWithoutPollingStation>(pageRequest);
        }

        private ICriterion ContainStatusConflict(ConflictStatusCode conflictCode)
        {
            return Restrictions.Gt(Projections
                    .SqlProjection(
                        String.Format("{{alias}}.StatusConflictCode & {0} as ConflictStatusCodeSet",
                            (int)(conflictCode))
                        , null, null), 0);
        }

        private ICriterion NotContainAcceptConflict(ConflictStatusCode conflictCode)
        {
            return Restrictions.Eq(Projections
                .SqlProjection(
                    String.Format("{{alias}}.AcceptConflictCode & {0} as ConflictStatusCodeSet", (int)(conflictCode))
                    , null, null), 0);
        }
        
        private ICriterion NotContainRejectConflict(ConflictStatusCode conflictCode)
        {
            return Restrictions.Eq(Projections
                            .SqlProjection(
                                String.Format("{{alias}}.RejectConflictCode & {0} as ConflictStatusCodeSet", (int)(conflictCode))
                                , null, null), 0);
        }

        private IList<SRVIdentityUser> GetUsersToBeNotified()
        {
            var role = Repository.Query<IdentityRole>().FirstOrDefault(x => x.Name == Transactions.Administrator);
            var users = Repository.Query<SRVIdentityUser>().Where(x => x.Roles.Contains(role)).ToList();
            return users;
        }

	}
}