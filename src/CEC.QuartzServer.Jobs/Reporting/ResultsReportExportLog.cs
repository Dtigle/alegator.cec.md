using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAISE.Domain;

namespace CEC.QuartzServer.Jobs.Reporting
{
    public class ResultsReportExportLog
    {
        public ResultsReportExportLog()
        {
            Elections = new List<ElectionItem>();
        }

        public List<ElectionItem> Elections { get; set; }

        public int GetCountOfNotExportedPollingStations()
        {
            return Elections.Sum(x => x.GetCountOfNotExportedPollingStations());
        }
    }

    public class ElectionItem
    {
        public ElectionItem()
        {
            Districts = new List<DistrictItem>();
        }

        public long ElectionId { get; set; }

        public List<DistrictItem> Districts { get; set; }

        public bool HasAnyBPRecieved()
        {
            var result = Districts.Any(x => x.HasAnyBPReceived());
            return result;
        }

        public int GetCountOfNotExportedPollingStations()
        {
            return Districts.Sum(x => x.GetCountOfNotExportedPollingStations());
        }
    }

    public class DistrictItem
    {
        public DistrictItem()
        {
            Villages = new List<VillageItem>();
        }

        public long DistrictId { get; set; }

        public string Name { get; set; }

        public List<VillageItem> Villages { get; set; }

        public bool HasAnyBPReceived()
        {
            var result =  Villages.Any(x => x.HasAnyBPReceived());
            return result;
        }

        public int GetCountOfNotExportedPollingStations()
        {
            return Villages.Sum(x => x.GetCountOfNotExportedPollingStations());
        }
    }

    public class VillageItem
    {
        public VillageItem()
        {
            PollingStations = new List<PollingStationItem>();
        }

        public long VillageId { get; set; }

        public string Name { get; set; }

        public List<PollingStationItem> PollingStations { get; set; }

        public bool HasAnyBPReceived()
        {
            var result = PollingStations.Any(x => x.BallotPaperReceived);
            return result;
        }

        public int GetCountOfNotExportedPollingStations()
        {
            return PollingStations.Count(x => !x.IsExported);
        }
    }

    public class PollingStationItem
    {
        public long PollingStationId { get; set; }

        public string Number { get; set; }

        public bool BallotPaperReceived { get; set; }

        public bool IsExported { get; set; }
    }
}
