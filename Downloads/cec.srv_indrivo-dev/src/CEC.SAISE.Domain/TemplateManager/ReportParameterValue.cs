namespace CEC.SAISE.Domain.TemplateManager
{
    public class ReportParameterValue : SaiseBaseEntity
    {
        public virtual string ValueContent { get; set; }
        //public virtual string ElectionCompetitorName { get; set; }
        //public virtual string ElectionCompetitorMemberName { get; set; }
        public virtual long? BallotCount { get; set; }
        // Navigation properties
        public virtual ElectionCompetitorMember ElectionCompetitorMember { get; set; }
        public virtual ElectionCompetitor ElectionCompetitor { get; set; }
        public virtual Document Document { get; set; }
        public virtual ReportParameter ReportParameter { get; set; }
    }
}
