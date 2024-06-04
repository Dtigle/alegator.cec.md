using System;
using System.Collections.Generic;
using CEC.SRV.BLL.Impl;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.BLL
{
    public interface IImportStatisticsBll
    {
        long GetTotalBydate(DateTime dateTime);
        void UpdateStatisticData(ImportStatistic statistic, StatisticChanges status);
        void Save(Dictionary<long, ImportStatistic> statistics);
    }
}