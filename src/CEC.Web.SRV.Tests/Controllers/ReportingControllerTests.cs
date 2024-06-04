using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CEC.SRV.BLL;
using CEC.Web.SRV.Models.Reporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.Web.SRV.Controllers;
using AutoMapper;
using Moq;
using CEC.Web.SRV.Profiles;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.Web.SRV.Tests.Controllers
{
    [TestClass]
    public class ReportingControllerTests : BaseControllerTests
    {
        private static Mock<IElectionBll> _electionBll;
        private static Mock<IPollingStationBll> _pollingStationBll;
		private static Mock<ILookupBll> _lookupBll;
        private static ReportingController _controller;
        private static List<Election> _elections;
        private static List<PollingStation> _pollingStations; 
        
        public ReportingControllerTests()
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
            _electionBll = new Mock<IElectionBll>();
            _pollingStationBll = new Mock<IPollingStationBll>();
            _lookupBll = new Mock<ILookupBll>();
			_controller = new ReportingController(_electionBll.Object, _pollingStationBll.Object, _lookupBll.Object);
            //BaseController = _controller;
        }

        [TestMethod]
        public void ListPrinting_returns_correct_view()
        {
            // Arrange

            ArrangeElectionsAndPollingStationsMocks();

            // Act

            var result = _controller.ListPrinting() as ViewResult;

            // Assert
            
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ListPrinting_Model_returns_correct_view()
        {
            // Arrange

            var model = GetModel();

            ArrangeElectionAndPollingStationMocks(model);
            ArrangeElectionsAndPollingStationsMocks();

            // Act

            var result = _controller.ListPrinting(model) as ViewResult;

            // Assert

            Assert.IsNotNull(result);
        }
        
        private static void ArrangeElectionsAndPollingStationsMocks()
        {
            _elections = GetElections();
            _pollingStations = GetPollingStations();

            _electionBll.Setup(x => x.GetActiveElections()).Returns(_elections);
            _pollingStationBll.Setup(x => x.GetAccessiblePollingStations()).Returns(_pollingStations);
        }

        private static void ArrangeElectionAndPollingStationMocks(ListPrintingModel model)
        {
            var election = GetElection0();
            var pollingStation = GetPollingStation0();

            _electionBll.Setup(x => x.Get<Election>(model.ElectionId)).Returns(election);
            _electionBll.Setup(x => x.Get<PollingStation>(model.PollingStationId)).Returns(pollingStation);
        }

        private static ListPrintingModel GetModel()
        {
            var model = new ListPrintingModel
            {
                ElectionDate = new DateTime(2014, 11, 30),
                ElectionId = 1,
                ElectionTitleRO = "Title_RO",
                ElectionTitleRU = "Title_RU",
                ManagerName = "Andrei Andriescu",
                ManagerTypeName = "Primar",
                PollingStationId = 1,
                PollingStationNr = "11",
                RegionName = "Chisinau"
            };

            return model;
        }

        private static List<Election> GetElections()
        {
            var elections = new List<Election>
            {
                GetElection0(),
                GetElection1()
            };
            
            return elections;
        }

        private static Election GetElection0()
        {
            return new Election
            {
                //AcceptAbroadDeclaration = true,
                //Comments = "Comentarii",
                //ElectionDate = new DateTime(2014, 11, 30),
                //ElectionType = new ElectionType
                //{
                //    Description = "Alegeri parlamentare in RM",
                //    Name = "Alegeri parlamentare"
                //},
                //SaiseId = null
            };
        }

        private static Election GetElection1()
        {
            return new Election
            {
                //AcceptAbroadDeclaration = false,
                //Comments = "Comentarii",
                //ElectionDate = new DateTime(2014, 12, 30),
                //ElectionType = new ElectionType
                //{
                //    Description = "Alegeri prezidentiale in RM",
                //    Name = "Alegeri prezidentiale"
                //},
                //SaiseId = null
            };
        }

        private static List<PollingStation> GetPollingStations()
        {
            var pollingStations = new List<PollingStation>
            {
                GetPollingStation0(),
                GetPollingStation1()
            };

            return pollingStations;
        }

        private static PollingStation GetPollingStation0()
        {
            var region = new Region(
                new RegionType
                {
                    Description = "capitala RM",
                    Name = "Chisinau",
                    Rank = 1
                });

            var geoLocation = new GeoLocation {Latitude = 40, Longitude = 20};

            return new PollingStation(region)
            {
                ContactInfo = "a@a.a",
                GeoLocation = geoLocation,
                Location = "Casa de Cultura, CENTRU",
                SaiseId = null,
                Number = "11",
                PollingStationAddress = new Address
                {
                    BuildingType = BuildingTypes.Administrative,
                    GeoLocation = geoLocation,
                    HouseNumber = 12,
                    PollingStation = null,
                    Street = new Street(
                        region,
                        new StreetType
                        {
                            Description = "strada noua, cu pavaj de ultima generatie",
                            Name = "alee"
                        },
                        "V. Alecsandri",
                        true)
                    {
                        Description = "The best street",
                        RopId = null,
                        SaiseId = null
                    }
                },
                SubNumber = "1"
            };
        }

        private static PollingStation GetPollingStation1()
        {
            var region = new Region(
               new RegionType
               {
                   Description = "capitala RM",
                   Name = "Chisinau",
                   Rank = 1
               });

            var geoLocation = new GeoLocation {Latitude = 20, Longitude = 40};

            return new PollingStation(region)
            {
                ContactInfo = "b@b.b",
                GeoLocation = geoLocation,
                Location = "Casa de Cultura, BOTANICA",
                SaiseId = null,
                Number = "11",
                PollingStationAddress = new Address
                {
                    BuildingType = BuildingTypes.Administrative,
                    GeoLocation = geoLocation,
                    HouseNumber = 12,
                    PollingStation = null,
                    Street = new Street(
                        region,
                        new StreetType
                        {
                            Description = "strada veche",
                            Name = "bulevard"
                        },
                        "Lermontov",
                        true)
                    {
                        Description = "strada veche",
                        RopId = null,
                        SaiseId = null
                    }
                },
                SubNumber = "2"
            };
        }
    }
}

