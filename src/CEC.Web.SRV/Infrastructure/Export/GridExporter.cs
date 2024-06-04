using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using CEC.SRV.BLL;
using CEC.Web.SRV.Properties;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace CEC.Web.SRV.Infrastructure.Export
{
    public abstract class GridExporter : IDisposable
    {
        private readonly string _dataName;
        private readonly Type _gridModelType;

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

            return new ExportFileInfo { FileStream = stream, MimeType = mimeType };
        }

        protected string _fileName;

        protected GridExporter(string dataName, Type gridModelType)
        {
            _dataName = dataName;
            _gridModelType = gridModelType;
        }

        public abstract void Export(JqGridResponse gridData);

        public abstract void Close();

        public string FileName
        {
            get { return _fileName; }
        }

        public string DataName
        {
            get { return _dataName; }
        }

        public Type GridModelType
        {
            get { return _gridModelType; }
        }

        protected IEnumerable<string> GetHeaders(Type gridModelType)
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

        protected string CreateFileName(string dataName, string ext)
        {
            var fileName = string.Format("{0}_{1}_{2}.{3}", dataName, SecurityHelper.GetLoggedUserId(),
                DateTime.Now.ToString("yyyyMMdd_HHmmss"), ext);
            return fileName;
        }

        protected abstract void GetSupportedFileInfo(out string extension, out string mimeType);

        public void Dispose()
        {
            Close();
        }
    }
}