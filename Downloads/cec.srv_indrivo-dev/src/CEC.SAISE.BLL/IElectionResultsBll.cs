using CEC.SAISE.BLL.Dto;
using CEC.SAISE.Domain;
using CEC.SAISE.Domain.TemplateManager;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL
{
    public interface IElectionResultsBll
    {
        Task<BallotPaperDto> GetBallotPaperAsync(long electionId, long pollingStationId);

        Task<BallotPaperDto> GetBallotPaperAsync(long ballotPaperId);
        Task<SaveUpdateResult> SaveUpdateResults(BallotPaperDataDto ballotPaperDto, BallotPaperStatus bpStatusToBeSet, long templateNameId);
        void TransferEDayData(string serverIpAddress, string remoteUserName, string remotePassword);
        List<DataTransferStage> GetDataTransferStages();
        bool CheckLinkedServerExists();
        bool AproveBallotPaper(List<long> model);
        Task<BallotPaperConsolidationDataExtendedDto> GetElectionResultsByCircumscription(long assignedCircumscriptionId, long electionRoundId, long templateNameId);
        Task<List<UnconfirmedPollingStationsDto>> GetUnconfirmedBallotPapers(long assignedCircumscriptionId, long electionRoundId);
        Task<SaveUpdateResult> SaveUpdateConsolidatedResults(BallotPaperConsolidationDataExtendedDto ballotPaperDto, long templateNameId);
        Task<BallotPaperHeaderDataDto> GetBallotPaperHeaderData(long electionId, long? pollingStationId, long circumscriptionId, long templateNameId);



    }
}
