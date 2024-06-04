
using System.Collections;
using System.Collections.Generic;
using Amdaris.Domain.Paging;
using CEC.SRV.BLL.Dto;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.SRV.Domain.Print;

namespace CEC.SRV.BLL
{
    public interface IExporterBll : IBll
    {
		void CreateSaiseExporter(long electionId, bool exportAllVoters);
		PageResponse<SaiseExporterStage> GetSaiseExporter(PageRequest pageRequest, long? saiseExporterId);
		PageResponse<SaiseExporterStage> GetHistorySaiseExporter(PageRequest pageRequest, long? saiseExporterId);
		SaiseExporter GetActiveSaiseExporter();
		int GetProgressOfSaiseExporter(long saiseExporterId);
        IEnumerable<SaiseExporter> GetUnProcessedSaiseExporter();
		bool GetFailedMessageOfSaiseExporter(long saiseExporterId);

        SaiseExporter GetSaiseExporterByEdayId(long eDayId);

        bool SetElectionListNr(List<long> pollingStationsId,long electionId);
    }
}