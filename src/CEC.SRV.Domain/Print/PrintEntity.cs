using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain;

namespace CEC.SRV.Domain.Print
{
    public abstract class PrintEntity : AuditedEntity<SRVIdentityUser>
    {
        /// <summary>
        /// Start date of exporting one polling station
        /// </summary>
        public virtual DateTimeOffset? StartDate { get; set; }

        /// <summary>
        /// End date of exporting one polling station
        /// </summary>
        public virtual DateTimeOffset? EndDate { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public virtual PrintStatus Status { get; set; }
    }
}
