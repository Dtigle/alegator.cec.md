using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate.Mapping;
using NHibernate.Type;

namespace CEC.SAISE.Domain.NHibernateMappings
{
	public class PoliticalPartyStatusOverrideMap : IAutoMappingOverride<PoliticalPartyStatusOverride>
	{
		public void Override(AutoMapping<PoliticalPartyStatusOverride> mapping)
		{
			mapping.Table("PoliticalPartyStatusOverride");
			mapping.Id(x => x.Id).Column("PoliticalPartyStatusOverrideId");
			mapping.Map(x => x.Status)
				.Column("PoliticalpartyStatus")
				.CustomType<EnumType<PoliticalPartyStatus>>().Not.Nullable();

			mapping.References(x => x.PoliticalParty).Column("ElectionCompetitorId").Not.Insert().Not.Update().ReadOnly();
			mapping.References(x => x.ElectionRound).Not.Nullable();
			mapping.References(x => x.AssignedCircumscription).Not.Nullable();
		}
	}
}