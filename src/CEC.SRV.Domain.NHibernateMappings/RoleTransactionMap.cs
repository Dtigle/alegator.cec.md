using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate.Type;

namespace CEC.SRV.Domain.NHibernateMappings
{
	public class RoleTransactionMap : IAutoMappingOverride<RoleTransaction>
	{
		public void Override(AutoMapping<RoleTransaction> mapping)
		{
			mapping.Schema(Schemas.RSA);
			mapping.References(x => x.Role).Not.Nullable();
			mapping.References(x => x.Transaction).Not.Nullable();
		}
	}
}
