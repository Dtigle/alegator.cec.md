using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class QueryResponseOutMap : IAutoMappingOverride<QueryResponseOut>
    {
        public void Override(AutoMapping<QueryResponseOut> mapping)
        {
            mapping.Id(x => x.Id).Column("Id");
        }
    }
}
