using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.ViewItem;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate.Type;

namespace CEC.SRV.Domain.NHibernateMappings.ViewItem
{
    public class ConflictCountAdminMap : IAutoMappingOverride<ConflictCountAdmin>
    {
        public void Override(AutoMapping<ConflictCountAdmin> mapping)
        {
            mapping.Schema(Schemas.Importer);
            mapping.Table("v_ConflictCountAdmin");
            mapping.Id(x => x.Id, "RegionId");
            mapping.ReadOnly();
        }
    }
}