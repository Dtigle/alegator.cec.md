using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.Web.SRV.Controllers;
using AutoMapper;
using Moq;
using CEC.Web.SRV.Profiles;
using Lib.Web.Mvc.JQuery.JqGrid;
using Amdaris.Domain.Paging;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.Web.SRV.Tests.Controllers
{
    [TestClass]
    public class StatisticsControllerTests : BaseControllerTests
    {
        private static Mock<IStatisticsBll> _bll;
        private static StatisticsController _controller;
  
        public StatisticsControllerTests()
        {
            Mapper.Initialize(arg =>
            {
                arg.AddProfile<IdentityUserProfile>();
                arg.AddProfile<LookupProfile>();
                arg.AddProfile<SrvGridModelsProfile>();
                arg.AddProfile<HistoryProfile>();
            });
        }

        [TestInitialize]
        public void Startup()
        {
            _bll = new Mock<IStatisticsBll>();
            _controller = new StatisticsController(_bll.Object);
            BaseController = _controller;
        }

        [TestMethod]
        public void LoadInfoTab_returns_correct_view()
        {
            // Arrange

            const string viewName = "_InfoTab";

            // Act
            var result = _controller.LoadInfoTab() as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(viewName, result.ViewName);
        }

        [TestMethod]
        public void LoadAgeDistributionTab_returns_correct_view()
        {
            // Arrange

            const string viewName = "_AgeDistributionTab";

            // Act
            var result = _controller.LoadAgeDistributionTab() as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(viewName, result.ViewName);
        }

        [TestMethod]
        public void LoadProblematicDataTab_returns_correct_view()
        {
            // Arrange

            const string viewName = "_ProblematicDataTab";

            // Act
            var result = _controller.LoadProblematicDataTab() as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(viewName, result.ViewName);
        }

        [TestMethod]
        public void LoadStatisticsFromImportTab_returns_correct_view()
        {
            // Arrange

            const string viewName = "_StatisticsFromImportTab";

            // Act
            var result = _controller.LoadStatisticsFromImportTab() as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(viewName, result.ViewName);
        }

        [TestMethod]
        public void LoadStatisticsForPollingStation_returns_correct_view()
        {
            // Arrange

            const string viewName = "_StatisticsForPollingStationTab";

            // Act
            var result = _controller.LoadStatisticsForPollingStation() as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(viewName, result.ViewName);
        }

        [TestMethod]
        public void ListStatisticsPollingStationsAjax_returns_correct_format()
        {
            // Arrange

            var request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };

            _bll.Setup(x => x.GetStatisticsForPollingStation(It.IsAny<PageRequest>()))
                .Returns(new PageResponse<PollingStationStatistics> { Items = new List<PollingStationStatistics>(), PageSize = 20, StartIndex = 1, Total = 2 });

            // Act

            var result = _controller.ListStatisticsPollingStationsAjax(request);

            // Assert

            _bll.Verify(x => x.GetStatisticsForPollingStation(It.IsAny<PageRequest>()), Times.Once());
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(JqGridJsonResult));
        }

        [TestMethod]
        public void GetTotalPeople_returns_correct_view()
        {
            // Arrange

            const long peopleNumber = 100;

            _bll.Setup(x => x.GetTotalNumberOfPeople()).Returns(peopleNumber);

            // Act
            
            var result = _controller.GetTotalPeople();

            // Assert

            AssertJsonLongData(peopleNumber, result);
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleWithoutDeads_returns_correct_view()
        {
            // Arrange

            const long peopleNumber = 100;

            _bll.Setup(x => x.GetTotalNumberOfPeopleWithoutDeads()).Returns(peopleNumber);

            // Act

            var result = _controller.GetTotalNumberOfPeopleWithoutDeads();

            // Assert

            AssertJsonLongData(peopleNumber, result);
        }

        [TestMethod]
        public void GetTotalNumberOfVoters_returns_correct_view()
        {
            // Arrange

            const long votersNumber = 100;

            _bll.Setup(x => x.GetTotalNumberOfVoters()).Returns(votersNumber);

            // Act

            var result = _controller.GetTotalNumberOfVoters();

            // Assert

            AssertJsonLongData(votersNumber, result);
        }

        [TestMethod]
        public void GetTotalNumberOfStayStatementDeclarations_returns_correct_view()
        {
            // Arrange

            const long declarationsNumber = 100;

            _bll.Setup(x => x.GetTotalNumberOfStayStatementDeclarations()).Returns(declarationsNumber);

            // Act

            var result = _controller.GetTotalNumberOfStayStatementDeclarations();

            // Assert

            AssertJsonLongData(declarationsNumber, result);
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleByGenderFemale_returns_correct_view()
        {
            GetTotalNumberOfPeopleByGender_returns_correct_view(GenderTypes.Female);
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleByGenderMale_returns_correct_view()
        {
            GetTotalNumberOfPeopleByGender_returns_correct_view(GenderTypes.Male);
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleByGenderUnknown_returns_correct_view()
        {
            GetTotalNumberOfPeopleByGender_returns_correct_view(GenderTypes.Unknown);
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleWithDoBMissing_returns_correct_view()
        {
            // Arrange

            const long peopleWithDoBMissing = 100;

            _bll.Setup(x => x.GetTotalNumberOfPeopleWithDoBMissing()).Returns(peopleWithDoBMissing);

            // Act

            var result = _controller.GetTotalNumberOfPeopleWithDoBMissing();

            // Assert

            AssertJsonLongData(peopleWithDoBMissing, result);
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleWithDocMissing_returns_correct_view()
        {
            // Arrange

            const long peopleWithDoBMissing = 100;

            _bll.Setup(x => x.GetTotalNumberOfPeopleWithDocMissing()).Returns(peopleWithDoBMissing);

            // Act

            var result = _controller.GetTotalNumberOfPeopleWithDocMissing();

            // Assert

            AssertJsonLongData(peopleWithDoBMissing, result);
        }

        [TestMethod]
        public void GetTotalNumberOfPeopleWithDocExpired_returns_correct_view()
        {
            // Arrange

            const long peopleWithDoBMissing = 100;

            _bll.Setup(x => x.GetTotalNumberOfPeopleWithDocExpired()).Returns(peopleWithDoBMissing);

            // Act

            var result = _controller.GetTotalNumberOfPeopleWithDocExpired();

            // Assert

            AssertJsonLongData(peopleWithDoBMissing, result);
        }

        [TestMethod]
        public void GetNumberOfPeopleForAgeIntervals_returns_correct_view()
        {
            // Arrange

            _bll.Setup(x => x.GetNumberOfPeopleForAgeIntervals((int)AgeIntervals.AgeLessThen18)).Returns(0);
            _bll.Setup(x => x.GetNumberOfPeopleForAgeIntervals((int)AgeIntervals.AgeBetween18And40)).Returns(1);
            _bll.Setup(x => x.GetNumberOfPeopleForAgeIntervals((int)AgeIntervals.AgeBetween41And65)).Returns(2);
            _bll.Setup(x => x.GetNumberOfPeopleForAgeIntervals((int)AgeIntervals.AgeBetween66And75)).Returns(3);
            _bll.Setup(x => x.GetNumberOfPeopleForAgeIntervals((int)AgeIntervals.AgeBetween76And90)).Returns(4);
            _bll.Setup(x => x.GetNumberOfPeopleForAgeIntervals((int)AgeIntervals.AgeGreaterThen90)).Returns(5);
            
            // Act

            var result = _controller.GetNumberOfPeopleForAgeIntervals();

            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(JsonResult));

            var resultData = (((JsonResult) result).Data);

            Assert.IsNotNull(resultData);
            Assert.IsInstanceOfType(resultData, typeof(IEnumerable<dynamic>));
            
            var resultDataList = ((IEnumerable<dynamic>)resultData).ToList();

            var ageIntervalsArray = Enum.GetValues(typeof (AgeIntervals));

            Assert.IsNotNull(ageIntervalsArray);
            Assert.AreEqual(resultDataList.Count, ageIntervalsArray.Length);

            foreach (AgeIntervals ageIntervals in ageIntervalsArray)
            {
                var resultDataListElement = resultDataList[(int) ageIntervals];
                Assert.IsNotNull(resultDataListElement);

                object resultDataListElementValue = GetDynamicPropertyValue(resultDataListElement, "value");
                Assert.IsInstanceOfType(resultDataListElementValue, typeof(long));
                Assert.AreEqual((long)ageIntervals, (long)resultDataListElementValue);

                object resultDataListElementText = GetDynamicPropertyValue(resultDataListElement, "text");
                Assert.IsInstanceOfType(resultDataListElementText, typeof(string));
                Assert.AreEqual(ageIntervals.GetEnumDescriptions(), resultDataListElementText);
            }
        }
      
        [TestMethod]
        public void GetNewDeadPeople_returns_correct_view()
        {
            // Arrange

            const long newVoters = 100;

            _bll.Setup(x => x.GetNewDeadPeople()).Returns(newVoters);

            // Act

            var result = _controller.GetNewDeadPeople();

            // Assert

            AssertJsonLongData(newVoters, result);
        }

        [TestMethod]
        public void GetPersonalDataChanges_returns_correct_view()
        {
            // Arrange

            const long newVoters = 100;

            _bll.Setup(x => x.GetPersonalDataChanges()).Returns(newVoters);

            // Act

            var result = _controller.GetPersonalDataChanges();

            // Assert

            AssertJsonLongData(newVoters, result);
        }

        [TestMethod]
        public void GetAddressesChanges_returns_correct_view()
        {
            // Arrange

            const long newVoters = 100;

            _bll.Setup(x => x.GetAddressesChanges()).Returns(newVoters);

            // Act

            var result = _controller.GetAddressesChanges();

            // Assert

            AssertJsonLongData(newVoters, result);
        }

        [TestMethod]
        public void GetImportSuccessful_returns_correct_view()
        {
            // Arrange

            const long newVoters = 100;

            _bll.Setup(x => x.GetImportSuccessful()).Returns(newVoters);

            // Act

            var result = _controller.GetImportSuccessful();

            // Assert

            AssertJsonLongData(newVoters, result);
        }

        [TestMethod]
        public void GetImportFailed_returns_correct_view()
        {
            // Arrange

            const long newVoters = 100;

            _bll.Setup(x => x.GetImportFailed()).Returns(newVoters);

            // Act

            var result = _controller.GetImportFailed();

            // Assert

            AssertJsonLongData(newVoters, result);
        }
        
        private static object GetDynamicPropertyValue(dynamic obj, string property)
        {
            return obj.GetType().GetProperty(property).GetValue(obj, null);
        }

        private static void AssertJsonLongData(long expected, object result)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(JsonResult));

            Assert.AreEqual(expected, ((JsonResult)result).Data);

        }

        private static void GetTotalNumberOfPeopleByGender_returns_correct_view(GenderTypes genderTypes)
        {
            // Arrange

            const long peopleByGender = 100;

            _bll.Setup(x => x.GetTotalNumberOfPeopleByGender(genderTypes)).Returns(peopleByGender);

            // Act

            var result = _controller.GetTotalNumberOfPeopleByGender(genderTypes);

            // Assert

            AssertJsonLongData(peopleByGender, result);
        }
    }
}

