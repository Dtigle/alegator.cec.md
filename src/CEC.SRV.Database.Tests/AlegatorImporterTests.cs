using CEC.SRV.Domain.Importer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CEC.SRV.Database.Tests
{
    [TestClass]
    public class AlegatorImporterTests
    {
        private string[][] _listOfPaterns = new[]
        {
            new[] {null,    "XБМ698969",        "XБМ",      "698969"},
            new[] {null,    "XVБМ734831",       "XVБМ",     "734831"},
            new[] {null,    "XXБМ530811",       "XXБМ",     "530811"},
            new[] {"",      "XXXXIIБМ508114",   "XXXXIIБМ", "508114"},
            new[] {"",      "XXXVIБМ500302",    "XXXVIБМ",  "500302"},
            new[] {null,    "XVIIБМ733262",     "XVIIБМ",   "733262"},
            new[] {null,    "XVIIБМ7332622",     "XVIIБМ",   ""},
            new[] {null,    "XVIIБМ7332",       "XVIIБМ",   ""},
            new[] {"",      "A27089517",        "A",        "27089517"},
            new[] {null,    "B27089517",        "B",        "27089517"},
            new[] {null,    "23434234",         "",         ""},
            new[] {"A",     "12345678",         "A",        "12345678"},
            new[] {"A",     "123",              "A",        ""},    
            new[] {"A",     "123456789",        "A",        ""}    


        };

        private string[][] _housePatters = new[]
        {
            new [] {"34/", "34", null},
            new [] {"34/34", "34", null},
            new [] {"34/23", "34", "23"},
            new [] {"34", "34", null},
            new [] {"123A", "123", "A"},
            new [] {"111 A", "111", "A"},
            new [] {"327,1", "327", "1"},
            new [] {"201-B", "201", "B"},
            new [] {"116\\2", "116", "2"},
            new [] {"116\\a", "116", "a"},
            new [] {"341AA", "341", "AA"},
            new [] {"146-146A", "146", "A"},
            new [] {"3739/37-39", "3739", "37-39"}, 
            new [] {"3527/35/27", "3527", "35/27"}, 
            new [] {"1000/1000<", "1000", "<"},
            
 
        };

        [TestMethod]
        public void CanHandleNullInHouseNr()
        {
            var rawData = new AlegatorData() {HouseNr = null};
            Assert.IsFalse(rawData.GetHouseNumber().HasValue);
        }

        [TestMethod]
        public void CanHandleGreatThen()
        {
            var rawData = new AlegatorData() { HouseNr = ">" };
            Assert.IsFalse(rawData.GetHouseNumber().HasValue);
        }

        [TestMethod]
        public void CanHandleMinusInHouseNr()
        {
            var rawData = new AlegatorData() { HouseNr = "-1/" };
            Assert.IsFalse(rawData.GetHouseNumber().HasValue);
        }

        [TestMethod]
        public void CanGetHouseNr()
        {
            var rawData = new AlegatorData();
            foreach (var housePatter in _housePatters)
            {
                rawData.HouseNr = housePatter[0];
                Assert.IsTrue(rawData.GetHouseNumber().Value.ToString() == housePatter[1], string.Format("Not matched expected: {0}, actual {1}", housePatter[1], rawData.GetHouseNumber()));
            }
        }

        [TestMethod]
        public void CanGetHouseSuffix()
        {
            var rawData = new AlegatorData();
            foreach (var housePatter in _housePatters)
            {
                rawData.HouseNr = housePatter[0];
                Assert.IsTrue(rawData.GetHouseSuffix() == housePatter[2], string.Format("Not matched expected: {0}, actual {1}, for string: {2}", housePatter[2], rawData.GetHouseSuffix(), rawData.HouseNr));
            }
        }

        [TestMethod]
        public void CanGetAlegatorDataDocSeria()
        {
            var data = new AlegatorData();
            foreach (var patern in _listOfPaterns)
            {
                data.DocSeria = patern[0];
                data.DocNumber = patern[1];

                Assert.IsTrue(data.GetDocumentSeria() == patern[2], string.Format("Not matched expected: {0}, actual {1}", patern[2], data.GetDocumentSeria()));
            }
        }


        [TestMethod]
        public void CanGetAlegatorDocNumber()
        {
            var data = new AlegatorData();
            foreach (var patern in _listOfPaterns)
            {
                data.DocSeria = patern[0];
                data.DocNumber = patern[1];

                Assert.IsTrue(data.GetDocumentNumber() == patern[3], string.Format("Not matched expected: {0}, actual {1}", patern[3], data.GetDocumentNumber()));
            }
        }

    }
}