using SoftTehnica.MvcReportViewer.ReportService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SoftTehnica.MvcReportViewer
{
    public static class ReportServiceHelpers
    {
        private static ReportService.ReportingService2010 _initializeReportingService(ReportViewerModel model)
        {
            var service = new ReportService.ReportingService2010();
            service.Url = model.ServerUrl + ((model.ServerUrl.ToSafeString().EndsWith("/")) ? "" : "/") + "ReportService2010.asmx";
            service.Credentials = model.Credentials ?? System.Net.CredentialCache.DefaultCredentials;
            if (model.Timeout.HasValue)
            {
                service.Timeout = model.Timeout.Value;
            }

            return service;
        }

        private static ReportServiceExecution.ReportExecutionService _initializeReportExecutionService(ReportViewerModel model)
        {
            var service = new ReportServiceExecution.ReportExecutionService();
            service.Url = model.ServerUrl + ((model.ServerUrl.ToSafeString().EndsWith("/")) ? "" : "/") + "ReportExecution2005.asmx";
            service.Credentials = model.Credentials;
            if (model.Timeout.HasValue)
            {
                service.Timeout = model.Timeout.Value;
            }

            return service;
        }

        public static ReportService.ItemParameter[] GetReportParameters(ReportViewerModel model, bool forRendering = false)
        {
            var service = _initializeReportingService(model);

            string historyID = null;
            ReportService.ParameterValue[] values = null;
            ReportService.DataSourceCredentials[] rsCredentials = null;
            service.Credentials = model.Credentials;
            ReportService.ItemParameter[] parameters = null;
            try
            {
                parameters = service.GetItemParameters(model.ReportPath, historyID, false, values, rsCredentials);    //set it to load the not for rendering so that it's hopefully quicker than the whole regular call
            }
            catch (Exception ex)
            {

            }
            // Go through the parameters that have been provided to the model so we can load the real values for any cascading parameter values
            if (model != null && model.Parameters != null && model.Parameters.Any())
            {
                var tempParameters = new List<ReportService.ParameterValue>();
                foreach (var parameter in parameters)
                {
                    if (model.Parameters.Any(s => s.Key == parameter.Name) && parameter.PromptUser)
                    {
                        var providedParameter = model.Parameters.FirstOrDefault(s => s.Key == parameter.Name);
                        if (providedParameter != null)
                        {
                            foreach (var value in providedParameter.Values.Where(x => !String.IsNullOrEmpty(x)))
                            {
                                tempParameters.Add(new ReportService.ParameterValue()
                                {
                                    Label = parameter.Name,
                                    Name = parameter.Name,
                                    Value = value
                                });
                            }
                        }
                    }
                }

                values = tempParameters.ToArray();
            }
            try
            {

                parameters = service.GetItemParameters(model.ReportPath, historyID, forRendering, values, rsCredentials);
            }
            catch (Exception ex)
            {

            }

            return parameters;
        }

        public static ReportExportResult ExportReportToFormat(ReportViewerModel model, ReportFormats format, int? startPage = 0, int? endPage = 0, bool bindData = false)
        {
            return ExportReportToFormat(model, format.GetName(), startPage, endPage, bindData);
        }

        public static ReportExportResult ExportReportToFormat(ReportViewerModel model, string format, int? startPage = 0, int? endPage = 0, bool bindData = false)
        {
            var service = _initializeReportExecutionService(model);
            var serviceReporting = _initializeReportingService(model);

            if (!string.IsNullOrEmpty(model.DataSourceDefinition))
            {
                var dataSourceDefinition = serviceReporting.GetDataSourceContents(model.DataSourceDefinition);
                dataSourceDefinition.ConnectString = model.ConnectString;
                dataSourceDefinition.UserName = model.UserName;
                dataSourceDefinition.Password = model.Password;
                serviceReporting.SetDataSourceContents(model.DataSourceDefinition, dataSourceDefinition);
            }

            ReportService.DataSourceCredentials[] rsCredentials = null;

            var definedReportParameters = GetReportParameters(model, true);

            var exportResult = new ReportExportResult();
            exportResult.CurrentPage = (startPage.ToInt32() <= 0 ? 1 : startPage.ToInt32());
            exportResult.SetParameters(definedReportParameters, model.Parameters);

            if (startPage == 0)
            {
                startPage = 1;
            }

            if (endPage == 0)
            {
                endPage = startPage;
            }

            var outputFormat = $"<OutputFormat>{format}</OutputFormat>";
            var encodingFormat = $"<Encoding>{model.Encoding.EncodingName}</Encoding>";
            var htmlFragment = ((format.ToUpper() == "HTML4.0" && model.UseCustomReportImagePath == false && model.ViewMode == ReportViewModes.View) ? "<HTMLFragment>false</HTMLFragment>" : "");
            var deviceInfo = $"<DeviceInfo>{outputFormat}<Toolbar>False</Toolbar>{htmlFragment}</DeviceInfo>";
            if (model.ViewMode == ReportViewModes.View && startPage.HasValue && startPage > 0)
            {
                if (model.EnablePaging)
                {
                    deviceInfo = $"<DeviceInfo>{outputFormat}{encodingFormat}<Toolbar>False</Toolbar>{htmlFragment}<Section>{startPage}</Section></DeviceInfo>";
                }
                else
                {
                    deviceInfo = $"<DeviceInfo>{outputFormat}{encodingFormat}<Toolbar>False</Toolbar>{htmlFragment}</DeviceInfo>";
                }
            }

            var reportParameters = new List<ReportServiceExecution.ParameterValue>();
            foreach (var parameter in exportResult.Parameters)
            {
                if (parameter.PromptUser)
                {
                    bool addedParameter = false;
                    foreach (var value in parameter.SelectedValues)
                    {
                        var reportParameter = new ReportServiceExecution.ParameterValue();
                        reportParameter.Name = parameter.Name;
                        reportParameter.Value = value;
                        reportParameters.Add(reportParameter);
                        addedParameter = true;
                    }

                    if (!addedParameter)
                    {
                        var reportParameter = new ReportServiceExecution.ParameterValue();
                        reportParameter.Name = parameter.Name;
                        reportParameters.Add(reportParameter);
                    }
                }
            }

            var executionHeader = new ReportServiceExecution.ExecutionHeader();
            service.ExecutionHeaderValue = executionHeader;

            ReportServiceExecution.ExecutionInfo executionInfo = null;
            string extension = null;
            string encoding = null;
            string mimeType = null;
            string[] streamIDs = null;
            ReportServiceExecution.Warning[] warnings = null;

            ReportService.Warning[] warnings2 = null;

            try
            {
                string historyID = null;
                executionInfo = service.LoadReport(model.ReportPath, string.IsNullOrEmpty(model.HistoryId) ? null : model.HistoryId);
                if (bindData)
                {
                    service.SetExecutionParameters(reportParameters.ToArray(), "en-us");

                    if (string.IsNullOrEmpty(model.HistoryId))
                    {
                        try
                        {
                            var itemParameters = serviceReporting.GetItemParameters(model.ReportPath, historyID, true, reportParameters.Select(s => new SoftTehnica.MvcReportViewer.ReportService.ParameterValue { Name = s.Name, Value = s.Value, Label = s.Label }).ToArray(), rsCredentials);
                            serviceReporting.SetItemParameters(model.ReportPath, itemParameters);
                            var t = serviceReporting.CreateItemHistorySnapshot(model.ReportPath, out warnings2);
                            model.HistoryId = t;
                        }
                        catch (Exception ex)
                        {

                        };
                    }

                    if (model.EnablePaging)
                    {
                        byte[] result = null;

                        result = service.Render2(format, deviceInfo, ReportServiceExecution.PageCountMode.Actual, out extension, out mimeType, out encoding, out warnings, out streamIDs);

                        if (model.Parameters.Any(s => s.Key == "togleKey"))
                        {
                            string[] parts = model.Parameters.FirstOrDefault(s => s.Key == "togleKey").Values[0].Split('|');

                            var newParts = parts.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                            foreach (var part in newParts.Distinct())
                            {
                                if (service.ToggleItem(part))
                                {
                                    result = service.Render2(format, deviceInfo, ReportServiceExecution.PageCountMode.Actual, out extension, out mimeType, out encoding, out warnings, out streamIDs);
                                }
                                else
                                {
                                    newParts = newParts.Where(x => x != part).ToArray();
                                }
                            }

                            if (newParts != null && newParts.Length > 0)
                            {
                                model.Parameters.FirstOrDefault(s => s.Key == "togleKey").Values[0] = string.Join("|", newParts);
                            }
                            else
                            {
                                model.Parameters.FirstOrDefault(s => s.Key == "togleKey").Values[0] = string.Empty;
                            }
                        }
                        exportResult.ReportData = result;
                    }
                    else
                    {
                        var result = service.Render(format, deviceInfo, out extension, out mimeType, out encoding, out warnings, out streamIDs);

                        exportResult.ReportData = result;
                    }


                }

                executionInfo = service.GetExecutionInfo();
            }
            catch (Exception ex)
            {
                executionInfo = service.GetExecutionInfo();
                //exportResult.ReportData = HtmlHelperExtensions.StringToBytesConverted(ex.Message);
                Console.WriteLine(ex.Message);
            }

            exportResult.ExecutionInfo = executionInfo;
            exportResult.Format = format;
            exportResult.MimeType = mimeType;
            exportResult.StreamIDs = (streamIDs == null ? new List<string>() : streamIDs.ToList());
            exportResult.Warnings = (warnings == null ? new List<ReportServiceExecution.Warning>() : warnings.ToList());

            if (executionInfo != null)
            {
                exportResult.TotalPages = executionInfo.NumPages;
            }

            return exportResult;
        }

        /// <summary>
        /// Searches a specific report for your provided searchText and returns the page that it located the text on.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="searchText">The text that you want to search in the report</param>
        /// <param name="startPage">Starting page for the search to begin from.</param>
        /// <returns></returns>
        public static int? FindStringInReport(ReportViewerModel model, string searchText, int? startPage = 0)
        {
            var service = _initializeReportExecutionService(model);

            var definedReportParameters = GetReportParameters(model, true);

            if (!startPage.HasValue || startPage == 0)
            {
                startPage = 1;
            }

            var exportResult = new ReportExportResult();
            exportResult.CurrentPage = startPage.ToInt32();
            exportResult.SetParameters(definedReportParameters, model.Parameters);

            var format = "HTML4.0";
            var outputFormat = $"<OutputFormat>{format}</OutputFormat>";
            var encodingFormat = $"<Encoding>{model.Encoding.EncodingName}</Encoding>";
            var htmlFragment = ((format.ToUpper() == "HTML4.0" && model.UseCustomReportImagePath == false && model.ViewMode == ReportViewModes.View) ? "<HTMLFragment>true</HTMLFragment>" : "");
            var deviceInfo = $"<DeviceInfo>{outputFormat}<Toolbar>False</Toolbar>{htmlFragment}</DeviceInfo>";
            if (model.ViewMode == ReportViewModes.View && startPage.HasValue && startPage > 0)
            {
                deviceInfo = $"<DeviceInfo>{outputFormat}{encodingFormat}<Toolbar>False</Toolbar>{htmlFragment}<Section>{startPage}</Section></DeviceInfo>";
            }

            var reportParameters = new List<ReportServiceExecution.ParameterValue>();
            foreach (var parameter in exportResult.Parameters)
            {
                bool addedParameter = false;
                foreach (var value in parameter.SelectedValues)
                {
                    var reportParameter = new ReportServiceExecution.ParameterValue();
                    reportParameter.Name = parameter.Name;
                    reportParameter.Value = value;
                    reportParameters.Add(reportParameter);

                    addedParameter = true;
                }

                if (!addedParameter)
                {
                    var reportParameter = new ReportServiceExecution.ParameterValue();
                    reportParameter.Name = parameter.Name;
                    reportParameters.Add(reportParameter);
                }
            }

            var executionHeader = new ReportServiceExecution.ExecutionHeader();
            service.ExecutionHeaderValue = executionHeader;

            ReportServiceExecution.ExecutionInfo executionInfo = null;
            string extension = null;
            string encoding = null;
            string mimeType = null;
            string[] streamIDs = null;
            ReportServiceExecution.Warning[] warnings = null;

            try
            {
                string historyID = null;
                executionInfo = service.LoadReport(model.ReportPath, historyID);
                service.SetExecutionParameters(reportParameters.ToArray(), "en-us");

                var result = service.Render2(format, deviceInfo, ReportServiceExecution.PageCountMode.Actual, out extension, out mimeType, out encoding, out warnings, out streamIDs);

                executionInfo = service.GetExecutionInfo();

                return service.FindString(startPage.ToInt32(), executionInfo.NumPages, searchText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return 0;
        }

        /// <summary>
        /// I'm using this method to run images through a "proxy" on the local site due to credentials used on the report being different than the currently running user.
        /// I ran into issues where my domain account was different than the user that executed the report so the images gave 500 errors from the website. Also my report server
        /// is only internally available so this solved that issue for me as well.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="reportContent">This is the raw html output of your report.</param>
        /// <returns></returns>
        public static string ReplaceImageUrls(ReportViewerModel model, string reportContent)
        {
            var reportServerDomainUri = new Uri(model.ServerUrl);
            var searchForUrl = $"SRC=\"{reportServerDomainUri.Scheme}://{reportServerDomainUri.DnsSafeHost}/";
            //replace image urls with image data instead due to having issues accessing the images as a different authenticated user
            var imagePathIndex = reportContent.IndexOf(searchForUrl);
            while (imagePathIndex > -1)
            {
                var endIndex = reportContent.IndexOf("\"", imagePathIndex + 5);   //account for the length of src="
                if (endIndex > -1)
                {
                    var imageUrl = reportContent.Substring(imagePathIndex + 5, endIndex - (imagePathIndex + 5));
                    reportContent = reportContent.Replace(imageUrl, $"{String.Format(model.ReportImagePath, imageUrl)}");
                }

                imagePathIndex = reportContent.IndexOf(searchForUrl, imagePathIndex + 5);
            }

            return reportContent;
        }
    }
}