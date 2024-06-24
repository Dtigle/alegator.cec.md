using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.Web.SRV.Controllers;

namespace CEC.Web.SRV.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests : BaseControllerTests
    {
        private static HomeController _controller;
        
        [TestInitialize]
        public void Startup()
        {
            _controller = new HomeController();
            BaseController = _controller;
        }

        [TestMethod]
        public void Index()
        {
            // Act

            var result = _controller.Index() as ViewResult;

            // Assert

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            
            // Act
            var result = _controller.About() as ViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Act
            var result = _controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
