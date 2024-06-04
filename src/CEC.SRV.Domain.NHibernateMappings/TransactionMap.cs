using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate.Type;

namespace CEC.SRV.Domain.NHibernateMappings
{
	public class TransactionMap : IAutoMappingOverride<Transaction>
	{
		public void Override(AutoMapping<Transaction> mapping)
		{
			mapping.Schema(Schemas.RSA);
		}
	}
}
