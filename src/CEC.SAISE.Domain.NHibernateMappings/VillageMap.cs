using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate.Type;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    //public class VillageMap : IAutoMappingOverride<Village>
    //{
    //    public void Override(AutoMapping<Village> mapping)
    //    {
    //        mapping.Table("Village");
    //        mapping.Id(x => x.Id).Column("VillageId");
    //        mapping.Map(x => x.RopUniqueId).Not.Nullable();
    //        mapping.Map(x => x.Number).Not.Nullable();
    //        mapping.Map(x => x.NameRo).Not.Nullable().Length(100);
    //        mapping.Map(x => x.NameRu).Nullable().Length(100);
    //        mapping.Map(x => x.OldName).Nullable().Length(100);
    //        mapping.Map(x => x.LocalCouncilSeats).Nullable();
    //        mapping.Map(x => x.Type).CustomType<EnumType<VillageType>>();
    //        mapping.References(x => x.District);
    //        mapping.HasMany(x => x.PollingStations);
    //    }
    //}
}