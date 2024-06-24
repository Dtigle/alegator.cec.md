using CEC.SRV.Domain.Importer;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class ImportStatisticMap : IAutoMappingOverride<ImportStatistic>
    {
        public void Override(AutoMapping<ImportStatistic> mapping)
        {
            mapping.Schema(Schemas.Importer);
            mapping.Map(x => x.Date).Not.Nullable();
        }
    }
}