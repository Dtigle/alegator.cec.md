using Amdaris.Domain.Paging;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Dto.TemplateManager;
using CEC.SAISE.Domain;
using CEC.SAISE.Domain.TemplateManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL
{
    public interface IDocumentsBll
    {
        Task<List<ReportParameterDto>> ListTemplateParameters(long templateNameId);
        Task<DocumentDto> GetDocumentAsync(long electionRoundId, long pollingStationId, long templateNameId);
        Task<SaveUpdateResult> SaveUpdateDocument(DocumentDto documentDto, long templateNameId, DocumentStatus bpStatusToBeSet);
        Task<List<ReportParameterValueDto>> ListDocumentParameters(long documentId);
        Task<SaveUpdateResult> SaveUpdateDocumentElectionResults(BallotPaperDataDto ballotPaperDto, long templateNameId, DocumentStatus bpStatusToBeSet);
        Task<SaveUpdateResult> SaveConsolidatedResultsDocument(BallotPaperConsolidationDataDto ballotPaperDto, long templateNameId);
        Task<DocumentDto> GetFinalReportDataAsync(long electionRoundId, long circumscriptionId, long templateNameId);
        Task<List<UnconfirmedPollingStationsDto>> GetUnconfirmedPollingStations(long electionRoundId, long circumscriptionId, long templateNameId);
        Task<Document> GetCECEDocumentAsync(long electionRoundId, long circumscriptionId, long templateNameId);
        Task<DocumentBallotPaperDto> GetBallotPaperDocumentAsync(long electionId, long templateNameId, long? pollingStationId, long? circumscriptionId);
        Task<string> CreateDocumentName(long templateNameId, long electionId, long? pollingStationId, long? circumscriptionId);
        Task<bool> SaveDocumentPath(long documentId, string fileName, int contentLength, string contentType);
        Task<bool> DocumentIsUploaded(long documentId);
        Task<PageResponse<PollintStationDocumentStageDto>> GetPollingStationDocuments(PageRequest pageRequest, long electionId, long templateNameId, long circumscriptionId);
        Task<DocumentDto> GetCircumscriptionDocumentAsync(long electionId, long circumscriptionId, long templateNameId);
        Task<long> GetElectionRoundIdByElection(long electionId);

    }
}
