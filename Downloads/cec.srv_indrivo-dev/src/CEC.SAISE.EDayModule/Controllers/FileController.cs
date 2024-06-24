using Amdaris;
using CEC.SAISE.BLL;
using CEC.SAISE.EDayModule.Models.File;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CEC.SAISE.EDayModule.Controllers
{
    [AllowAnonymous]
    public class FileController : BaseDataController
    {
        private readonly IConfigurationBll _configurationBll;
        private readonly IUserBll _userBll;
        private readonly ILogger _logger;
        private readonly IAuditEvents _auditEvents;
        private readonly IDocumentsBll _documentsBll;
        private readonly IMinIoBll _minIo;

        public FileController(IConfigurationBll configurationBll
            , IUserBll userBll, ILogger logger
            , IAuditEvents auditEvents, IDocumentsBll documentsBll, IMinIoBll minIo)

        {
            _configurationBll = configurationBll;
            _userBll = userBll;
            _logger = logger;
            _auditEvents = auditEvents;
            _documentsBll = documentsBll;
            _minIo = minIo;
        }

        [HttpPost]        
        public async Task<ActionResult> UploadPdf(HttpPostedFileBase file)//, DocumentSaveModel model)
        {

            DocumentSaveModel model = null;

            if (Request.Form["model"] != null)
            {
                string modelJson = Request.Form["model"];
                model = JsonConvert.DeserializeObject<DocumentSaveModel>(modelJson);
            }
            if (file != null && file.ContentLength > 0)
            {

                var documentName = await _documentsBll.CreateDocumentName(model.TemplateNameId, model.ElectionId, model.PollingStationId, model.CircumscriptionId);

                var documentIsUploaded = await _documentsBll.DocumentIsUploaded(model.DocumentId);
                if (!documentIsUploaded)
                {
                    string fileExtension = Path.GetExtension(file.FileName);
                    // Create a unique object name using a GUID and other strings
                    //string uniqueNamePart = Guid.NewGuid().ToString();
                    string additionalString = documentName;


                    string objectName = additionalString + fileExtension;

                    string contentType = file.ContentType;

                    using (Stream fileStream = file.InputStream)
                    {
                        string result = _minIo.UploadFile(objectName, fileStream, contentType);
                    }

                    await _documentsBll.SaveDocumentPath(model.DocumentId, documentName, file.ContentLength, file.ContentType);
                    return Json(new { Success = true, Message = "Document was uploaded with success" }); 

                }
                return Json(new { Success = false, Message = "Document already uploaded" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = "You have not specified a file." });
            }
        }
    }
}