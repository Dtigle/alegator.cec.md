using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class DataTransferStageMap: IAutoMappingOverride<DataTransferStage>
    {
        public void Override(AutoMapping<DataTransferStage> mapping)
        {
            mapping.Id(x => x.Id).Column("Id");
            mapping.Map(x => x.TableName).Not.Nullable().Column("TableName");
            mapping.Map(x => x.Processed).Not.Nullable().Column("Processed");
            mapping.Map(x => x.Total).Not.Nullable().Column("Total");
            mapping.IgnoreProperty(x => x.Percent);
        }
    }
}
