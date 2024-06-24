using Amdaris.Domain;
using Amdaris.Domain.Identity;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV;
using CEC.Web.SRV.Infrastructure;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SAISE.Admin.WebApp.Infrastructure
{
    public static class SyncHelper
    {
        private static readonly object LockObj = new object();

        public static void CreateUpdateDelete<T>(SyncItem syncItem) where T : AuditedEntity<IdentityUser>
        {
            var sessionFactory = IoC.GetSessionFactory();
            var repository = new SrvRepository(sessionFactory);

            try
            {

                T entity = null;

                //if (syncItem.Operation != EnumEntityOperations.Create)
                {
                    entity = repository.Get<T>(syncItem.Id);
                }

                if (syncItem.Operation == EnumEntityOperations.Delete || syncItem.Operation == EnumEntityOperations.UnDelete || syncItem.Operation == EnumEntityOperations.PersistentDelete)
                {
                    Synchronise(sessionFactory, entity, syncItem.Operation);
                    return;
                }

                switch (typeof(T).Name)
                {
                    case "RegionType":
                        var entRegionType = entity as RegionType;
                        if (entRegionType == null)
                        {
                            syncItem.Operation = EnumEntityOperations.Create;
                            entRegionType = new RegionType();
                            entRegionType.SetId(syncItem.Id);
                        }
                        entRegionType.Name = (string)syncItem.Model["Name"];
                        entRegionType.Description = (string)syncItem.Model["Description"];
                        entRegionType.Rank = (byte)Convert.ChangeType(syncItem.Model["Rank"], typeof(byte));
                        entity = entRegionType as T;
                        break;
                    case "Region":
                        var entRegion = entity as Region;
                        long? parentId = (long?)syncItem.Model["ParentRegionId"];
                        var regionType = repository.LoadProxy<RegionType>((long)syncItem.Model["RegionTypeId"]);
                        var parentRegion = parentId != null ? repository.LoadProxy<Region>(parentId.Value) : null;

                        if (entRegion == null)
                        {
                            syncItem.Operation = EnumEntityOperations.Create;
                            entRegion = new Region(parentRegion, regionType);
                            entRegion.SetId(syncItem.Id);
                        }
                        else
                        {
                            entRegion.ChangeParent(parentRegion);
                            entRegion.ChangeRegionType(regionType);
                        }
                        entRegion.Name = (string)syncItem.Model["Name"];
                        entRegion.Description = (string)syncItem.Model["Description"];
                        entRegion.HasStreets = (bool)syncItem.Model["HasStreets"];
                        entRegion.StatisticIdentifier = (long?)syncItem.Model["StatisticIdentifier"];
                        entRegion.StatisticCode = (long?)syncItem.Model["StatisticCode"];
                        entity = entRegion as T;
                        break;
                    case "Street":
                        var entStreet = entity as Street;
                        var streetType = repository.LoadProxy<StreetType>((long)syncItem.Model["StreetTypeId"]);
                        var region = repository.LoadProxy<Region>((long)syncItem.Model["RegionId"]);

                        if (entStreet == null)
                        {
                            syncItem.Operation = EnumEntityOperations.Create;
                            entStreet = new Street(region, streetType, (string)syncItem.Model["Name"]);
                            entStreet.SetId(syncItem.Id);
                        }
                        else
                        {
                            entStreet.ChangeRegion(region);
                            entStreet.StreetType = streetType;
                        }
                        entStreet.Name = (string)syncItem.Model["Name"];
                        entStreet.Description = (string)syncItem.Model["Description"];
                        entity = entStreet as T;
                        break;
                    case "StreetType":
                        var entStreetType = entity as StreetType;
                        if (entStreetType == null)
                        {
                            syncItem.Operation = EnumEntityOperations.Create;
                            entStreetType = new StreetType();
                            entStreetType.SetId(syncItem.Id);
                        }
                        entStreetType.Name = (string)syncItem.Model["Name"];
                        entStreetType.Description = (string)syncItem.Model["Description"];
                        entity = entStreetType as T;
                        break;
                    case "Gender":
                        var entGender = entity as Gender;
                        if (entGender == null)
                        {
                            syncItem.Operation = EnumEntityOperations.Create;
                            entGender = new Gender();
                            entGender.SetId(syncItem.Id);
                        }
                        entGender.Name = (string)syncItem.Model["Name"];
                        entGender.Description = (string)syncItem.Model["Description"];
                        entity = entGender as T;
                        break;
                    case "DocumentType":
                        var entDocumentType = entity as DocumentType;
                        if (entDocumentType == null)
                        {
                            syncItem.Operation = EnumEntityOperations.Create;
                            entDocumentType = new DocumentType();
                            entDocumentType.SetId(syncItem.Id);
                        }
                        entDocumentType.Name = (string)syncItem.Model["Name"];
                        entDocumentType.Description = (string)syncItem.Model["Description"];
                        entDocumentType.IsPrimary = (bool)syncItem.Model["IsPrimary"];
                        if (entDocumentType.IsPrimary)
                        {
                            repository.Query<DocumentType>()
                                .Where(x => x.IsPrimary)
                                .ForEach(x =>
                                {
                                    x.IsPrimary = false;
                                    repository.SaveOrUpdate(x);
                                });
                        }

                        entity = entDocumentType as T;
                        break;
                    case "PersonStatusType":
                        var entPersonStatusType = entity as PersonStatusType;
                        if (entPersonStatusType == null)
                        {
                            syncItem.Operation = EnumEntityOperations.Create;
                            entPersonStatusType = new PersonStatusType();
                            entPersonStatusType.SetId(syncItem.Id);
                        }
                        entPersonStatusType.Name = (string)syncItem.Model["Name"];
                        entPersonStatusType.Description = (string)syncItem.Model["Description"];
                        entPersonStatusType.IsExcludable = (bool)syncItem.Model["IsExcludable"];
                        entity = entPersonStatusType as T;
                        break;
                    case "PersonAddressType":
                        var entPersonAddressType = entity as PersonAddressType;
                        if (entPersonAddressType == null)
                        {
                            syncItem.Operation = EnumEntityOperations.Create;
                            entPersonAddressType = new PersonAddressType();
                            entPersonAddressType.SetId(syncItem.Id);
                        }
                        entPersonAddressType.Name = (string)syncItem.Model["Name"];
                        entPersonAddressType.Description = (string)syncItem.Model["Description"];
                        entity = entPersonAddressType as T;
                        break;
                    case "PollingStation":
                        var entPollingStation = entity as PollingStation;
                        var regionPollingStation = repository.LoadProxy<Region>((long)syncItem.Model["RegionId"]);

                        if (entPollingStation == null)
                        {
                            syncItem.Operation = EnumEntityOperations.Create;
                            entPollingStation = new PollingStation(regionPollingStation);
                            entPollingStation.SetId(syncItem.Id);
                        }
                        entPollingStation.Number = (string)syncItem.Model["Number"];
                        entPollingStation.ChangeRegion(regionPollingStation);
                        entPollingStation.Location = (string)syncItem.Model["NameRo"];
                        entPollingStation.PollingStationType = (PollingStationTypes)(long)syncItem.Model["PollingStationType"];
                        entPollingStation.GeoLocation = new GeoLocation
                        {
                            Latitude = syncItem.Model["Latitude"] != null ? (double)syncItem.Model["Latitude"] : (double?)null,
                            Longitude = syncItem.Model["Longitude"] != null ? (double)syncItem.Model["Longitude"] : (double?)null
                        };
                        entPollingStation.ContactInfo = (string)syncItem.Model["Phone"];
                        entity = entPollingStation as T;
                        break;
                    case "ElectionType":
                        var entElectionType = entity as ElectionType;
                        var electionTypeCircumscriptionList = repository.LoadProxy<CircumscriptionList>((long)syncItem.Model["CircumscriptionListId"]);
                        if (entElectionType == null)
                        {
                            syncItem.Operation = EnumEntityOperations.Create;
                            entElectionType = new ElectionType();
                            entElectionType.SetId(syncItem.Id);
                        }
                        entElectionType.Name = (string)syncItem.Model["Name"];
                        entElectionType.Description = (string)syncItem.Model["Description"];
                        entElectionType.Code = (int)(long)syncItem.Model["Code"];
                        entElectionType.ElectionCompetitorType = (ElectionCompetitorType)(long)syncItem.Model["ElectionCompetitorType"];
                        entElectionType.ElectionRoundsNo = (int)(long)syncItem.Model["ElectionRoundsNo"];
                        entElectionType.ElectionArea = (ElectionArea)(long)syncItem.Model["ElectionArea"];
                        entElectionType.AcceptResidenceDoc = (bool)syncItem.Model["AcceptResidenceDoc"];
                        entElectionType.AcceptVotingCert = (bool)syncItem.Model["AcceptVotingCert"];
                        entElectionType.AcceptAbroadDeclaration = (bool)syncItem.Model["AcceptAbroadDeclaration"];
                        entElectionType.CircumscriptionList = electionTypeCircumscriptionList;
                        entity = entElectionType as T;
                        break;
                    case "Election":
                        var entElection = entity as Election;
                        var electionType = repository.LoadProxy<ElectionType>((long)syncItem.Model["ElectionTypeId"]);
                        if (entElection == null)
                        {
                            syncItem.Operation = EnumEntityOperations.Create;
                            entElection = new Election();
                            entElection.SetId(syncItem.Id);
                            entElection.StatusDate = DateTimeOffset.Now;
                        }
                        entElection.ElectionType = electionType;
                        entElection.NameRo = (string)syncItem.Model["Description"];
                        entElection.NameRu = string.Empty;
                        entElection.Description = (string)syncItem.Model["Description"];
                        entElection.Status = (ElectionStatus)(long)syncItem.Model["Status"];

                        entity = entElection as T;
                        break;
                    case "ElectionRound":
                        var entElectionRound = entity as ElectionRound;
                        var election = repository.LoadProxy<Election>((long)syncItem.Model["ElectionId"]);
                        if (entElectionRound == null)
                        {
                            syncItem.Operation = EnumEntityOperations.Create;
                            entElectionRound = new ElectionRound();
                            entElectionRound.SetId(syncItem.Id);
                        }
                        entElectionRound.Election = election;
                        entElectionRound.Number = (int)(long)syncItem.Model["Number"];
                        entElectionRound.NameRo = "Tur " + (int)(long)syncItem.Model["Number"];
                        entElectionRound.NameRu = null;
                        entElectionRound.Description = null;
                        entElectionRound.ElectionDate = (DateTime)syncItem.Model["ElectionDate"];
                        entElectionRound.CampaignStartDate = (DateTime?)syncItem.Model["CampaignStartDate"];
                        entElectionRound.CampaignEndDate = (DateTime?)syncItem.Model["CampaignEndDate"];
                        entElectionRound.ReportsPath = null;
                        entElectionRound.Status = (ElectionRoundStatus)(long)syncItem.Model["Status"];
                        entity = entElectionRound as T;
                        break;
                    case "Circumscription":
                        var entCircumscription = entity as Circumscription;
                        var circumscriptionList = repository.LoadProxy<CircumscriptionList>((long)syncItem.Model["CircumscriptionListId"]);
                        var regionCircumscription = repository.LoadProxy<Region>((long)syncItem.Model["RegionId"]);
                        if (entCircumscription == null)
                        {
                            syncItem.Operation = EnumEntityOperations.Create;
                            entCircumscription = new Circumscription();
                            entCircumscription.SetId(syncItem.Id);
                        }
                        entCircumscription.Number = (string)syncItem.Model["Number"];
                        entCircumscription.NameRo = (string)syncItem.Model["NameRo"];
                        entCircumscription.NameRu = (string)syncItem.Model["NameRu"];
                        entCircumscription.CircumscriptionList = circumscriptionList;
                        entCircumscription.Region = regionCircumscription;
                        entity = entCircumscription as T;
                        break;
                    case "CircumscriptionList":
                        var entCircumscriptionList = entity as CircumscriptionList;
                        if (entCircumscriptionList == null)
                        {
                            syncItem.Operation = EnumEntityOperations.Create;
                            entCircumscriptionList = new CircumscriptionList();
                            entCircumscriptionList.SetId(syncItem.Id);
                        }
                        entCircumscriptionList.Name = (string)syncItem.Model["Name"];
                        entity = entCircumscriptionList as T;
                        break;
                }
                Synchronise(sessionFactory, entity, syncItem.Operation);
            }
            catch (Exception ex)
            {
                sessionFactory.GetCurrentSession().Close();
            }
        }


        private static void Synchronise<T>(ISessionFactory sessionFactory, T entity, EnumEntityOperations option) where T : AuditedEntity<IdentityUser>
        {
            lock (LockObj)
            {
                var repository = new SrvRepository(sessionFactory);
                using (var transaction = sessionFactory.GetCurrentSession().BeginTransaction(IsolationLevel.RepeatableRead))
                {
                    try
                    {
                        transaction.Begin();
                        switch (option)
                        {
                            case EnumEntityOperations.Create:
                                var metadata = sessionFactory.GetClassMetadata(typeof(T)) as NHibernate.Persister.Entity.AbstractEntityPersister;
                                var table = metadata.TableName;
                                var userId = HttpContext.Current.User.Identity.GetUserId();
                                var user = repository.LoadProxy<IdentityUser, string>(userId);
                                entity.CreatedBy = user;
                                entity.Created = DateTimeOffset.Now;
                                var sql = sessionFactory.GetCurrentSession().CreateSQLQuery(string.Format("SET IDENTITY_INSERT {0} ON", table));
                                sql.UniqueResult();
                                sessionFactory.GetCurrentSession().Save(entity, entity.Id);
                                break;
                            case EnumEntityOperations.Update:
                                repository.SaveOrUpdate(entity);
                                break;
                            case EnumEntityOperations.Delete:
                                if (entity != null)
                                {
                                    repository.Delete(entity);
                                }
                                break;
                            case EnumEntityOperations.PersistentDelete:
                                if (entity != null)
                                {
                                    var result = repository.TryToDeletePersistent<T>(entity.Id);
                                    if (!result) repository.Delete(entity);
                                }
                                break;
                            case EnumEntityOperations.UnDelete:
                                if (entity != null)
                                {
                                    repository.UnDelete<T>(entity.Id);
                                }
                                break;
                        }
                        sessionFactory.GetCurrentSession().Flush();
                        transaction.Commit();
                        sessionFactory.GetCurrentSession().Close();
                    }
                    catch (Exception ex)
                    {
                        sessionFactory.GetCurrentSession().Flush();
                        transaction.Rollback();
                        sessionFactory.GetCurrentSession().Close();
                        transaction.Dispose();
                    }
                }
            }
        }
    }
}