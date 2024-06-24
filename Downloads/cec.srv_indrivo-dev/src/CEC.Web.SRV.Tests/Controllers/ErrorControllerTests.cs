using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.Web.SRV.Controllers;
using Moq;

namespace CEC.Web.SRV.Tests.Controllers
{
    [TestClass]
    public class ErrorControllerTests
    {
        private static ErrorController _errorController;
        
        [TestInitialize]
        public void Startup()
        {
            var mockHttpContext = new Mock<HttpContextBase>();
            var mockResponse = new Mock<HttpResponseBase>();
            
            mockHttpContext.SetupGet(x => x.Response).Returns(mockResponse.Object);

            _errorController = new ErrorController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };
        }

        [TestMethod]
        public void NotFound_returns_correct_view()
        {
            // Arrange
            const string errorPath = "ErrorPath";
            
            // Act
            var result = _errorController.NotFound(errorPath);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void NotFound_has_correct_model_error_path()
        {
            // Arrange
            const string errorPath = "ErrorPath";
            
            // Act
            var result = _errorController.NotFound(errorPath);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(HandleErrorInfo));
            Assert.IsNotNull(((HandleErrorInfo)result.Model).Exception);
            Assert.AreEqual(((HandleErrorInfo)result.Model).Exception.Source, errorPath);
        }

        [TestMethod]
        public void NotFound_has_valid_model()
        {
            // Arrange
            const string errorPath = "ErrorPath";

            // Act
            var result = _errorController.NotFound(errorPath);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_errorController.ViewData.ModelState.IsValid);
        }
    }
}
