
using System.IO;


namespace CEC.SAISE.EDayModule.Infrastructure.Export
{
    public class ExportFileInfo
    {
        public Stream FileStream { get; set; }

        public string MimeType { get; set; }
    }
}