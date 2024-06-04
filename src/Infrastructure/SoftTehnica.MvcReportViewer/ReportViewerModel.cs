using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftTehnica.MvcReportViewer
{
    public class ReportViewerModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string ServerUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ReportPath { get; set; }


        public string DataSourceDefinition { get; set; }

        public string ConnectString { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// This indicates whether or not to replace image urls from your report server to image urls on your local site to act as a proxy
        /// *useful if your report server is not accessible publicly*
        /// </summary>
        public bool UseCustomReportImagePath { get; set; }

        /// <summary>
        /// This is the local URL on your website that will handle returning images for you, be sure to use the replacement variable {0} in your string to represent the original image URL that came from your report server.
        /// </summary>
        public string ReportImagePath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Net.ICredentials Credentials { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ReportParameter> Parameters { get; set; }

        public bool ShowHiddenParameters { get; set; }

        public ReportViewModes ViewMode { get; set; }
        public System.Text.Encoding Encoding { get; set; }

        /// <summary>
        /// This indicates whether or not the report will be preloaded when the page loads initially or if it will be an ajax request.
        /// </summary>
        public bool AjaxLoadInitialReport { get; set; }

        /// <summary>
        /// Setting this to 'true' enables the paging control and renders a single page at a time. Setting this to 'false' removes the paging control and shows all pages at once.
        /// </summary>
        public bool EnablePaging { get; set; }
        public int? Timeout { get; set; }

        public ReportViewerModel()
        {
            this.Parameters = new List<ReportParameter>();
            this.ViewMode = ReportViewModes.View;
        }

        public void AddParameter(ReportParameter param)
        {
            if (!param.Key.HasValue()) { return; }

            if (this.Parameters.Any(s => s.Key == param.Key))
            {
                this.Parameters.FirstOrDefault(s => s.Key == param.Key).Values = param.Values;
            }
            else
            {
                this.Parameters.Add(param);
            }
        }

        private static List<string> KEYS_TO_IGNORE = new List<string>() { "ReportViewerEnablePaging", "reportPath", "startPage", "searchText", "page", "format" };
        private static string[] VALUE_SEPARATORS = new string[] { "," };

        public void BuildParameters(HttpRequestBase request)
        {
            foreach (var key in request.QueryString.AllKeys.Where(x => !KEYS_TO_IGNORE.Contains(x)))
            {
                this.AddParameter(new ReportParameter(key, request.QueryString[key].ToSafeString().ToStringList(VALUE_SEPARATORS).ToArray()));
            }

            foreach (var key in request.Form.AllKeys.Where(x => !KEYS_TO_IGNORE.Contains(x)))
            {
                this.AddParameter(new ReportParameter(key, request.Form[key].ToSafeString().ToStringList(VALUE_SEPARATORS).ToArray()));
            }
        }

        public Dictionary<string, string> IsMissingAnyRequiredParameterValues(List<ReportParameterInfo> parameters)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            var nonBlankParameters = parameters.Where(x => !x.AllowBlank && !x.Nullable);
            var matchedParameters = this.Parameters.Where(x => nonBlankParameters.Select(p => p.Name).Contains(x.Key));
            var missingValueParameters = matchedParameters.Where(x => x.Values == null || x.Values.Length == 0 || (x.Values.Where(v => v == null || v == String.Empty)).Any());

            foreach (var missingValueParameter in missingValueParameters)
            {
                result.Add(missingValueParameter.Key, nonBlankParameters.FirstOrDefault(s => s.Name == missingValueParameter.Key).Prompt);
            }


            return result;
        }

        public string HistoryId { get; set; }
    }
}