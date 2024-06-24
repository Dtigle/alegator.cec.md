using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class PersonMap : IAutoMappingOverride<Person>
    {
        public void Override(AutoMapping<Person> mapping)
        {
            mapping.Schema("SRV");
            mapping.Table("People");
            mapping.Map(x => x.Idnp).Not.Nullable()
                .Unique().UniqueKey("UX_Person_IDNP").Length(13);

            mapping.Map(x => x.FirstName).Not.Nullable();
            mapping.Map(x => x.Surname).Not.Nullable();

            mapping.Map(x => x.ExportedToSaise).Not.Nullable().Default("0");

            mapping.References(x => x.Gender).Not.Nullable();

            mapping.HasMany(x => x.Addresses)
                .Access.CamelCaseField(Prefix.Underscore);

            mapping.HasMany(x => x.PersonStatuses)
                .Access.CamelCaseField(Prefix.Underscore).Cascade.All().Inverse();

            mapping.Component(x => x.Document).ColumnPrefix("doc_");

            mapping.IgnoreProperty(x => x.EligibleAddress);

            mapping.IgnoreProperty(x => x.CurrentStatus);
            mapping.IgnoreProperty(x => x.DataFromEday);
            mapping.Map(x => x.Age).Formula("FLOOR(DATEDIFF(day, DateOfBirth, GETDATE())/365.25)").Access.CamelCaseField(Prefix.Underscore);
            mapping.Map(x => x.FullName).Formula("LTRIM(RTRIM(ISNULL(Surname, '') + ' ' + ISNULL(FirstName, '') + ' ' + ISNULL(MiddleName, '')))").Access.CamelCaseField(Prefix.Underscore);
            //mapping.Map(x => x.ElectionListNr).Nullable().Column("electionListNr");
        }
    }

}