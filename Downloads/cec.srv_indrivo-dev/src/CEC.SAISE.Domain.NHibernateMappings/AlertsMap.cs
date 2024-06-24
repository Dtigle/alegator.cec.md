using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class AlertsMap
    {
        public void Override(AutoMapping<Alerts> mapping)
        {
            mapping.Table("Alerts");
            mapping.Id(x => x.Id).Column("AlertId");
            mapping.References(x => x.Voter).Not.Nullable().Column("VoterId");
            mapping.Map(x => x.FirstName).Nullable().Column("FirstName");
            mapping.Map(x => x.LastName).Nullable().Column("LastName");
            mapping.Map(x => x.Patronymic).Nullable().Column("Patronymic");
            mapping.Map(x => x.Idnp).Nullable().Column("Idnp");
            mapping.Map(x => x.DateOfBirth).Nullable().Column("DateOfBirth");
            mapping.Map(x => x.Adress).Nullable().Column("Adress");
            mapping.Map(x => x.DocumentNumber).Nullable().Column("DocumentNumber");
            mapping.References(x => x.PollingStation).Nullable().Column("PollingStationId");
            mapping.Map(x => x.PollingStationAdress).Nullable().Column("PollingStationAdress");
            mapping.Map(x => x.DateRegistration).Nullable().Column("DateRegistration");
        }

    }
}
