using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain;

namespace CEC.SAISE.Domain
{
    public class ElectionType : EntityWithTypedId<long>
    {
        public const long BaseList = 0;
        public const long Parliamentary = 1;
        public const long Local = 2;
        public const long Local_PrimarLocal = 10;
        public const long Local_ConsilieriLocal = 14;
        public const long Local_ConsilieriRaional = 18;
        public const long Local_ConsilieriMunicipali = 22;
        public const long Local_PrimarGeneral = 26;
        public const long Referendum = 3;


        public virtual string TypeName { get; set; }

        public virtual string Description { get; set; }

        public virtual short ElectionArea { get; set; }

        public virtual short ElectionCompetitorType { get; set; }

        public virtual short ElectionRoundsNo { get; set; }

        public virtual bool AcceptResidenceDoc { get; set; }

        public virtual bool AcceptVotingCert { get; set; }

        public virtual bool AcceptAbroadDeclaration { get; set; }
    }
}
