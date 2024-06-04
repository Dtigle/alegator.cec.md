using CEC.SRV.Domain.Importer;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class RspRegistrationDataMap : IAutoMappingOverride<RspRegistrationData>
    {
        public void Override(AutoMapping<RspRegistrationData> mapping)
        {
            mapping.Schema(Schemas.Importer);
            //mapping.Map(x => x.DateOfExpiration).CustomSqlType(NHibernateUtil.DateTime2.Name).CustomType(NHibernateUtil.DateTime2.Name);
            mapping.Map(x => x.DateOfRegistration).CustomSqlType(NHibernateUtil.DateTime2.Name).CustomType(NHibernateUtil.DateTime2.Name);
            //mapping.References(x => x.RspModificationData).Not.Nullable();

            mapping.Map(x => x.HouseNumber).Column("houseNr");
            mapping.Map(x => x.ApartmentNumber).Column("apNr");
            mapping.Map(x => x.ApartmentSuffix).Column("apSuffix");
        }
    }
}