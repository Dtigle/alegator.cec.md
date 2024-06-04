using System.Collections.Generic;
using Amdaris.Domain.Paging;
using CEC.SRV.BLL.Dto;
using CEC.SRV.Domain.Dto;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.SRV.Domain.Print;

namespace CEC.SRV.BLL
{
    public interface IPrintBll : IBll
    {
        void CreatePrintSession(long electionId, IEnumerable<long> pollingStationIds);
        void CreatePrintSession(long electionId, IList<ExportPollingStationDto> exportPollingStationsDto);
        void CancelPrintSession(long printSessionId);
        void ProcessPrintSession(SSRSPrintParameters printParams);
		IList<PrintSession> GetPrintSessionByStatus(PrintStatus status);
		IList<ExportPollingStation> GetExportPollingStationsByPrintSession(long printSessionId);
		PageResponse<ExportPollingStation> GetExportPollingStations(PageRequest pageRequest, long? printSessionId);
		PageResponse<ExportPollingStation> GetHistoryExportPollingStations(PageRequest pageRequest, long? printSessionId);
		PageResponse<PrintSession> GetExportPrintSessions(PageRequest pageRequest);
		PageResponse<SaiseExporter> GetSaiseExporter(PageRequest pageRequest);
        byte[] RequestStayStatementReport(SSRSPrintParameters ssrsParameters, long stayStatementId);
    }
}