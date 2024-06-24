using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Amdaris;
using Amdaris.NHibernateProvider;
using CEC.QuartzServer.Core;
using CEC.QuartzServer.Jobs.Common;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Lookup;
using Quartz;
using RSP.CEC.WebClient;
using RSP.CEC.WebClient.RspCecService;
using RSP.CEC.WebClient.RspClassifierService;
using ResultData = RSP.CEC.WebClient.RspCecService.ResultData;

namespace CEC.QuartzServer.Jobs.Import
{
    [DisallowConcurrentExecution]
    internal class RspModificationImporterJob : SrvJob, IInterruptableJob
    {
        private readonly ISRVRepository _repository;
        private readonly ILogger _logger;
        private readonly IConfigurationSettingManager _configurationSettingManager;
        private bool _interruptPending;
        private const string rspStreetsClassifierName = "CF 37603221.0282.01";
        private const string rspSignature = "WSCEC";
        public const string unknowValue = "<";

        public RspModificationImporterJob(ISRVRepository repository, ILogger logger, IConfigurationSettingManager configurationSettingManager)
        {
            _repository = repository;
            _logger = logger;
            _configurationSettingManager = configurationSettingManager;
        }

        protected internal override void ExecuteInternal(IJobExecutionContext context)
        {
            var rspUser = _configurationSettingManager.Get("RspUser").Value;
            var rspPass = _configurationSettingManager.Get("RspPass").Value;
            var updateClassifiersEnabled = _configurationSettingManager.Get("UpdateClassifiersEnabled").GetValue<bool>();

            var timeStatistics = new List<long>();

            _logger.Warning("Start process at " + DateTime.Now);

            using (var client = new CecClient())
            {
                client.ClientCredentials.UserName.UserName = rspUser;
                client.ClientCredentials.UserName.Password = rspPass;

                IdnpRequestData modifiedIdnpResponse = null;

                var globalStopWatch = new Stopwatch();
                globalStopWatch.Start();

                do
                {
                    try
                    {
                        if (updateClassifiersEnabled)
                        {
                            UpdateRspStreetsClassifiers(rspUser, rspPass);
                        }

                        modifiedIdnpResponse = client.getChangedPhysicalPersonList();

                        ValidateResponse(modifiedIdnpResponse.result);
                        _logger.Trace(string.Format("Received {0} IDNPs with changes.", modifiedIdnpResponse.idnp.Length));

                        var callStopwatch = new Stopwatch();

                        foreach (var idnp in modifiedIdnpResponse.idnp)
                        {
                            if (_interruptPending)
                            {
                                _logger.Trace("Job interrupted by user.");
                                return;
                            }

                            try
                            {
                                callStopwatch.Reset();
                                callStopwatch.Start();

                                var physicalPersonData = client.getPhysicalPersonData(idnp);

                                PhysicalPersonData person;
                                using (var uow = new NhUnitOfWork())
                                {
                                    person = ProcessPersonData(physicalPersonData, idnp);
                                    uow.Complete();
                                }

                                client.confirmReceivedPerson(idnp);

                                callStopwatch.Stop();
                                timeStatistics.Add(callStopwatch.ElapsedMilliseconds);
                                _logger.Trace(string.Format("Idnp: {0}, {1} {2} - processeed in {3} milleseconds",
                                    idnp, physicalPersonData.person.firstName, physicalPersonData.person.lastName,
                                    callStopwatch.ElapsedMilliseconds));


                            }
                            catch (RspException rspEx)
                            {
                                _logger.Error(rspEx, string.Format("IDNP: {0}", idnp));
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex, string.Format("Something went wrong with idnp {0}", idnp));
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        globalStopWatch.Stop();
                        _logger.Error(ex);
                        ShowStatistics(timeStatistics);
                        return;
                    }

                } while (modifiedIdnpResponse.idnp != null && modifiedIdnpResponse.idnp.Length > 0);

                globalStopWatch.Stop();

                var globalStr = string.Format("Job has worked for {0}", globalStopWatch.Elapsed);

                _logger.Warning(globalStr);

                ShowStatistics(timeStatistics);
            }
        }

        public PhysicalPersonData ProcessPersonData(PhysicalPersonRequestData physicalPersonData, long idnp)
        {
            ValidateResponse(physicalPersonData.result);

            var person = physicalPersonData.result.resultCode == RspSerivceKnownErrors.Success
                ? physicalPersonData.person
                : new PhysicalPersonData
                {
                    idnp = idnp,
                    lastName = unknowValue,
                    firstName = unknowValue,
                    secondName = unknowValue,
                    birthDate = new DateTime(1900, 1, 1),
                    sexCode = 3, // SexCode = 3 is unknow sex by RSP SexCode classifier
                    identDocument =
                        new DocumentData()
                        {
                            issueDate = new DateTime(1900, 1, 1),
                            docTypeCode = 0 // undefined document type in RSP DocTypeCodes classifier
                        }
                };

            UpdateSrvData(person, physicalPersonData.result.resultCode != null
                                  && RspSerivceKnownErrors.ErrorsToRemove.Contains(physicalPersonData.result.resultCode.Value));
            return person;
        }

        private void UpdateRspStreetsClassifiers(string rspUser, string rspPass)
        {
            using (var uow = new NhUnitOfWork())
            {
                using (var classifierClient = new ClassifierClient())
                {
                    classifierClient.ClientCredentials.UserName.UserName = rspUser;
                    classifierClient.ClientCredentials.UserName.Password = rspPass;

                    //var clasifiersNamesResponse = classifierClient.getClassifiersNames(rspSignature);
                    //todo: some sort of revision checking should be implemented

                    var streetsClassifierData = classifierClient.getClassifier(rspSignature, rspStreetsClassifierName);
                    if (streetsClassifierData.res.resultCode != null && streetsClassifierData.res.resultCode != 0)
                    {
                        _logger.Debug(string.Format("errorCode {0}, message: {1}",
                            streetsClassifierData.res.resultCode, streetsClassifierData.res.errorText));
                        return;
                    }

                    //read rsp streets from local store
                    var rspStreetsLocalStore = _repository.Query<StreetTypeCode>().ToList();

                    foreach (var rowData in streetsClassifierData.classifier.row)
                    {
                        if (rowData == null)
                        {
                            _logger.Debug("rowData null");
                            continue;
                        }

                        if (rowData.items == null)
                        {
                            _logger.Debug("rowData.items = null");
                            continue;
                        }

                        var id_Item = rowData.items.First(x => x.fieldNames == "id");
                        long id_value;
                        if (long.TryParse(id_Item.values, out id_value))
                        {
                            var localStreetItem = rspStreetsLocalStore.FirstOrDefault(x => x.Id == id_value);
                            if (localStreetItem == null)
                            {
                                var nameItem = rowData.items.First(x => x.fieldNames == "name");
                                var nameRusItem = rowData.items.First(x => x.fieldNames == "namerus");
                                var namePrintItem = rowData.items.First(x => x.fieldNames == "nameprint");
                                var newRspStreetItem = new StreetTypeCode
                                {
                                    Name = nameItem.values,
                                    Namerus = nameRusItem.values,
                                    Docprint = namePrintItem.values,
                                    RspStreetTypeCodeId = id_value
                                };

                                _repository.SaveOrUpdate(newRspStreetItem);
                            }
                        }
                    }
                }
                uow.Complete();
            }
        }

        private void ShowStatistics(List<long> timeStatistics)
        {
            _logger.Warning(string.Format("Average call time {0}", timeStatistics.Average()));
            _logger.Warning(string.Format("Max call time {0}", timeStatistics.Max()));
            _logger.Warning(string.Format("Min call time {0}", timeStatistics.Min()));
        }

        private void UpdateSrvData(PhysicalPersonData person, bool toRemove)
        {
            var rspModificationData = new RspModificationData()
            {
                Idnp = person.idnp.ToString().PadLeft(13, '0'),
                FirstName = person.firstName,
                LastName = person.lastName,
                SecondName = person.secondName,
                Birthdate = person.birthDate.Value,
                SexCode = person.sexCode.ToString(),
                Dead = person.dead.HasValue && person.dead.Value,
                CitizenRm = person.citizenRM.HasValue && person.citizenRM.Value,
                Created = DateTimeOffset.Now

            };

            if (person.identDocument != null)
            {
                rspModificationData.DocumentTypeCode = person.identDocument.docTypeCode.GetValueOrDefault();
                rspModificationData.Seria = person.identDocument.series;
                rspModificationData.Number = person.identDocument.number;
                rspModificationData.IssuedDate = person.identDocument.issueDate;
                rspModificationData.ValidBydate = person.identDocument.expirationDate;
                rspModificationData.Validity = person.identDocument.validity.GetValueOrDefault();
            }

            if (person.registration != null)
            {
                foreach (var registrationData in person.registration)
                {
                    var registration = registrationData.ToRegistrationData();
                    rspModificationData.Registrations.Add(registration);
                }
            }

            if (toRemove)
            {
                rspModificationData.SetToRemove();
            }

            SaveRspModificationData(rspModificationData);
        }

        private void SaveRspModificationData(RspModificationData entity)
        {
            _repository.SaveOrUpdate(entity);
        }

        private void ValidateResponse(ResultData result)
        {
            if (result == null)
            {
                return;
            }

            if (result.resultCode.HasValue && !RspSerivceKnownErrors.ValidResults.Contains(result.resultCode.Value))
            {
                throw new RspException(result.resultCode, result.errorText);
            }
        }

        public void Interrupt()
        {
            _interruptPending = true;
        }
    }
}