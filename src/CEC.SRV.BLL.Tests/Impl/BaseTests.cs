using System;
using System.Linq;
using Amdaris.Domain.Paging;
using Amdaris.NHibernateProvider.Test;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.BLL.ReportService;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain.Print;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using System.Collections.Generic;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.SRV.BLL.Dto;
using NHibernate;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Importer.ToSaise;
using System.Reflection;
using System.Linq.Expressions;
using NHibernate.Linq;
using NHibernate.Persister.Entity;
using Moq;

namespace CEC.SRV.BLL.Tests.Impl
{
    public class BaseTests<TBll, TRep> : BaseRepositoryTests where TRep : class, IRepository
    {
        #region Fields

        public TRep Repository;
        public TBll Bll;
        public bool IsMockedRepository;
        private readonly Dictionary<Type, long> _ids;
        private readonly Dictionary<Type, Dictionary<long, IEntity>> _db;
        private Mock<TRep> _mockRepository;

        #endregion Fields

        #region Initialize

        public BaseTests()
        {
            _ids = new Dictionary<Type, long>();
            _db = new Dictionary<Type, Dictionary<long, IEntity>>();
            IsMockedRepository = false;
        }

        [TestInitialize]
        public void Startup()
        {
            _db.ForEach(x => x.Value.Clear());
            if (!IsMockedRepository)
            {
                InitializeMockedRepositoryAndBll();
            }
        }

        public void InitializeMockedRepositoryAndBll()
        {
            Repository = CreateRepository();
            Bll = CreateBll<TBll>();
            IsMockedRepository = true;
        }

        public TBll InitializeRepositoryAndBll<TR>() where TR : class, IRepository
        {
            Repository = CreateRealRepository<TR>();
            Bll = CreateBll<TBll>();
            IsMockedRepository = false;
            return Bll;
        }

        public TB InitializeRepositoryAndBll<TR, TB>() where TR : class, IRepository where TB : TBll
        {
            Repository = CreateRealRepository<TR>();
            TB bll = CreateBll<TB>();
            Bll = bll;
            IsMockedRepository = false;
            return bll;
        }

        private TRep CreateRepository()
        {
            _mockRepository = new Mock<TRep>();
            MockAllDbs();
            return _mockRepository.Object;
        }

        private void MockAllDbs()
        {
            MockDb<Gender>();
            MockDb<DocumentType>();
            MockDb<PersonStatusType>();
            MockDbEntity<RspModificationData>();
            MockDbEntity<RspRegistrationData>();
            MockDb<Person>();
            MockDb<Region>();
            MockDb<Address>();
            MockDb<Street>();
            MockDb<PersonAddress>();
            MockDb<RegionType>();
            MockDb<StreetType>();
            MockDb<PersonAddressType>();
            MockDb<LinkedRegion>();
            MockDb<PollingStation>();
            MockDbEntity<MappingAddress>();
            MockDb<PublicAdministration>();
            MockDb<ManagerType>();
            MockDb<ElectionType>();
            MockDb<Election>();
            MockDb<NotificationReceiver>();
            MockDb<Notification>();
            MockDb<Event>();
            MockDb<PersonStatus>();
            MockDbEntity<AbroadVoterRegistration>();
            MockDb<SaiseExporter>();
            MockDbEntity<PrintSession>();
            MockDbEntity<ExportPollingStation>();
            MockDbEntity<StreetTypeCode>();
            MockDb<SaiseExporterStage>();
            MockDbEntity<RsaUser>();
            MockDb<AdditionalUserInfo>();
            MockDb<StayStatement>();
            MockDbEntity<PersonFullAddress>();
            MockDbEntity<PollingStationStatistics>();
            MockDbEntity<PersonByConflict>();
            MockDbEntity<RspConflictData>();
            MockDbEntity<RspConflictDataAdmin>();
            MockDbEntity<ImportStatistic>();
            MockDbEntity<RegionWithFullyQualifiedName>();
            MockDb2<IdentityRole>();
            MockDb2<SRVIdentityUser>();
        }

        public TRep CreateRealRepository<TR>()
        {
            var constructorInfo = typeof(TR).GetConstructor(new[] { typeof(ISessionFactory) });

            if (constructorInfo != null)
            {
                return (TRep)(constructorInfo.Invoke(new object[] { SessionFactory }));
            }
            return null;
        }

        public T CreateBll<T>() where T : TBll
        {
            try
            {
                var constructorInfo = typeof(T).GetConstructor(new[] { typeof(TRep) });

                if (constructorInfo != null)
                {
                    Bll = (T)(constructorInfo.Invoke(new object[] { Repository }));
                }

                return (T)Bll;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        private void MockDbEntity<T>() where T : Entity
        {
            var type = typeof(T);
            var list = new Dictionary<long, IEntity>();

            if (!_db.ContainsKey(type))
            {
                _db.Add(type, list);
            }
            else
            {
                list = _db[type];
            }

            if (!_ids.ContainsKey(type))
            {
                _ids.Add(type, 0);
            }

            _mockRepository.Setup(x => x.Query<T>()).Returns(() => new EnumerableQuery<T>(list.Values.Cast<T>()));
            _mockRepository.Setup(x => x.Get<T>(It.IsAny<long>())).Returns((long id) => list.Values.Cast<T>().FirstOrDefault(x => x.Id == id));
            _mockRepository.Setup(x => x.LoadProxy<T>(It.IsAny<long>())).Returns((long id) => list.Values.Cast<T>().FirstOrDefault(x => x.Id == id));
            _mockRepository.Setup(x => x.SaveOrUpdate(It.IsAny<T>())).Callback((T entity) => OurSaveOrUpdate(entity));
            _mockRepository.Setup(x => x.Delete(It.IsAny<T>())).Callback((T entity) => OurDelete<T>(entity.Id));
            _mockRepository.Setup(x => x.Page<T>(It.IsAny<PageRequest>())).Returns((PageRequest request) => OurPage<T>(request));

        }

        private void MockDb<T>() where T : SoftEntity<IdentityUser>
        {
            MockDbEntity<T>();

            var srvRepository = _mockRepository as Mock<ISRVRepository>;
            if (srvRepository == null) return;
            srvRepository.Setup(x => x.UnDelete<T>(It.IsAny<long>())).Callback((long id) => OurUnDelete<T>(id));
        }

        private void MockDb2<T>() where T : EntityWithTypedId<string>
        {
            var type = typeof(T);
            var list = new Dictionary<long, IEntity>();

            if (!_db.ContainsKey(type))
            {
                _db.Add(type, list);
            }
            else
            {
                list = _db[type];
            }

            if (!_ids.ContainsKey(type))
            {
                _ids.Add(type, 0);
            }

            _mockRepository.Setup(x => x.Query<T>()).Returns(() => new EnumerableQuery<T>(list.Values.Cast<T>()));
            _mockRepository.Setup(x => x.Get<T, string>(It.IsAny<string>())).Returns((string id) => list.Values.Cast<T>().FirstOrDefault(x => x.Id == id));
            _mockRepository.Setup(x => x.SaveOrUpdate(It.IsAny<T>())).Callback((T entity) => OurSaveOrUpdate2(entity));
            _mockRepository.Setup(x => x.Delete(It.IsAny<T>())).Callback((T entity) => OurDelete2<T>(entity.Id));
        }

        protected Mock<IQueryOver<T, T>> MockQueryOver<T>() where T : Entity
        {
            var srvRepository = _mockRepository as Mock<ISRVRepository>;
            if (srvRepository == null) return null;

            var queryOver = new Mock<IQueryOver<T, T>>();
            var queryOverUdfBuilder = new Mock<QueryOverUdfBuilder<T, T>>();

            queryOver.Setup(x => x.Where(It.IsAny<Expression<Func<T, bool>>>())).Returns(queryOver.Object);
            queryOver.Setup(x => x.And(It.IsAny<Expression<Func<T, bool>>>())).Returns(queryOver.Object);

            //queryOver.Setup(x => x.WithUdf(It.IsAny<AbstractUdfCriterion>())).Returns(queryOverUdfBuilder.Object);
            //queryOverUdfBuilder.Setup(x => x.HasPropertyIn(It.IsAny<Expression<Func<T, object>>>())).Returns(queryOver.Object);

            srvRepository.Setup(x => x.QueryOver<T>()).Returns(queryOver.Object);

            return queryOver;
        }

        private PageResponse<T> OurPage<T>(PageRequest request) where T : Entity
        {
            var type = typeof(T);

            var response = new PageResponse<T>
            {
                StartIndex = (request.PageNumber - 1) * request.PageSize,
                Total = _db[type].Count,
                PageSize = request.PageSize
            };

            response.Items = _db[type].Values.Skip(response.StartIndex).Take(request.PageSize).Cast<T>().ToList();

            return response;
        }

        private void OurSaveOrUpdate<T>(T entity) where T : Entity
        {
            var type = typeof(T);

            if (entity.Id == 0)
            {
                _ids[type]++;
                entity.SetId(_ids[type]);
                _db[type].Add(_ids[type], entity);
            }
            else
            {
                if (_db[type].ContainsKey(entity.Id))
                {
                    _db[type][entity.Id] = entity;
                }
                else
                {
                    entity.SetId(entity.Id);
                    _db[type].Add(entity.Id, entity);
                }
            }
        }

        private void OurSaveOrUpdate2<T>(T entity) where T : EntityWithTypedId<string>
        {
            var type = typeof(T);
            var eId = Convert.ToInt64(entity.Id);

            if (eId == 0)
            {
                _ids[type]++;
                entity.SetId(_ids[type].ToString());
                _db[type].Add(_ids[type], entity);
            }
            else
            {
                if (_db[type].ContainsKey(eId))
                {
                    _db[type][eId] = entity;
                }
                else
                {
                    _db[type].Add(eId, entity);
                }
            }
        }

        private void OurDelete<T>(long id) where T : Entity
        {
            var type = typeof(T);

            if (_db[type].ContainsKey(id))
            {
                var entity = _db[type][id] as SoftEntity<IdentityUser>;
                if (entity != null)
                {
                    entity.Deleted = DateTime.Now;
                    entity.DeletedBy = GetCurrentUser();
                }
                else
                {
                    _db[type].Remove(id);
                }
            }
        }

        private void OurDelete2<T>(string id) where T : EntityWithTypedId<string>
        {
            var type = typeof(T);
            var eId = Convert.ToInt64(id);

            if (_db[type].ContainsKey(eId))
            {
                _db[type].Remove(eId);
            }
        }

        private void OurUnDelete<T>(long id) where T : SoftEntity<IdentityUser>
        {
            var type = typeof(T);

            if (_db[type].ContainsKey(id))
            {
                var entity = _db[type][id] as SoftEntity<IdentityUser>;
                if (entity != null)
                {
                    entity.Deleted = null;
                    entity.DeletedBy = null;
                }
            }
        }

        protected T LastCreated<T>() where T : IEntity
        {
            return (T)_db[typeof(T)].Values.Last();
        }

        #endregion Initialize

        #region Interaction with db

        protected T GetFirstObjectFromDbTable<T>(Func<T> newObjBuilder, bool forceOverride = false, object id = null) where T : class, IEntity
        {
            var obj = GetFirstObjectFromDbTable<T>();

            if ((obj != null) && (!forceOverride))
            {
                return obj;
            }

            obj = newObjBuilder();
            SaveOrUpdate(obj, id);
            return obj;
        }

        protected T GetFirstObjectFromDbTable<T>(Expression<Func<T, bool>> condition, Func<T> newObjBuilder, object id = null) where T : class, IEntity
        {
            var obj = GetFirstObjectFromDbTable(condition);

            if (obj != null)
            {
                return obj;
            }

            obj = newObjBuilder();
            SaveOrUpdate(obj, id);
            return obj;
        }

        protected T GetFirstObjectFromDbTable<T>() where T : IEntity
        {
            return Repository.Query<T>().FirstOrDefault();
        }

        protected T GetFirstObjectFromDbTable<T>(Expression<Func<T, bool>> condition) where T : class, IEntity
        {
            var query = Repository.Query<T>();

            try
            {
                return query.Where(condition).FirstOrDefault();
            }
            catch (Exception)
            {
                return query.Where(condition.Compile()).FirstOrDefault();
            }
        }

        protected List<T> GetAllObjectsFromDbTable<T>() where T : IEntity
        {
            return Repository.Query<T>().ToList();
        }

        protected List<T> GetAllObjectsFromDbTable<T>(Expression<Func<T, bool>> condition) where T : IEntity
        {
            try
            {
                return Repository.Query<T>().Where(condition).ToList();
            }
            catch
            {
                return Repository.Query<T>().Where(condition.Compile()).ToList();
            }
        }

        protected List<long> GetAllIdsFromDbTable<T>() where T : Entity
        {
            return Repository.Query<T>().Select(x => x.Id).ToList();
        }

        protected List<long> GetAllIdsFromDbTable<T>(Expression<Func<T, bool>> condition) where T : Entity
        {
            try
            {
                return Repository.Query<T>().Where(condition).Select(x => x.Id).ToList();
            }
            catch
            {
                return Repository.Query<T>().Where(condition.Compile()).Select(x => x.Id).ToList();
            }
        }

        private void SaveOrUpdate<T>(T obj, object id) where T : class, IEntity
        {
            if (id == null)
            {
                Repository.SaveOrUpdate(obj);
                Session.Flush();
            }
            else
            {
                if (IsMockedRepository)
                {
                    if (id is long)
                    {
                        var entityId = (long)id;
                        var entity = obj as Entity;
                        if (entity == null) return;
                        entity.SetId(entityId);
                        Repository.SaveOrUpdate(entity);
                    }
                    else
                    {
                        var entityId = id.ToString();
                        var entity = obj as EntityWithTypedId<string>;
                        if (entity == null) return;
                        entity.SetId(entityId);
                        Repository.SaveOrUpdate(obj);
                    }
                }
                else
                {
                    SetIdentityInsert(obj, true);
                    Session.Save(obj, id);
                    Session.Flush();
                    SetIdentityInsert(obj, false);
                }
            }
        }

        private void SetIdentityInsert<T>(T obj, bool on)
        {
            var metaData = SessionFactory.GetClassMetadata(obj.GetType());
            var persister = (AbstractEntityPersister)metaData;
            if (persister.IsIdentifierAssignedByInsert)
            {
                var tableName = persister.TableName;
                var query = string.Format("SET IDENTITY_INSERT {0} O{1}", tableName, on ? "N" : "FF");
                Session.CreateSQLQuery(query).ExecuteUpdate();
            }
        }

        protected long GetDbTableCount<T>() where T : IEntity
        {
            return Repository.Query<T>().LongCount();
        }

        protected long GetDbTableCount<T>(Func<T, bool> condition) where T : IEntity
        {
            return Repository.Query<T>().LongCount(condition);
        }

        protected T GetLastCreatedObject<T>() where T : AuditedEntity<IdentityUser>
        {
            if (IsMockedRepository)
            {
                return LastCreated<T>();
            }
            var query = Repository.Query<T>();
            return query.FirstOrDefault(x => x.Created == query.Max(y => y.Created));
        }

        protected T GetLastDeletedObject<T>() where T : SoftEntity<IdentityUser>
        {
            var query = Repository.Query<T>().ToList();
            return query.FirstOrDefault(x => x.Deleted == query.Max(y => y.Deleted));
        }

        protected T GetLastCreatedSrvObject<T>() where T : AuditedEntity<SRVIdentityUser>
        {
            var query = Repository.Query<T>();
            return query.FirstOrDefault(x => x.Created == query.Max(y => y.Created));
        }

        protected T GetLastDeletedSrvObject<T>() where T : SoftEntity<SRVIdentityUser>
        {
            var query = Repository.Query<T>();
            return query.FirstOrDefault(x => x.Deleted == query.Max(y => y.Deleted));
        }

        protected void DeleteEntity<T>(T entity) where T : SoftEntity<IdentityUser>
        {
            entity.Deleted = DateTime.Now;
            SafeExec(Repository.SaveOrUpdate, entity);
        }

        protected void AddRegionToCurrentUser(Region region)
        {
            var user = GetCurrentUser();
            user.AddRegion(region);
            Repository.SaveOrUpdate(user);
            Session.Flush();
        }

        #endregion Interaction with db

        #region user roles

        protected void SetAdministratorRole()
        {
            SetRole("Administrator");
        }

        protected void SetRegistratorRole()
        {
            SetRole("Registrator");
        }

        private void SetRole(string role)
        {
            var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, "1");
            var claimName = new Claim(ClaimTypes.Name, "test@user.com");
            var claimRole = new Claim(ClaimTypes.Role, role);
            var claimsIdentity = new ClaimsIdentity(new[] { claimNameIdentifier, claimName, claimRole }, "TestAuthentication");
            ClaimsPrincipal.Current.AddIdentity(claimsIdentity);
            GetCurrentUser(role);
        }

        protected SRVIdentityUser GetCurrentUser(string role = "Administrator")
        {
            var id = SecurityHelper.GetLoggedUserId();
            return GetFirstObjectFromDbTable(x => x.Id == id, () =>
            {
                var user = GetSrvIdentityUser();
                user.Roles.First(x => x.Name == "Administrator").Name = role;
                return user;
            }, id);
        }

        protected bool RegionIsAccessibleForCurrentUser(long regionId)
        {
            return GetCurrentUser().Regions.Select(y => y.Id).Contains(regionId);
        }

        protected bool EligiblePersonRegionIsAccessibleForCurrentUser(Person person)
        {
            var personAddress = person.Addresses.FirstOrDefault(y => y.IsEligible);
            return (personAddress != null) &&
                   RegionIsAccessibleForCurrentUser(personAddress.Address.Street.Region.Id);
        }

        #endregion user roles

        #region Assert Methods

        protected void AssertListsAreEqual<T>(List<T> list1, List<T> list2) where T : Entity
        {
            Assert.AreEqual(list1.Count, list2.Count);

            var counts = new Dictionary<long, int>();

            list1.ForEach(x =>
            {
                if (counts.ContainsKey(x.Id)) counts[x.Id]++;
                else counts.Add(x.Id, 1);
            });

            list2.ForEach(x =>
            {
                if (counts.ContainsKey(x.Id)) counts[x.Id]--;
                else Assert.Fail("The lists are not equal");
            });

            Assert.IsTrue(counts.Values.All(x => x == 0));
        }

        protected void AssertListsAreEqual<T, TR>(List<T> list1, List<TR> list2, Func<T, string> list1Transform, Func<TR, string> list2Transform)
        {
            Assert.AreEqual(list1.Count, list2.Count);

            string key;
            var counts = new Dictionary<string, int>();

            list1.ForEach(x =>
            {
                key = list1Transform(x);
                if (counts.ContainsKey(key)) counts[key]++;
                else counts.Add(key, 1);
            });

            list2.ForEach(x =>
            {
                key = list2Transform(x);
                if (counts.ContainsKey(key)) counts[key]--;
                else Assert.Fail("The lists are not equal");
            });

            Assert.IsTrue(counts.Values.All(x => x == 0));
        }

        protected void AssertObjectListsAreEqual<T>(List<T> list1, List<T> list2)
        {
            Assert.AreEqual(list1.Count(), list2.Count());

            var counts = new Dictionary<object, int>();

            list1.ForEach(x =>
            {
                if (counts.ContainsKey(x)) counts[x]++;
                else counts.Add(x, 1);
            });

            list2.ForEach(x =>
            {
                if (counts.ContainsKey(x)) counts[x]--;
                else Assert.Fail("The lists are not equal");
            });

            Assert.IsTrue(counts.Values.All(x => x == 0));
        }

        private void ActAndAssertPage<T>(PageResponse<T> page, PageRequest request, bool checkPageSize) where T : class
        {
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Items);
            Assert.IsTrue(page.Items.Count <= request.PageSize);

            if (checkPageSize)
            {
                Assert.AreEqual(request.PageSize, page.PageSize);
            }
        }

        private bool AddPageResponseToTheListAndIncrementPageRequestNumber<TR>(List<long> list, PageResponse<TR> page, PageRequest request, Func<TR, long> resultTransform)
            where TR : class
        {
            var count = page.Items.Count;
            list.AddRange(page.Items.Select(resultTransform));
            request.PageNumber++;
            return count > 0;
        }

        protected void ActAndAssertAllPages<TP, TR>(Func<PageRequest, TP, PageResponse<TR>> bllFunc, TP parameter, Func<TR, long> resultTransform, List<long> expectedTotalList, bool checkPageSize = true, bool needSort = true, bool onlyFirstAndAnotherRandomPage = true)
            where TR : class
        {
            var request = onlyFirstAndAnotherRandomPage ? GetPageRequest(needSort) : GetBigPageRequest(expectedTotalList.Count, needSort);
            PageResponse<TR> page;
            var list = new List<long>();

            do
            {
                page = SafeExecFunc(bllFunc, request, parameter);
                ActAndAssertPage(page, request, checkPageSize);
            }
            while ((!onlyFirstAndAnotherRandomPage) && AddPageResponseToTheListAndIncrementPageRequestNumber(list, page, request, resultTransform));

            if (!onlyFirstAndAnotherRandomPage)
            {
                AssertObjectListsAreEqual(list, expectedTotalList);
            }
            else
            {
                AssertObjectListsAreEqual(
                    page.Items.Select(resultTransform).ToList(),
                    expectedTotalList.OrderBy(x => x).Take(request.PageSize).ToList());

                var count = (int)Math.Ceiling(((double)page.Total) / request.PageSize);
                if (count <= 1) return;

                var r = new Random();
                request.PageNumber = r.Next(2, count);

                page = SafeExecFunc(bllFunc, request, parameter);
                ActAndAssertPage(page, request, checkPageSize);

                AssertObjectListsAreEqual(
                    page.Items.Select(resultTransform).ToList(),
                    expectedTotalList.OrderBy(x => x).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList());
            }
        }

        protected void ActAndAssertAllPages<TP1, TP2, TR>(Func<PageRequest, TP1, TP2, PageResponse<TR>> bllFunc, TP1 parameter1, TP2 parameter2,
            Func<TR, long> resultTransform, List<long> expectedTotalList, bool checkPageSize = true, bool needSort = true, bool onlyFirstAndAnotherRandomPage = true)
            where TR : class
        {
            var request = onlyFirstAndAnotherRandomPage ? GetPageRequest(needSort) : GetBigPageRequest(expectedTotalList.Count, needSort);
            PageResponse<TR> page;
            var list = new List<long>();

            do
            {
                page = SafeExecFunc(bllFunc, request, parameter1, parameter2);
                ActAndAssertPage(page, request, checkPageSize);
            }
            while ((!onlyFirstAndAnotherRandomPage) && AddPageResponseToTheListAndIncrementPageRequestNumber(list, page, request, resultTransform));

            if (!onlyFirstAndAnotherRandomPage)
            {
                AssertObjectListsAreEqual(list, expectedTotalList);
            }
            else
            {
                AssertObjectListsAreEqual(
                    page.Items.Select(resultTransform).ToList(),
                    expectedTotalList.OrderBy(x => x).Take(request.PageSize).ToList());

                var count = (int)Math.Ceiling(((double)page.Total) / request.PageSize);
                if (count <= 1) return;

                var r = new Random();
                request.PageNumber = r.Next(2, count);

                page = SafeExecFunc(bllFunc, request, parameter1, parameter2);
                ActAndAssertPage(page, request, checkPageSize);

                AssertObjectListsAreEqual(
                    page.Items.Select(resultTransform).ToList(),
                    expectedTotalList.OrderBy(x => x).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList());
            }
        }

        protected void ActAndAssertAllPages<T, TP>(Func<PageRequest, TP, PageResponse<T>> bllFunc, TP parameter, List<long> expectedTotalList, bool checkPageSize = true, bool needSort = true, bool onlyFirstAndAnotherRandomPage = true) where T : Entity
        {
            ActAndAssertAllPages(bllFunc, parameter, x => x.Id, expectedTotalList, checkPageSize, needSort, onlyFirstAndAnotherRandomPage);
        }

        protected void ActAndAssertAllPages<T, TP1, TP2>(Func<PageRequest, TP1, TP2, PageResponse<T>> bllFunc, TP1 parameter1, TP2 parameter2, List<long> expectedTotalList, bool checkPageSize = true, bool needSort = true, bool onlyFirstAndAnotherRandomPage = true) where T : Entity
        {
            ActAndAssertAllPages(bllFunc, parameter1, parameter2, x => x.Id, expectedTotalList, checkPageSize, needSort, onlyFirstAndAnotherRandomPage);
        }

        protected void ActAndAssertAllPages<TR>(Func<PageRequest, PageResponse<TR>> bllFunc, Func<TR, long> resultTransform, List<long> expectedTotalList, bool checkPageSize = true, bool needSort = true, bool onlyFirstAndAnotherRandomPage = true)
            where TR : class
        {
            var request = onlyFirstAndAnotherRandomPage ? GetPageRequest(needSort) : GetBigPageRequest(expectedTotalList.Count, needSort);
            PageResponse<TR> page;
            var list = new List<long>();

            do
            {
                page = SafeExecFunc(bllFunc, request);
                ActAndAssertPage(page, request, checkPageSize);
            }
            while ((!onlyFirstAndAnotherRandomPage) && AddPageResponseToTheListAndIncrementPageRequestNumber(list, page, request, resultTransform));

            if (!onlyFirstAndAnotherRandomPage)
            {
                AssertObjectListsAreEqual(list, expectedTotalList);
            }
            else
            {
                AssertObjectListsAreEqual(
                    page.Items.Select(resultTransform).ToList(),
                    expectedTotalList.OrderBy(x => x).Take(request.PageSize).ToList());

                var count = (int)Math.Ceiling(((double)page.Total) / request.PageSize);
                if (count <= 1) return;

                var r = new Random();
                request.PageNumber = r.Next(2, count);

                page = SafeExecFunc(bllFunc, request);
                ActAndAssertPage(page, request, checkPageSize);

                AssertObjectListsAreEqual(
                    page.Items.Select(resultTransform).ToList(),
                    expectedTotalList.OrderBy(x => x).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList());
            }
        }

        protected void ActAndAssertAllPages<T>(Func<PageRequest, PageResponse<T>> bllFunc, List<long> expectedTotalList, bool checkPageSize = true, bool needSort = true, bool onlyFirstAndAnotherRandomPage = true) where T : Entity
        {
            ActAndAssertAllPages(bllFunc, x => x.Id, expectedTotalList, checkPageSize, needSort, onlyFirstAndAnotherRandomPage);
        }

        protected void AssertDeletedEntity<T>(long entityId, bool deleted) where T : SoftEntity<IdentityUser>
        {
            var entity = GetFirstObjectFromDbTable<T>(x => x.Id == entityId);

            Assert.IsNotNull(entity);
            Assert.AreEqual(deleted, entity.Deleted != null);
        }

        protected void ActAndAssertLongValue(Func<long> func, long expectedValue)
        {
            Assert.AreEqual(expectedValue, SafeExecFunc(func));
        }

        protected void ActAndAssertLongValue<T>(Func<T, long> func, T parameter, long expectedValue)
        {
            Assert.AreEqual(expectedValue, SafeExecFunc(func, parameter));
        }

        #endregion Assert Methods

        #region Safe Exec Methods

        protected void SafeExec(Action func, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;

            try
            {
                func();
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);
        }

        protected void SafeExec<T1>(Action<T1> func, T1 param1, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;

            try
            {
                func(param1);
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);
        }

        protected void SafeExec<T1, T2>(Action<T1, T2> func, T1 param1, T2 param2, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;

            try
            {
                func(param1, param2);
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);
        }

        protected void SafeExec<T1, T2, T3>(Action<T1, T2, T3> func, T1 param1, T2 param2, T3 param3, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;

            try
            {
                func(param1, param2, param3);
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);
        }

        protected void SafeExec<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 param1, T2 param2, T3 param3, T4 param4, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;

            try
            {
                func(param1, param2, param3, param4);
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);
        }

        protected void SafeExec<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;

            try
            {
                func(param1, param2, param3, param4, param5);
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);
        }

        protected void SafeExec<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;

            try
            {
                func(param1, param2, param3, param4, param5, param6);
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);
        }

        protected void SafeExec<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;

            try
            {
                func(param1, param2, param3, param4, param5, param6, param7);
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);
        }

        protected void SafeExec<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;

            try
            {
                func(param1, param2, param3, param4, param5, param6, param7, param8);
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);
        }

        protected TR SafeExecFunc<TR>(Func<TR> func, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;
            var result = default(TR);

            try
            {
                result = func();
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);

            return result;
        }

        protected TR SafeExecFunc<T1, TR>(Func<T1, TR> func, T1 param1, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;
            var result = default(TR);

            try
            {
                result = func(param1);
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);

            return result;
        }

        protected TR SafeExecFunc<T1, T2, TR>(Func<T1, T2, TR> func, T1 param1, T2 param2, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;
            var result = default(TR);

            try
            {
                result = func(param1, param2);
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);

            return result;
        }

        protected TR SafeExecFunc<T1, T2, T3, TR>(Func<T1, T2, T3, TR> func, T1 param1, T2 param2, T3 param3, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;
            var result = default(TR);

            try
            {
                result = func(param1, param2, param3);
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);

            return result;
        }

        protected TR SafeExecFunc<T1, T2, T3, T4, TR>(Func<T1, T2, T3, T4, TR> func, T1 param1, T2 param2, T3 param3, T4 param4, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;
            var result = default(TR);

            try
            {
                result = func(param1, param2, param3, param4);
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);

            return result;
        }

        protected TR SafeExecFunc<T1, T2, T3, T4, T5, TR>(Func<T1, T2, T3, T4, T5, TR> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;
            var result = default(TR);

            try
            {
                result = func(param1, param2, param3, param4, param5);
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);

            return result;
        }

        protected TR SafeExecFunc<T1, T2, T3, T4, T5, T6, TR>(Func<T1, T2, T3, T4, T5, T6, TR> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;
            var result = default(TR);

            try
            {
                result = func(param1, param2, param3, param4, param5, param6);
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);

            return result;
        }

        protected TR SafeExecFunc<T1, T2, T3, T4, T5, T6, T7, TR>(Func<T1, T2, T3, T4, T5, T6, T7, TR> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;
            var result = default(TR);

            try
            {
                result = func(param1, param2, param3, param4, param5, param6, param7);
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);

            return result;
        }

        protected TR SafeExecFunc<T1, T2, T3, T4, T5, T6, T7, T8, TR>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TR> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            var srvThrowExists = false;
            var throwExists = false;
            var result = default(TR);

            try
            {
                result = func(param1, param2, param3, param4, param5, param6, param7, param8);
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);

            return result;
        }

        protected void SafeExec(string funcName, object[] parameters = null, Type[] types = null, bool isStaticMethod = false, bool isPublicMethod = false, object target = null,
            bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            if (target == null)
            {
                target = Bll;
            }

            if (parameters == null)
            {
                parameters = new object[0];
            }

            BindingFlags flags = (isStaticMethod ? BindingFlags.Static : BindingFlags.Instance) |
                                 (isPublicMethod ? BindingFlags.Public : BindingFlags.NonPublic);

            var srvThrowExists = false;
            var throwExists = false;

            try
            {
                var invokeTarget = isStaticMethod ? null : target;
                var method = ((types == null) || isStaticMethod) ?
                    target.GetType().GetMethod(funcName, flags) :
                    target.GetType().GetMethod(funcName, flags, null, types, new ParameterModifier[] { });

                if (method != null)
                {
                    method.Invoke(invokeTarget, parameters);
                }
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);
        }

        protected TR SafeExecFunc<TR>(string funcName, object[] parameters = null, Type[] types = null, bool isStaticMethod = false, bool isPublicMethod = false, object target = null,
            bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            if (target == null)
            {
                target = Bll;
            }

            if (parameters == null)
            {
                parameters = new object[0];
            }

            BindingFlags flags = (isStaticMethod ? BindingFlags.Static : BindingFlags.Instance) |
                                 (isPublicMethod ? BindingFlags.Public : BindingFlags.NonPublic);

            var srvThrowExists = false;
            var throwExists = false;
            var result = default(TR);

            try
            {
                var method = ((types == null) || isStaticMethod)
                    ? target.GetType().GetMethod(funcName, flags)
                    : target.GetType().GetMethod(funcName, flags, null, types, new ParameterModifier[] { });

                if (method != null)
                {
                    result = (TR)(method.Invoke(isStaticMethod ? null : target, parameters));
                }
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);

            return result;
        }

        protected TR SafeGetField<TR>(string fieldName, bool isStaticField = false, bool isPublicField = false, object target = null,
          bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            if (target == null)
            {
                target = Bll;
            }

            BindingFlags flags = (isStaticField ? BindingFlags.Static : BindingFlags.Instance) |
                                 (isPublicField ? BindingFlags.Public : BindingFlags.NonPublic);

            var srvThrowExists = false;
            var throwExists = false;
            var result = default(TR);

            try
            {
                var field = target.GetType().GetField(fieldName, flags);

                if (field != null)
                {
                    result = (TR)(field.GetValue(isStaticField ? null : target));
                }
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);

            return result;
        }

        protected void SafeSetField(string fieldName, object value, bool isStaticField = false, bool isPublicField = false, object target = null,
          bool expSrvThrowExists = false, bool expThrowExists = false, string expLocalizationKey = null, string expMessage = null)
        {
            if (target == null)
            {
                target = Bll;
            }

            BindingFlags flags = (isStaticField ? BindingFlags.Static : BindingFlags.Instance) |
                                 (isPublicField ? BindingFlags.Public : BindingFlags.NonPublic);

            var srvThrowExists = false;
            var throwExists = false;

            try
            {
                var field = target.GetType().GetField(fieldName, flags);

                if (field != null)
                {
                    field.SetValue(isStaticField ? null : target, value);
                }
            }
            catch (SrvException ex)
            {
                AssertSrvException(ex, expSrvThrowExists, expLocalizationKey, expMessage, out srvThrowExists);
            }
            catch (Exception)
            {
                throwExists = true;
            }

            AssertSafeExec(throwExists, expThrowExists, srvThrowExists, expSrvThrowExists);
        }

        private void AssertSrvException(SrvException ex, bool expSrvThrowExists, string expLocalizationKey, string expMessage, out bool srvThrowExists)
        {
            srvThrowExists = true;

            if (expSrvThrowExists)
            {
                if (expLocalizationKey != null)
                {
                    Assert.AreEqual(expLocalizationKey, ex.LocalizationKey);
                }
                if (expMessage != null)
                {
                    Assert.AreEqual(expMessage, ex.Message);
                }
            }
        }

        private void AssertSafeExec(bool throwExists, bool expThrowExists, bool srvThrowExists, bool expSrvThrowExists)
        {
            Assert.AreEqual(expSrvThrowExists, srvThrowExists);
            Assert.AreEqual(expThrowExists, throwExists);
        }

        # endregion Safe Exec Methods

        #region Entities

        protected MappingAddress GetMappingAddress()
        {
            return new MappingAddress()
            {
                RspAdministrativeCode = 1,
                RspHouseNr = 1,
                RspHouseSuf = "A",
                RspStreetCode = 1,
                SrvAddressId = 1
            };
        }

        protected Address GetAddressWithUnknownStreetTypeAndNoResidenceRegion()
        {
            return new Address()
            {
                BuildingType = BuildingTypes.Undefined,
                GeoLocation = new GeoLocation() { Latitude = 90, Longitude = 90 },
                HouseNumber = 25,
                PollingStation = GetFirstObjectFromDbTable(GetPollingStation),
                Street = GetFirstObjectFromDbTable(
                    x => (x.StreetType.Id == StreetType.UnknownStreetType) && (x.Region.Id == Region.NoResidenceRegionId),
                    GetStreetWithUnknownStreetTypeAndNoResidenceRegion)
            };
        }

        protected Address GetAddress()
        {
            return new Address()
            {
                BuildingType = BuildingTypes.Administrative,
                GeoLocation = new GeoLocation() { Latitude = 40, Longitude = 20 },
                HouseNumber = 12,
                PollingStation = GetFirstObjectFromDbTable(GetPollingStation),
                Street = GetFirstObjectFromDbTable(GetStreet)
            };
        }

        protected Address GetAddressWithoutStreets()
        {
            return new Address()
            {
                BuildingType = BuildingTypes.Administrative,
                GeoLocation = new GeoLocation() { Latitude = 40, Longitude = 20 },
                HouseNumber = 15,
                PollingStation = GetFirstObjectFromDbTable(GetPollingStation),
                Street = GetFirstObjectFromDbTable(x => (x.Region != null) && (!x.Region.HasStreets), GetStreetWithRegionWithoutStreets)
            };
        }

        protected Address GetAddressWithoutStreets2()
        {
            return new Address()
            {
                BuildingType = BuildingTypes.Administrative,
                GeoLocation = new GeoLocation() { Latitude = 40, Longitude = 20 },
                HouseNumber = 17,
                PollingStation = GetFirstObjectFromDbTable(GetPollingStation),
                Street = GetFirstObjectFromDbTable(GetStreetWithRegionWithoutStreets2, true)
            };
        }

        protected Address GetAddressWithNullPollingStation()
        {
            return new Address()
            {
                BuildingType = BuildingTypes.Administrative,
                GeoLocation = new GeoLocation() { Latitude = 40, Longitude = 20 },
                HouseNumber = 14,
                Street = GetFirstObjectFromDbTable(GetStreet)
            };
        }

        protected Address GetAddressWithNullHouseNumber()
        {
            return new Address()
            {
                BuildingType = BuildingTypes.Administrative,
                GeoLocation = new GeoLocation() { Latitude = 40, Longitude = 20 },
                PollingStation = GetFirstObjectFromDbTable(GetPollingStation),
                Street = GetFirstObjectFromDbTable(GetStreet)
            };
        }

        protected Street GetStreetWithUnknownStreetTypeAndNoResidenceRegion()
        {
            return new Street(
                GetFirstObjectFromDbTable(x => x.Id == Region.NoResidenceRegionId, GetRegionWithRegistruId2, Region.NoResidenceRegionId),
                GetFirstObjectFromDbTable(x => x.Id == StreetType.UnknownStreetType, GetStreetType2, StreetType.UnknownStreetType),
                "Dokuceaev",
                true)
            {
                Description = "Dokuceaev"
            };
        }

        protected Street GetStreet()
        {
            return GetStreet(GetFirstObjectFromDbTable(GetRegion));
        }

        protected Street GetStreet(Region region)
        {
            return new Street(
                region,
                GetFirstObjectFromDbTable(GetStreetType),
                "V. Alecsandri",
                true)
            {
                Description = "The best street"
            };
        }

        protected Street GetDeletedStreet()
        {
            return new Street(
                GetFirstObjectFromDbTable(GetRegion),
                GetFirstObjectFromDbTable(GetStreetType),
                "I. Creanga",
                true)
            {
                Description = "The best street",
                Deleted = DateTime.Now
            };
        }

        protected Street GetStreetWithRopId()
        {
            return new Street(
                GetFirstObjectFromDbTable(GetRegion),
                GetFirstObjectFromDbTable(GetStreetType),
                "name",
                true)
            {
                Description = "description",
                RopId = 1
            };
        }

        protected Street GetStreetWithRegionWithoutStreets()
        {
            return new Street(
                GetFirstObjectFromDbTable(x => !x.HasStreets, GetRegionWithoutStreets),
                GetFirstObjectFromDbTable(GetStreetType),
                "V. Alecsandri ",
                true)
            {
                Description = "The best street"
            };
        }

        protected Street GetStreetWithRegionWithoutStreets2()
        {
            return new Street(
                GetFirstObjectFromDbTable(GetRegionWithoutStreets2, true),
                GetFirstObjectFromDbTable(GetStreetType),
                "V. Alecsandri  ",
                true)
            {
                Description = "The best street"
            };
        }

        protected Region GetRegion()
        {
            return new Region(GetFirstObjectFromDbTable(GetRegionType))
            {
                Circumscription = 1,
                Description = "capitala RM",
                GeoLocation = new GeoLocation() { Latitude = 10, Longitude = 30 },
                HasStreets = true,
                Name = "Chisinau"
            };
        }

        protected Region GetRegionWithRegistruId()
        {
            return new Region(GetFirstObjectFromDbTable(GetRegionType))
            {
                Circumscription = 3,
                Description = "description",
                GeoLocation = new GeoLocation() { Latitude = 50, Longitude = 50 },
                HasStreets = true,
                Name = "Ungheni",
                RegistruId = 1
            };
        }

        protected Region GetRegionWithRegistruId2()
        {
            return new Region(GetFirstObjectFromDbTable(GetRegionType))
            {
                Circumscription = 4,
                Description = "descriere",
                GeoLocation = new GeoLocation() { Latitude = 60, Longitude = 60 },
                HasStreets = true,
                Name = "Calarasi",
                RegistruId = 2
            };
        }

        protected Region GetRegionWithRank0()
        {
            return new Region(GetFirstObjectFromDbTable(x => x.Rank == 0, GetRegionType))
            {
                Circumscription = 1,
                Description = "capitala RM",
                GeoLocation = new GeoLocation() { Latitude = 10, Longitude = 30 },
                HasStreets = true,
                Name = "Chisinau"
            };
        }

        protected Region GetRegionWithRank1()
        {
            return new Region(GetFirstObjectFromDbTable(x => x.Rank == 1, GetRegionTypeWithRank1))
            {
                Circumscription = 2,
                Description = "capitala RM",
                GeoLocation = new GeoLocation() { Latitude = 10, Longitude = 30 },
                HasStreets = true,
                Name = "Chisinauu"
            };
        }

        protected Region GetRegionWithRank2()
        {
            return new Region(GetFirstObjectFromDbTable(x => x.Rank == 1, GetRegionTypeWithRank2))
            {
                Circumscription = 3,
                Description = "capitala RM",
                GeoLocation = new GeoLocation() { Latitude = 10, Longitude = 30 },
                HasStreets = true,
                Name = "Chisinauuu"
            };
        }

        protected Region GetRegionWithoutCircumscription()
        {
            return new Region(GetFirstObjectFromDbTable(GetRegionType))
            {
                Circumscription = null,
                Description = "capitala RM",
                GeoLocation = new GeoLocation() { Latitude = 10, Longitude = 30 },
                HasStreets = true,
                Name = "Chisinau"
            };
        }

        protected Region GetRegionWithParent()
        {
            return new Region(
                GetFirstObjectFromDbTable(x => x.Parent == null, GetRegion),
                GetFirstObjectFromDbTable(GetRegionType))
            {
                Circumscription = 2,
                Description = "capitala de nord",
                GeoLocation = new GeoLocation() { Latitude = 30, Longitude = 10 },
                HasStreets = true,
                Name = "Balti"
            };
        }

        protected Region GetDeletedRegionWithStreets()
        {
            return new Region(GetFirstObjectFromDbTable(GetRegionType))
            {
                Circumscription = 1,
                Description = "capitala RM",
                GeoLocation = new GeoLocation() { Latitude = 10, Longitude = 30 },
                HasStreets = true,
                Name = "Chisinau",
                Deleted = DateTime.Now
            };
        }

        protected Region GetRegionWithoutStreets()
        {
            return new Region(GetFirstObjectFromDbTable(GetRegionType))
            {
                Circumscription = 10,
                Description = "capitala RM",
                GeoLocation = new GeoLocation() { Latitude = 10, Longitude = 30 },
                HasStreets = false,
                Name = "Balti"
            };
        }

        protected Region GetRegionWithoutStreets2()
        {
            return new Region(GetFirstObjectFromDbTable(GetRegionType))
            {
                Circumscription = 11,
                Description = "capitala",
                GeoLocation = new GeoLocation() { Latitude = 20, Longitude = 30 },
                HasStreets = false,
                Name = "Orhei"
            };
        }

        protected RegionType GetRegionType()
        {
            return new RegionType()
            {
                Description = "oras mare care are in subordine alte orase",
                Name = "municipiu",
                Rank = 0
            };
        }

        protected RegionType GetRegionTypeWithRank1()
        {
            return new RegionType()
            {
                Description = "oras mare care are in subordine alte orase",
                Name = "municipiu cu rank 1",
                Rank = 1
            };
        }

        protected RegionType GetRegionTypeWithRank2()
        {
            return new RegionType()
            {
                Description = "oras mare care are in subordine alte orase",
                Name = "municipiu cu rank 2",
                Rank = 2
            };
        }

        protected StreetType GetStreetType()
        {
            return new StreetType()
            {
                Description = "strada noua, cu pavaj de ultima generatie",
                Name = "alee"
            };
        }

        protected StreetType GetStreetType2()
        {
            return new StreetType()
            {
                Description = "strada veche",
                Name = "ulita"
            };
        }

        protected StreetTypeCode GetStreetTypeCode()
        {
            return new StreetTypeCode()
            {
                Docprint = "docprint1",
                Name = "code1",
                Namerus = "kod1"
            };
        }

        protected StreetTypeCode GetStreetTypeCode2()
        {
            return new StreetTypeCode()
            {
                Docprint = "docprint2",
                Name = "code2",
                Namerus = "kod2"
            };
        }

        protected PollingStation GetPollingStation()
        {
            var region = GetFirstObjectFromDbTable(GetRegion);
            return GetPollingStation(region);
        }

        protected PollingStation GetPollingStation(Region region)
        {
            var geoLocation = new GeoLocation() { Latitude = 40, Longitude = 20 };

            var pollingStation = new PollingStation(region)
            {
                ContactInfo = "a@a.a",
                GeoLocation = geoLocation,
                Location = "Casa de Cultura, CENTRU",
                SaiseId = null,
                Number = "11",
                PollingStationAddress = GetFirstObjectFromDbTable(GetAddressWithNullPollingStation),
                SubNumber = "1"
            };

            pollingStation.PollingStationAddress.PollingStation = pollingStation;

            return pollingStation;
        }

        protected PollingStation GetPollingStationWithoutStreets()
        {
            var region = GetFirstObjectFromDbTable(GetRegionWithoutStreets);

            var geoLocation = new GeoLocation() { Latitude = 40, Longitude = 20 };

            var pollingStation = new PollingStation(region)
            {
                ContactInfo = "a@a.a",
                GeoLocation = geoLocation,
                Location = "Casa de Cultura, CENTRU",
                SaiseId = null,
                Number = "11",
                PollingStationAddress = null,
                SubNumber = "1"
            };

            return pollingStation;
        }

        protected PersonAddress GetPersonAddressWithUnknownStreetTypeAndNoResidenceRegion()
        {
            return new PersonAddress()
            {
                Address = GetFirstObjectFromDbTable(
                    x => x.Street.StreetType.Id == StreetType.UnknownStreetType &&
                         x.Street.Region.Id == Region.NoResidenceRegionId,
                    GetAddressWithUnknownStreetTypeAndNoResidenceRegion),
                ApNumber = 23,
                ApSuffix = "U",
                IsEligible = true,
                Person = GetFirstObjectFromDbTable(GetPerson),
                PersonAddressType = GetFirstObjectFromDbTable(GetPersonAddressType)
            };
        }

        protected PersonAddress GetPersonAddress()
        {
            return new PersonAddress()
            {
                Address = GetFirstObjectFromDbTable(GetAddress),
                ApNumber = 1,
                ApSuffix = "A",
                IsEligible = true,
                Person = GetFirstObjectFromDbTable(GetPerson),
                PersonAddressType = GetFirstObjectFromDbTable(GetPersonAddressType)
            };
        }

        protected PersonAddress GetNonEligiblePersonAddressWithoutStreets()
        {
            return new PersonAddress()
            {
                Address = GetFirstObjectFromDbTable(x =>
                    (x.Street != null) && (x.Street.Region != null) && (!x.Street.Region.HasStreets),
                    GetAddressWithoutStreets),
                ApNumber = 2,
                ApSuffix = "B",
                IsEligible = false,
                Person = GetFirstObjectFromDbTable(GetPerson),
                PersonAddressType = GetFirstObjectFromDbTable(GetPersonAddressType)
            };
        }

        protected PersonAddress GetEligiblePersonAddressWithoutStreets()
        {
            return new PersonAddress()
            {
                Address = GetFirstObjectFromDbTable(x =>
                    (x.Street != null) && (x.Street.Region != null) && (!x.Street.Region.HasStreets),
                    GetAddressWithoutStreets),
                ApNumber = 3,
                ApSuffix = "C",
                IsEligible = true,
                Person = GetFirstObjectFromDbTable(GetPerson),
                PersonAddressType = GetFirstObjectFromDbTable(GetPersonAddressType)
            };
        }

        protected PersonAddress GetEligiblePersonAddressWithoutStreets2()
        {
            return new PersonAddress()
            {
                Address = GetFirstObjectFromDbTable(GetAddressWithoutStreets2, true),
                ApNumber = 4,
                ApSuffix = "D",
                IsEligible = true,
                Person = GetFirstObjectFromDbTable(GetPerson),
                PersonAddressType = GetFirstObjectFromDbTable(GetPersonAddressType)
            };
        }

        protected PersonAddress GetNonEligiblePersonAddressWithStreets()
        {
            return new PersonAddress()
            {
                Address = GetFirstObjectFromDbTable(x =>
                    (x.Street != null) && (x.Street.Region != null) && (!x.Street.Region.HasStreets),
                    GetAddress),
                ApNumber = 2,
                ApSuffix = "B",
                IsEligible = false,
                Person = GetFirstObjectFromDbTable(GetPerson),
                PersonAddressType = GetFirstObjectFromDbTable(GetPersonAddressType)
            };
        }

        protected PersonAddress GetPersonAddressWithNullPollingStation()
        {
            return new PersonAddress()
            {
                Address = GetFirstObjectFromDbTable(GetAddressWithNullPollingStation),
                ApNumber = 1,
                ApSuffix = "A",
                IsEligible = true,
                Person = GetFirstObjectFromDbTable(GetPerson),
                PersonAddressType = GetFirstObjectFromDbTable(GetPersonAddressType)
            };
        }

        protected Person GetPerson()
        {
            var person = new Person("1234567890123")
            {
                FirstName = "Alex",
                MiddleName = "Alex",
                Surname = "Ivan",
                DateOfBirth = new DateTime(1976, 2, 2),
                Gender = GetFirstObjectFromDbTable(GetGender),
                Document = GetPersonDocument(),
                ExportedToSaise = false
            };

            person.ModifyStatus(
                GetFirstObjectFromDbTable(x => x.Name.StartsWith("Aleg"), GetPersonStatusType),
                "confirmation");

            return person;
        }

        protected Person GetPersonWithEligibleAddress()
        {
            var person = new Person("1234237890123")
            {
                FirstName = "Alex5",
                MiddleName = "Alex5",
                Surname = "Ivan5",
                DateOfBirth = new DateTime(1976, 3, 3),
                Gender = GetFirstObjectFromDbTable(GetGender),
                Document = GetPersonDocument(),
                ExportedToSaise = false
            };

            person.ModifyStatus(
                GetFirstObjectFromDbTable(x => x.Name.StartsWith("Aleg"), GetPersonStatusType),
                "confirmation");

            person.SetEligibleAddress(GetFirstObjectFromDbTable(GetPersonAddress));

            return person;
        }

        protected Person GetPersonWithEligibleAddressAndNullPollingStation()
        {
            var person = new Person("1234237890123")
            {
                FirstName = "Alex25",
                MiddleName = "Alex25",
                Surname = "Ivan25",
                DateOfBirth = new DateTime(1976, 1, 2),
                Gender = GetFirstObjectFromDbTable(GetGender),
                Document = GetPersonDocument(),
                ExportedToSaise = false
            };

            person.ModifyStatus(
                GetFirstObjectFromDbTable(x => x.Name.StartsWith("Aleg"), GetPersonStatusType),
                "confirmation");

            person.SetEligibleAddress(GetFirstObjectFromDbTable(GetPersonAddressWithNullPollingStation));

            return person;
        }

        protected Person GetPersonWithNonEligibleEligibleAddressWithoutStreets()
        {
            var person = new Person("1234327891023")
            {
                FirstName = "Alex7",
                MiddleName = "Alex7",
                Surname = "Ivan7",
                DateOfBirth = new DateTime(1970, 3, 3),
                Gender = GetFirstObjectFromDbTable(GetGender),
                Document = GetPersonDocument(),
                ExportedToSaise = false
            };

            person.ModifyStatus(
                GetFirstObjectFromDbTable(x => x.Name.StartsWith("Aleg"), GetPersonStatusType),
                "confirmation");

            person.SetEligibleAddress(GetFirstObjectFromDbTable(x =>
                (!x.IsEligible) && (x.Address != null) && (x.Address.Street != null) && (x.Address.Street.Region != null) &&
                (!x.Address.Street.Region.HasStreets), GetNonEligiblePersonAddressWithoutStreets));

            return person;
        }

        protected Person GetPersonWithNonEligibleEligibleAddressWithStreets()
        {
            var person = new Person("1234327891023")
            {
                FirstName = "Alex7",
                MiddleName = "Alex7",
                Surname = "Ivan7",
                DateOfBirth = new DateTime(1970, 3, 3),
                Gender = GetFirstObjectFromDbTable(GetGender),
                Document = GetPersonDocument(),
                ExportedToSaise = false
            };

            person.ModifyStatus(
                GetFirstObjectFromDbTable(x => x.Name.StartsWith("Aleg"), GetPersonStatusType),
                "confirmation");

            person.SetEligibleAddress(GetFirstObjectFromDbTable(x =>
                (!x.IsEligible) && (x.Address != null) && (x.Address.Street != null) && (x.Address.Street.Region != null) &&
                (!x.Address.Street.Region.HasStreets), GetNonEligiblePersonAddressWithStreets));

            return person;
        }

        protected Person GetPersonWithEligibleEligibleAddressWithoutStreets()
        {
            var person = new Person("1234355591023")
            {
                FirstName = "Alex8",
                MiddleName = "Alex8",
                Surname = "Ivan8",
                DateOfBirth = new DateTime(1970, 3, 3),
                Gender = GetFirstObjectFromDbTable(GetGender),
                Document = GetPersonDocument(),
                ExportedToSaise = false
            };

            person.ModifyStatus(
                GetFirstObjectFromDbTable(x => x.Name.StartsWith("Aleg"), GetPersonStatusType),
                "confirmation");

            person.SetEligibleAddress(GetFirstObjectFromDbTable(x =>
                (x.IsEligible) && (x.Address != null) && (x.Address.Street != null) && (x.Address.Street.Region != null) &&
                (!x.Address.Street.Region.HasStreets), GetEligiblePersonAddressWithoutStreets));

            return person;
        }

        protected Person GetPersonWithEligibleEligibleAddressWithoutStreets2()
        {
            var person = new Person("1234323429102")
            {
                FirstName = "Alex9",
                MiddleName = "Alex9",
                Surname = "Ivan9",
                DateOfBirth = new DateTime(1970, 3, 3),
                Gender = GetFirstObjectFromDbTable(GetGender),
                Document = GetPersonDocument(),
                ExportedToSaise = false
            };

            person.ModifyStatus(
                GetFirstObjectFromDbTable(x => x.Name.StartsWith("Aleg"), GetPersonStatusType),
                "confirmation");

            person.SetEligibleAddress(GetFirstObjectFromDbTable(GetEligiblePersonAddressWithoutStreets2, true));

            return person;
        }

        protected Person GetDeadPerson()
        {
            var person = new Person("1234657890123")
            {
                FirstName = "Alex2",
                MiddleName = "Alex2",
                Surname = "Ivan2",
                DateOfBirth = new DateTime(1971, 2, 2),
                Gender = GetFirstObjectFromDbTable(GetGender),
                Document = GetPersonDocument(),
                ExportedToSaise = false
            };

            person.ModifyStatus(
                GetFirstObjectFromDbTable(x => x.Name.StartsWith("Decedat"), GetPersonDeadStatusType),
                "confirmation");

            return person;
        }

        protected Gender GetGender()
        {
            return new Gender()
            {
                Description = "Masculin",
                Name = "M"
            };
        }

        protected PersonDocument GetPersonDocument()
        {
            return new PersonDocument()
            {
                IssuedBy = "of22",
                IssuedDate = new DateTime(1990, 12, 12),
                Number = "111111",
                Seria = "A",
                Type = GetFirstObjectFromDbTable(GetDocumentType),
                ValidBy = new DateTime(2020, 12, 12)
            };
        }

        protected DocumentType GetDocumentType()
        {
            return new DocumentType()
            {
                Description = "buletin de identitate",
                IsPrimary = true,
                Name = "BI"
            };
        }

        protected DocumentType GetNonPrimaryDocumentType()
        {
            return new DocumentType()
            {
                Description = "buletin de identitate",
                IsPrimary = false,
                Name = "BI2"
            };
        }

        protected PersonAddressType GetPersonAddressType()
        {
            return new PersonAddressType()
            {
                Name = "domiciliu",
                Description = "adresa de domiciliu"
            };
        }

        protected PublicAdministration GetPublicAdministration()
        {
            return new PublicAdministration(
                GetFirstObjectFromDbTable(GetRegion),
                GetFirstObjectFromDbTable(GetManagerType));
        }

        protected ManagerType GetManagerType()
        {
            return new ManagerType()
            {
                Description = "primar interimar",
                Name = "primar interimar"
            };
        }

        protected ElectionType GetElectionType()
        {
            return new ElectionType()
            {
                Name = "Alegeri Parlamentare",
                Description = "Alegeri Parlamentare in RM"
            };
        }

        protected Election GetElection()
        {
            return new Election()
            {
                Description = "Alegeri parlamentare in Republica Moldova 2014",
                StatusDate = new DateTime(2014, 10, 31),
                ElectionType = GetFirstObjectFromDbTable(GetElectionType)
            };
        }

        protected Election GetElectionWithSaiseId()
        {
            return new Election()
            {
                Description = "Alegeri parlamentare in RM 2014",
                StatusDate = new DateTime(2014, 10, 30),
                ElectionType = GetFirstObjectFromDbTable(GetElectionType)
            };
        }

        protected Election GetElectionWithAbroadDeclarations()
        {
            return new Election()
            {
                Description = "Alegeri parlamentare in Republica Moldova 2014",
                StatusDate = new DateTime(2014, 10, 31),
                ElectionType = GetFirstObjectFromDbTable(GetElectionType)
            };
        }

        protected NotificationReceiver GetNotificationReceiver()
        {
            return new NotificationReceiver(
                GetFirstObjectFromDbTable(GetNotification),
                GetCurrentUser())
            {
                NotificationIsRead = false
            };
        }

        protected Notification GetNotification()
        {
            return new Notification(
                GetFirstObjectFromDbTable(GetEvent),
                "notification");
        }

        protected Event GetEvent()
        {
            return new Event(EventTypes.Update, "entityType", 1);
        }

        protected PersonStatus GetPersonStatus()
        {
            return new PersonStatus()
            {
                Confirmation = "confirmation",
                IsCurrent = true,
                Person = GetFirstObjectFromDbTable(GetPerson),
                StatusType = GetFirstObjectFromDbTable(GetPersonStatusType)
            };
        }

        protected PersonStatusType GetPersonStatusType()
        {
            return new PersonStatusType()
            {
                Description = "description",
                IsExcludable = false,
                Name = "Alegător"
            };
        }

        protected PersonStatusType GetPersonDeadStatusType()
        {
            return new PersonStatusType()
            {
                Description = "description",
                IsExcludable = true,
                Name = "Decedat"
            };
        }

        protected StayStatement GetStayStatement()
        {
            return new StayStatement(
                GetFirstObjectFromDbTable(GetPerson),
                GetFirstObjectFromDbTable(GetPersonAddress),
                GetFirstObjectFromDbTable(GetPersonAddress),
                GetFirstObjectFromDbTable(GetElection)
                );
        }

        protected PollingStationStatistics GetPollingStationStatistics()
        {
            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);
            var region = GetFirstObjectFromDbTable(GetRegion);

            return new PollingStationStatistics()
            {
                PollingStation = pollingStation.Number,
                PollingStationId = pollingStation.Id,
                RegionId = region.Id,
                RegionName = region.Name,
                VotersCount = 10
            };
        }

        protected PersonFullAddress GetPersonFullAddress()
        {
            var person = GetFirstObjectFromDbTable(GetPerson);
            var personAddressType = GetFirstObjectFromDbTable(GetPersonAddressType);
            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);
            var region = pollingStation.Region;

            return new PersonFullAddress()
            {
                ApNumber = 12,
                ApSuffix = "A",
                FullAddress = "12A",
                IsEligible = true,
                Person = person,
                PersonAddressType = personAddressType,
                PollingStation = pollingStation,
                Region = region,
                RegionHasStreets = true,
                AssignedUser = new IdentityUserRegionView()
                {
                    IdentityUser = GetCurrentUser(),
                    Region = region
                }
            };
        }

        protected AbroadVoterRegistration GetAbroadVoterRegistration()
        {
            return new AbroadVoterRegistration(GetFirstObjectFromDbTable(GetPerson),
                "abroad", "residence", "country", 20, 20, "b.b@b.com", "192.168.222.222");
        }

        protected PersonByConflict GetPersonByConflict()
        {
            return new PersonByConflict()
            {
                Person = GetFirstObjectFromDbTable(GetPerson)
            };
        }

        protected RspRegistrationData GetRspRegistrationData()
        {
            var person = GetFirstObjectFromDbTable(x => x.EligibleAddress != null, GetPersonWithEligibleAddress);
            return GetRspRegistrationData(person);
        }

        protected RspRegistrationData GetRspRegistrationData(Person person)
        {
            var address = person.EligibleAddress;
            var doc = person.Document;
            var region = GetFirstObjectFromDbTable(x => x.RegistruId != null, GetRegionWithRegistruId);
            var personAddressType = GetFirstObjectFromDbTable(GetPersonAddressType);

            var rspModificationData = new RspModificationData
            {
                Idnp = person.Idnp,
                AcceptConflictCode = ConflictStatusCode.None,

                Birthdate = person.DateOfBirth,
                CitizenRm = true,
                Comments = "comments",
                Dead = false,
                DocumentTypeCode = DocumentType.Parse(5),
                ValidBydate = doc.ValidBy,
                FirstName = person.FirstName,

                IssuedDate = doc.IssuedDate,
                LastName = person.Surname,

                Number = doc.Number,

                RejectConflictCode = ConflictStatusCode.None,
                SecondName = person.MiddleName,
                Seria = doc.Seria,
                SexCode = person.Gender.Name,
                Source = SourceEnum.Alegator,
                StatusConflictCode = ConflictStatusCode.StatusConflict,
                StatusDate = DateTime.Now,
                StatusMessage = "message",
                Validity = true
            };

            var rspRegistrationData = new RspRegistrationData()
            {
                Administrativecode = region.RegistruId.HasValue ? (int)region.RegistruId.Value : 0,
                ApartmentNumber = address.ApNumber.HasValue ? address.ApNumber.Value : 0,
                ApartmentSuffix = address.ApSuffix,
                HouseNumber = address.Address.HouseNumber.HasValue ? address.Address.HouseNumber.Value : 0,
                HouseSuffix = address.Address.Suffix,
                Locality = region.Name,
                Region = region.Name,
                RegTypeCode = (int?)personAddressType.Id,
                StreetCode = 1,
                DateOfRegistration = DateTime.Now,
                IsInConflict = true,
                DateOfExpiration = new DateTime(2020, 10, 10),
                RspModificationData = rspModificationData
            };

            rspModificationData.Registrations.Add(rspRegistrationData);
            rspRegistrationData.RspModificationData = rspModificationData;

            return rspRegistrationData;
        }


        public RspModificationData GetRspModificationData()
        {
            return GetRspRegistrationData().RspModificationData;
        }

        protected SaiseExporter GetSaiseExporter()
        {
            var election = GetFirstObjectFromDbTable(x => x.ElectionRounds != null, GetElectionWithSaiseId);
            return new SaiseExporter(1);
        }

        protected SaiseExporterStage GetSaiseExporterStage()
        {
            var saiseExporter = GetFirstObjectFromDbTable(GetSaiseExporter);

            return new SaiseExporterStage(saiseExporter)
            {
                StageType = SaiseExporterStageType.ElectionValidation,
                Description = "description",
                Status = SaiseExporterStageStatus.Failed
            };
        }

        protected PrintSession GetPrintSession()
        {
            var election = GetFirstObjectFromDbTable(GetElection);
            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);
            var pollingStations = new List<PollingStation>() { pollingStation };

            return new PrintSession(election, pollingStations)
            {
                Status = PrintStatus.InProgress
            };
        }

        protected PrintSession GetPendingPrintSession()
        {
            var election = GetFirstObjectFromDbTable(GetElection);
            var pollingStation = GetFirstObjectFromDbTable(GetPollingStation);
            var pollingStations = new List<PollingStation>() { pollingStation };

            return new PrintSession(election, pollingStations)
            {
                Status = PrintStatus.Pending
            };
        }

        protected ExportPollingStation GetExportPollingStation()
        {
            return new ExportPollingStation(
                GetFirstObjectFromDbTable(GetPrintSession),
                GetFirstObjectFromDbTable(GetPollingStation)
                );
        }

        protected SRVIdentityUser GetSrvIdentityUser()
        {
            var user = new SRVIdentityUser()
            {
                AdditionalInfo = GetFirstObjectFromDbTable(GetAdditionalUserInfo),
                Comments = "comments",
                IsBuiltIn = true,
                PasswordHash = "hash",
                SecurityStamp = "stamp",
                UserName = "user"
            };

            var role = GetFirstObjectFromDbTable(x => x.Name == "Administrator", GetIdentityRole);

            if (!user.Roles.Contains(role))
            {
                user.Roles.Add(role);
            }

            return user;
        }

        protected SRVIdentityUser GetNonBuiltInSrvIdentityUser()
        {
            var user = new SRVIdentityUser()
            {
                AdditionalInfo = this.GetFirstObjectFromDbTable(GetAdditionalUserInfo),
                Comments = "comments2",
                IsBuiltIn = false,
                PasswordHash = "hash2",
                SecurityStamp = "stamp2",
                UserName = "user2"
            };

            var role = this.GetFirstObjectFromDbTable(x => x.Name == "Administrator", GetIdentityRole);

            if (!user.Roles.Contains(role))
            {
                user.Roles.Add(role);
            }

            return user;
        }

        protected AdditionalUserInfo GetAdditionalUserInfo()
        {
            return new AdditionalUserInfo()
            {
                DateOfBirth = new DateTime(1983, 10, 10),
                Email = "e.e@e.e",
                FirstName = "firstname",
                Gender = this.GetFirstObjectFromDbTable(this.GetGender),
                LandlinePhone = "landlinephone",
                LastName = "lastname",
                MobilePhone = "mobilephone",
                WorkInfo = "workinfo"
            };
        }

        protected RsaUser GetRsaUser()
        {
            return new RsaUser()
            {
                Comments = "comments",
                LoginName = "login",
                Password = "password",
                Region = GetFirstObjectFromDbTable(GetRegion),
                Source = SourceEnum.Rsp,
                StatusDate = DateTime.Now,
                StatusMessage = "message",
                Created = DateTime.Now
            };
        }

        protected IdentityRole GetIdentityRole()
        {
            return new IdentityRole("Administrator");
        }

        protected IdentityRole GetRegistratorIdentityRole()
        {
            return new IdentityRole("Registrator");
        }

        protected LinkedRegionsFullName GetLinkedRegionsFullName()
        {
            return new LinkedRegionsFullName()
            {
                FullyQualifiedName = "FullyQualifiedName",
                LinkedRegion = GetFirstObjectFromDbTable(GetLinkedRegion)
            };
        }

        protected LinkedRegion GetLinkedRegion()
        {
            return new LinkedRegion(
                new List<Region>()
                {
                    GetFirstObjectFromDbTable(GetRegion)
                });
        }

        protected ImportStatistic GetImportStatistic()
        {
            var region = GetFirstObjectFromDbTable(GetRegion);

            return new ImportStatistic()
            {
                ChangedStatus = 1,
                Conflicted = 1,
                Date = DateTime.Now.Date,
                Error = 1,
                New = 1,
                Region = region.Id,
                ResidenceChnaged = 1,
                Total = 1,
                Updated = 1
            };
        }

        protected PageRequest GetPageRequest(bool needSort = false)
        {
            var request = new PageRequest()
            {
                PageNumber = 1,
                PageSize = 20,
                FilterGroups = new List<FilterGroup>(),
                SortFields = new List<SortField>()
            };

            if (needSort)
            {
                request.SortFields.Add(new SortField() { Ascending = true, Property = "Id" });
            }

            return request;
        }

        protected PageRequest GetBigPageRequest(int totalCount, bool needSort)
        {
            var request = GetPageRequest(needSort);
            request.PageSize = 1 + (totalCount / 2);
            return request;
        }

        #endregion Entities

        #region overrided methods

        protected override void LoadData()
        {

        }

        #endregion overrided methods
    }
}
