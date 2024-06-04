using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NHibernate.Type;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class AssignedPollingStationMap : IAutoMappingOverride<AssignedPollingStation>
    {
        public void Override(AutoMapping<AssignedPollingStation> mapping)
        {
            mapping.Table("AssignedPollingStation");
            mapping.Id(x => x.Id).Column("AssignedPollingStationId");
            mapping.Map(x => x.Type).CustomType<EnumType<AssignedPollingStationType>>().Not.Nullable();
            mapping.Map(x => x.Status).CustomType<EnumType<AssignedPollingStationStatus>>().Not.Nullable();
            mapping.Map(x => x.IsOpen).Not.Nullable();
            mapping.Map(x => x.IsOpeningEnabled).Not.Nullable();
            mapping.Map(x => x.IsElectionResultEnabled).Not.Nullable();
            mapping.Map(x => x.IsTurnoutEnabled).Not.Nullable();
            mapping.Map(x => x.OpeningVoters).Not.Nullable();
            mapping.Map(x => x.EstimatedNumberOfVoters).Not.Nullable();
            mapping.Map(x => x.NumberOfRoBallotPapers).Not.Nullable();
            mapping.Map(x => x.NumberOfRuBallotPapers).Not.Nullable();
            mapping.Map(x => x.ImplementsEVR).Not.Nullable();
            mapping.Map(x => x.NumberPerElection).Nullable();

            mapping.References(x => x.ElectionRound);
            mapping.References(x => x.AssignedCircumscription);
            mapping.References(x => x.PollingStation);

            //mapping.HasMany(x => x.ElectionTurnouts)
            //    .ExtraLazyLoad()
            //    .Cascade.AllDeleteOrphan()
            //    .Inverse()
            //    .Access.CamelCaseField(Prefix.Underscore);
        }
    }
}