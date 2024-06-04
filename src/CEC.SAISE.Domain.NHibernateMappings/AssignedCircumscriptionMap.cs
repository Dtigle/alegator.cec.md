using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class AssignedCircumscriptionMap : IAutoMappingOverride<AssignedCircumscription>
    {
        public void Override(AutoMapping<AssignedCircumscription> mapping)
        {
            mapping.Table("AssignedCircumscription");
            mapping.Id(x => x.Id).Column("AssignedCircumscriptionId");
            mapping.Map(x => x.Number).Not.Nullable();
            mapping.Map(x => x.NameRo).Not.Nullable();
            mapping.Map(x => x.CircumscriptionId).Not.Nullable();
            mapping.References(x => x.ElectionRound).Not.Nullable();
            mapping.References(x => x.Region).Not.Nullable();
        }
    }
}
