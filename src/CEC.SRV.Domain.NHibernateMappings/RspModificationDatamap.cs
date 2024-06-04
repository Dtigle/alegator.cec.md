using CEC.SRV.Domain.Importer;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate;
using NHibernate.Type;

namespace CEC.SRV.Domain.NHibernateMappings
{
    public class RspModificationDataMap : IAutoMappingOverride<RspModificationData>
    {
        public void Override(AutoMapping<RspModificationData> mapping)
        {
            mapping.Map(x => x.IssuedDate).CustomSqlType(NHibernateUtil.DateTime2.Name).CustomType(NHibernateUtil.DateTime2.Name);
            mapping.Map(x => x.ValidBydate).CustomSqlType(NHibernateUtil.DateTime2.Name).CustomType(NHibernateUtil.DateTime2.Name);

            mapping.Map(x => x.Seria).Column("series");
            mapping.Map(x => x.IssuedDate).Column("issuedate");
            mapping.Map(x => x.ValidBydate).Column("expirationdate");
            mapping.Map(x => x.DocumentTypeCode).Column("doctypecode");

            mapping.HasMany(x => x.Registrations).Cascade.SaveUpdate();

            mapping.Map(x => x.StatusConflictCode).CustomType<EnumType<ConflictStatusCode>>();
            mapping.Map(x => x.AcceptConflictCode).CustomType<EnumType<ConflictStatusCode>>();
            mapping.Map(x => x.RejectConflictCode).CustomType<EnumType<ConflictStatusCode>>();
        }
    }
}