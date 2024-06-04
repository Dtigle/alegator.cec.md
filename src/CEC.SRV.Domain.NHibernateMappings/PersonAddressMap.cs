using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class PersonAddressMap : IAutoMappingOverride<PersonAddress>
    {
        public void Override(AutoMapping<PersonAddress> mapping)
        {
            mapping.References(x => x.Person).Not.Nullable().Unique().UniqueKey("UX_PersonAddress_Person_Address");
            mapping.References(x => x.Address).Not.Nullable().Unique().UniqueKey("UX_PersonAddress_Person_Address");
            mapping.Map(x => x.ApSuffix).Length(10);
            mapping.References(x => x.PersonAddressType).Not.Nullable();
            mapping.Map(x => x.DateOfRegistration).CustomSqlType(NHibernateUtil.DateTime2.Name).CustomType(NHibernateUtil.DateTime2.Name);
            mapping.Map(x => x.DateOfExpiration).CustomSqlType(NHibernateUtil.DateTime2.Name).CustomType(NHibernateUtil.DateTime2.Name);
            mapping.References(x => x.PersonFullAddress).Column("personAddressId").Not.Update().Not.Insert().ReadOnly();
        }
    }
}