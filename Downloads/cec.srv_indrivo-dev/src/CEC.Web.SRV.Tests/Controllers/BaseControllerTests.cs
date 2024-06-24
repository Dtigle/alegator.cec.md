using System;
using System.Reflection;
using System.Security.Claims;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.Web.SRV.Controllers;
using CEC.Web.SRV.Properties;
using System.IO;
using Lib.Web.Mvc.JQuery.JqGrid;
using CEC.Web.SRV.Infrastructure.Export;
using Moq;
using System.Collections.Generic;

namespace CEC.Web.SRV.Tests.Controllers
{
    
    public class BaseControllerTests
    {
        public BaseController BaseController { get; set; }
        
        [TestMethod]
        public void GetFile_returns_correct_file_if_file_exists()
        {
            // Arrange

            const string fn = "aaa.txt";
            var tempFolder = Settings.Default.ExportTempFolder;
            var fullPath = Path.Combine(tempFolder, fn);

            var dirCreated = CreateDirectoryIfNotExists(tempFolder);
            var fileCreated = CreateFileIfNotExists(fullPath);

            // Pre Assert

            Assert.IsNotNull(BaseController);
            Assert.IsTrue(File.Exists(fullPath));
            
            // Act
            
            var result = BaseController.GetFile(fn) as FileStreamResult;

            // Assert
            
            Assert.IsNotNull(result);
            Assert.AreEqual(result.FileDownloadName, fn);

            // Post Assert Clean

            DeleteFileIfNeeds(fullPath, fileCreated);
            DeleteDirectoryIfNeeds(tempFolder, dirCreated);
        }
        
        [TestMethod]
        public void GetFile_returns_correct_file_attr_if_file_exists()
        {
            // Arrange

            const string fn = "aaa.txt";
            var tempFolder = Settings.Default.ExportTempFolder;
            var fullPath = Path.Combine(tempFolder, fn);

            var dirCreated = CreateDirectoryIfNotExists(tempFolder);
            var fileCreated = CreateFileIfNotExists(fullPath);

            // Pre Assert

            Assert.IsNotNull(BaseController);
            Assert.IsTrue(File.Exists(fullPath));

            // Act

            var result = BaseController.GetFile(fn) as FileStreamResult;

            // Assert
            
            Assert.IsNotNull(result);
            Assert.IsTrue(result.FileStream.CanRead);

            // Post Assert Clean

            DeleteFileIfNeeds(fullPath, fileCreated);
            DeleteDirectoryIfNeeds(tempFolder, dirCreated);
        }

        public void ExportGridDataTest<T>(ExportType type, string dataSetName)
        {
            // Arrange

            var request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };

            ClaimsPrincipal.Current.AddIdentity(
                new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, "value")
                    }));

            // Pre Assert

            Assert.IsNotNull(BaseController);

            const string url = "url";

            var urlMock = new Mock<UrlHelper>();
            urlMock.Setup(x => x.Action(It.IsAny<string>(), It.IsAny<object>())).Returns(url);

            BaseController.Url = urlMock.Object;

            // Act

            var result = BaseController.GetType().GetMethod("ExportGridData", BindingFlags.Instance | BindingFlags.NonPublic).
                Invoke(BaseController, new object[]
                {
                    request, 
                    type, 
                    dataSetName, 
                    typeof(T), 
                    new Func<JqGridRequest, JqGridJsonResult>(GetJqGridJsonResult)
                });

            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(JsonResult));

            Assert.AreEqual(url, ((JsonResult)result).Data);
          
        }

        private static bool CreateDirectoryIfNotExists(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                return true;
            }
            return false;
        }

        private static bool CreateFileIfNotExists(string fileName)
        {
            if (File.Exists(fileName)) return false;
            File.Create(fileName).Close();
            return true;
        }

        private static void DeleteDirectoryIfNeeds(string folder, bool needs)
        {
            if (needs && Directory.Exists(folder))
            {
               Directory.Delete(folder);
            }
        }

        private static void DeleteFileIfNeeds(string fileName, bool needs)
        {
            if (needs && File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        private static JqGridJsonResult GetJqGridJsonResult(JqGridRequest request)
        {
            var response = new JqGridResponse();

            response.Records.Add(new JqGridRecord("id", new List<object> { "x", "y", "z" }));

            var result = new JqGridJsonResult { Data = response };

            return result;
        }
      
    }
}
