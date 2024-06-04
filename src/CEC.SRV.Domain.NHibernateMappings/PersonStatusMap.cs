using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class PersonStatusMap : IAutoMappingOverride<PersonStatus>
    {
        public void Override(AutoMapping<PersonStatus> mapping)
        {
            mapping.References(x => x.Person).Not.Nullable();
            mapping.References(x => x.StatusType).Not.Nullable();
            mapping.Map(x => x.Confirmation).Not.Nullable();
            mapping.Map(x => x.IsCurrent).Not.Nullable();
        }
    }
}