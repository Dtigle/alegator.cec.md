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
    public class BllTests : BaseBllTests
    {
        #region Tests

        #region Get

        [TestMethod]
        public void BaseGetAddressById_returns_correct_result()
        {
            GetByIdTest(GetAddress);
        }

        [TestMethod]
        public void BaseGetStreetById_returns_correct_result()
        {
            GetByIdTest(GetStreet);
        }

        [TestMethod]
        public void BaseGetRegionById_returns_correct_result()
        {
            GetByIdTest(GetRegion);
        }

        [TestMethod]
        public void BaseGetRegionTypeById_returns_correct_result()
        {
            GetByIdTest(GetRegionType);
        }

        [TestMethod]
        public void BaseGetStreetTypeById_returns_correct_result()
        {
            GetByIdTest(GetStreetType);
        }

        [TestMethod]
        public void BaseGetPollingStationById_returns_correct_result()
        {
            GetByIdTest(GetPollingStation);
        }

        [TestMethod]
        public void BaseGetPersonAddressById_returns_correct_result()
        {
            GetByIdTest(GetPersonAddress);
        }

        [TestMethod]
        public void BaseGetPersonById_returns_correct_result()
        {
            GetByIdTest(GetPerson);
        }

        [TestMethod]
        public void BaseGetPersonAddressTypeById_returns_correct_result()
        {
            GetByIdTest(GetPersonAddressType);
        }

        [TestMethod]
        public void BaseGetPublicAdministrationById_returns_correct_result()
        {
            GetByIdTest(GetPublicAdministration);
        }

        [TestMethod]
        public void BaseGetManagerTypeById_returns_correct_result()
        {
            GetByIdTest(GetManagerType);
        }

        [TestMethod]
        public void BaseGetGenderById_returns_correct_result()
        {
            GetByIdTest(GetGender);
        }

        [TestMethod]
        public void BaseGetDocumentTypeById_returns_correct_result()
        {
            GetByIdTest(GetDocumentType);
        }

        [TestMethod]
        public void BaseGetElectionTypeById_returns_correct_result()
        {
            GetByIdTest(GetElectionType);
        }

        [TestMethod]
        public void BaseGetElectionById_returns_correct_result()
        {
            GetByIdTest(GetElection);
        }

        [TestMethod]
        public void BaseGetNotificationReceiverById_returns_correct_result()
        {
            GetByIdTest(GetNotificationReceiver);
        }

        [TestMethod]
        public void BaseGetNotificationById_returns_correct_result()
        {
            GetByIdTest(GetNotification);
        }

        [TestMethod]
        public void BaseGetEventById_returns_correct_result()
        {
            GetByIdTest(GetEvent);
        }

        [TestMethod]
        public void BaseGetPersonStatusById_returns_correct_result()
        {
            GetByIdTest(GetPersonStatus);
        }

        [TestMethod]
        public void BaseGetPersonStatusTypeById_returns_correct_result()
        {
            GetByIdTest(GetPersonStatusType);
        }

        [TestMethod]
        public void BaseGetStayStatementById_returns_correct_result()
        {
            GetByIdTest(GetPersonStatusType);
        }

        [TestMethod]
        public void BaseGetAbroadVoterRegistrationById_returns_correct_result()
        {
            GetByIdTest(GetAbroadVoterRegistration);
        }

        [TestMethod]
        public void BaseGetSaiseExporterById_returns_correct_result()
        {
            GetByIdTest(GetSaiseExporter);
        }

        [TestMethod]
        public void BaseGetPrintSessionById_returns_correct_result()
        {
            GetByIdTest(GetPrintSession);
        }

        [TestMethod]
        public void BaseGetExportPollingStationById_returns_correct_result()
        {
            GetByIdTest(GetExportPollingStation);
        }

        [TestMethod]
        public void BaseGetStreetTypeCodeById_returns_correct_result()
        {
            GetByIdTest(GetStreetTypeCode);
        }

        [TestMethod]
        public void BaseGetAdditionalUserInfoById_returns_correct_result()
        {
            GetByIdTest(GetAdditionalUserInfo);
        }

        [TestMethod]
        public void BaseGetSaiseExporterStageById_returns_correct_result()
        {
            GetByIdTest(GetSaiseExporterStage);
        }

        [TestMethod]
        public void BaseGetRsaUserById_returns_correct_result()
        {
            GetByIdTest(GetRsaUser);
        }
        
        #endregion Get

        #region GetAll

        [TestMethod]
        public void BaseGetAllAddresses_returns_correct_result()
        {
            GetAllTest<Address>();
        }

        [TestMethod]
        public void BaseGetAllStreets_returns_correct_result()
        {
            GetAllTest<Street>();
        }

        [TestMethod]
        public void BaseGetAllRegions_returns_correct_result()
        {
            GetAllTest<Region>();
        }

        [TestMethod]
        public void BaseGetAllRegionTypes_returns_correct_result()
        {
            GetAllTest<RegionType>();
        }

        [TestMethod]
        public void BaseGetAllStreetTypes_returns_correct_result()
        {
            GetAllTest<StreetType>();
        }

        [TestMethod]
        public void BaseGetAllPollingStations_returns_correct_result()
        {
            GetAllTest<PollingStation>();
        }

        [TestMethod]
        public void BaseGetAllPersonAddresses_returns_correct_result()
        {
            GetAllTest<PersonAddress>();
        }

        [TestMethod]
        public void BaseGetAllPersons_returns_correct_result()
        {
            GetAllTest<Person>();
        }

        [TestMethod]
        public void BaseGetAllPersonAddressTypes_returns_correct_result()
        {
            GetAllTest<PersonAddressType>();
        }

        [TestMethod]
        public void BaseGetAllPublicAdministrations_returns_correct_result()
        {
            GetAllTest<PublicAdministration>();
        }

        [TestMethod]
        public void BaseGetAllManagerTypes_returns_correct_result()
        {
            GetAllTest<ManagerType>();
        }

        [TestMethod]
        public void BaseGetAllGenders_returns_correct_result()
        {
            GetAllTest<Gender>();
        }

        [TestMethod]
        public void BaseGetAllDocumentTypes_returns_correct_result()
        {
            GetAllTest<DocumentType>();
        }

        [TestMethod]
        public void BaseGetAllElectionTypes_returns_correct_result()
        {
            GetAllTest<ElectionType>();
        }

        [TestMethod]
        public void BaseGetAllElections_returns_correct_result()
        {
            GetAllTest<Election>();
        }

        [TestMethod]
        public void BaseGetAllNotificationReceivers_returns_correct_result()
        {
            GetAllTest<NotificationReceiver>();
        }

        [TestMethod]
        public void BaseGetAllNotifications_returns_correct_result()
        {
            GetAllTest<Notification>();
        }

        [TestMethod]
        public void BaseGetAllEvents_returns_correct_result()
        {
            GetAllTest<Event>();
        }

        [TestMethod]
        public void BaseGetAllPersonStatuses_returns_correct_result()
        {
            GetAllTest<PersonStatus>();
        }

        [TestMethod]
        public void BaseGetAllPersonStatusTypes_returns_correct_result()
        {
            GetAllTest<PersonStatusType>();
        }

        [TestMethod]
        public void BaseGetAllStayStatements_returns_correct_result()
        {
            GetAllTest<StayStatement>();
        }

        [TestMethod]
        public void BaseGetAllPollingStationStatistics_returns_correct_result()
        {
            GetAllTest<PollingStationStatistics>();
        }

        [TestMethod]
        public void BaseGetAllPersonFullAddresses_returns_correct_result()
        {
            GetAllTest<PersonFullAddress>();
        }

        [TestMethod]
        public void BaseGetAllAbroadVoterRegistrations_returns_correct_result()
        {
            GetAllTest<AbroadVoterRegistration>();
        }

        [TestMethod]
        public void BaseGetAllPersonsByConflict_returns_correct_result()
        {
            GetAllTest<PersonByConflict>();
        }

        [TestMethod]
        public void BaseGetAllRspModificationData_returns_correct_result()
        {
            GetAllTest<RspModificationData>();
        }

        [TestMethod]
        public void BaseGetAllRspConflictModificationData_returns_correct_result()
        {
            GetAllTest<RspConflictData>();
        }

        [TestMethod]
        public void BaseGetAllRspConflictModificationDataAdmin_returns_correct_result()
        {
            GetAllTest<RspConflictDataAdmin>();
        }

        [TestMethod]
        public void BaseGetAllSaiseExporters_returns_correct_result()
        {
            GetAllTest<SaiseExporter>();
        }

        [TestMethod]
        public void BaseGetAllPrintSessions_returns_correct_result()
        {
            GetAllTest<PrintSession>();
        }

        [TestMethod]
        public void BaseGetAllExportPollingStations_returns_correct_result()
        {
            GetAllTest<ExportPollingStation>();
        }

        [TestMethod]
        public void BaseGetAllStreetTypeCodes_returns_correct_result()
        {
            GetAllTest<StreetTypeCode>();
        }

        [TestMethod]
        public void BaseGetAllAdditionalUserInfos_returns_correct_result()
        {
            GetAllTest<AdditionalUserInfo>();
        }

        [TestMethod]
        public void BaseGetAllSaiseExporterStages_returns_correct_result()
        {
            GetAllTest<SaiseExporterStage>();
        }

        [TestMethod]
        public void BaseGetAllRsaUsers_returns_correct_result()
        {
            GetAllTest<RsaUser>();
        }
        
        #endregion GetAll

        #region GetPage
        
        [TestMethod]
        public void BaseGetAddressesPage_returns_correct_result()
        {
            GetByPageRequestTest<Address>();
        }

        [TestMethod]
        public void BaseGetStreetsPage_returns_correct_result()
        {
            GetByPageRequestTest<Street>();
        }

        [TestMethod]
        public void BaseGetRegionsPage_returns_correct_result()
        {
            GetByPageRequestTest<Region>();
        }

        [TestMethod]
        public void BaseGetRegionTypesPage_returns_correct_result()
        {
            GetByPageRequestTest<RegionType>();
        }

        [TestMethod]
        public void BaseGetStreetTypesPage_returns_correct_result()
        {
            GetByPageRequestTest<StreetType>();
        }

        [TestMethod]
        public void BaseGetPollingStationsPage_returns_correct_result()
        {
            GetByPageRequestTest<PollingStation>();
        }

        [TestMethod]
        public void BaseGetPersonAddressesPage_returns_correct_result()
        {
            GetByPageRequestTest<PersonAddress>();
        }

        [TestMethod]
        public void BaseGetPersonsPage_returns_correct_result()
        {
            GetByPageRequestTest<Person>();
        }

        [TestMethod]
        public void BaseGetPersonAddressTypesPage_returns_correct_result()
        {
            GetByPageRequestTest<PersonAddressType>();
        }

        [TestMethod]
        public void BaseGetPublicAdministrationsPage_returns_correct_result()
        {
            GetByPageRequestTest<PublicAdministration>();
        }

        [TestMethod]
        public void BaseGetManagerTypesPage_returns_correct_result()
        {
            GetByPageRequestTest<ManagerType>();
        }

        [TestMethod]
        public void BaseGetGendersPage_returns_correct_result()
        {
            GetByPageRequestTest<Gender>();
        }

        [TestMethod]
        public void BaseGetDocumentTypesPage_returns_correct_result()
        {
            GetByPageRequestTest<DocumentType>();
        }

        [TestMethod]
        public void BaseGetElectionTypesPage_returns_correct_result()
        {
            GetByPageRequestTest<ElectionType>();
        }

        [TestMethod]
        public void BaseGetElectionsPage_returns_correct_result()
        {
            GetByPageRequestTest<Election>();
        }

        [TestMethod]
        public void BaseGetNotificationReceiversPage_returns_correct_result()
        {
            GetByPageRequestTest<NotificationReceiver>();
        }

        [TestMethod]
        public void BaseGetNotificationsPage_returns_correct_result()
        {
            GetByPageRequestTest<Notification>();
        }

        [TestMethod]
        public void BaseGetEventsPage_returns_correct_result()
        {
            GetByPageRequestTest<Event>();
        }

        [TestMethod]
        public void BaseGetPersonStatussesPage_returns_correct_result()
        {
            GetByPageRequestTest<PersonStatus>();
        }

        [TestMethod]
        public void BaseGetPersonStatusTypesPage_returns_correct_result()
        {
            GetByPageRequestTest<PersonStatusType>();
        }

        [TestMethod]
        public void BaseGetStayStatementsPage_returns_correct_result()
        {
            GetByPageRequestTest<StayStatement>();
        }

        [TestMethod]
        public void BaseGetPollingStationStatisticsPage_returns_correct_result()
        {
            GetByPageRequestTest<PollingStationStatistics>();
        }

        [TestMethod]
        public void BaseGetPersonFullAddressesPage_returns_correct_result()
        {
            GetByPageRequestTest<PersonFullAddress>();
        }

        [TestMethod]
        public void BaseGetAbroadVoterRegistrationsPage_returns_correct_result()
        {
            GetByPageRequestTest<AbroadVoterRegistration>();
        }

        [TestMethod]
        public void BaseGetPersonsByConflictPage_returns_correct_result()
        {
            GetByPageRequestTest<PersonByConflict>();
        }

        [TestMethod]
        public void BaseGetRspModificationDataPage_returns_correct_result()
        {
            GetByPageRequestTest<RspModificationData>();
        }

        [TestMethod]
        public void BaseGetRspConflictModificationDataPage_returns_correct_result()
        {
            GetByPageRequestTest<RspConflictData>();
        }

        [TestMethod]
        public void BaseGetRspConflictModificationDataAdminPage_returns_correct_result()
        {
            GetByPageRequestTest<RspConflictDataAdmin>();
        }

        [TestMethod]
        public void BaseGetSaiseExportersPage_returns_correct_result()
        {
            GetByPageRequestTest<SaiseExporter>();
        }

        [TestMethod]
        public void BaseGetPrintSessionsPage_returns_correct_result()
        {
            GetByPageRequestTest<PrintSession>();
        }

        [TestMethod]
        public void BaseGetExportPollingStationsPage_returns_correct_result()
        {
            GetByPageRequestTest<ExportPollingStation>();
        }

        [TestMethod]
        public void BaseGetStreetTypeCodesPage_returns_correct_result()
        {
            GetByPageRequestTest<StreetTypeCode>();
        }

        [TestMethod]
        public void BaseGetAdditionalUserInfosPage_returns_correct_result()
        {
            GetByPageRequestTest<AdditionalUserInfo>();
        }

        [TestMethod]
        public void BaseGetSaiseExporterStagesPage_returns_correct_result()
        {
            GetByPageRequestTest<SaiseExporterStage>();
        }

        [TestMethod]
        public void BaseGetRsaUsersPage_returns_correct_result()
        {
            GetByPageRequestTest<RsaUser>();
        }

        #endregion GetPage

        #region Delete

        [TestMethod]
        public void BaseDeleteAddressById_has_correct_logic()
        {
            DeleteByIdTest(GetAddress);
        }

        [TestMethod]
        public void BaseDeleteStreetById_has_correct_logic()
        {
            DeleteByIdTest(GetStreet);
        }
        
        [TestMethod]
        public void BaseDeleteRegionById_returns_correct_result()
        {
            DeleteByIdTest(GetRegion);
        }

        [TestMethod]
        public void BaseDeleteRegionTypeById_returns_correct_result()
        {
            DeleteByIdTest(GetRegionType);
        }

        [TestMethod]
        public void BaseDeleteStreetTypeById_returns_correct_result()
        {
            DeleteByIdTest(GetStreetType);
        }

        [TestMethod]
        public void BaseDeletePollingStationById_returns_correct_result()
        {
            DeleteByIdTest(GetPollingStation);
        }

        [TestMethod]
        public void BaseDeletePersonAddressById_returns_correct_result()
        {
            DeleteByIdTest(GetPersonAddress);
        }

        [TestMethod]
        public void BaseDeletePersonById_returns_correct_result()
        {
            DeleteByIdTest(GetPerson);
        }

        [TestMethod]
        public void BaseDeletePersonAddressTypeById_returns_correct_result()
        {
            DeleteByIdTest(GetPersonAddressType);
        }

        [TestMethod]
        public void BaseDeletePublicAdministrationById_returns_correct_result()
        {
            DeleteByIdTest(GetPublicAdministration);
        }

        [TestMethod]
        public void BaseDeleteManagerTypeById_returns_correct_result()
        {
            DeleteByIdTest(GetManagerType);
        }

        [TestMethod]
        public void BaseDeleteGenderById_returns_correct_result()
        {
            DeleteByIdTest(GetGender);
        }

        [TestMethod]
        public void BaseDeleteDocumentTypeById_returns_correct_result()
        {
            DeleteByIdTest(GetDocumentType);
        }

        [TestMethod]
        public void BaseDeleteElectionTypeById_returns_correct_result()
        {
            DeleteByIdTest(GetElectionType);
        }

        [TestMethod]
        public void BaseDeleteElectionById_returns_correct_result()
        {
            DeleteByIdTest(GetElection);
        }

        [TestMethod]
        public void BaseDeleteNotificationReceiverById_returns_correct_result()
        {
            DeleteByIdTest(GetNotificationReceiver);
        }

        [TestMethod]
        public void BaseDeleteNotificationById_returns_correct_result()
        {
            DeleteByIdTest(GetNotification);
        }

        [TestMethod]
        public void BaseDeleteEventById_returns_correct_result()
        {
            DeleteByIdTest(GetEvent);
        }

        [TestMethod]
        public void BaseDeletePersonStatusById_returns_correct_result()
        {
            DeleteByIdTest(GetPersonStatus);
        }

        [TestMethod]
        public void BaseDeletePersonStatusTypeById_returns_correct_result()
        {
            DeleteByIdTest(GetPersonStatusType);
        }

        [TestMethod]
        public void BaseDeleteStayStatementById_returns_correct_result()
        {
            DeleteByIdTest(GetStayStatement);
        }

        [TestMethod]
        public void BaseDeleteSaiseExporterById_returns_correct_result()
        {
            DeleteByIdTest(GetSaiseExporter);
        }

        [TestMethod]
        public void BaseDeleteAdditionalUserInfoById_returns_correct_result()
        {
            DeleteByIdTest(GetAdditionalUserInfo);
        }

        [TestMethod]
        public void BaseDeleteSaiseExporterStageById_returns_correct_result()
        {
            DeleteByIdTest(GetSaiseExporterStage);
        }

        #endregion Delete

        #region UnDelete

        [TestMethod]
        public void BaseUnDeleteAddressById_has_correct_logic()
        {
            UnDeleteByIdTest(GetAddress);
        }

        [TestMethod]
        public void BaseUnDeleteStreetById_has_correct_logic()
        {
            UnDeleteByIdTest(GetStreet);
        }

        [TestMethod]
        public void BaseUnDeleteRegionById_returns_correct_result()
        {
            UnDeleteByIdTest(GetRegion);
        }

        [TestMethod]
        public void BaseUnDeleteRegionTypeById_returns_correct_result()
        {
            UnDeleteByIdTest(GetRegionType);
        }

        [TestMethod]
        public void BaseUnDeleteStreetTypeById_returns_correct_result()
        {
            UnDeleteByIdTest(GetStreetType);
        }

        [TestMethod]
        public void BaseUnDeletePollingStationById_returns_correct_result()
        {
            UnDeleteByIdTest(GetPollingStation);
        }

        [TestMethod]
        public void BaseUnDeletePersonAddressById_returns_correct_result()
        {
            UnDeleteByIdTest(GetPersonAddress);
        }

        [TestMethod]
        public void BaseUnDeletePersonById_returns_correct_result()
        {
            UnDeleteByIdTest(GetPerson);
        }

        [TestMethod]
        public void BaseUnDeletePersonAddressTypeById_returns_correct_result()
        {
            UnDeleteByIdTest(GetPersonAddressType);
        }

        [TestMethod]
        public void BaseUnDeletePublicAdministrationById_returns_correct_result()
        {
            UnDeleteByIdTest(GetPublicAdministration);
        }

        [TestMethod]
        public void BaseUnDeleteManagerTypeById_returns_correct_result()
        {
            UnDeleteByIdTest(GetManagerType);
        }

        [TestMethod]
        public void BaseUnDeleteGenderById_returns_correct_result()
        {
            UnDeleteByIdTest(GetGender);
        }

        [TestMethod]
        public void BaseUnDeleteDocumentTypeById_returns_correct_result()
        {
            UnDeleteByIdTest(GetDocumentType);
        }

        [TestMethod]
        public void BaseUnDeleteElectionTypeById_returns_correct_result()
        {
            UnDeleteByIdTest(GetElectionType);
        }

        [TestMethod]
        public void BaseUnDeleteElectionById_returns_correct_result()
        {
            UnDeleteByIdTest(GetElection);
        }

        [TestMethod]
        public void BaseUnDeleteNotificationReceiverById_returns_correct_result()
        {
            UnDeleteByIdTest(GetNotificationReceiver);
        }

        [TestMethod]
        public void BaseUnDeleteNotificationById_returns_correct_result()
        {
            UnDeleteByIdTest(GetNotification);
        }

        [TestMethod]
        public void BaseUnDeleteEventById_returns_correct_result()
        {
            UnDeleteByIdTest(GetEvent);
        }

        [TestMethod]
        public void BaseUnDeletePersonStatusById_returns_correct_result()
        {
            UnDeleteByIdTest(GetPersonStatus);
        }

        [TestMethod]
        public void BaseUnDeletePersonStatusTypeById_returns_correct_result()
        {
            UnDeleteByIdTest(GetPersonStatusType);
        }

        [TestMethod]
        public void BaseUnDeleteStayStatementById_returns_correct_result()
        {
            UnDeleteByIdTest(GetStayStatement);
        }

        [TestMethod]
        public void BaseUnDeleteSaiseExporterById_returns_correct_result()
        {
            UnDeleteByIdTest(GetSaiseExporter);
        }

        [TestMethod]
        public void BaseUnDeleteAdditionalUserInfoById_returns_correct_result()
        {
            UnDeleteByIdTest(GetAdditionalUserInfo);
        }

        [TestMethod]
        public void BaseUnDeleteSaiseExporterStageById_returns_correct_result()
        {
            UnDeleteByIdTest(GetSaiseExporterStage);
        }

        #endregion UnDelete

        #region GetByName

        [TestMethod]
        public void BaseGetRegionTypeByName_returns_correct_result()
        {
            GetByNameTest(GetRegionType);
        }

        [TestMethod]
        public void BaseGetStreetTypeByName_returns_correct_result()
        {
            GetByNameTest(GetStreetType);
        }

        [TestMethod]
        public void BaseGetPersonAddressTypeByName_returns_correct_result()
        {
            GetByNameTest(GetPersonAddressType);
        }

        [TestMethod]
        public void BaseGetManagerTypeByName_returns_correct_result()
        {
            GetByNameTest(GetManagerType);
        }

        [TestMethod]
        public void BaseGetGenderByName_returns_correct_result()
        {
            GetByNameTest(GetGender);
        }

        [TestMethod]
        public void BaseGetDocumentTypeByName_returns_correct_result()
        {
            GetByNameTest(GetDocumentType);
        }

        [TestMethod]
        public void BaseGetElectionTypeByName_returns_correct_result()
        {
            GetByNameTest(GetElectionType);
        }

        [TestMethod]
        public void BaseGetPersonStatusTypeByName_returns_correct_result()
        {
            GetByNameTest(GetPersonStatusType);
        }

        #endregion GetByName

        #region VerificationIsDeletedLookupTrueCase

        [TestMethod]
        public void BaseVerificationIsRegionTypeDeletedLookupTrueCase_returns_correct_result()
        {
            VerificationIsDeletedLookupTrueCaseTest(GetRegionType);
        }

        [TestMethod]
        public void BaseVerificationIsStreetTypeDeletedLookupTrueCase_returns_correct_result()
        {
            VerificationIsDeletedLookupTrueCaseTest(GetStreetType);
        }

        [TestMethod]
        public void BaseVerificationIsPersonAddressTypeDeletedLookupTrueCase_returns_correct_result()
        {
            VerificationIsDeletedLookupTrueCaseTest(GetPersonAddressType);
        }

        [TestMethod]
        public void BaseVerificationIsManagerTypeDeletedLookupTrueCase_returns_correct_result()
        {
            VerificationIsDeletedLookupTrueCaseTest(GetManagerType);
        }

        [TestMethod]
        public void BaseVerificationIsGenderDeletedLookupTrueCase_returns_correct_result()
        {
            VerificationIsDeletedLookupTrueCaseTest(GetGender);
        }

        [TestMethod]
        public void BaseVerificationIsDocumentTypeDeletedLookupTrueCase_returns_correct_result()
        {
            VerificationIsDeletedLookupTrueCaseTest(GetDocumentType);
        }

        [TestMethod]
        public void BaseVerificationIsElectionTypeDeletedLookupTrueCase_returns_correct_result()
        {
            VerificationIsDeletedLookupTrueCaseTest(GetElectionType);
        }

        [TestMethod]
        public void BaseVerificationIsPersonStatusTypeDeletedLookupTrueCase_returns_correct_result()
        {
            VerificationIsDeletedLookupTrueCaseTest(GetPersonStatusType);
        }

        #endregion VerificationIsDeletedLookupTrueCase

        #region VerificationIsDeletedLookupFalseCase

        [TestMethod]
        public void BaseVerificationIsRegionTypeDeletedLookupFalseCase_returns_correct_result()
        {
            VerificationIsDeletedLookupFalseCaseTest(GetRegionType);
        }

        [TestMethod]
        public void BaseVerificationIsStreetTypeDeletedLookupFalseCase_returns_correct_result()
        {
            VerificationIsDeletedLookupFalseCaseTest(GetStreetType);
        }

        [TestMethod]
        public void BaseVerificationIsPersonAddressTypeDeletedLookupFalseCase_returns_correct_result()
        {
            VerificationIsDeletedLookupFalseCaseTest(GetPersonAddressType);
        }

        [TestMethod]
        public void BaseVerificationIsManagerTypeDeletedLookupFalseCase_returns_correct_result()
        {
            VerificationIsDeletedLookupFalseCaseTest(GetManagerType);
        }

        [TestMethod]
        public void BaseVerificationIsGenderDeletedLookupFalseCase_returns_correct_result()
        {
            VerificationIsDeletedLookupFalseCaseTest(GetGender);
        }

        [TestMethod]
        public void BaseVerificationIsDocumentTypeDeletedLookupFalseCase_returns_correct_result()
        {
            VerificationIsDeletedLookupFalseCaseTest(GetDocumentType);
        }

        [TestMethod]
        public void BaseVerificationIsElectionTypeDeletedLookupFalseCase_returns_correct_result()
        {
            VerificationIsDeletedLookupFalseCaseTest(GetElectionType);
        }

        [TestMethod]
        public void BaseVerificationIsPersonStatusTypeDeletedLookupFalseCase_returns_correct_result()
        {
            VerificationIsDeletedLookupFalseCaseTest(GetPersonStatusType);
        }

        #endregion VerificationIsDeletedLookupFalseCase

        #region VerificationIsDeletedSrvTrueCase

        [TestMethod]
        public void BaseVerificationIsAddressDeletedSrvTrueCase_returns_correct_result()
        {
            VerificationIsDeletedSrvTrueCaseTest(GetAddress);
        }

        [TestMethod]
        public void BaseVerificationIsPollingStationDeletedSrvTrueCase_returns_correct_result()
        {
            VerificationIsDeletedSrvTrueCaseTest(GetPollingStation);
        }

        [TestMethod]
        public void BaseVerificationIsPersonAddressDeletedSrvTrueCase_returns_correct_result()
        {
            VerificationIsDeletedSrvTrueCaseTest(GetPersonAddress);
        }

        [TestMethod]
        public void BaseVerificationIsPersonDeletedSrvTrueCase_returns_correct_result()
        {
            VerificationIsDeletedSrvTrueCaseTest(GetPerson);
        }

        [TestMethod]
        public void BaseVerificationIsPublicAdministrationDeletedSrvTrueCase_returns_correct_result()
        {
            VerificationIsDeletedSrvTrueCaseTest(GetPublicAdministration);
        }

        [TestMethod]
        public void BaseVerificationIsElectionDeletedSrvTrueCase_returns_correct_result()
        {
            VerificationIsDeletedSrvTrueCaseTest(GetElection);
        }

        [TestMethod]
        public void BaseVerificationIsNotificationReceiverDeletedSrvTrueCase_returns_correct_result()
        {
            VerificationIsDeletedSrvTrueCaseTest(GetNotificationReceiver);
        }

        [TestMethod]
        public void BaseVerificationIsNotificationDeletedSrvTrueCase_returns_correct_result()
        {
            VerificationIsDeletedSrvTrueCaseTest(GetNotification);
        }

        [TestMethod]
        public void BaseVerificationIsEventDeletedSrvTrueCase_returns_correct_result()
        {
            VerificationIsDeletedSrvTrueCaseTest(GetEvent);
        }

        [TestMethod]
        public void BaseVerificationIsPersonStatusDeletedSrvTrueCase_returns_correct_result()
        {
            VerificationIsDeletedSrvTrueCaseTest(GetPersonStatus);
        }

        [TestMethod]
        public void BaseVerificationIsSaiseExporterDeletedSrvTrueCase_returns_correct_result()
        {
            VerificationIsDeletedSrvTrueCaseTest(GetSaiseExporter);
        }

        [TestMethod]
        public void BaseVerificationIsAdditionalUserInfoDeletedSrvTrueCase_returns_correct_result()
        {
            VerificationIsDeletedSrvTrueCaseTest(GetAdditionalUserInfo);
        }

        #endregion VerificationIsDeletedSrvTrueCase

        #region VerificationIsDeletedSrvFalseCase

        [TestMethod]
        public void BaseVerificationIsAddressDeletedSrvFalseCase_returns_correct_result()
        {
            VerificationIsDeletedSrvFalseCaseTest(GetAddress);
        }

        [TestMethod]
        public void BaseVerificationIsPollingStationDeletedSrvFalseCase_returns_correct_result()
        {
            VerificationIsDeletedSrvFalseCaseTest(GetPollingStation);
        }

        [TestMethod]
        public void BaseVerificationIsPersonAddressDeletedSrvFalseCase_returns_correct_result()
        {
            VerificationIsDeletedSrvFalseCaseTest(GetPersonAddress);
        }

        [TestMethod]
        public void BaseVerificationIsPersonDeletedSrvFalseCase_returns_correct_result()
        {
            VerificationIsDeletedSrvFalseCaseTest(GetPerson);
        }

        [TestMethod]
        public void BaseVerificationIsPublicAdministrationDeletedSrvFalseCase_returns_correct_result()
        {
            VerificationIsDeletedSrvFalseCaseTest(GetPublicAdministration);
        }

        [TestMethod]
        public void BaseVerificationIsElectionDeletedSrvFalseCase_returns_correct_result()
        {
            VerificationIsDeletedSrvFalseCaseTest(GetElection);
        }

        [TestMethod]
        public void BaseVerificationIsNotificationReceiverDeletedSrvFalseCase_returns_correct_result()
        {
            VerificationIsDeletedSrvFalseCaseTest(GetNotificationReceiver);
        }

        [TestMethod]
        public void BaseVerificationIsNotificationDeletedSrvFalseCase_returns_correct_result()
        {
            VerificationIsDeletedSrvFalseCaseTest(GetNotification);
        }

        [TestMethod]
        public void BaseVerificationIsEventDeletedSrvFalseCase_returns_correct_result()
        {
            VerificationIsDeletedSrvFalseCaseTest(GetEvent);
        }

        [TestMethod]
        public void BaseVerificationIsPersonStatusDeletedSrvFalseCase_returns_correct_result()
        {
            VerificationIsDeletedSrvFalseCaseTest(GetPersonStatus);
        }

        [TestMethod]
        public void BaseVerificationIsSaiseExporterDeletedSrvFalseCase_returns_correct_result()
        {
            VerificationIsDeletedSrvFalseCaseTest(GetSaiseExporter);
        }

        [TestMethod]
        public void BaseVerificationIsAdditionalUserInfoDeletedSrvFalseCase_returns_correct_result()
        {
            VerificationIsDeletedSrvFalseCaseTest(GetAdditionalUserInfo);
        }

        #endregion VerificationIsDeletedSrvFalseCase

        [TestMethod]
        public void GetLocalitiesByNullNodeIdAndAdminRole_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            MockQueryOver<Region>().Setup(x => x.List()).Returns(GetAllObjectsFromDbTable<Region>(x => x.Parent.Id == null));

            var expectedRegions = GetAllObjectsFromDbTable<Region>(x => x.Parent == null);

            // Act

            var regions = SafeExecFunc(Bll.GetLocalities, (int?)null);

            // Assert

            Assert.IsNotNull(regions);
            AssertListsAreEqual(expectedRegions, regions.ToList());
        }

        [TestMethod]
        public void GetLocalitiesByNullNodeIdAndRegistratorRole_returns_correct_result()
        {
            // Arrange

            SetRegistratorRole();

            MockQueryOver<Region>().Setup(x => x.List()).Returns(GetAllObjectsFromDbTable<Region>());

            var expectedRegions = GetAllObjectsFromDbTableWithUdfPropertyIn<Region>(x => x.Id, UdfRegionsCriterion());

            var parentIds = expectedRegions.Select(x => x.Parent.Id).ToList();
            var matches = expectedRegions.Select(x => x.Id).Intersect(parentIds);
            var diffs = parentIds.Except(matches);
            expectedRegions = expectedRegions.Where(x => diffs.Contains(x.Parent.Id)).ToList();

            // Act

            var regions = SafeExecFunc(Bll.GetLocalities, (int?)null);

            // Assert

            Assert.IsNotNull(regions);
            AssertListsAreEqual(expectedRegions, regions.ToList());
        }

        [TestMethod]
        public void GetLocalitiesByNotNullNodeIdAndAdminRole_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.Parent != null, GetRegionWithParent);
            int? nodeId = (int)region.Parent.Id;

            MockQueryOver<Region>().Setup(x => x.List()).Returns(GetAllObjectsFromDbTable<Region>(x => (x.Parent != null) && (x.Parent.Id == nodeId)));

            var expectedRegions = GetAllObjectsFromDbTable<Region>(x => (x.Parent != null) && (x.Parent.Id == nodeId));

            // Act

            var regions = SafeExecFunc(Bll.GetLocalities, nodeId);

            // Assert

            Assert.IsNotNull(regions);
            AssertListsAreEqual(expectedRegions, regions.ToList());
        }

        [TestMethod]
        public void GetLocalitiesByNotNullNodeIdAndRegistratorRole_returns_correct_result()
        {
            // Arrange

            SetRegistratorRole();

            var region = GetFirstObjectFromDbTable(x => x.Parent != null, GetRegionWithParent);
            int? nodeId = (int)region.Parent.Id;

            MockQueryOver<Region>().Setup(x => x.List()).Returns(GetAllObjectsFromDbTable<Region>(x => (x.Parent != null) && (x.Parent.Id == nodeId)));

            var expectedRegions = GetAllObjectsFromDbTableWithUdfPropertyIn<Region>(x => (x.Parent != null) && (x.Parent.Id == nodeId), x => x.Id, UdfRegionsCriterion());

            // Act

            var regions = SafeExecFunc(Bll.GetLocalities, nodeId);

            // Assert

            Assert.IsNotNull(regions);
            AssertListsAreEqual(expectedRegions, regions.ToList());
        }

        [TestMethod]
        public void GetCurrentUserRegions_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var expectedRegionIds =
                GetFirstObjectFromDbTable(
                    x => string.Equals(x.Id, SecurityHelper.GetLoggedUserId()), GetSrvIdentityUser).Regions.Select(x => x.Id).ToList();

            // Act

            var regionIds = SafeExecFunc<long[]>("GetCurrentUserRegions");

            // Assert

            Assert.IsNotNull(regionIds);
            AssertObjectListsAreEqual(expectedRegionIds, regionIds.ToList());
        }

        [TestMethod]
        public void GetRegionTypesByFilter_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.RegionType.Rank == 0, GetRegion);
            var regionId = region.Id;
            var expectedRegionTypes = GetAllObjectsFromDbTable<RegionType>(x => x.Rank > region.RegionType.Rank);

            // Act

            var regionTypes = SafeExecFunc(Bll.GetRegionTypesByFilter, regionId);

            // Assert

            Assert.IsNotNull(regionTypes);
            AssertListsAreEqual(expectedRegionTypes, regionTypes.ToList());
        }

        [TestMethod]
        public void GetPublicAdministration_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var expectedPublicAdministration = GetFirstObjectFromDbTable((x => x.Region != null), GetPublicAdministration);
            var regionId = expectedPublicAdministration.Region.Id;
            
            // Act

            var publicAdministration = SafeExecFunc(Bll.GetPublicAdministration, regionId);

            // Assert

            Assert.IsNotNull(publicAdministration);
            Assert.AreSame(expectedPublicAdministration, publicAdministration);
        }

        [TestMethod]
        public void GetStreet_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var expectedStreet = GetFirstObjectFromDbTable(x => (x.Region != null) && (x.StreetType != null), GetStreet);

            var streetName = expectedStreet.Name;
            var regionId = expectedStreet.Region.Id;
            var streetTypeId = expectedStreet.StreetType.Id;

            // Act

            var street = SafeExecFunc(Bll.GetStreet, streetName, regionId, streetTypeId);

            // Assert

            Assert.IsNotNull(street);
            Assert.AreSame(expectedStreet, street);
        }

        //[TestMethod]
        public void GetRegionWithNullParentId_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => (x.RegionType != null) && (x.Parent == null), GetRegion);

            string name = region.Name;
            long regionTypeId = region.RegionType.Id;
            
            var expectedRegions = GetAllObjectsFromDbTable<Region>(x => (x.Name == name) && (x.RegionType != null) && (x.RegionType.Id == regionTypeId) && (x.Parent == null));

            // Act
            
            var regions = SafeExecFunc(Bll.GetRegion, name, regionTypeId, (long?)null);
            
            // Assert

            Assert.IsNotNull(regions);
            AssertListsAreEqual(expectedRegions, regions.ToList());
        }

        [TestMethod]
        public void GetRegionWithNotNullParentId_returns_correct_result()
        {
            // Arrange
            
            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => (x.RegionType != null) && (x.Parent != null), GetRegionWithParent);

            var name = region.Name;
            var regionTypeId = region.RegionType.Id;
            long? parentId = region.Parent.Id;
            
            var expectedRegions = GetAllObjectsFromDbTable<Region>(
                x => (x.Name == name) && (x.RegionType != null) && (x.RegionType.Id == regionTypeId) && (x.Parent != null) && (x.Parent.Id == parentId.Value));

            // Act

            var regions = SafeExecFunc(Bll.GetRegion, name, regionTypeId, parentId);
            
            // Assert

            Assert.IsNotNull(regions);
            AssertListsAreEqual(expectedRegions, regions.ToList());
        }

        [TestMethod]
        public void GetAddressWithNullHouseNumber_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var address = GetFirstObjectFromDbTable(x => (x.Street != null) && (x.HouseNumber == null), GetAddressWithNullHouseNumber);

            var streetId = address.Street.Id;
            var suffix = address.Suffix;

            var expectedAddresses = GetAllObjectsFromDbTable<Address>(x => (x.Street != null) && (x.Street.Id == streetId) && (x.HouseNumber == null) && (x.Suffix == suffix));

            // Act

            var addresses = SafeExecFunc(Bll.GetAddress, streetId, (long?)null, suffix);
            
            // Assert

            Assert.IsNotNull(addresses);
            AssertListsAreEqual(expectedAddresses, addresses.ToList());
        }

        [TestMethod]
        public void GetAddressWithNotNullHouseNumber_returns_correct_result()
        {
            // Arrange
            
            SetAdministratorRole();

            var address = GetFirstObjectFromDbTable(x => (x.Street != null) && (x.HouseNumber != null), GetAddress);

            var streetId = address.Street.Id;
            long? houseNumber = address.HouseNumber;
            var suffix = address.Suffix;

            var expectedAddresses = GetAllObjectsFromDbTable<Address>(x => (x.Street != null) && (x.Street.Id == streetId) && (x.HouseNumber == houseNumber) && (x.Suffix == suffix));

            // Act
            
            var addresses = SafeExecFunc(Bll.GetAddress, streetId, houseNumber, suffix);

            // Assert
            Assert.IsNotNull(addresses);
            AssertListsAreEqual(expectedAddresses, addresses.ToList());
        }

        [TestMethod]
        public void IsRegionAccessibleToCurrentAdminUser_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            // Act

            var result = SafeExecFunc(Bll.IsRegionAccessibleToCurrentUser, 0L);
            
            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsAccessibleRegionAccessibleToCurrentRegistratorUser_returns_correct_result()
        {
            // Arrange

            InitializeRepositoryAndBll<SrvRepository>();

            SetRegistratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);
            var regionId = region.Id;

            AddRegionToCurrentUser(region);
            
            // Act

            var result = SafeExecFunc(Bll.IsRegionAccessibleToCurrentUser, regionId);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsNotAccessibleRegionAccessibleToCurrentRegistratorUser_returns_correct_result()
        {
            // Arrange

            InitializeRepositoryAndBll<SrvRepository>();

            SetRegistratorRole();

            var region = GetFirstObjectFromDbTable(GetRegion);
            var regionId = region.Id;

            var queryOver = MockQueryOver<Region>();
            queryOver.Setup(x => x.Future<int>()).Returns(new List<int>() { 1, 2 });

            // Act

            var result = SafeExecFunc(Bll.IsRegionAccessibleToCurrentUser, regionId);
            
            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void VerificationIsRegionDeletedTrueCase_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var region = GetFirstObjectFromDbTable(x => x.HasStreets && (x.Deleted != null), GetDeletedRegionWithStreets);
            var regionId = region.Id;

            // Act & Assert

            VerificationIsRegionDeletedTrueCaseTest(regionId);
        }

        [TestMethod]
        public void VerificationIsRegionDeletedFalseCase_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var regionId = GetFirstObjectFromDbTable(x => x.HasStreets && (x.Deleted == null), GetRegion).Id;
            
            // Act & Assert

            VerificationIsRegionDeletedFalseCaseTest(regionId);
        }

        [TestMethod]
        public void CreateNotification_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            const EventTypes eventType = EventTypes.Update;
            var person = GetFirstObjectFromDbTable(GetPerson);
            INotificationEntity entity = person;
            var entityId = person.Id;
            const string message = "Person updated";
            var users = new List<SRVIdentityUser>() { GetCurrentUser() };

            var notificationType = string.Format("{0} {1}", 
                eventType.GetEnumDescriptions(),
                MUI.ResourceManager.GetString(entity.GetNotificationType()));

            // Act
            
            SafeExec("CreateNotification",
                    new object[] {eventType, person, entityId, message, users},
                    new Type[] {typeof (EventTypes), entity.GetType(), typeof (long), typeof (string), users.GetType()});

            // Assert

            var notification = GetLastCreatedObject<Notification>();

            Assert.IsNotNull(notification);
            Assert.IsNotNull(notification.Event);
            Assert.AreEqual(eventType, notification.Event.EventType);
            Assert.AreEqual(notificationType, notification.Event.EntityType);
            Assert.AreEqual(entityId, notification.Event.EntityId);
            Assert.AreEqual(message, notification.Message);
            
            AssertListsAreEqual(users, notification.Receivers.ToList(), x => x.Id, x => x.User.Id);
        }

        [TestMethod]
        public void CreateNotificationWithoutGivenUsers_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            const EventTypes eventType = EventTypes.Update;
            var person = GetFirstObjectFromDbTable(GetPerson);
            INotificationEntity entity = person;
            var entityId = person.Id;
            const string message = "Person updated";

            var adminRole = GetFirstObjectFromDbTable<IdentityRole>(x => x.Name == RolesStrings.Administrator);
            var users = GetAllObjectsFromDbTable<SRVIdentityUser>(x => x.Roles.Contains(adminRole));
            
            var notificationType = string.Format("{0} {1}",
                eventType.GetEnumDescriptions(),
                MUI.ResourceManager.GetString(entity.GetNotificationType()));

            // Act

            SafeExec("CreateNotification",
                  new object[] { eventType, person, entityId, message },
                  new[] { typeof(EventTypes), entity.GetType(), typeof(long), typeof(string) });

            // Assert

            var notification = GetLastCreatedObject<Notification>();

            Assert.IsNotNull(notification);
            Assert.IsNotNull(notification.Event);
            Assert.AreEqual(eventType, notification.Event.EventType);
            Assert.AreEqual(notificationType, notification.Event.EntityType);
            Assert.AreEqual(entityId, notification.Event.EntityId);
            Assert.AreEqual(message, notification.Message);

            AssertListsAreEqual(users, notification.Receivers.ToList(), x => x.Id, x => x.User.Id);
        }

        #endregion Tests

        #region Common Tests

        private void GetByIdTest<T>(Func<T> newObjBuilder) where T : Entity
        {
            // Arrange

            SetAdministratorRole();

            var expectedEntity = GetFirstObjectFromDbTable(newObjBuilder);
            var id = expectedEntity.Id;

            // Act

            var entity = SafeExecFunc(Bll.Get<T>, id);

            // Assert

            Assert.IsNotNull(entity);
            Assert.AreSame(expectedEntity, entity);
        }

        private void GetAllTest<T>() where T : Entity
        {
            // Arrange

            SetAdministratorRole();

            var expectedEntities = GetAllObjectsFromDbTable<T>();

            // Act

            var entities = SafeExecFunc(Bll.GetAll<T>);

            // Assert

            Assert.IsNotNull(entities);
            AssertListsAreEqual(expectedEntities, entities.ToList());
        }

        private void GetByPageRequestTest<T>() where T : Entity
        {
            // Arrange

            SetAdministratorRole();

            var expectedList = GetAllIdsFromDbTable<T>();

            // Act and Assert

            ActAndAssertAllPages(Bll.Get<T>, expectedList);
        }

        private void DeleteByIdTest<T>(Func<T> newObjBuilder) where T : SoftEntity<IdentityUser>
        {
            // Arrange

            SetAdministratorRole();

            var expectedEntity = GetFirstObjectFromDbTable(x => x.Deleted == null, newObjBuilder);
            var id = expectedEntity.Id;

            // Act

            SafeExec(Bll.Delete<T>, id);

            // Assert

            var entity = GetFirstObjectFromDbTable<T>(x => x.Id == id);

            Assert.IsNotNull(entity);
            Assert.IsNotNull(entity.Deleted);
        }

        private void UnDeleteByIdTest<T>(Func<T> newObjBuilder) where T : SoftEntity<IdentityUser>
        {
            // Arrange

            SetAdministratorRole();

            var expectedEntity = GetFirstObjectFromDbTable(newObjBuilder);
            var id = expectedEntity.Id;

            if (expectedEntity.Deleted == null)
            {
                expectedEntity.Deleted = DateTime.Now;
                expectedEntity.DeletedBy = GetCurrentUser();
                Repository.SaveOrUpdate(expectedEntity);
            }

            // Act

            SafeExec(Bll.UnDelete<T>, id);

            // Assert

            var entity = GetFirstObjectFromDbTable<T>(x => x.Id == id);

            Assert.IsNotNull(entity);
            Assert.IsNull(entity.Deleted);
        }

        private void GetByNameTest<T>(Func<T> newObjBuilder) where T : Lookup, new()
        {
            // Arrange

            SetAdministratorRole();

            var expectedEntity = GetFirstObjectFromDbTable(newObjBuilder);
            var name = expectedEntity.Name;

            // Act

            var entity = SafeExecFunc(Bll.GetByName<T>, name);

            // Assert

            Assert.IsNotNull(entity);
            Assert.AreSame(expectedEntity, entity);
        }

        private void VerificationIsDeletedLookupTrueCaseTest<T>(Func<T> newObjBuilder) where T : Lookup
        {
            // Arrange

            SetAdministratorRole();

            var entity = GetFirstObjectFromDbTable(x => x.Deleted != null, () =>
            {
                var obj = newObjBuilder();
                obj.Deleted = DateTime.Now;
                return obj;
            });
            var id = entity.Id;

            // Act & Assert

            SafeExec(Bll.VerificationIsDeletedLookup<T>, id, true, false, "Error_EntityIsDeleted", MUI.Error_EntityIsDeleted);
        }

        private void VerificationIsDeletedLookupFalseCaseTest<T>(Func<T> newObjBuilder) where T : Lookup
        {
            // Arrange

            SetAdministratorRole();

            var entity = GetFirstObjectFromDbTable(x => x.Deleted == null, () =>
            {
                var obj = newObjBuilder();
                obj.Deleted = null;
                return obj;
            });
            var id = entity.Id;

            // Act & Assert

            SafeExec(Bll.VerificationIsDeletedLookup<T>, id);
        }

        private void VerificationIsDeletedSrvTrueCaseTest<T>(Func<T> newObjBuilder) where T : SRVBaseEntity
        {
            // Arrange

            SetAdministratorRole();

            var entity = GetFirstObjectFromDbTable(x => x.Deleted != null, () =>
            {
                var obj = newObjBuilder();
                obj.Deleted = DateTime.Now;
                return obj;
            });
            var id = entity.Id;

            // Act & Assert

            SafeExec(Bll.VerificationIsDeletedSrv<T>, id, true, false, "Error_EntityIsDeleted", MUI.Error_EntityIsDeleted);
        }

        private void VerificationIsDeletedSrvFalseCaseTest<T>(Func<T> newObjBuilder) where T : SRVBaseEntity
        {
            // Arrange

            SetAdministratorRole();

            var entity = GetFirstObjectFromDbTable(x => x.Deleted == null, () =>
            {
                var obj = newObjBuilder();
                obj.Deleted = null;
                return obj;
            });
            var id = entity.Id;

            // Act & Assert

            SafeExec(Bll.VerificationIsDeletedSrv<T>, id);
        }

        #endregion Common Tests

    }
}
