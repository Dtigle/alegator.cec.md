using CEC.SRV.Domain.ViewItem;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SRV.Domain.NHibernateMappings.ViewItem
{
    public class VoterViewItemMap : IAutoMappingOverride<VoterViewItem>
    {
        public void Override(AutoMapping<VoterViewItem> mapping)
        {
            mapping.Schema(Schemas.RSA);
            mapping.Table("v_PeopleWithFullAddress");
            mapping.Id(x => x.Id, "person_id");
            mapping.ReadOnly();

            mapping.Map(x => x.Status).Column("Statut");
            mapping.Map(x => x.StatusId).Column("IdStatut");
            mapping.Map(x => x.Address).Column("FullAddress");
            mapping.Map(x => x.AddressTypeId).Column("IdTipAdresa");
            mapping.Map(x => x.AddressType).Column("NumeTipAdresa");
            mapping.Map(x => x.DocumentNumber).Column("Doc");
            mapping.Map(x => x.AddressExpirationDate).Column("DataExpirareAdresa");
            mapping.Map(x => x.RegionHasStreets).Column("HasStreets");
            mapping.Map(x => x.StreeetId).Column("StreetId");
            mapping.Map(x => x.AddressId).Column("AddressId");
            mapping.Map(x => x.electionListNr).Column("electionListNr");

        }
    }
}