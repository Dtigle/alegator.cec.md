using CEC.SAISE.BLL.Dto;
using CEC.SAISE.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL
{
    public interface IElectionResultsBll
    {
        Task<BallotPaperDto> GetBallotPaperAsync(long electionId, long pollingStationId);

        Task<BallotPaperDto> GetBallotPaperAsync(long ballotPaperId);
        Task<SaveUpdateResult> SaveUpdateResults(BallotPaperDataDto ballotPaperDto, BallotPaperStatus bpStatusToBeSet);
        void TransferEDayData(string serverIpAddress, string remoteUserName, string remotePassword);
        List<DataTransferStage> GetDataTransferStages();
        bool CheckLinkedServerExists();
        bool AproveBallotPaper(List<long> model);
        

    }
}
