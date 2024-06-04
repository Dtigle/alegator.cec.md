using CEC.SRV.Domain.Importer;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class ConflictShareMap : IAutoMappingOverride<ConflictShare>
    {
        public void Override(AutoMapping<ConflictShare> mapping)
        {
            mapping.Schema(Schemas.RSA);
            mapping.References(x => x.Source).Column("sourceId").Not.Nullable().ForeignKey("FK_ConflictSharing_Regions_sourceId");
            mapping.References(x => x.Destination).Column("destinationId").Not.Nullable().ForeignKey("FK_ConflictSharing_Regions_destinationId");
            mapping.References(x => x.Reason).Column("reasonId").Not.Nullable().ForeignKey("FK_ConflictSharing_Reasons_reasonId");
            mapping.References(x => x.Conflict).Column("rspConflictDataId").Not.Nullable().ForeignKey("FK_ConflictSharing_Conflicts_rspConflictDataId");
        }
    }
}