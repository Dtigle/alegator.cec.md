using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using CEC.SRV.BLL;
using CEC.Web.SRV.Properties;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace CEC.Web.SRV.Infrastructure.Export
{
    public static class ExportHelper
    {
        private static readonly Dictionary<string, string> mimeTypes = new Dictionary<string, string>
        {
            {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {".csv", "application/text"}
        };

        public static ExportFileInfo GetFile(string fileName)
        {
            var fileExt = Path.GetExtension(fileName);
            string mimeType = null;
            if (fileExt == null || !mimeTypes.TryGetValue(fileExt, out mimeType))
            {
                mimeType = "application/text";
            }

            var tempFolder = Settings.Default.ExportTempFolder;
            var fullPath = Path.Combine(tempFolder, fileName);
            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);

            return new ExportFileInfo {FileStream = stream, MimeType = mimeType};
        }

        public static string ToExcel(string dataName, Type gridModelType, JqGridResponse gridData)
        {
            var fileName = CreateFileName(dataName, "xlsx");
            var tempFolder = Settings.Default.ExportTempFolder;
            var path = Path.Combine(tempFolder, fileName);

            var document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            var workbookpart = document.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData(InsertData(gridModelType, gridData)));

            // Add Sheets to the Workbook.
            var sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            var sheet = new Sheet()
            {
                Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = dataName
            };
            sheets.Append(sheet);

            workbookpart.Workbook.Save();

            // Close the document.
            document.Close();

            return fileName;
        }

        public static string ToExcel1(string dataName, Type gridModelType, JqGridResponse gridData)
        {
            var fileName = CreateFileName(dataName, "xlsx");
            var tempFolder = Settings.Default.ExportTempFolder;
            var path = Path.Combine(tempFolder, fileName);

            using (var xl = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
            {
                //create workbook part
                var workbookPart = xl.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                var sheets = workbookPart.Workbook.AppendChild(new Sheets());

                //create worksheet part, and add it to the sheets collection in workbook
                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                //worksheetPart.Worksheet = new Worksheet(new SheetData());

                // Append a new worksheet and associate it with the workbook.
                var sheet = new Sheet()
                {
                    Id = xl.WorkbookPart.GetIdOfPart(worksheetPart), 
                    SheetId = 1, 
                    Name = dataName
                };
                sheets.Append(sheet);

                var writer = OpenXmlWriter.Create(worksheetPart);

                var headers = GetHeaders(gridModelType);
                writer.WriteStartElement(new Worksheet());
                writer.WriteStartElement(new SheetData());

                WriteRow(writer, headers);
                WriteGridRows(writer, gridData.Records);

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Close();

                workbookPart.Workbook.Save();
                xl.Close();
            }

            return fileName;
        }

        public static void WriteGridRows(OpenXmlWriter writer, IEnumerable<JqGridRecord> gridRecords)
        {
            foreach (var gridRecord in gridRecords)
            {
                WriteRow(writer, gridRecord.Values);
            }
        }

        private static void WriteRow(OpenXmlWriter writer, IEnumerable<object> cellValues)
        {
            writer.WriteStartElement(new Row());

            foreach (var cellValue in cellValues)
            {
                var val = new CellValue(Convert.ToString(cellValue));
                var cell = new Cell(){CellValue = val, DataType = CellValues.String};
                writer.WriteElement(cell);
            }

            writer.WriteEndElement();
        }

        private static IEnumerable<Row> InsertData(Type gridModelType, JqGridResponse gridData)
        {
            var result = new List<Row>();
            var headers = GetHeaders(gridModelType);

            result.Add(new Row(headers.Select(
                    x => new Cell
                    {
                        CellValue = new CellValue(x), 
                        DataType = CellValues.SharedString
                    })));
            result.AddRange(gridData.Records.Select(
                    x => new Row(x.Values.Select(
                        y => new Cell
                        {
                            CellValue = new CellValue(Convert.ToString(y)),
                            DataType = CellValues.SharedString
                        }))));

            return result;
        }

        public static string ToCSV(string dataName, Type gridModelType, JqGridResponse gridData)
        {
            var fileName = CreateFileName(dataName, "csv");
            var tempFolder = Settings.Default.ExportTempFolder;
            var path = Path.Combine(tempFolder, fileName);

            var gridHeaders = GetHeaders(gridModelType);
            using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine(string.Join(",", gridHeaders));
                foreach (var record  in gridData.Records)
                {
                    writer.WriteLine(string.Join(",", record.Values));
                }

                writer.Flush();
            }

            return fileName;
        }

        private static IEnumerable<string> GetHeaders(Type gridModelType)
        {
            var result = new List<string>();
            var props = gridModelType.GetProperties();

            foreach (var pi in props)
            {
                var displayAttr = (DisplayAttribute)pi.GetCustomAttribute(typeof (DisplayAttribute));
                if (displayAttr != null)
                {
                    result.Add(displayAttr.GetName());
                }
            }
            return result;
        }

        private static string CreateFileName(string dataName, string ext)
        {
            var fileName = string.Format("{0}_{1}_{2}.{3}", dataName, SecurityHelper.GetLoggedUserId(),
                DateTime.Now.ToString("yyyyMMdd_HHmmss"), ext);
            return fileName;
        }
    }
}