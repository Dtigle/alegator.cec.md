using System.IO;

namespace CEC.Web.SRV.Infrastructure.Export
{
    public class ExportFileInfo
    {
        public Stream FileStream { get; set; }

        public string MimeType { get; set; }
    }
}