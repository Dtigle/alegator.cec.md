using System;
using System.Collections.Generic;

namespace SAISE.Domain
{
    public class SaiseElection : SaiseEntity
    {
        public virtual SaiseElectionType Type { get; set; }

        public virtual DateTime DateOfElection { get; set; }

        public virtual bool IsSubTypeOfLocalElection()
        {
            return (Type.Id & SaiseElectionType.Local) == SaiseElectionType.Local;
        }

        public virtual bool IsSubTypeOfMayorElection()
        {
            return ((Type.Id == SaiseElectionType.Local_PrimarLocal) ||
                    (Type.Id == SaiseElectionType.Local_PrimarGeneral));
        }
    }
}