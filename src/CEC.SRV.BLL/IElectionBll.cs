using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.BLL
{
    public interface IElectionBll : IBll
    {
        void SaveOrUpdate(long? id, DateTime electionDate, string nameRo, string nameRu, string description, 
            long electionTypeId, ElectionStatus status, string statusReason, string reportsPath);

        IList<ElectionRound> GetElectionRounds(long electionId);

        IList<Election> GetActiveElections();
        Election GetCurrentElection();
        void VerificationIfElectionHasReference(long electionId);
    }
}
