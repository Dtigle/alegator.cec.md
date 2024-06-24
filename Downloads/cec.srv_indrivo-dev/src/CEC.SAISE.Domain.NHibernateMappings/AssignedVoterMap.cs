using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate.Type;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class AssignedVoterMap : IAutoMappingOverride<AssignedVoter>
    {
        public void Override(AutoMapping<AssignedVoter> mapping)
        {
            mapping.Table("AssignedVoter");
            mapping.Id(x => x.Id).Column("AssignedVoterId");
            mapping.Map(x => x.Category).Not.Nullable();
            mapping.Map(x => x.Status).CustomType<EnumType<AssignedVoterStatus>>().Not.Nullable();
            mapping.Map(x => x.Comment).Nullable();
            mapping.Map(x => x.ElectionListNr).Nullable();
            mapping.References(x => x.Region);
            mapping.References(x => x.PollingStation);
            mapping.References(x => x.RequestingPollingStation);
            mapping.References(x => x.Voter);
            mapping.HasMany(x => x.VoterCertificats).Inverse()
                .Cascade.All();
        }
    }
}