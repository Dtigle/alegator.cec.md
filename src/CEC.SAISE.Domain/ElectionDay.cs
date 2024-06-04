using Amdaris.Domain;
using System;

namespace CEC.SAISE.Domain
{
    public class ElectionDay : Entity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTimeOffset ElectionDayDate { get; set; }
        public virtual DateTime? StartDateToReportDb { get; set; }
        public virtual DateTime? EndDateToReportDb { get; set; }
    }
}
