using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class IdnpSearchLogEntryMap : IAutoMappingOverride<IdnpSearchLogEntry>
    {
        public void Override(AutoMapping<IdnpSearchLogEntry> mapping)
        {
            mapping.Table("PV_LOG_SEARCH_IDNP");
            mapping.Id(x => x.Id).Column("ID");
            mapping.Map(x => x.SystemUserId);
            mapping.Map(x => x.Idnp);
            mapping.Map(x => x.Found);
            //mapping.Map(x => x.LogTime);
            mapping.Map(x => x.VotingStatus);
            mapping.Map(x => x.VoterStatus);
            mapping.Map(x => x.AssignedVoterStatus);
        }
    }
}