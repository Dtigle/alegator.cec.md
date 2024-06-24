using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class LinkedRegionMap : IAutoMappingOverride<LinkedRegion>
    {
        public void Override(AutoMapping<LinkedRegion> mapping)
        {
            mapping.HasManyToMany(x => x.Regions)
                .Access.CamelCaseField(Prefix.Underscore)
                .ExtraLazyLoad()
                .Schema(Schemas.RSA)
                .ParentKeyColumn("linkedRegionId")
                .ChildKeyColumn("regionId")
                .Table("LinkedRegions_Region")
                .ForeignKeyConstraintNames("FK_LinkedRegions_Region_LinkedRegions_linkedRegionId", "FK_LinkedRegions_Region_Regions_regionId");

        }
    }
}