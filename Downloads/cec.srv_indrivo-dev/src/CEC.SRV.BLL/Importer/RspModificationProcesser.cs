using System;
using System.Collections.Generic;
using System.Linq;
using Amdaris;
using CEC.SRV.BLL.Impl;
using CEC.SRV.BLL.Repositories;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Lookup;
using Common.Logging;

namespace CEC.SRV.BLL.Importer
{
    public class RspModificationProcesser : ImportProcesser<RspModificationData>
    {
        private readonly IImportBll _importBll;
        private readonly IImportStatisticsBll _statisticsBll;
        private readonly ISRVRepository _repository;
        private readonly Dictionary<long, ImportStatistic> _statistics;

        public RspModificationProcesser(ISRVRepository repository, IImportBll importBll, IImportStatisticsBll statisticsBll, ILogger logger, int batchSize)
            : base(repository, logger, batchSize)
        {
            _repository = repository;
            _importBll = importBll;
            _statisticsBll = statisticsBll;
            _statistics = new Dictionary<long, ImportStatistic>();
        }

        public Dictionary<long, ImportStatistic> GetOverallStatistic()
        {
            return _statistics;
        }

        protected override void ProcessInternal(RspModificationData rawData)
        {
            var status = StatisticChanges.None;

            var person = _importBll.Import(rawData, ref status);
                
            SavePerson(person, status);

            long regionId = (person.EligibleAddress != null)
                ? person.EligibleAddress.Address.Street.Region.Id
                : GetRegion(rawData);

            var statistic = GetStatisticByRegion(regionId);

            _statisticsBll.UpdateStatisticData(statistic, status);
            
        }

        protected override void NotifySuccess(RspModificationData rawData)
        {
            Logger.Info(string.Format("Processing succes for {0} ", rawData.ToString()));
            if (rawData.Status == RawDataStatus.Error) return;
            //if (rawData.Status == RawDataStatus.Retry)
            //    rawData.AcceptConflict(rawData.StatusConflictCode);
            rawData.SetEnd();

            _repository.SaveOrUpdate(rawData);
        }

        protected override void NotifyFailure(RspModificationData rawData)
        {
            rawData.SetError("Error on Processing RawModification data");

            var rspRegistrationData = rawData.Registrations.FirstOrDefault();
            if (rspRegistrationData != null)
                _statisticsBll.UpdateStatisticData(GetStatisticByRegion(rspRegistrationData.Administrativecode), StatisticChanges.Error);
            _repository.SaveOrUpdate(rawData);
        }

        private long GetRegion(RspModificationData rawData)
        {
            var registration = rawData.Registrations.FirstOrDefault();
            var regions = _repository.Query<Region>().Where(x => x.StatisticIdentifier == registration.Administrativecode);
            var firstOrDefault = regions.FirstOrDefault();
            return firstOrDefault != null ? firstOrDefault.Id : 0;
        }

        private void SavePerson(Person person, StatisticChanges status)
        {
            if (!status.HasFlag(StatisticChanges.ConflictAddress) && !status.HasFlag(StatisticChanges.ConflictPolling) && !status.HasFlag(StatisticChanges.ConflictStatus))
                _repository.SaveOrUpdate(person);
        }

        private ImportStatistic GetStatisticByRegion(long region)
        {
            ImportStatistic result;
            if (!_statistics.TryGetValue(region, out result))
            {
                result = new ImportStatistic();
                _statistics.Add(region, result);
            }
            return result;
        }
    }
}