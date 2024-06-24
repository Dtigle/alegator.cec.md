using System.Linq;

namespace CEC.SRV.Domain.Lookup
{
    public class ElectionType : Lookup
    {
        public const long Local_Referendum = 1;
        public const long National_Referendum = 2;
        public const long Parliamentary = 3;
        public const long Locals = 4;
        public const long Locals_General = 5;
        public const long Locals_New = 6;
        private readonly long[] SubTypesOfLocalElections = {Local_Referendum, Locals, Locals_General, Locals_New};
        
        public virtual bool IsLocal()
        {
            return SubTypesOfLocalElections.Contains(this.Id);
        }

        public virtual int Code { get; set; }

        public virtual ElectionArea ElectionArea { get; set; }

        public virtual ElectionCompetitorType ElectionCompetitorType { get; set; }

        public virtual int ElectionRoundsNo { get; set; }

        /* Accept residence documents */
        public virtual bool AcceptResidenceDoc { get; set; }

        /* Accept voting certificate */
        public virtual bool AcceptVotingCert { get; set; }

        public virtual bool AcceptAbroadDeclaration { get; set; }

        public virtual CircumscriptionList CircumscriptionList { get; set; }

    }
}