using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace SAISE.Domain.NHibernateMappings
{
    //public class SaiseVillageMap : IAutoMappingOverride<SaiseVillage>
    //{
    //    public void Override(AutoMapping<SaiseVillage> mapping)
    //    {
    //        mapping.ReadOnly();
    //        mapping.Table("Village");
    //        mapping.Id(x => x.Id).Column("VillageId");
    //        mapping.References(x => x.District).Not.Nullable().Column("DistrictId");
    //        mapping.Map(x => x.NameRo).Not.Nullable().Column("NameRo");
    //    }
    //}
}