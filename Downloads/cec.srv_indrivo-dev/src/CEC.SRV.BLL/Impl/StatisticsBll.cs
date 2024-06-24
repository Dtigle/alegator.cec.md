using System;
using System.Linq;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider.Utils;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Lookup;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace CEC.SRV.BLL.Impl
{
    public class StatisticsBll : Bll, IStatisticsBll
    {
        public StatisticsBll(ISRVRepository repository)
            : base(repository)
        {
        }

        public IQueryOver<Person,Person> BaseQuery()
        {
            Person person = null;
            PersonStatus personStatus = null;
            PersonStatusType personStatusType = null;
            return Repository.QueryOver(() => person)
                .JoinAlias(x => x.PersonStatuses, () => personStatus)
                .JoinAlias(() => personStatus.StatusType, () => personStatusType)
                .Where(() => person.Deleted == null && !personStatusType.IsExcludable && personStatus.IsCurrent);
        }
        

        public IQueryOver<Person, Person> FilterQuery(IQueryOver<Person, Person> query, long? regionId, long? pollingStationId)
        {
            PersonAddress personAddress = null;
            Address address = null;
            Street street = null;
            Region region = null;

            query = query
                .JoinAlias(x => x.Addresses, () => personAddress, JoinType.LeftOuterJoin)
                .JoinAlias(() => personAddress.Address, () => address, JoinType.LeftOuterJoin)
                .JoinAlias(() => address.Street, () => street, JoinType.LeftOuterJoin)
                .JoinAlias(() => street.Region, () => region, JoinType.InnerJoin)
                .Where(() => personAddress.IsEligible);

            if (regionId.HasValue)
            {
                query = query.WithUdf(new RegionChildsFilterCriterion(regionId.Value)).HasPropertyIn( x => region.Id);
            }

            if (pollingStationId.HasValue)
            {
                query = query.Where(() => address.PollingStation.Id == pollingStationId);
            }

            return query;
        }
        public IQueryOver<PollingStationStatistics, PollingStationStatistics> FilterQuery(IQueryOver<PollingStationStatistics, PollingStationStatistics> query, long? regionId, long? pollingStationId)
        {
            if (regionId.HasValue)
            {
                query = query.WithUdf(new RegionChildsFilterCriterion(regionId.Value)).HasPropertyIn( x => x.RegionId);
            }

            if (pollingStationId.HasValue)
            {
                query = query.Where(x => x.PollingStationId == pollingStationId);
            }

            return query;
        }


        
        public IQueryOver<Person, Person> RegisrtratorBaseQuery(IQueryOver<Person, Person> query)
        {
            return query;
        }

        public IQueryOver<Person, Person> AdministratorBaseQuery(IQueryOver<Person, Person> query)
        {
            return query;
        }

        public long ReturnLongTypeFromQuery(IQueryOver<Person, Person> query)
        {
            return query.Select(Projections.RowCountInt64())
                    .FutureValue<long>()
                    .Value;
        } 
        
        public long GetTotalNumberOfPeople(long? regionId, long? pollingStationId)
        {
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                Person p = null;
                return ReturnLongTypeFromQuery(AdministratorBaseQuery(FilterQuery(Repository.QueryOver(() => p).Where(x => x.Deleted == null), regionId, pollingStationId)));
            }

            Person person = null;
            Region region = null;
            var query = FilterQuery(Repository.QueryOver(() => person), regionId, pollingStationId);
            query = RegisrtratorBaseQuery(query)
                .Where(() =>  person.Deleted == null)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => region.Id);
            return ReturnLongTypeFromQuery(query); 
        }

        public long GetTotalNumberOfPeopleWithoutDeads(long? regionId, long? pollingStationId)
        {
            // TODO revise status lookup constat usage
            Person person = null;
            PersonStatus personStatus = null;
            PersonStatusType personStatusType = null;
            var queryA = FilterQuery(Repository.QueryOver(() => person)
                .JoinAlias(x => x.PersonStatuses, () => personStatus)
                .JoinAlias(() => personStatus.StatusType, () => personStatusType)
                .Where(() => person.Deleted == null && personStatusType.Id != 2 && personStatus.IsCurrent), regionId, pollingStationId);
          
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return ReturnLongTypeFromQuery(AdministratorBaseQuery(queryA));
            }

            Region region = null;
            var queryR = RegisrtratorBaseQuery(queryA)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => region.Id);
            return ReturnLongTypeFromQuery(queryR);
        }

        public long GetTotalNumberOfVoters(long? regionId, long? pollingStationId)
        {
            Person person = null;
            PersonStatus personStatus = null;
            PersonStatusType personStatusType = null;
            var query = FilterQuery(Repository.QueryOver(() => person)
                .JoinAlias(x => x.PersonStatuses, () => personStatus)
                .JoinAlias(() => personStatus.StatusType, () => personStatusType)
                .Where(() => person.Deleted == null && !personStatusType.IsExcludable && personStatus.IsCurrent), regionId, pollingStationId);
                    
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return ReturnLongTypeFromQuery(AdministratorBaseQuery(query));
            }
            
            Region region = null;
            query = RegisrtratorBaseQuery(query)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => region.Id);
            return ReturnLongTypeFromQuery(query);
        }

        public long GetTotalNumberOfDeads(long? regionId, long? pollingStationId)
        {
            Person person = null;
            PersonStatus personStatus = null;
            PersonStatusType personStatusType = null;
            var query = FilterQuery(Repository.QueryOver(() => person)
                .JoinAlias(x => x.PersonStatuses, () => personStatus)
                .JoinAlias(() => personStatus.StatusType, () => personStatusType)
                .Where(() => person.Deleted == null && personStatusType.Id == PersonStatusType.Death && personStatus.IsCurrent), regionId, pollingStationId);

            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return ReturnLongTypeFromQuery(AdministratorBaseQuery(query));
            }

            Region region = null;
            query = RegisrtratorBaseQuery(query)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => region.Id);
            return ReturnLongTypeFromQuery(query);
        }

        public long GetTotalNumberOfMilitary(long? regionId, long? pollingStationId)
        {
            Person person = null;
            PersonStatus personStatus = null;
            PersonStatusType personStatusType = null;
            var query = FilterQuery(Repository.QueryOver(() => person)
                .JoinAlias(x => x.PersonStatuses, () => personStatus)
                .JoinAlias(() => personStatus.StatusType, () => personStatusType)
                .Where(() => person.Deleted == null && personStatusType.Id == PersonStatusType.Military && personStatus.IsCurrent), regionId, pollingStationId);

            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return ReturnLongTypeFromQuery(AdministratorBaseQuery(query));
            }

            Region region = null;
            query = RegisrtratorBaseQuery(query)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => region.Id);
            return ReturnLongTypeFromQuery(query);

        }

        public long GetTotalNumberOfDetainee(long? regionId, long? pollingStationId)
        {
            Person person = null;
            PersonStatus personStatus = null;
            PersonStatusType personStatusType = null;
            var query = FilterQuery(Repository.QueryOver(() => person)
                .JoinAlias(x => x.PersonStatuses, () => personStatus)
                .JoinAlias(() => personStatus.StatusType, () => personStatusType)
                .Where(() => person.Deleted == null && personStatusType.Id == PersonStatusType.Detainee && personStatus.IsCurrent), regionId, pollingStationId);

            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return ReturnLongTypeFromQuery(AdministratorBaseQuery(query));
            }

            Region region = null;
            query = RegisrtratorBaseQuery(query)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => region.Id);
            return ReturnLongTypeFromQuery(query);

        }

        public long GetTotalNumberOfStatementAbroad(long? regionId, long? pollingStationId)
        {
            Person person = null;
            PersonStatus personStatus = null;
            PersonStatusType personStatusType = null;
            var query = FilterQuery(Repository.QueryOver(() => person)
                .JoinAlias(x => x.PersonStatuses, () => personStatus)
                .JoinAlias(() => personStatus.StatusType, () => personStatusType)
                .Where(() => person.Deleted == null && personStatusType.Id == PersonStatusType.StatementAbroad && personStatus.IsCurrent), regionId, pollingStationId);

            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return ReturnLongTypeFromQuery(AdministratorBaseQuery(query));
            }

            Region region = null;
            query = RegisrtratorBaseQuery(query)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => region.Id);
            return ReturnLongTypeFromQuery(query);

        }

        public long GetTotalNumberOfStayStatementDeclarations(long? regionId, long? pollingStationId)
        {
            //TODO: apply filter for regionId and pollingStationId

            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return Repository.Query<StayStatement>().Where(x => x.Deleted == null).Count();
            }
            StayStatement stayStatement = null;
            Person person = null;
            PersonAddress personAddress = null;
            Address address = null;
            Street street = null;
            Region region = null;
            return Repository.QueryOver(() => stayStatement)
                .JoinAlias(x => x.Person, () => person, JoinType.InnerJoin)
                .JoinAlias(() => person.Addresses, () => personAddress, JoinType.LeftOuterJoin)
                .JoinAlias(() => personAddress.Address, () => address, JoinType.LeftOuterJoin)
                .JoinAlias(() => address.Street, () => street, JoinType.LeftOuterJoin)
                .JoinAlias(() => street.Region, () => region, JoinType.InnerJoin)
                .Where(() => personAddress.IsEligible && stayStatement.Deleted == null)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => region.Id)
                .Select(Projections.RowCountInt64())
                .FutureValue<long>()
                .Value;
        }

        public long GetTotalNumberOfPeopleByGender(GenderTypes gender, long? regionId, long? pollingStationId)
        {
            Gender genderPerson = null;
            var queryA = FilterQuery(BaseQuery()
                    .JoinAlias(x => x.Gender, () => genderPerson)
                    .Where(() => genderPerson.Id == (long)gender), regionId, pollingStationId);
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return ReturnLongTypeFromQuery(AdministratorBaseQuery(queryA));
            }

            Region region = null;
            var queryR = RegisrtratorBaseQuery(queryA)
               .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => region.Id);
            return ReturnLongTypeFromQuery(queryR);
        }

        public long GetTotalNumberOfPeopleWithDoBMissing(long? regionId, long? pollingStationId)
        {
            var queryA = FilterQuery(BaseQuery()
                    .Where(x => x.DateOfBirth == null), regionId, pollingStationId);
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return ReturnLongTypeFromQuery(AdministratorBaseQuery(queryA));
            }
           
            Region region = null;
            var queryR = RegisrtratorBaseQuery(queryA)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => region.Id);
            return ReturnLongTypeFromQuery(queryR);
        }

        public long GetTotalNumberOfPeopleWithAddressMissing(long? regionId, long? pollingStationId)
		{
			PersonAddress personAddress = null;
			Address address = null;
			Street street = null;
			Region region = null;

			var queryA = Repository.QueryOver(() => personAddress)
				.JoinAlias(x => personAddress.Address, () => address)
				.JoinAlias(() => address.Street, () => street)
				.JoinAlias(() => street.Region, () => region)
				.Where(x => x.PersonAddressType.Id == PersonAddressType.NoResidence);

            //
            // Same filters applied in FilterQuery function
            //
            if (regionId.HasValue)
            {
                queryA = queryA.WithUdf(new RegionChildsFilterCriterion(regionId.Value)).HasPropertyIn(x => region.Id);
            }

            if (pollingStationId.HasValue)
            {
                queryA = queryA.Where(() => address.PollingStation.Id == pollingStationId);
            }

            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
				return queryA.RowCount();
            }

			queryA.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => region.Id);
            return queryA.RowCount();
        }

        public long GetTotalNumberOfPeopleWithDocMissing(long? regionId, long? pollingStationId)
        {
            var queryA = FilterQuery(BaseQuery()
				.Where(x =>  x.Document.Type.Id == DocumentType.NoDocument), regionId, pollingStationId);
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return ReturnLongTypeFromQuery(AdministratorBaseQuery(queryA));
            }
            
            Region region = null;
            var queryR = RegisrtratorBaseQuery(queryA)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => region.Id);
            return ReturnLongTypeFromQuery(queryR);
        }

        public long GetTotalNumberOfPeopleWithDocExpired(long? regionId, long? pollingStationId)
        {
            var queryA = FilterQuery(BaseQuery()
                .Where(x => x.Document.ValidBy != null && x.Document.ValidBy < DateTime.UtcNow), regionId, pollingStationId);
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return ReturnLongTypeFromQuery(AdministratorBaseQuery(queryA));
            }
            
            Region region = null;
            var queryR = RegisrtratorBaseQuery(queryA)
                .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => region.Id);
            return ReturnLongTypeFromQuery(queryR);
        }

        public long GetNumberOfPeopleForAgeIntervals(int ageInterval, long? regionId, long? pollingStationId)
        {
            var minDate = new DateTime();
            var maxDate = new DateTime();
            switch (ageInterval)
            {
                case 0:
                    minDate = DateTime.Now.Date.AddYears(-18);
                    break;
                case 1:
                    maxDate = DateTime.Now.Date.AddYears(-18);
                    minDate = DateTime.Now.Date.AddYears(-41);
                    break;
                case 2:
                    maxDate = DateTime.Now.Date.AddYears(-41);
                    minDate = DateTime.Now.Date.AddYears(-66);
                    break;
                case 3:
                    maxDate = DateTime.Now.Date.AddYears(-66);
                    minDate = DateTime.Now.Date.AddYears(-76);
                    break;
                case 4:
                    maxDate = DateTime.Now.Date.AddYears(-76);
                    minDate = DateTime.Now.Date.AddYears(-91);
                    break;
                case 5:
                    maxDate = DateTime.Now.Date.AddYears(-91);
                    break;
            }
            
            var queryA = FilterQuery(BaseQuery(), regionId, pollingStationId);
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                if (ageInterval == 0 )
                {
                    return ReturnLongTypeFromQuery(AdministratorBaseQuery(queryA.Where(x => x.DateOfBirth > minDate)));
                }
                if (ageInterval == 5 )
                {
                    return ReturnLongTypeFromQuery(AdministratorBaseQuery(queryA.Where(x => x.DateOfBirth <= maxDate)));
                }
                return ReturnLongTypeFromQuery(queryA.Where(x => x.DateOfBirth > minDate && x.DateOfBirth <= maxDate));
            }
           
            Region region = null;
            var queryR = RegisrtratorBaseQuery(queryA);
            if (ageInterval == 0)
            {
                return ReturnLongTypeFromQuery(queryR.Where(x => x.DateOfBirth > minDate)
                    .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => region.Id));
            }
            if (ageInterval == 5)
            {
                return ReturnLongTypeFromQuery(queryR.Where(x => x.DateOfBirth <= maxDate)
                    .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => region.Id));
            }
            return ReturnLongTypeFromQuery(queryR.Where(x => x.DateOfBirth > minDate && x.DateOfBirth <= maxDate)
                    .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => region.Id));
        }

        public long GetNewVoters()
        {
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return 0;
            }
            
            return 0;
        }

        public long GetNewDeadPeople()
        {
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return 0;
            }
            
            return 0;
        }

        public long GetPersonalDataChanges()
        {
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return 0;
            }
            
            return 0;
        }

        public long GetAddressesChanges()
        {
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return 0;
            }
            
            return 0;
        }

        public long GetImportSuccessful()
        {
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return 0;
            }
            
            return 0;
        }

        public long GetImportFailed()
        {
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return 0;
            }
            
            return 0;
        }

        public PageResponse<PollingStationStatistics> GetStatisticsForPollingStation(PageRequest pageRequest, long? regionId, long? pollingStationId)
        {
            var result = FilterQuery(Repository.QueryOver<PollingStationStatistics>(), regionId, pollingStationId);
            if (!SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                result = result
                    .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                    .HasPropertyIn(x => x.RegionId);
            }
            return result
                .RootCriteria
                .CreatePage<PollingStationStatistics>(pageRequest);
        }

		public PageResponse<ProblematicDataPollingStationStatistics> GetStatisticsForProblematicDataPollingStation(PageRequest pageRequest)
		{
			var result = Repository.QueryOver<ProblematicDataPollingStationStatistics>();
			if (!SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
			{
				result = result
					.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
					.HasPropertyIn(x => x.RegionId);
			}
			return result
				.RootCriteria
				.CreatePage<ProblematicDataPollingStationStatistics>(pageRequest);
			
		}
        #region Import Statistics
        public PageResponse<ImportStatisticsGridDto> GetImportStatistics(PageRequest pageRequest, long? regionId)
        {
            ImportStatistic importStatistic = null;
            ImportStatisticsGridDto importData = null;
            
            var query = Repository.QueryOver(() => importStatistic);

            if (regionId != null)
            {
                query = query
                    .WithUdf(new RegionChildsFilterCriterion(regionId.Value)).HasPropertyIn(x => importStatistic.Region);
            }
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Registrator))
            {
                query = query
                    .WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                    .HasPropertyIn(x => importStatistic.Region);
            }
            
            query
                .Select(Projections.ProjectionList()
                .Add(Projections.Group<ImportStatistic>(x => x.Date).WithAlias(() => importData.ImportDateTime))
                .Add(Projections.Count<ImportStatistic>(x => x.Region).WithAlias(() => importData.RegionCount))
               ).TransformUsing(Transformers.AliasToBean<ImportStatisticsGridDto>());

            var countCriteria = query.Clone();

            countCriteria.Select(Projections.CountDistinct<ImportStatistic>(x => x.Date));
            countCriteria.Cacheable();
            
            return query.RootCriteria.CreatePage<ImportStatisticsGridDto>(pageRequest);
        }
        
        public ImportStatisticsDto GetImportStatistics(DateTime importDataTime, long regionId)
        {
            ImportStatisticsDto importDto = null;
            ImportStatistic import = null;
            return Repository.QueryOver(() => import)
                .Where(x => x.Date == importDataTime)
                .WithUdf(new RegionChildsFilterCriterion(regionId)).HasPropertyIn(x => import.Region)
                .SelectList(list => list
                    .SelectGroup(() => import.Date)
                    .SelectSum(() => import.New).WithAlias(()=> importDto.New)
                    .SelectSum(() => import.Conflicted).WithAlias(()=> importDto.Conflicted)
                    .SelectSum(() => import.Updated).WithAlias(()=> importDto.Updated)
                    .SelectSum(() => import.Error).WithAlias(()=> importDto.Error)
                    .SelectSum(() => import.Total).WithAlias(()=> importDto.Total)
                    .SelectSum(() => import.ResidenceChnaged).WithAlias(()=> importDto.ResidenceChanged)
                    .SelectSum(() => import.ChangedStatus).WithAlias(()=> importDto.ChangedStatus)
                ).TransformUsing(Transformers.AliasToBean<ImportStatisticsDto>())
                .Future<ImportStatisticsDto>()
                .SingleOrDefault()
                ;

        }

        #endregion
    }
}