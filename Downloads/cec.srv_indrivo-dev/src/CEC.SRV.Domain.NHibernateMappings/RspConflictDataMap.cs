using CEC.SRV.Domain.Importer;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate;
using NHibernate.Mapping;
using NHibernate.Type;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class RspConflictDataMap : IAutoMappingOverride<RspConflictData>
    {
        public void Override(AutoMapping<RspConflictData> mapping)
        {
            mapping.Table("[v_RspModificationConflictData]");
            mapping.Id(x => x.Id, "rspRegistrationDataId");
            mapping.Schema(Schemas.Importer);
            mapping.ReadOnly();

            mapping.References(x => x.SrvRegion).Column("regionId")
                .ReadOnly();
            mapping.Map(x => x.Issuedate).CustomSqlType(NHibernateUtil.DateTime2.Name).CustomType(NHibernateUtil.DateTime2.Name);
            mapping.Map(x => x.Expirationdate).CustomSqlType(NHibernateUtil.DateTime2.Name).CustomType(NHibernateUtil.DateTime2.Name);
            mapping.Map(x => x.DateOfExpiration).CustomSqlType(NHibernateUtil.DateTime2.Name).CustomType(NHibernateUtil.DateTime2.Name);
            mapping.Map(x => x.StatusConflictCode).CustomType<EnumType<ConflictStatusCode>>();
            mapping.Map(x => x.AcceptConflictCode).CustomType<EnumType<ConflictStatusCode>>();
            mapping.Map(x => x.RejectConflictCode).CustomType<EnumType<ConflictStatusCode>>();
            mapping.Map(x => x.StreetName);

            mapping.HasMany(x => x.Shares);
        }
    }
}