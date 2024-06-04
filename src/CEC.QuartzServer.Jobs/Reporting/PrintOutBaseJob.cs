using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Amdaris;
using CEC.QuartzServer.Core;
using CEC.QuartzServer.Jobs.Common;

namespace CEC.QuartzServer.Jobs.Reporting
{
    public abstract class PrintOutBaseJob : SrvJob
    {
        private readonly ILogger _logger;
        private readonly string _reportServer;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _ignorableDistrictNumbers;

        protected PrintOutBaseJob(ILogger logger, IConfigurationSettingManager configurationSettingManager)
        {
            _logger = logger;

            _reportServer = configurationSettingManager.Get("SSRS_ReportExecutionService").Value;
            _ignorableDistrictNumbers = configurationSettingManager.Get("IgnorableDistrictsNumbers").Value;
            _userName = configurationSettingManager.Get("SSRS_UserName").Value;
            _password = configurationSettingManager.Get("SSRS_Password").Value;
            var pathToExportFolder = configurationSettingManager.Get("ListPrintingJob_ExportPath").Value;
            RootExportDirectory = new DirectoryInfo(pathToExportFolder);
            TryParseAndSetIgnorableDistrictsNumbers();
        }

        public ILogger Logger 
        {
            get { return _logger; }
        }

        public int[] IgnorableDistrictNumbers { get; private set; }

        public string ReportServerUri
        {
            get { return _reportServer; }
        }

        public string ReportPageHeight { get; protected set; }

        public DirectoryInfo RootExportDirectory { get; private set; }

        public NetworkCredential GetCredentials()
        {
            return new NetworkCredential(_userName, _password);
        }

        protected void TryParseAndSetIgnorableDistrictsNumbers()
        {
            if (string.IsNullOrWhiteSpace(_ignorableDistrictNumbers))
            {
                IgnorableDistrictNumbers = new int[0];
                return;
            }

            var splited = _ignorableDistrictNumbers.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var list = new List<int>();
            foreach (var s in splited)
            {
                int result;
                if (int.TryParse(s, out result))
                {
                    list.Add(result);
                }
                else
                {
                    throw new Exception(string.Format("Can't parse '{0}' to int", s));
                }
            }

            IgnorableDistrictNumbers = list.ToArray();
        }

        protected void WriteToFile(string path, string fileName, XDocument xdoc)
        {
            using (var fs = new FileStream(string.Format("{0}\\{1}", path, fileName), FileMode.Create, FileAccess.Write))
            {
                XmlWriter xwriter = XmlWriter.Create(fs, new XmlWriterSettings { NewLineChars = "\n", Indent = true });
                xdoc.Save(xwriter);
                xwriter.Flush();
                xwriter.Close();
            }
        }

        protected XDocument CreateXDocForSiteExport<T>(IEnumerable<T> list, RefAction<T, string, string> action) where T : class
        {
            var xdoc = new XDocument();
            var root = new XElement("dropdown");
            foreach (T item in list)
            {
                string p1 = null;
                string p2 = null;

                action(item, out p1, out p2);

                root.Add(
                    new XElement("values",
                        new XElement("id", p1),
                        new XElement("text", p2)));
            }

            xdoc.Add(root);

            return xdoc;
        }

        protected void LogRequestException(Uri requestUri, WebException webException)
        {
            using (var ms = new MemoryStream())
            {
                if (webException.Response != null)
                {
                    var responseStream = webException.Response.GetResponseStream();
                    if (responseStream != null)
                    {
                        responseStream.CopyTo(ms);
                    }
                }
                ms.Position = 0;
                var content = Encoding.Default.GetString(ms.ToArray());
                Logger.Debug(string.Format("Request throw error {0}", requestUri));
                Logger.Error(webException, "\r\nDetails: " + content);
            }

        }
    }

    public delegate void RefAction<in T1, T2, T3>(T1 p1, out T2 p2, out T3 p3);
}
