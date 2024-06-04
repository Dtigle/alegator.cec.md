using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.ViewItem;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate.Type;

namespace CEC.SRV.Domain.NHibernateMappings.ViewItem
{
    public class ConflictCountRegistratorMap : IAutoMappingOverride<ConflictCountRegistrator>
    {
        public void Override(AutoMapping<ConflictCountRegistrator> mapping)
        {
            mapping.Schema(Schemas.Importer);
            mapping.Table("v_ConflictCountRegistrator");
            mapping.Id(x => x.Id, "RegionId");
            mapping.ReadOnly();
        }
    }
}