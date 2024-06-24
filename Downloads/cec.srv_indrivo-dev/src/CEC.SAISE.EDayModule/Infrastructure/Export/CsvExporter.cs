using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using Lib.Web.Mvc.JQuery.JqGrid;
using CEC.SAISE.EDayModule.Properties;

namespace CEC.SAISE.EDayModule.Infrastructure.Export
{
    public class CsvExporter : GridExporter
    {
        private readonly FileStream _fileStream;
        private readonly StreamWriter _writer;

        public CsvExporter(string dataName, Type gridModelType)
            : base(dataName, gridModelType)
        {
            _fileName = CreateFileName(dataName, "csv");

            bool IsExists = System.IO.Directory.Exists(Settings.Default.ExportTempFolder);
            if (!IsExists) System.IO.Directory.CreateDirectory(Settings.Default.ExportTempFolder);
            var tempFolder = Settings.Default.ExportTempFolder;
            var path = Path.Combine(tempFolder, _fileName);

            _fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            _writer = new StreamWriter(_fileStream, Encoding.UTF8);
            var headers = GetHeaders(GridModelType);
            WriteRow(new List<string> { "sep=;" });
            WriteRow(headers);
        }

        public override void Export(JqGridResponse gridData)
        {
            foreach (var record in gridData.Records)
            {
                WriteRow(record.Values);
            }
        }

        public override void Close()
        {
            if (_writer != null)
            {
                _writer.Close();
                _fileStream.Dispose();
            }
        }

        protected override void GetSupportedFileInfo(out string extension, out string mimeType)
        {
            extension = ".csv";
            mimeType = "application/text";
        }

        private void WriteRow(IEnumerable<object> cellValues)
        {
            _writer.WriteLine(string.Join(";", cellValues));
        }
    }
}