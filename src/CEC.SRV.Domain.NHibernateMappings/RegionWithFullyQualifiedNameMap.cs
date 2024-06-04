using System.Management.Instrumentation;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings
{
	public class RegionWithFullyQualifiedNameMap : IAutoMappingOverride<RegionWithFullyQualifiedName>
    {
		public void Override(AutoMapping<RegionWithFullyQualifiedName> mapping)
        {
            mapping.ReadOnly();
			mapping.Id(x => x.Id, "regionId");
		    mapping.Map(x => x.SaiseId);
			mapping.Table("v_HierarchicalRegions");
            mapping.Schema(Schemas.Lookup);
        }
    }
}