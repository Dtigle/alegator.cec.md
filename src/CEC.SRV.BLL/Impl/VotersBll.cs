using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.WebPages;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider.Utils;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.BLL.Infrastructure;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Constants;
using CEC.SRV.Domain.Lookup;
using CEC.SRV.Domain.ViewItem;
using CEC.Web.SRV.Resources;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Envers.Query;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Util;
using Quartz.Util;

namespace CEC.SRV.BLL.Impl
{
    public class VotersBll : Bll, IVotersBll
    {
        private readonly ISessionFactory _sessionFactory;
        public VotersBll(ISRVRepository repository, ISessionFactory sessionFactory) : base(repository)
        {
            _sessionFactory = sessionFactory;
        }

        public PageResponse<Person> Get(PageRequest pageRequest)
        {
            PersonAddress address = null;

            return Repository.QueryOver<Person>()
                .JoinAlias(u => u.Addresses, () => address, JoinType.LeftOuterJoin)
                .Where(x => address.IsEligible)
                .Fetch(x => x.Gender).Eager
                .Fetch(x => x.PersonStatuses).Eager
                .RootCriteria
                .CreatePage<Person>(pageRequest);
        }

        public PageResponse<Person> GetIdnp(PageRequest pageRequest, string idnp)
        {
            PersonAddress address = null;
            Person person = null;
            PersonStatus personStatus = null;

            return Repository.QueryOver(() => person)
                .JoinAlias(u => u.Addresses, () => address, JoinType.LeftOuterJoin, Restrictions.Eq(Projections.Property<PersonAddress>(x => x.IsEligible), true))
                .JoinAlias(x => x.PersonStatuses, () => personStatus)
                .Where(x => x.Idnp == idnp && personStatus.IsCurrent)
                .Fetch(x => x.Gender).Eager
                .RootCriteria
                .CreatePage<Person>(pageRequest);
        }

        public PageResponse<VoterViewItem> GetByFilters(PageRequest pageRequest, long? regionId, long? pollingStationId)
        {

            VoterViewItem voter = null;

            var q = Repository.QueryOver(() => voter);

            if (SecurityHelper.LoggedUserIsInRole(Transactions.Registrator))
            {
                //                long[] userRegionIds = GetCurrentUserRegions();
                //                q = q.Where(() => voter.RegionId.IsIn(userRegionIds));

                q = q.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => voter.RegionId);

                // original version
                //                q.JoinAlias(x => personFullAddress.AssignedUser, () => identityUserRegionView)
                //                    .Where(x => identityUserRegionView.IdentityUser.Id == SecurityHelper.GetLoggedUserId());
            }

            if (regionId.HasValue)
            {
                q =
                    q.WithUdf(new RegionChildsFilterCriterion(regionId.Value))
                        .HasPropertyIn(x => voter.RegionId);
            }

            if (pollingStationId.HasValue)
            {
                q = q.Where(() => voter.PollingStationId == pollingStationId);
            }


            return q.RootCriteria
                .CreatePage<VoterViewItem>(pageRequest);
        }


        public PageResponse<VoterViewItem> GetByFilters(PageRequest pageRequest,
            long? regionId,
            long? pollingStationId,
            long? localityId = null,
            long? streetId = null,
            long? addressId = null,
            int? houseNumber = null,
            int? apNumber = null,
            string apSuffix = null,
            string surname = null,
            string excludeIdnp = null)
        {

            VoterViewItem voter = null;

            var q = Repository.QueryOver(() => voter);

            if (SecurityHelper.LoggedUserIsInRole(Transactions.Registrator))
            {
                //                long[] userRegionIds = GetCurrentUserRegions();
                //                q = q.Where(() => voter.RegionId.IsIn(userRegionIds));

                q = q.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => voter.RegionId);

                // original version
                //                q.JoinAlias(x => personFullAddress.AssignedUser, () => identityUserRegionView)
                //                    .Where(x => identityUserRegionView.IdentityUser.Id == SecurityHelper.GetLoggedUserId());
            }

            if (regionId.HasValue)
            {
                q =
                    q.WithUdf(new RegionChildsFilterCriterion(regionId.Value))
                        .HasPropertyIn(x => voter.RegionId);
            }

            if (pollingStationId.HasValue)
            {
                q = q.Where(() => voter.PollingStationId == pollingStationId);
            }

            // has address filters
            if (
                localityId.HasValue || streetId.HasValue || addressId.HasValue || houseNumber.HasValue || !string.IsNullOrEmpty(apSuffix)
                )
            {
                if (localityId.HasValue)
                {
                    q = q.Where(() => voter.RegionId == localityId.Value);
                }

                if (streetId.HasValue)
                {
                    q = q.Where(() => voter.StreeetId == streetId.Value);
                }

                if (addressId.HasValue)
                {
                    q = q.Where(() => voter.AddressId == addressId.Value);
                }

                if (houseNumber.HasValue)
                {
                    q = q.Where(() => voter.HouseNumber == houseNumber.Value);
                }

                if (apNumber.HasValue)
                {
                    if (apNumber.Value == 0)
                    {
                        q = q.Where(() => voter.ApNumber == apNumber.Value || voter.ApNumber == null);
                    }
                    else
                    {
                        q = q.Where(() => voter.ApNumber == apNumber.Value);
                    }
                }

                if (!string.IsNullOrEmpty(apSuffix))
                {
                    q = q.Where(() => voter.ApSuffix == apSuffix);
                }
            }

            if (!string.IsNullOrEmpty(surname))
            {
                q = q.Where(() => voter.Surname == surname);
            }

            if (!string.IsNullOrEmpty(excludeIdnp))
            {
                q = q.Where(() => voter.Idnp != excludeIdnp);
            }


            return q.RootCriteria
                .CreatePage<VoterViewItem>(pageRequest);

        }

        public PageResponse<VoterRow> GetByFilters2(PageRequest pageRequest,
            long? regionId,
            long? pollingStationId,
            long? localityId = null,
            long? streetId = null,
            long? addressId = null,
            int? houseNumber = null,
            int? apNumber = null,
            string apSuffix = null,
            string surname = null,
            string excludeIdnp = null
            )
        {
            pageRequest.SortFields.Add(new SortField { Ascending = true, Property = "Id" });

            PersonStatus personStatus = null;
            Person person = null;
            VoterRow voter = null;
            PersonStatusType personStatusType = null;
            PersonAddressType personAddressType = null;
            PersonFullAddress personFullAddress = null;
            PersonAddress personAddress = null;
            Address address = null;
            Street street = null;
            IdentityUserRegionView identityUserRegionView = null;
            Gender gender = null;

            var q = Repository.QueryOver(() => personFullAddress)
                .JoinAlias(x => x.Person, () => person)
                .JoinAlias(x => person.PersonStatuses, () => personStatus)
                .JoinAlias(x => personStatus.StatusType, () => personStatusType)
                .JoinAlias(x => x.PersonAddressType, () => personAddressType)
                .JoinAlias(() => person.Gender, () => gender);

            if (SecurityHelper.LoggedUserIsInRole(Transactions.Registrator))
            {
                q.JoinAlias(x => personFullAddress.AssignedUser, () => identityUserRegionView)
                    .Where(x => identityUserRegionView.IdentityUser.Id == SecurityHelper.GetLoggedUserId());
            }

            if (regionId.HasValue)
            {
                q = q.WithUdf(new RegionChildsFilterCriterion(regionId.Value)).HasPropertyIn(x => personFullAddress.Region.Id);
            }

            if (pollingStationId.HasValue)
            {
                q = q.Where(() => personFullAddress.PollingStation.Id == pollingStationId);
            }

            // has address filters
            if (
                localityId.HasValue || streetId.HasValue || addressId.HasValue || houseNumber.HasValue || !string.IsNullOrEmpty(apSuffix)
                )
            {
                q = q.JoinAlias(() => person.Addresses, () => personAddress)
                    .JoinAlias(() => personAddress.Address, () => address);


                if (localityId.HasValue)
                {
                    q = q.JoinAlias(() => address.Street, () => street)
                        .Where(() => street.Region.Id == localityId.Value);
                }

                if (streetId.HasValue)
                {
                    q = q.Where(() => address.Street.Id == streetId.Value);
                }

                if (addressId.HasValue)
                {
                    q = q.Where(() => address.Id == addressId.Value);
                }

                if (houseNumber.HasValue)
                {
                    q = q.Where(() => address.HouseNumber == houseNumber.Value);
                }

                if (apNumber.HasValue)
                {
                    if (apNumber.Value == 0)
                    {
                        q = q.Where(() => personAddress.ApNumber == apNumber.Value || personAddress.ApNumber == null);
                    }
                    else
                    {
                        q = q.Where(() => personAddress.ApNumber == apNumber.Value);
                    }
                }

                if (!string.IsNullOrEmpty(apSuffix))
                {
                    q = q.Where(() => personAddress.ApSuffix == apSuffix);
                }
            }

            if (!string.IsNullOrEmpty(surname))
            {
                q = q.Where(() => person.Surname == surname);
            }

            if (!string.IsNullOrEmpty(excludeIdnp))
            {
                q = q.Where(() => person.Idnp != excludeIdnp);
            }

            q.Where(x => personStatus.IsCurrent).And(x => x.IsEligible);

            q.Select(Projections.ProjectionList()
               .Add(Projections.Property<Person>(x => person.Id).WithAlias(() => voter.Id))
               .Add(Projections.Property<Person>(x => person.Idnp).WithAlias(() => voter.Idnp))
               .Add(Projections.Property<Person>(x => person.Surname).WithAlias(() => voter.Surname))
               .Add(Projections.Property<Person>(x => person.FirstName).WithAlias(() => voter.FirstName))
               .Add(Projections.Property<Person>(x => person.MiddleName).WithAlias(() => voter.MiddleName))
               .Add(Projections.Property<Person>(x => person.DateOfBirth).WithAlias(() => voter.BirthDate))
               .Add(Projections.Property<Person>(x => person.Age).WithAlias(() => voter.Age))
               .Add(Projections.Property<Person>(x => person.Created).WithAlias(() => voter.Created))
               .Add(Projections.Property<Person>(x => person.Modified).WithAlias(() => voter.Modified))
               .Add(Projections.Property<Person>(x => person.Deleted).WithAlias(() => voter.Deleted))
               .Add(Projections.Property<Gender>(x => gender.Name).WithAlias(() => voter.Gender))
               .Add(Projections.Property<PersonFullAddress>(x => x.FullAddress).WithAlias(() => voter.Address))
               .Add(Projections.Property<PersonFullAddress>(x => x.ApNumber).WithAlias(() => voter.ApartmentNumber))
               .Add(
                   Projections.Property<PersonFullAddress>(x => x.RegionHasStreets)
                       .WithAlias(() => voter.RegionHasStreets))
               .Add(
                   Projections.Property<PersonFullAddress>(x => x.DateOfExpiration)
                       .WithAlias(() => voter.AddressExpirationDate))
               .Add(Projections.Property<PersonAddress>(x => x.ApSuffix).WithAlias(() => voter.ApartmentSuffix))
               .Add(
                   Projections.Property<PersonDocument>(x => person.Document.DocumentNumber)
                       .WithAlias(() => voter.DocumentNumber))
               .Add(Projections.Property<PersonStatusType>(x => personStatusType.Name).WithAlias(() => voter.Status))
               .Add(Projections.Property<PersonStatusType>(x => personStatusType.Id).WithAlias(() => voter.StatusId))
               .Add(
                   Projections.Property<PersonAddressType>(x => personAddressType.Name)
                       .WithAlias(() => voter.AddressType))
               .Add(
                   Projections.Property<PersonAddressType>(x => personAddressType.Id)
                       .WithAlias(() => voter.AddressTypeId))
               ).TransformUsing(Transformers.AliasToBean<VoterRow>());

            return q.RootCriteria
                .CreatePage<VoterRow>(pageRequest);
        }

        public Person GetByIdnp(string idnp)
        {
            PersonAddress address = null;
            Person person = null;
            PersonStatus personStatus = null;

            return Repository.QueryOver(() => person)
                .JoinAlias(u => u.Addresses, () => address, JoinType.LeftOuterJoin)

                .JoinAlias(x => x.PersonStatuses, () => personStatus)
                .Where(x => x.Idnp == idnp)
                .SingleOrDefault();
        }

        /// <summary>
        /// Voters per apartament/ap.suffix
        /// </summary>
        /// <returns></returns>
        public int GetVotersCount(PersonAddress address)
        {
            if (address == null ||
                address.Address == null)
                return 0;

            PersonAddress addr = null;
            Person person = null;
            PersonStatus personStatus = null;
            PersonStatusType statusType = null;

            var statusSubQuery = QueryOver.Of<PersonStatus>()
                .Where(x => x.Person.Id == person.Id)
                .Select(Projections.Max<PersonStatus>(x => x.Id));

            return Repository.QueryOver<PersonAddress>()
                .JoinAlias(x => x.Address, () => addr)
                .JoinAlias(x => x.Person, () => person)
                .JoinAlias(() => person.PersonStatuses, () => personStatus)
                .JoinAlias(() => personStatus.StatusType, () => statusType)
                .Where(x => x.ApNumber == address.ApNumber &&
                            x.ApSuffix == address.ApSuffix &&
                            addr.Id == address.Address.Id &&
                            personStatus.IsCurrent && !statusType.IsExcludable)
                .Select(Projections.RowCount())
                .FutureValue<int>()
                .Value;
        }

        public string UpdateStatus(long personId, long statusId, string confirmation)
        {
            var person = Get<Person>(personId);
            var status = Get<PersonStatusType>(statusId);
            person.ModifyStatus(status, confirmation);


            var errorMessage = "";
            try
            {
                Repository.SaveOrUpdate(person);
            }
            catch (Exception e)
            {
                errorMessage = "A avut loc o eroare la updatarea statutului.";
            }


            var regionId = person.EligibleAddress.Address.Street.Region.Id;
            var users = Repository.QueryOver<SRVIdentityUser>()
                .WithUdf(new UsersWithAccessToRegionCriterion(regionId))
                .HasPropertyIn(x => x.Id)
                .List();

            var notificationMessage = string.Format(MUI.Notification_PersonStatus_Change, person.Idnp, confirmation);
            CreateNotification(EventTypes.Update, person, person.Id, notificationMessage, users);

            if (errorMessage.Length > 0)
            {
                return errorMessage;
            }
            return notificationMessage;
        }

        public PageResponse<Region> SearchRegion(PageRequest pageRequest)
        {
            var query = Repository.QueryOver<Region>();
            if (!SecurityHelper.LoggedUserIsInRole("Administrator"))
            {
                query.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => x.Id);
            }

            return query.RootCriteria.CreatePage<Region>(pageRequest);
        }

        public PageResponse<Election> SearchActiveElections(PageRequest pageRequest, DateTime? electionDate)
        {
            PageResponse<Election> result = null;
            if (electionDate.HasValue)
            {
                var initialDate = electionDate.Value.Date;
                var finalDate = new DateTime(electionDate.Value.Year, electionDate.Value.Month, electionDate.Value.Day, 23, 59, 59);

                var subquery = Repository.Query<ElectionRound>()
                  .Where(x => x.ElectionDate >= initialDate && x.ElectionDate <= finalDate)
                  .Select(x => x.Election.Id);

                result = Repository.QueryOver<Election>()
                     .WhereRestrictionOn(t => t.Id).IsInG(subquery)
                     .OrderBy(x => x.Id).Desc
                     .RootCriteria.CreatePage<Election>(pageRequest);
            }
            else
            {
                result = Repository.QueryOver<Election>().Fetch(s => s.ElectionRounds).Eager
                     .Where(x => x.Deleted == null)
                     .OrderBy(x => x.ElectionRounds).Desc
                     .TransformUsing(Transformers.DistinctRootEntity)
                     .RootCriteria.CreatePage<Election>(pageRequest);
            }
            return result;
        }

        public PageResponse<ElectionRound> SearchElectionRounds(PageRequest pageRequest, long electionId)
        {
            var result = Repository.QueryOver<ElectionRound>()
                .Where(x => x.Deleted == null && x.Election.Id == electionId)
                .OrderBy(x => x.ElectionDate).Asc
                .RootCriteria.CreatePage<ElectionRound>(pageRequest);
            result.Items = result.Items;
            return result;
        }

        public PageResponse<PollingStation> SearchPollingStations(PageRequest pageRequest, long? regionId)
        {
            var query = Repository.QueryOver<PollingStation>()
            .Where(x => x.Deleted == null);

            if (regionId.HasValue)
            {
                if (IsRegionAccessibleToCurrentUser(regionId.Value))
                {
                    query.WithUdf(new RegionChildsFilterCriterion(regionId.Value)).HasPropertyIn(x => x.Region.Id);
                }
                return query.RootCriteria.CreatePage<PollingStation>(pageRequest);
            }
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Registrator))
            {
                query.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                    .HasPropertyIn(x => x.Region.Id);
            }
            query.OrderBy(x => x.FullNumber).Asc();
            return query.RootCriteria.CreatePage<PollingStation>(pageRequest);
        }
        public PageResponse<PollingStation> SearchPollingStations(PageRequest pageRequest, long? regionId, string q)
        {
            var query = Repository.QueryOver<PollingStation>()
                .Where(x => x.Deleted == null);

            if (q != null && !q.IsEmpty())
            {
                query = query.Where(
                        Restrictions.On<PollingStation>(x => x.FullNumber).IsInsensitiveLike(q, MatchMode.Anywhere) ||
                        Restrictions.On<PollingStation>(x => x.Location).IsInsensitiveLike(q, MatchMode.Anywhere)
                );
            }

            if (regionId.HasValue)
            {
                if (IsRegionAccessibleToCurrentUser(regionId.Value))
                {
                    query.WithUdf(new RegionChildsFilterCriterion(regionId.Value)).HasPropertyIn(x => x.Region.Id);
                }
                return query.RootCriteria.CreatePage<PollingStation>(pageRequest);
            }
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Registrator))
            {
                query.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId()))
                    .HasPropertyIn(x => x.Region.Id);
            }
            query.OrderBy(x => x.FullNumber).Asc();
            return query.RootCriteria.CreatePage<PollingStation>(pageRequest);
        }

        public PageResponse<AddressWithPollingStation> SearchAddress(PageRequest pageRequest, long? regionId, long? streetId = null)
        {
            var query = Repository.QueryOver<AddressWithPollingStation>();

            if (regionId.HasValue)
            {
                query.Where(x => x.RegionId == regionId.Value);
            }

            if (pageRequest.SortFields.Count == 0)
            {
                pageRequest.SortFields.Add(new SortField()
                {
                    Ascending = true,
                    Property = "Address.Street.Name"
                });
                pageRequest.SortFields.Add(new SortField()
                {
                    Ascending = true,
                    Property = "Address.HouseNumber"
                });
            }

            if (streetId.HasValue)
            {
                query.Where(x => x.StreetId == streetId.Value);
            }

            if (!SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                query.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => x.RegionId);
            }

            return query.RootCriteria.CreatePage<AddressWithPollingStation>(pageRequest);
        }

        public PageResponse<StayStatement> GetStayStatementForPerson(PageRequest pageRequest, long personId)
        {
            return Repository.QueryOver<StayStatement>()
                .Where(x => x.Person.Id == personId)
                .RootCriteria
                .CreatePage<StayStatement>(pageRequest);
        }

        public PageResponse<StreetView> SearchStreets(PageRequest pageRequest, long? regionId, long? streetId = null)
        {
            var query = Repository.QueryOver<StreetView>();

            if (regionId.HasValue)
            {
                query.Where(x => x.RegionId == regionId.Value);
            }

            if (streetId.HasValue)
            {
                query.Where(x => x.StreetId == streetId.Value);
            }

            if (!SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                query.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => x.RegionId);
            }

            return query.RootCriteria.CreatePage<StreetView>(pageRequest);
        }

        public PageResponse<StayStatement> GetStayStatements(PageRequest pageRequest)
        {
            if (SecurityHelper.LoggedUserIsInRole(Transactions.Administrator))
            {
                return Repository.QueryOver<StayStatement>()
                    .RootCriteria
                    .CreatePage<StayStatement>(pageRequest);
            }

            StayStatement stayStatment = null;
            PersonFullAddress personFullAddress = null;
            IdentityUserRegionView identityUser = null;

            var dt = QueryOver.Of(() => personFullAddress)
                .JoinAlias(x => x.AssignedUser, () => identityUser)
                .Where(
                    x =>
                        personFullAddress.Id == stayStatment.BaseAddress.Id ||
                        personFullAddress.Id == stayStatment.DeclaredStayAddress.Id)
                .And(x => identityUser.IdentityUser.Id == SecurityHelper.GetLoggedUserId())
                .Select(Projections.Constant("1"));

            return Repository.QueryOver(() => stayStatment)
                .WithSubquery.WhereExists(dt)
                .RootCriteria
                .CreatePage<StayStatement>(pageRequest);
        }

        public long CreateStayStatement(long id, long personId, long addressId, int? apNumber, string apSuffix, long electionId)
        {
            var person = Repository.LoadProxy<Person>(personId);
            var address = Repository.LoadProxy<Address>(addressId);
            var election = Repository.LoadProxy<Election>(electionId);

            var declaredStayAddress = new PersonAddress
            {
                Person = person,
                Address = address,
                IsEligible = true,
                ApNumber = apNumber,
                ApSuffix = apSuffix,
                PersonAddressType = Repository.LoadProxy<PersonAddressType>(3)
            };

            var stayStatement = new StayStatement(person, person.EligibleAddress, declaredStayAddress, election);
            person.SetEligibleAddress(stayStatement);

            Repository.SaveOrUpdate(stayStatement);

            var regionOfBaseAddressId = stayStatement.BaseAddress.Address.Street.Region.Id;
            var users1 = Repository.QueryOver<SRVIdentityUser>()
                .WithUdf(new UsersWithAccessToRegionCriterion(regionOfBaseAddressId))
                .HasPropertyIn(x => x.Id)
                .List();
            var regionOfDeclaredAddressId = stayStatement.DeclaredStayAddress.Address.Street.Region.Id;
            var users2 = Repository.QueryOver<SRVIdentityUser>()
                .WithUdf(new UsersWithAccessToRegionCriterion(regionOfDeclaredAddressId))
                .HasPropertyIn(x => x.Id)
                .List();

            var users = users1.Union(users2).Distinct();
            string notificationMessage = string.Format(MUI.Notification_StayStatement_Creation, stayStatement.Id, stayStatement.Person.Idnp);
            CreateNotification(EventTypes.New, stayStatement, stayStatement.Id, notificationMessage, users);
            return stayStatement.Id;
        }

        public long CreateStayStatement(long personId, long pollingStationId, long electionId)
        {
            var pollingStation = Get<PollingStation>(pollingStationId);
            var person = Get<Person>(personId);
            var unknownStreetType = Get<StreetType>(StreetType.UnknownStreetType);
            var virtualStreetName = string.Format("${0}", pollingStation.Id);
            var election = Repository.LoadProxy<Election>(electionId);

            var baseAddress = pollingStation.Addresses
                .FirstOrDefault(x => x.Street.StreetType.Id == StreetType.UnknownStreetType &&
                                     x.Street.Name == virtualStreetName &&
                                     x.HouseNumber.GetValueOrDefault() == 0);

            var street = new Street(pollingStation.Region, unknownStreetType, virtualStreetName, true)
            {
                Description = string.Format("Autogenerated for PS: {0}", pollingStation.FullNumber)
            };


            if (baseAddress == null)
            {
                baseAddress = new Address { Street = street, PollingStation = pollingStation };
                pollingStation.AddAddress(baseAddress);
                Repository.SaveOrUpdate(street);
                Repository.SaveOrUpdate(baseAddress);
                Repository.SaveOrUpdate(pollingStation);
            }

            var declaredStayAddress = new PersonAddress
            {
                Person = person,
                Address = baseAddress,
                IsEligible = true,
                PersonAddressType = Repository.LoadProxy<PersonAddressType>(3)
            };

            var stayStatement = new StayStatement(person, person.EligibleAddress, declaredStayAddress, election);
            person.SetEligibleAddress(stayStatement);

            Repository.SaveOrUpdate(stayStatement);

            var regionOfBaseAddressId = stayStatement.BaseAddress.Address.Street.Region.Id;
            var users1 = Repository.QueryOver<SRVIdentityUser>()
                .WithUdf(new UsersWithAccessToRegionCriterion(regionOfBaseAddressId))
                .HasPropertyIn(x => x.Id)
                .List();
            var regionOfDeclaredAddressId = stayStatement.DeclaredStayAddress.Address.Street.Region.Id;
            var users2 = Repository.QueryOver<SRVIdentityUser>()
                .WithUdf(new UsersWithAccessToRegionCriterion(regionOfDeclaredAddressId))
                .HasPropertyIn(x => x.Id)
                .List();

            var users = users1.Union(users2).Distinct();
            string notificationMessage = string.Format(MUI.Notification_StayStatement_Creation, stayStatement.Id, stayStatement.Person.Idnp);
            CreateNotification(EventTypes.New, stayStatement, stayStatement.Id, notificationMessage, users);
            return stayStatement.Id;
        }

        public bool ElectionUniqueStayStatement(long personId, long electionId)
        {
            var stayStatement =
                Repository.Query<StayStatement>()
                    .Where(x => x.Person.Id == personId && x.ElectionInstance.Id == electionId && x.DeclaredStayAddress.IsEligible);
            return stayStatement.Any();
        }

        public void CancelStayStatement(long stayStatementId)
        {
            var stayStatement = Repository.LoadProxy<StayStatement>(stayStatementId);

            var regionOfDeclaredAddressId = stayStatement.DeclaredStayAddress.Address.Street.Region.Id;
            var users2 = Repository.QueryOver<SRVIdentityUser>()
                .WithUdf(new UsersWithAccessToRegionCriterion(regionOfDeclaredAddressId))
                .HasPropertyIn(x => x.Id)
                .List();

            var usersIds = users2.Select(x => x.Id);
            if (!SecurityHelper.LoggedUserIsInRole("Administrator") && !SecurityHelper.LoggedUserIsInRole("System"))
            {
                var loggedUserId = SecurityHelper.GetLoggedUserId();
                if (!usersIds.Contains(loggedUserId))
                {
                    throw new SrvException("StayStatement_CancellationDenied_Message", MUI.StayStatement_CancellationDenied_Message);
                }
            }

            // reset DeclaredAddress to NOT Eligible
            stayStatement.DeclaredStayAddress.IsEligible = false;
            Repository.SaveOrUpdate(stayStatement.DeclaredStayAddress);

            // restoring BaseAddress (previous) as Eligible
            stayStatement.BaseAddress.IsEligible = true;
            Repository.SaveOrUpdate(stayStatement.BaseAddress);

            Delete<StayStatement>(stayStatementId);
            //  create notification
            var regionOfBaseAddressId = stayStatement.BaseAddress.Address.Street.Region.Id;
            var users1 = Repository.QueryOver<SRVIdentityUser>()
                .WithUdf(new UsersWithAccessToRegionCriterion(regionOfBaseAddressId))
                .HasPropertyIn(x => x.Id)
                .List();


            var users = users1.Union(users2).Distinct();
            string notificationMessage = string.Format(MUI.Notification_StayStatement_Cancellation, stayStatement.Id, stayStatement.Person.Idnp);
            CreateNotification(EventTypes.Delete, stayStatement, stayStatementId, notificationMessage, users);
        }

        public string UpdateAddress(long personId, long addressId, int? apNumber, string apSufix)
        {
            var person = Get<Person>(personId);

            var oldEligibleAddress = person.EligibleAddress.GetFullPersonAddress(true);
            var regionOfBaseAddressId = person.EligibleAddress.Address.Street.Region.Id;
            var address = Get<Address>(addressId);

            if (address.PollingStation == null)
            {
                throw new SrvException("Voters_ChangeAddressError_NoPollingStation", MUI.Voters_ChangeAddressError_NoPollingStation);
            }


            person.EligibleAddress.Address = address;
            person.EligibleAddress.ApNumber = apNumber;
            person.EligibleAddress.ApSuffix = apSufix;

            var errorMessage = "";
            try
            {
                Repository.SaveOrUpdate(person);
            }
            catch (Exception e)
            {
                errorMessage = "A avut loc o eroare la updatarea adresei.";
            }

            var users1 = Repository.QueryOver<SRVIdentityUser>()
                .WithUdf(new UsersWithAccessToRegionCriterion(regionOfBaseAddressId))
                .HasPropertyIn(x => x.Id)
                .List();
            var regionOfDeclaredAddressId = person.EligibleAddress.Address.Street.Region.Id;
            var users2 = Repository.QueryOver<SRVIdentityUser>()
                .WithUdf(new UsersWithAccessToRegionCriterion(regionOfDeclaredAddressId))
                .HasPropertyIn(x => x.Id)
                .List();

            var users = users1.Union(users2).Distinct();
            string notificationMessage = string.Format(MUI.Notification_PersonAddress_Change, person.Idnp, oldEligibleAddress, person.EligibleAddress.GetFullPersonAddress(true));
            CreateNotification(EventTypes.Update, person, person.Id, notificationMessage, users);

            if (errorMessage.Length > 0)
            {
                return errorMessage;
            }
            return notificationMessage;
        }

        public Person GetPersonWithEligibleResidence(long personId)
        {
            var person = Get<Person>(personId);
            if (person.EligibleAddress.IsEligible && person.EligibleAddress.PersonAddressType.Id == 3)
            {
                throw new SrvException("Voters_NotDeletePermisions", MUI.Voters_NotDeletePermisions);
            }

            var regionId = person.EligibleAddress.Address.Street.Region.Id;
            var region = Get<Region>(regionId);
            if (!region.HasStreets)
            {
                throw new SrvException("ChangeAddress_NotPermitted", MUI.ChangeAddress_NotPermitted);
            }
            return person;
        }

        public IList<Election> GetElection()
        {
            return Repository.QueryOver<Election>().Where(x => x.Deleted == null).List();
        }

        public bool VerificationSameAddress(long personAddressId, long declaredAddressId)
        {
            var personAddress = Get<PersonAddress>(personAddressId);
            var declaredAddress = Get<Address>(declaredAddressId);
            return personAddress.Address.Street.Region.Id == declaredAddress.Street.Region.Id;
        }

        public bool VerificationSameRegion(long personAddressId, long regionId)
        {
            var region = Get<Region>(regionId);
            var personAddress = Get<PersonAddress>(personAddressId);
            return region.Id == personAddress.Address.Street.Region.Id;
        }

        public long SaveAbroadVoterRegistration(long personId, string abroadAddress, string residenceAddress, string abroadAddresCountry, double abroadAddressLat, double abroadAddressLong, string email, string ipAddress)
        {
            var person = Repository.Get<Person>(personId);

            var abroadVR = new AbroadVoterRegistration(person, abroadAddress, residenceAddress, abroadAddresCountry, abroadAddressLat, abroadAddressLong, email, ipAddress);

            Repository.SaveOrUpdate(abroadVR);

            //// 3 -> lookup id of Parlament election
            //var parliamentElectionType = Repository.Get<ElectionType>(3);
            //Election election;
            //if(parliamentElectionType != null)
            //election = Repository.Query<Election>()
            //    .Where(x => x.ElectionType == parliamentElectionType)
            //    .OrderByDescending(o => o.ElectionDate)
            //    .First();

            //if(election != null)
            //{

            //}

            return abroadVR.Id;
        }

        public List<AbroadVoterRegistration> GetAbroadVotersAddress()
        {
            return Repository.Query<AbroadVoterRegistration>().Where(x => x.AbroadAddressLat != null && x.AbroadAddressLong != null).ToList<AbroadVoterRegistration>();
        }

        public IList<CountryStatisticGroupedDto> GetRegionOfVotes(string country = null)
        {
            //var result = new Dictionary<string, int>();
            CountryStatisticGroupedDto dto = null;

            return Repository.QueryOver<AbroadVoterRegistration>()
                .SelectList(list => list
                    .SelectGroup(g => g.AbroadAddressCountry).WithAlias(() => dto.Country)
                    .SelectCount(g => g.Id).WithAlias(() => dto.Count)
                )
                .TransformUsing(Transformers.AliasToBean<CountryStatisticGroupedDto>())
                .List<CountryStatisticGroupedDto>();

            //foreach (var item in queryResult)
            //    result.Add(item[0].ToString(), (int) item[1]);

            //return result;
        }


        public IList<StatisticTimelineDto> GetAbroadVotersTimeline()
        {
            StatisticTimelineDto dto = null;

            return Repository.QueryOver<AbroadVoterRegistration>().SelectList(list => list
                .SelectGroup(g => g.CreationDate).WithAlias(() => dto.Date)
                .SelectCount(g => g.Id).WithAlias(() => dto.Count))
                .TransformUsing(Transformers.AliasToBean<StatisticTimelineDto>())
                .List<StatisticTimelineDto>();


        }

        public bool IsRegisteredToElection(long id)
        {
            return Repository.QueryOver<AbroadVoterRegistration>().Where(x => x.Person.Id == id).RowCount() != 0;
        }

        public void UpdatePollingStation(long personId, long pollingStationId)
        {
            var pollingStation = Get<PollingStation>(pollingStationId);
            var person = Get<Person>(personId);
            var oldPollingStation = (person.EligibleAddress != null) ? person.EligibleAddress.Address.PollingStation : null;
            var oldPollingStationNumber = oldPollingStation != null ? oldPollingStation.FullNumber : "[N/A]";
            var unknownStreetType = Get<StreetType>(StreetType.UnknownStreetType);
            var virtualStreetName = string.Format("${0}", pollingStation.Id);

            var baseAddress = pollingStation.Addresses
                .FirstOrDefault(x => x.Street.StreetType.Id == StreetType.UnknownStreetType &&
                                     x.Street.Name == virtualStreetName &&
                                     x.HouseNumber.GetValueOrDefault() == 0);
            if (baseAddress == null)
            {
                var street = new Street(pollingStation.Region, unknownStreetType, virtualStreetName, true);
                street.Description = string.Format("Autogenerated for PS: {0}", pollingStation.FullNumber);
                baseAddress = new Address { Street = street, PollingStation = pollingStation };
                pollingStation.AddAddress(baseAddress);
                Repository.SaveOrUpdate(street);
                Repository.SaveOrUpdate(baseAddress);
                Repository.SaveOrUpdate(pollingStation);
            }

            if (person.EligibleAddress != null)
            {
                person.EligibleAddress.Address = baseAddress;
            }
            else
            {
                var personAddress = new PersonAddress
                {
                    Address = baseAddress,
                    Person = person,
                    PersonAddressType = Get<PersonAddressType>(PersonAddressType.Residence)
                };

                Repository.SaveOrUpdate(personAddress);

                person.SetEligibleAddress(personAddress);
            }
            Repository.SaveOrUpdate(person);


            var users = Repository.QueryOver<SRVIdentityUser>()
                .WithUdf(new UsersWithAccessToRegionCriterion(pollingStation.Region.Id))
                .HasPropertyIn(x => x.Id)
                .List();

            string notificationMessage = string.Format(MUI.Notification_PersonPollingStation_Change,
                person.Idnp, oldPollingStationNumber, pollingStation.FullNumber);
            CreateNotification(EventTypes.Update, person, person.Id, notificationMessage, users);
        }

        public IList<PollingStation> GetRegionPollingStationsByPerson()
        {
            var query = Repository.QueryOver<PollingStation>();

            if (SecurityHelper.LoggedUserIsInRole(Transactions.Registrator))
            {
                query.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => x.Region.Id);
            }

            return query.List();
        }

        public void VerificationSameRegion(long[] peopleIds)
        {
            var firstPerson = Get<Person>(peopleIds[0]);
            var firstPersonRegionId = firstPerson.EligibleAddress.Address.Street.Region.Id;
            foreach (var personId in peopleIds)
            {
                var person = Get<Person>(personId);


                var regionId = person.EligibleAddress.Address.Street.Region.Id;

                if (person.EligibleAddress.IsEligible && person.EligibleAddress.PersonAddressType.Id == 3)
                {
                    throw new SrvException("Voters_NotDeletePermisions", MUI.Voters_NotDeletePermisions);
                }
                var region = Get<Region>(regionId);
                if (region.HasStreets)
                {
                    throw new SrvException("ChangePollingStation_NotPermitted",
                        MUI.ChangePollingStation_NotPermitted);
                }

                if (firstPersonRegionId != regionId)
                {
                    throw new SrvException("ChangeVotersData_NotPermitted", MUI.ChangeVotersData_NotPermitted);
                }
            }
        }

        public void ChangePollingStation(long pollingStationId, List<long> peopleIds)
        {
            foreach (var personId in peopleIds)
            {
                UpdatePollingStation(personId, pollingStationId);
            }
        }

        public RegionStreetsType GetRegionStreetsType()
        {
            var query = Repository.QueryOver<Region>();
            if (!SecurityHelper.LoggedUserIsInRole("Administrator"))
            {
                query.WithUdf(new AccessibleRegionsCriterion(SecurityHelper.GetLoggedUserId())).HasPropertyIn(x => x.Id);
            }
            var regions = query.List();

            var withOutStreets = regions.Count(x => !x.HasStreets);
            var withStreets = regions.Count(x => x.HasStreets);

            //var withOutStreets = regions.Select(x => x.HasStreets).Count();
            //var withStreets = regions.Select(x => x.HasStreets == false).Count();

            if (withStreets > 0 && withOutStreets > 0)
            {
                return RegionStreetsType.Mixed;
            }
            return withStreets > 0 ? RegionStreetsType.WithStreets : RegionStreetsType.WithoutStreets;
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
        public async Task AdddElectionNumberList(string query)
        {
            await Task.Run(() =>
            {
                using (IStatelessSession session = _sessionFactory.OpenStatelessSession())
                {
                    session.CreateSQLQuery("Execute dbo.People_AddElectionListNumber @query= :newValue ")
                        .SetString("newValue", query)
                        .ExecuteUpdate();
                }

            }).ConfigureAwait(false);
        }

        #region Voter's profile
        public PageResponse<PersonAddress> GetAddressHistory(PageRequest pageRequest, long personId)
        {
            // aceasta implementare nu tine cont de istoricul modificarilor de adresa

            //            return Repository.QueryOver<PersonAddress>()
            //                .Where(x => x.Person.Id == personId)
            //                .RootCriteria
            //                .CreatePage<PersonAddress>(pageRequest);

            var q = ((SrvRepository)Repository).GetAuditer().CreateQuery()
                .ForRevisionsOfEntity(typeof(PersonAddress), true, false)
                .Add(AuditEntity.Property("Person.Id").Eq(personId));

            if (pageRequest != null && pageRequest.SortFields.Count > 0)
                foreach (var sf in pageRequest.SortFields)
                {
                    if (sf.Ascending)
                    {
                        q.AddOrder(AuditEntity.Property(sf.Property).Asc());
                    }
                    else
                    {
                        q.AddOrder(AuditEntity.Property(sf.Property).Desc());
                    }
                }
            else
            {
                q.AddOrder(AuditEntity.RevisionNumber().Asc());
            }

            var list = q.GetResultList<PersonAddress>();

            foreach (var personAddress in list)
            {
                personAddress.PersonAddressType = Repository.Get<PersonAddressType>(personAddress.PersonAddressType.Id);
                personAddress.Address = Repository.Get<Address>(personAddress.Address.Id);
            }

            return new PageResponse<PersonAddress>
            {
                Items = list,
                StartIndex = 1,
                PageSize = list.Count,
                Total = list.Count
            };
        }

        public PageResponse<PollingStation> GetPollingStationHistory(PageRequest pageRequest, long personId)
        {
            // aceasta implementare nu tine cont de istoricul modificarilor de adresa

            //            var stations = Repository.Query<PersonAddress>()
            //                .Where(x => x.Person.Id == personId && x.Address.PollingStation != null)
            //                .Select(x => x.Address.PollingStation.Id)
            //                .ToArray();

            PageResponse<PersonAddress> addresses = GetAddressHistory(null, personId);

            List<long> psIds = new List<long>();
            foreach (var address in addresses.Items)
            {
                if (address.Address?.PollingStation != null)
                {
                    psIds.Add(address.Address.PollingStation.Id);
                }
            }

            return Repository.QueryOver<PollingStation>()
                .WhereRestrictionOn(x => x.Id).IsIn(psIds)
                .RootCriteria.CreatePage<PollingStation>(pageRequest);
        }

        public PageResponse<PersonDocument> GetIdentityDocumentsHistory(long personId)
        {
            var list = ((SrvRepository)Repository).GetAuditer().CreateQuery()
                .ForRevisionsOfEntity(typeof(Person), true, false)
                .Add(AuditEntity.Id().Eq(personId))
                .AddOrder(AuditEntity.Property("Modified").Desc())
                .GetResultList();

            var documents = new PageResponse<PersonDocument>();
            documents.Items = new List<PersonDocument>(list.Count);


            var person = Repository.Get<Person>(personId);
            if (person != null)
            {
                list.Add(person);
            }

            foreach (Person p in list)
            {
                if (p.Document == null) continue;
                var doc = new PersonDocument();

                var skip =
                    documents.Items.Any(
                        item => item.Seria.Equals(p.Document.Seria) && item.Number.Equals(p.Document.Number));
                if (skip) continue;

                doc.Seria = p.Document.Seria;
                doc.Number = p.Document.Number;
                doc.IssuedDate = p.Document.IssuedDate;
                doc.IssuedBy = p.Document.IssuedBy;
                doc.ValidBy = p.Document.ValidBy;
                doc.Type = p.Document.Type;
                documents.Items.Add(doc);
            }
            documents.Total = documents.Items.Count;
            documents.PageSize = documents.Items.Count;
            documents.StartIndex = 1;

            return documents;
        }
        #endregion
    }
}