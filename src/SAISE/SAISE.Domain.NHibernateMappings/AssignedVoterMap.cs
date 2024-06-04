using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace SAISE.Domain.NHibernateMappings
{
    public class AssignedVoterMap : IAutoMappingOverride<AssignedVoter>
    {
        public void Override(AutoMapping<AssignedVoter> mapping)
        {
            mapping.Table("AssignedVoter");
            mapping.Id(x => x.Id).Column("AssignedVoterId");
            mapping.Map(x => x.RegionId).Not.Nullable().Column("RegionId");
            mapping.References(x => x.RequestingPollingStation).Not.Nullable().Column("RequestingPollingStationId");
            mapping.References(x => x.PollingStation).Not.Nullable().Column("PollingStationId");
            mapping.References(x => x.Voter).Not.Nullable().Column("VoterId");
            mapping.Map(x => x.Category).Not.Nullable().Column("Category");
            mapping.Map(x => x.Status).Not.Nullable().Column("Status");
            mapping.Map(x => x.EditedUserId).Not.Nullable().Column("EditUserId");
            mapping.Map(x => x.EditedDate).Not.Nullable().Column("EditDate");
            mapping.Map(x => x.Version).Not.Nullable().Column("Version");
        }
    }
}