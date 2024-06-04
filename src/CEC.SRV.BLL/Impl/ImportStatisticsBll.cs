using System;
using System.Collections.Generic;
using Amdaris.Domain;
using CEC.SRV.Domain.Importer;

namespace CEC.SRV.BLL.Impl
{
    public class ImportStatisticsBll : IImportStatisticsBll
    {
        private readonly IRepository _repository;

        public ImportStatisticsBll(IRepository repository)
        {
            _repository = repository;
            
        }

        public long GetTotalBydate(DateTime dateTime)
        {
            return 0;
        }

        public void UpdateStatisticData(ImportStatistic statistic, StatisticChanges status)
        {
            statistic.Total++;
            statistic.Error += (status & StatisticChanges.Error) != 0 ? 1 : 0;
            statistic.Conflicted += (status & StatisticChanges.Conflict) != 0 ? 1 : 0;
            statistic.Updated += IsUpdated(status) ? 1 : 0;
            statistic.ChangedStatus += IsChangedStatus(status) ? 1 : 0;
            statistic.ResidenceChnaged += IsChangedResidence(status) ? 1 : 0;
            statistic.New += IsNew(status) ? 1 : 0;
        }
        
        public void Save(Dictionary<long, ImportStatistic> statistics)
        {
            foreach (var importStatistic in statistics)
            {
                importStatistic.Value.Date = DateTime.Now.Date;
                importStatistic.Value.Region = importStatistic.Key;
                _repository.SaveOrUpdate(importStatistic.Value);
            }
        }
        
        private static bool IsNew(StatisticChanges status)
        {
            return (status & StatisticChanges.New) == StatisticChanges.New && (status & StatisticChanges.Conflict) == 0;
        }
        
        private static bool IsChangedResidence(StatisticChanges status)
        {
            return (status & StatisticChanges.AddressUpdate) == StatisticChanges.AddressUpdate && (status & StatisticChanges.New) != StatisticChanges.New;
        }

        private static bool IsChangedStatus(StatisticChanges status)
        {
            return (status & StatisticChanges.StatusUpdate) == StatisticChanges.StatusUpdate && (status & StatisticChanges.New) != StatisticChanges.New;
        }

        private static bool IsUpdated(StatisticChanges status)
        {
            return (status & StatisticChanges.Update) == StatisticChanges.Update && (status & StatisticChanges.New) != StatisticChanges.New;
        }

        
    }
}