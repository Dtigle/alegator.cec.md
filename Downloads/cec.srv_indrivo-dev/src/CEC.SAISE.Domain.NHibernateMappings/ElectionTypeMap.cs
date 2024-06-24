using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class ElectionTypeMap : IAutoMappingOverride<ElectionType>
    {
        public void Override(AutoMapping<ElectionType> mapping)
        {
            mapping.Table("ElectionType");
            mapping.Id(x => x.Id).Column("ElectionTypeId");
            mapping.Map(x => x.TypeName).Not.Nullable().Length(50).Column("TypeName");
            mapping.Map(x => x.Description).Length(100).Nullable().Column("Description");
            mapping.Map(x => x.ElectionArea).Length(100).Nullable().Column("ElectionArea");
            mapping.Map(x => x.ElectionCompetitorType).Length(100).Nullable().Column("ElectionCompetitorType");
            mapping.Map(x => x.ElectionRoundsNo).Length(100).Nullable().Column("ElectionRoundsNo");
            mapping.Map(x => x.AcceptResidenceDoc).Length(100).Nullable().Column("AcceptResidenceDoc");
            mapping.Map(x => x.AcceptVotingCert).Length(100).Nullable().Column("AcceptVotingCert");
            mapping.Map(x => x.AcceptAbroadDeclaration).Length(100).Nullable().Column("AcceptAbroadDeclaration");
        }
    }
}
