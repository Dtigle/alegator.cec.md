using CEC.SRV.Domain.Importer;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class RawDataMap : IAutoMappingOverride<RawData>
    {
        public void Override(AutoMapping<RawData> mapping)
        {
            mapping.IgnoreProperty(x => x.Source);
            mapping.Map(x => x.StatusMessage).Length(4001);
        }
    }
}