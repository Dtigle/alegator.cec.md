using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.ViewItem;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NHibernate.Type;

namespace CEC.SRV.Domain.NHibernateMappings.ViewItem
{
    public class ConflictViewItemMap : IAutoMappingOverride<ConflictViewItem>
    {
        public void Override(AutoMapping<ConflictViewItem> mapping)
        {
            mapping.Schema(Schemas.Importer);
            mapping.Table("v_RspModificationConflictData_Improved");
            mapping.Id(x => x.Id, "rspRegistrationDataId");
            mapping.ReadOnly();


            mapping.Map(x => x.StatusConflictCode).CustomType<EnumType<ConflictStatusCode>>();
            mapping.Map(x => x.AcceptConflictCode).CustomType<EnumType<ConflictStatusCode>>();
            mapping.Map(x => x.RejectConflictCode).CustomType<EnumType<ConflictStatusCode>>();
        }
    }
}