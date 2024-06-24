using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.ReportService;

namespace CEC.SRV.BLL.Exporters
{
    public static class SSRSClient
    {
        private static Dictionary<string, string> _supportedFormats = new Dictionary<string, string>
        {
            {"PDF", "pdf"},
            {"EXCEL", "xls"},
            {"EXCELOPENXML", "xlsx"},
            {"CSV", "csv"},
            {"MHTML", "mhtml"}
        };

        public static ReportData RequestReportStream(SSRSPrintParameters printParams, 
            IEnumerable<ParameterValue> reportParameters, string reportFormat = "PDF")
        {
            return RequestReportStream(printParams.ServerUrl, printParams.ReportName, 
                printParams.GetCredentials(), reportParameters, reportFormat);
        }

        public static ReportData RequestReportStream(string serverUrl, string reportPath, ICredentials ssrsCredentials, 
            IEnumerable<ParameterValue> reportParameters, string reportFormat = "PDF")
        {
            string fileExtension = GetFormatExtension(reportFormat);

            string devInfo = string.Format(@"<DeviceInfo><OutputFormat>{0}</OutputFormat><DpiX>600</DpiX><DpiY>600</DpiY></DeviceInfo>", reportFormat);
            string encoding;
            string mimeType;
            string extension;
            Warning[] warnings = null;
            string[] streamIDs = null;

            var ssrs = new ReportExecutionService();
            ssrs.Timeout = 2000000;
            ssrs.Credentials = ssrsCredentials;
            ssrs.Url = serverUrl;

            var execHeader = new ExecutionHeader();
            ssrs.ExecutionHeaderValue = execHeader;

            ssrs.LoadReport(reportPath, null);
            ssrs.SetExecutionParameters(reportParameters.ToArray(), "en-us");

            var result = ssrs.Render(reportFormat, devInfo, out extension, out encoding,
                out mimeType, out warnings, out streamIDs);

            return new ReportData {Report = result, FileExtension = fileExtension};
        }

        public static string GetFormatExtension(string reportFormat)
        {
            string fileExtension;
            var isFormatSupported = _supportedFormats.TryGetValue(reportFormat.ToUpper(), out fileExtension);
            if (isFormatSupported)
            {
                return fileExtension;
            }

            var supportedFormats = string.Join(", ", _supportedFormats.Keys);
            throw new NotSupportedException(string.Format("Report format {0} is not supported. Supported formats are: '{1}'", reportFormat, supportedFormats));
        }
    }

    public class ReportData
    {
        public byte[] Report { get; set; }
        public string FileExtension { get; set; }
    }
}
