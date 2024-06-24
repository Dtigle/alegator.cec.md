using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amdaris;
using CEC.SRV.BLL;
using CEC.SRV.Domain;
using CEC.Web.Api.Dtos;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using CEC.Web.Api.Models;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using CEC.SRV.Domain.Lookup;
using CEC.Web.Api.GetElectionDataSaise;
using NHibernate.Properties;

namespace CEC.Web.Api.Controllers
{
    [RoutePrefix("PollingStation")]
    [NHibernateSession]
    public class PollingStationController : ApiController
    {
        private readonly IVotersBll _votersBll;
        private readonly IElectionBll _electionsBll;
        private readonly ILogger _logger;
        private readonly IEmailClientHelper _emailClientHelper;
        private readonly IPollingStationBll _pollingStationBll;

        public PollingStationController(IPollingStationBll PollingStationBll,IVotersBll voters, IElectionBll elections, IEmailClientHelper emailClientHelper, ILogger logger)
        {
            _votersBll = voters;
            _electionsBll = elections;
            _emailClientHelper = emailClientHelper;
            _logger = logger;
            _pollingStationBll = PollingStationBll;
        }

		[HttpPost]
		[Route("Assigned")]
		public async Task<PersonInfo> GetAssigned(CheckTheListRequest checkTheListRequest)
        {
            try
            {
                #region CapchaRegion
                var httpClient = new HttpClient();
                var privateKey = ConfigurationManager.AppSettings["RecaptchaPrivateKey"];
                var userIP = ((HttpContextBase)this.Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                const string uri = "https://www.google.com/recaptcha/api/siteverify";

                var postData = new List<KeyValuePair<string, string>>
                 {
                     new KeyValuePair<string, string>("secret", privateKey),
                     new KeyValuePair<string, string>("remoteip", userIP),
	                 //new KeyValuePair<string, string>("challenge", checkTheListRequest.Challenge),
	                 new KeyValuePair<string, string>("response", checkTheListRequest.Response)
                 };

                HttpContent content = new FormUrlEncodedContent(postData);

                var responseFromServer = await httpClient.PostAsync(uri, content)
                        .ContinueWith((postTask) => postTask.Result.EnsureSuccessStatusCode())
                        .ContinueWith((readTask) => readTask.Result.Content.ReadAsStringAsync().Result);

                var responceObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseFromServer);

                if (!(responceObj["success"].ToString().ToLower() == "true"))
                {
                    return new PersonInfo
                    {
                        ReCaptchaError = true,
                        WarningMessage = "Introduceți corect codul din imagine"
                    };
                }
                #endregion

                var isActive = ConfigurationManager.AppSettings["ActivePeriod"].ToString();
                var eDayIsGenerated = ConfigurationManager.AppSettings["EDayIsGenerated"].ToString();
                var circumscriptionListId = int.Parse(ConfigurationManager.AppSettings["CircumscriptionListId"]);
                var electionRoundId = int.Parse(ConfigurationManager.AppSettings["ElectionRoundId"]);
                var warning = string.Empty;
                var person = GetValidPerson(checkTheListRequest.Idnp, out warning);

                if (person == null)
                {
                    return new PersonInfo
                    {
                        WarningMessage = warning
                    };
                }
             
                PollingStation pollingStation = null;
                Address pollingStationAddress = null;
                GeoLocation geoLocation = null;
                if (person.EligibleAddress != null)
                {
                    var personAddress = person.EligibleAddress;
                    if (personAddress.Address.PollingStation != null)
                    {
                        pollingStation = personAddress.Address.PollingStation;
                        pollingStationAddress = pollingStation != null ? pollingStation.PollingStationAddress : null;
                        geoLocation = ValidateGeoLocation(pollingStationAddress);
                    }
                }
                string CircumNr = null;
                PollingStationInfo pollingStationInfo = null;
                if (pollingStation != null)
                {
                    pollingStationInfo = new PollingStationInfo
                    {
                        Number = pollingStation.FullNumber,
                        LocationDescription = pollingStation.Location,
                        Address = pollingStationAddress == null
                            ? null
                            : pollingStationAddress.GetFullAddress(true, false, false),
                        PhoneNumber = pollingStation.ContactInfo,

                        LongY = geoLocation == null ? null : geoLocation.Longitude,
                        LatX = geoLocation == null ? null : geoLocation.Latitude
                    };                   
                }

                var arrPersonYear = person.DateOfBirth.Year.ToString().ToCharArray();
                if (arrPersonYear.Length > 0)
                {
                    arrPersonYear[2] = 'x';
                }

                if (isActive == "0")  //Perioada electoralã nu este activã
                {
                    pollingStationInfo.Number = pollingStation.Number;
                }
                person.DataFromEday = new DataFromEday();
                if (eDayIsGenerated == "1")
                {
                    try
                    {
                        getVoterDataEday.GetDataVotersClient client = new getVoterDataEday.GetDataVotersClient();

                        var pEd = client.GetVoter(checkTheListRequest.Idnp);
                        if (pEd != null)
                        {
                            person.DataFromEday = new DataFromEday
                            {
                                VoterCeritcateNumber = pEd.VoterCertificatatNumber,
                            };
                        }
                    }
                    catch
                    {

                    }

                }
                else
                {
                    if (pollingStation != null)
                    {
                        ElectionCircumscriptionPollingStation result = null;
                        try
                        {
                            using (ElectionsServiceClient client = new GetElectionDataSaise.ElectionsServiceClient())
                            {
                                result = client.GetElectionCircumscriptionPollingStation(pollingStation.Id, electionRoundId, circumscriptionListId);
                            }
                        }
                        catch 
                        {
                        }
                        if (result != null)
                        {
                            CircumNr = result.NameRo;
                            if (result.PollingStationNumber != null)
                            {
                                pollingStationInfo.Number = result.PollingStationNumber;
                            }
                        }
                    }
                }

                var pi = new PersonInfoWS
                {
                    Name = $"{GetFirstChar(person.FirstName)} {GetFirstChar(person.Surname)}",
                    DOB = new string(arrPersonYear),
                    WarningMessage = warning,
                    electionListNr = person.ElectionListNr?.ToString(),
                    PollingStation = pollingStationInfo,
                    Circumscription = CircumNr,
                    VoterCertificate = person.DataFromEday.VoterCeritcateNumber != null
                };

                LogClientInfo(checkTheListRequest.Idnp);

                return pi;
            }
            catch (Exception ex)
            {
                _logger.Trace(ex.Message);
                _logger.Warning(ex.Message);
                _logger.Error(ex);
                _logger.Debug(ex.Message);
                throw;
            }
        }

		[Route("captchaKey")]
		public string GetRecaptchaPublicKey()
		{
            try
            {
                var captchaPublicKey = ConfigurationManager.AppSettings["RecaptchaPublicKey"];
                return captchaPublicKey;
            }
            catch (Exception ex)
            {
                _logger.Trace(ex.Message);
                _logger.Warning(ex.Message);
                _logger.Error(ex);
                _logger.Debug(ex.Message);
                throw;
            }
		}

		[Route("AssignedForRegistration/{idnp:regex(^(09|20){1}[0-9]{11}$)}")]
		public PersonFullInfo GetAssignedForRegistration(string idnp)
        {
            var warning = string.Empty;
            var person = _votersBll.GetByIdnp(idnp);
            var IsAlreadyRegistered = _votersBll.IsRegisteredToElection(person.Id);
            if (IsAlreadyRegistered) warning = "Persoana cu IDNP:" + idnp+ " a fost deja înregistrată";

            if (person == null || IsAlreadyRegistered)
                return new PersonFullInfo { WarningMessage = warning };

            var personAddress = person.EligibleAddress;
            var fullAdress = personAddress.GetFullPersonAddress(true);
            if (personAddress.Address.Street.Region.Parent != null)
                fullAdress = personAddress.Address.Street.Region.Parent.GetFullName() + ", " + fullAdress;

            var pi = new PersonFullInfo
            {
                Name = person.FirstName,
                Surname = person.Surname,
                DOB = person.DateOfBirth.Year,
                Residence = fullAdress,
                WarningMessage = warning,
            };
            
            LogClientInfo(idnp);

            return pi;
        }

        [Route("Election")]
        public ElectionInfo GetCurrentActiveElection()
        {
            var election = _electionsBll.GetCurrentElection();
            if(election != null)
                return new ElectionInfo() { Id= election.Id, Name = election.ElectionType.Name, Data = election.ElectionRounds.LastOrDefault().ElectionDate.ToString("dd/MM/yy") };
            else
                return new ElectionInfo();
        }

		public bool GetAbroadDeclaration()
		{
			var election = _electionsBll.GetCurrentElection();
			return election != null;
		}

        public void SaveReport(ReportModel model)
        {
            var warning = string.Empty;
            var person = GetValidPerson(model.IDNP, out warning);
             
            var voterId = _votersBll.SaveAbroadVoterRegistration(person.Id, model.AbroadAddress, model.ResidenceAddress, model.AbroadAddresCountry, 
                    model.AbroadAddressLat, model.AbroadAddressLong, model.Email, GetClientIp());

            _emailClientHelper.ImagePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/cecmd_.png");
            _emailClientHelper.Send(model.Email, "Confirmare", GetHtmlBody(model.Election, voterId));

            LogClientInfo(model.IDNP);

        }

        /// <summary>
        /// Check for coordinates of polling station.
        /// Latitude and longitude are in selected square - geographically
        /// </summary>
        /// <param name="pollingStationAddress">Polling Station</param>
        /// <returns>Polling Station Geolocation</returns>
        private static GeoLocation ValidateGeoLocation(Address pollingStationAddress)
        {
            if (pollingStationAddress == null) return null;

            GeoLocation geoLocation = pollingStationAddress.GeoLocation;

            if (geoLocation != null &&
               !(geoLocation.Latitude > 40 && geoLocation.Latitude < 49 &&
                 geoLocation.Longitude > 26 && geoLocation.Longitude < 31))
            {
                geoLocation = null;
            }

            return geoLocation;
        }

        private string GetHtmlBody(string election, long ID)
        {
            string body = @"<div style=""position:absolute; top: 10px; left:10px;"">
                                <table>
                                    <tr>
                                        <td> <img src=""cid:{0}"" alt=""Alternate Text"" /> </td>
                                        <td style=""padding-left: 80px; color: #428bca; font-weight: 500; line-height: 1.1; font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-size: 24px; "">Comisia Electorală Centrală a Republicii Moldova</td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td style=""padding: 30px; color: #525252; font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-size: 14px; line-height: 1.42857; "">
                                    Solicitarea Dvs. de a participa la <span>" + election + " </span> peste hotarele Republicii Moldova a fost procesată și înregistrată cu numărul <span> " + ID + "</span>. </td>" +
                                    @"</tr>
                                </table>
                            </div>";
            return body;
        }

        private string GetFirstChar(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value.First() + ".";
        }

      
        private Person GetValidPerson(string idnp, out string warning)
        {
            warning = string.Empty;
            var person = _votersBll.GetByIdnp(idnp);

            if (person == null || person.CurrentStatus == null || person.CurrentStatus.StatusType.IsExcludable)
            {
                _logger.Warning(warning = string.Format("Persoana cu idnp:{0} nu a fost găsită", idnp));
                return null;
            }
            
            var personAddress = person.EligibleAddress;
          

            if (personAddress == null)
            {
                _logger.Warning(warning = string.Format("Persoana cu idnp:{0} are probleme cu viza de reședință", idnp));
                return null;
            }

            //var pollingStation = personAddress.Address.PollingStation;

            //if (pollingStation == null)
            //{
            //    _logger.Warning(warning = string.Format("Persoana cu idnp:{0} nu este legata cu Sectia de Votare", idnp));
            //    return null;
            //}

            return person;
        }

        private void LogClientInfo(string idnp)
        {
            string clientIP = GetClientIp();
            string browserID = GetBrowserID();

            _logger.Trace(string.Format("[IP:{0}, BROWSER:{1}, IDNP:{2}]",
                clientIP, browserID, idnp));
        }

        private string GetClientIp()
        {
            string clientIP = "<Unknown_IP>";
            if (Request.Properties.ContainsKey("MS_HttpContext"))
            {
                var request = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request;
                clientIP = request.UserHostAddress;
            }
            return clientIP;
        }

        private string GetBrowserID()
        {
            string browserID = "<Unknown_Browser>";
            if (Request.Properties.ContainsKey("MS_HttpContext"))
            {
                var request = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request;
                browserID = request.Browser.Id;
            }
            return browserID;
        }


        #region ToRemove
        //[Route("AssignedByIDNP")]
        //public PersonInfoWS GetAssignedByIDNP(string IDNP)
        //{
        //    try
        //    {
        //        // var IDNP = checkTheListRequest?.Idnp;
        //        var warning = string.Empty;
        //        var person = GetValidPerson(IDNP, out warning);

        //        var isActive = ConfigurationManager.AppSettings["ActivePeriod"].ToString();


        //        if (person == null)
        //        {
        //            return new PersonInfoWS
        //            {
        //                WarningMessage = warning
        //            };
        //        }


        //        var personAddress = person.EligibleAddress;
        //        var pollingStation = personAddress.Address.PollingStation;
        //        var pollingStationAddress = pollingStation?.PollingStationAddress;
        //        var geoLocation = ValidateGeoLocation(pollingStationAddress);
        //        string CircumNr = null;

        //        PollingStationInfo pollingStationInfo = null;
        //        if (pollingStation != null)
        //        {
        //            pollingStationInfo = new PollingStationInfo
        //            {
        //                Number = pollingStation.FullNumber,
        //                LocationDescription = pollingStation.Location,
        //                Address = pollingStationAddress == null
        //                    ? null
        //                    : pollingStationAddress.GetFullAddress(true, false, false),
        //                PhoneNumber = pollingStation.ContactInfo,

        //                LongY = geoLocation == null ? null : geoLocation.Longitude,
        //                LatX = geoLocation == null ? null : geoLocation.Latitude
        //            };
        //        }

        //        var arrPersonYear = person.DateOfBirth.ToString();
        //        //if (arrPersonYear.Length > 0)
        //        //{
        //        //    arrPersonYear[2] = 'x';
        //        //}
        //        /**/

        //        if (isActive == "0")  //Perioada electoralã nu este activã
        //        {
        //            pollingStationInfo.Number = pollingStation.Number;
        //        }
        //        else
        //        {
        //            #region MethodAndrei
        //            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SaiseConnectionString"].ConnectionString))
        //            {
        //                using (SqlCommand cmd = new SqlCommand("SELECT  e.status,eps.numberPerElection,e.principal " +
        //                "FROM [SAISE].[ElectionPollingStations] eps " +
        //                "join [SAISE].[ElectionRounds] er on eps.electionRoundId = er.electionRoundId " +
        //                "join [SAISE].[Elections] e on er.electionId = e.electionId " +
        //                "where eps.pollingStationId = " + pollingStation.Id + " order by eps.electionPollingStationId desc", conn))
        //                {
        //                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
        //                    {
        //                        DataTable tbl = new DataTable();
        //                        sda.Fill(tbl);
        //                        bool scrutinActiv = false;
        //                        bool scrutinPrincipal = false;
        //                        foreach (DataRow item in tbl.Rows)
        //                        {
        //                            if (item[0].ToString() == "1")
        //                            {
        //                                scrutinActiv = true;//sectia este implicata in scrutinuri
        //                                if (!scrutinPrincipal)
        //                                {
        //                                    pollingStationInfo.Number = item[1].ToString();
        //                                    CircumNr = item[1].ToString();
        //                                    if (item[2].ToString() == "1")   //scrutin principal
        //                                    {
        //                                        scrutinPrincipal = true;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        if (scrutinActiv == false)
        //                        {
        //                            pollingStationInfo.Number = pollingStation.Number;
        //                        }

        //                    }
        //                }
        //            }

        //            /*Existen?a certificatului cu drept de vot*/
        //            /*Afi?area datelor depinde dacã a fost generatã BD pentru aplica?ia ”Ziua votului” pentru ”Perioadã electoralã activã”=da.*/
        //            var EdayGenerated = ConfigurationManager.AppSettings["EDayIsGenerated"].ToString();
        //            if (EdayGenerated == "1")
        //            {
        //                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["EdayConnectionString"].ConnectionString))
        //                {
        //                    using (SqlCommand cmd = new SqlCommand("SELECT  e.status,eps.scrutinNumber,e.principal " +
        //  "FROM [SAISE].[ElectionPollingStations] eps " +
        //  "join [SAISE].[ElectionRounds] er on eps.electionRoundId = er.electionRoundId " +
        //  "join [SAISE].[Elections] e on er.electionId = e.electionId " +
        //  "where eps.pollingStationId = " + pollingStation.Id + " order by eps.electionPollingStationId desc", conn))
        //                    {
        //                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
        //                        {
        //                            DataTable tbl = new DataTable();
        //                            sda.Fill(tbl);
        //                            bool scrutinActiv = false;
        //                            bool scrutinPrincipal = false;
        //                            foreach (DataRow item in tbl.Rows)
        //                            {
        //                                if (item[0].ToString() == "1")
        //                                {
        //                                    scrutinActiv = true;//sectia este implicata in scrutinuri
        //                                    if (!scrutinPrincipal)
        //                                    {
        //                                        pollingStationInfo.Number = item[1].ToString();
        //                                        CircumNr = item[1].ToString();
        //                                        if (item[2].ToString() == "1")   //scrutin principal
        //                                        {
        //                                            scrutinPrincipal = true;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                            if (scrutinActiv == false)
        //                            {
        //                                pollingStationInfo.Number = pollingStation.Number;
        //                            }

        //                        }
        //                    }
        //                }

        //            }

        //            #endregion

        //            try
        //            {

        //                person.DataFromEday = new DataFromEday();

        //                getVoterDataEday.GetDataVotersClient client = new getVoterDataEday.GetDataVotersClient();

        //                var pEd = client.GetVoter(IDNP);
        //                if (pEd != null)
        //                {
        //                    person.DataFromEday = new DataFromEday
        //                    {
        //                        CircumscriptionName = pEd.CircumscriptionName,
        //                        PollingStationNumber = pEd.PollingStationNumber,
        //                        VoterCeritcateNumber = pEd.VoterCertificatatNumber,
        //                        VoterNumberList = pEd.VoterNumberList,
        //                        CircumscriptionNumber = pEd.CircumscriptionNumber

        //                    };
        //                }

        //                pollingStation.Number = person.DataFromEday.PollingStationNumber ?? pollingStation.Number;
        //                CircumNr = person.DataFromEday.CircumscriptionNumber;
        //            }
        //            catch
        //            {

        //            }

        //        }
        //        /**/

        //        var pi = new PersonInfoWS
        //        {
        //            Name = $"{person.Surname} {person.FirstName} {person.Surname}",
        //            IDNP = person.Idnp,
        //            Gender = person.Gender?.Name,
        //            DOB = arrPersonYear,
        //            Residence = personAddress.GetFullPersonAddress(true),
        //            WarningMessage = warning,
        //            electionListNr = person.DataFromEday.VoterNumberList,
        //            PollingStation = pollingStationInfo,
        //            CircumscriptionNr = CircumNr,
        //            VoterCertificate = person.DataFromEday.VoterCeritcateNumber != null
        //        };

        //        LogClientInfo(IDNP);

        //        return pi;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Trace(ex.Message);
        //        _logger.Warning(ex.Message);
        //        _logger.Error(ex);
        //        _logger.Debug(ex.Message);
        //        throw;
        //    }
        //}
        #endregion




    }
}