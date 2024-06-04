using System;
using Amdaris.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain.Importer
{
    public class ImportStatistic : Entity
    {
        public virtual DateTime Date { get; set; }
        public virtual long New { get; set; }
        public virtual long Conflicted { get; set; }
        public virtual long Updated { get; set; }
        public virtual long Total { get; set; }
        public virtual long ChangedStatus { get; set; }
        public virtual long ResidenceChnaged { get; set; }
        public virtual long Error { get; set; }
        public virtual long Region { get; set; }

        public virtual void Add(ImportStatistic statistic)
        {
            New += statistic.New;
            Conflicted += statistic.Conflicted;
            Updated += statistic.Updated;
            Error += statistic.Error;
            Total += statistic.Total;
            ChangedStatus += statistic.ChangedStatus;
            ResidenceChnaged += statistic.ResidenceChnaged;
        }
    }
}