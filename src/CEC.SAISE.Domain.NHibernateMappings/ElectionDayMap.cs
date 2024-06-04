using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace CEC.SAISE.Domain.NHibernateMappings
{
    public class ElectionDayMap : IAutoMappingOverride<ElectionDay>
    {
        public void Override(AutoMapping<ElectionDay> mapping)
        {
            mapping.Table("ElectionDay");
            mapping.Id(x => x.Id).Column("ElectionDayId");
            mapping.Map(x => x.Name).Not.Nullable().Column("Name");
            mapping.Map(x => x.Description).Not.Nullable().Column("Description");
            mapping.Map(x => x.ElectionDayDate).Not.Nullable().Column("ElectionDayDate");
            mapping.Map(x => x.StartDateToReportDb).Nullable().Column("StartDateToReportDb");
            mapping.Map(x => x.EndDateToReportDb).Nullable().Column("EndDateToReportDb");
        }
    }
}
