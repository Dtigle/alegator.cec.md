using CEC.SAISE.Domain.TemplateManager;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.Domain.NHibernateMappings.TemplateManagerMappings
{
    public class ReportParameterValueMap : IAutoMappingOverride<ReportParameterValue>
    {
        public void Override(AutoMapping<ReportParameterValue> mapping)
        {
            mapping.Table("ReportParameterValues");
            mapping.Id(x => x.Id).Column("ReportParameterValueId");
            mapping.References(x => x.Document).Column("DocumentId");
            mapping.References(x => x.ReportParameter).Column("ReportParameterId");
            mapping.References(x => x.ElectionCompetitor).Column("ElectionCompetitorId");
            mapping.References(x => x.ElectionCompetitorMember).Column("ElectionCompetitorMemberId"); 
            mapping.Map(x => x.ValueContent);
            //mapping.Map(x => x.ElectionCompetitorMemberName).Nullable();
            //mapping.Map(x => x.ElectionCompetitorName).Nullable();
            mapping.Map(x => x.BallotCount).Nullable();

        }
    }
}
