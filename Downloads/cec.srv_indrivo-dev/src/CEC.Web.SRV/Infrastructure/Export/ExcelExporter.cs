using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using CEC.SRV.BLL;
using CEC.Web.SRV.Properties;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace CEC.Web.SRV.Infrastructure.Export
{
    public class ExcelExporter : GridExporter
    {
        private SpreadsheetDocument _doc;
        private OpenXmlWriter _writer;
        private WorkbookPart _workbookPart;

        public ExcelExporter(string dataName, Type gridModelType) 
            : base(dataName, gridModelType)
        {
            _fileName = CreateFileName(dataName, "xlsx");
            var tempFolder = Settings.Default.ExportTempFolder;
            var path = Path.Combine(tempFolder, _fileName);

            _doc = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);
            CreateXlDocStructure();
        }

        public override void Export(JqGridResponse gridData)
        {
            foreach (var gridRecord in gridData.Records)
            {
                WriteRow(gridRecord.Values);
            }
        }

        public override void Close()
        {
            if (_writer != null)
            {
                _writer.WriteEndElement();
                _writer.WriteEndElement();
                _writer.Close();

                _workbookPart.Workbook.Save();
            }

            if (_doc != null)
            {
                _doc.Close();
                _doc.Dispose();
            }
        }

        protected override void GetSupportedFileInfo(out string extension, out string mimeType)
        {
            extension = ".xslx";
            mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }

        private void CreateXlDocStructure()
        {
            //create workbook part
            _workbookPart = _doc.AddWorkbookPart();
            _workbookPart.Workbook = new Workbook();
            var sheets = _workbookPart.Workbook.AppendChild(new Sheets());

            //create worksheet part, and add it to the sheets collection in workbook
            var worksheetPart = _workbookPart.AddNewPart<WorksheetPart>();

            // Append a new worksheet and associate it with the workbook.
            var sheet = new Sheet()
            {
                Id = _doc.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = DataName
            };
            sheets.Append(sheet);

            _writer = OpenXmlWriter.Create(worksheetPart);

            var headers = GetHeaders(GridModelType);
            _writer.WriteStartElement(new Worksheet());
            _writer.WriteStartElement(new SheetData());

            WriteRow(headers);
        }

        private void WriteRow(IEnumerable<object> cellValues)
        {
            _writer.WriteStartElement(new Row());

            foreach (var cellValue in cellValues)
            {
                var val = new CellValue(Convert.ToString(cellValue));
                var cell = new Cell(){CellValue = val, DataType = CellValues.String};
                _writer.WriteElement(cell);
            }

            _writer.WriteEndElement();
        }
    }
}