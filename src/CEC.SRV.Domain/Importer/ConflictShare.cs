using System.Text;
using Amdaris.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain.Importer
{
    public class ConflictShare: SRVBaseEntity
    {
        public virtual RspConflictData Conflict { get; set; }

        public virtual Region Source { get; set; }
        public virtual Region Destination { get; set; }

        public virtual ConflictShareReasonTypes Reason { get; set; }
        public virtual string Note { get; set; }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Id").Append(Id).Append("\n");
            sb.Append("SourceRegion").Append(Source != null? Source.Name:"-").Append("\n");
            sb.Append("Destination").Append(Destination != null ? Destination.Name : "-").Append("\n");
            sb.Append("Note").Append(Note).Append("\n");
            return sb.ToString();
        }
    }
}
