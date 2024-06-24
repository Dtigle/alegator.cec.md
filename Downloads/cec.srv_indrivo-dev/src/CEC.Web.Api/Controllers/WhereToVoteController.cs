using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using Amdaris.Domain;
using Amdaris.Domain.Paging;
using CEC.SRV.BLL.Dto;
using CEC.SRV.BLL.Extensions;
using CEC.SRV.Domain.Lookup;
using CEC.SRV.BLL;
using CEC.Web.Api.Models;
using Newtonsoft.Json;
using CEC.SRV.Domain;
using CEC.Web.Api.Infrastructure;
using CEC.Web.Api.Dtos;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CEC.Web.Api.GetElectionDataSaise;
using Region = CEC.SRV.Domain.Lookup.Region;

namespace CEC.Web.Api.Controllers
{
    [System.Web.Http.RoutePrefix("WhereToVote")]
    [NHibernateSession]
    public class WhereToVoteController : ApiController
    {
        private readonly ILookupBll _lookupBll;
        private readonly IAddressBll _addressBll;
       
        public WhereToVoteController(ILookupBll lookupBll, IAddressBll addressBll)
        {
            _lookupBll = lookupBll;
            _addressBll = addressBll;
        }

        [System.Web.Http.Route("RegionsOfLevel1/{filter}")]
        public IEnumerable<IdentityInfo> GetRegionsOfLevel1(string filter)
        {
            Regex regionsReg = new Regex("^[a-zA-Z ăîâșțĂÎÂȘȚ]{1,}$");

            if (string.Equals(filter, "_") || regionsReg.IsMatch(filter))
            {
                return _lookupBll.GetRegionsOfLevel1ByFilter(this.GetFilterWithDiacritics(filter))
                    .Where(x => x.Id != Region.NoResidenceRegionId && !x.Name.Contains("Transnistria"))
                    .Select(x => new IdentityInfo { Id = x.Id, Name = x.GetFullName() });
            }
            else
            {
                return new List<IdentityInfo>();
            }
        }

        [System.Web.Http.Route("Regions/{parentId}/{filter}")]
        public IEnumerable<IdentityInfo> GetRegions(long parentId, string filter)
        {
            Regex regionsReg = new Regex("^[a-zA-Z ăîâșțĂÎÂȘȚ]{1,}$");

            if (string.Equals(filter, "_") || regionsReg.IsMatch(filter))
            {
                return _lookupBll.GetRegionsByParentIdAndFilter(parentId, this.GetFilterWithDiacritics(filter))
                    .Where(x => !x.Name.Contains("exteritorial"))
                    .Select(x => new IdentityInfo {Id = x.Id, Name = x.GetFullName()})
                    .ToList();
            }
            else
            {
                return new List<IdentityInfo>();
            }
        }
        
		[System.Web.Http.Route("GetRegions")]
		[System.Web.Http.HttpPost]
		public JsonResult GetRegions(StreetRequest streetRequest)
		{
			var request = new Select2Request
			{
				page = streetRequest.page,
				pageLimit = streetRequest.pageLimit,
				q = streetRequest.q
			};
			var pageRequest = request.ToPageRequest("Name", ComparisonOperator.Contains);
			var data = _addressBll.SearchStreetsForPublic(pageRequest, streetRequest.RegionId);
			var response = new Select2PagedResponse(data.Items.ToSelectSelect2List(x => x.Id, x => x.GetFullName()).ToList(), data.Total, data.PageSize);
			return new JsonResult { Data = response, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

        [System.Web.Http.Route("HasStreets/{regionId}")]
        [System.Web.Http.HttpGet]
        public Boolean RegionHasStreets(long regionId)
        {
            Region region = _lookupBll.Get<Region>(regionId);
            return (region != null) && region.HasStreets;
        }

        [System.Web.Http.Route("PollingStations/{regionId}")]
        public IEnumerable<PollingStationInfo> GetPollingStations(long regionId)
        {
            List<PollingStationInfo> pollingStationsInfo = new List<PollingStationInfo>();

            IEnumerable<PollingStation> pollingStations = _lookupBll.GetPollingStationsHierarhicallyByRegion(regionId);
            
            foreach (PollingStation pollingStation in pollingStations)
            {
                pollingStationsInfo.Add(this.GetPollingStationInfo(pollingStation));
            }

            return pollingStationsInfo;
        }

        [System.Web.Http.Route("Addresses/{streetId}")]
        public IEnumerable<AddressInfo> GetAddresses(long streetId)
        {
            List<AddressInfo> addressesInfo = new List<AddressInfo>();
            IEnumerable<Address> addresses = _addressBll.GetAddressesByStreetId(streetId);

            AddressInfo addressInfo;
           
            foreach (Address address in addresses)
            {
                addressInfo = new AddressInfo();
       
                addressInfo.Id = address.Id;
                addressInfo.Name = address.GetBuildingNumber();
                addressInfo.PollingStation = this.GetPollingStationInfo(address.PollingStation);
       
                addressesInfo.Add(addressInfo);
            }

            return addressesInfo;
        }
        
        private PollingStationInfo GetPollingStationInfo(PollingStation pollingStation)
        {
            var circumscriptionListId = int.Parse(ConfigurationManager.AppSettings["CircumscriptionListId"]);
            var electionRoundId = int.Parse(ConfigurationManager.AppSettings["ElectionRoundId"]);
            PollingStationInfo pollingStationInfo = (pollingStation == null)
                ? null
                : new PollingStationInfo
                {
                    Number = "",
                    LocationDescription = pollingStation.Location,
                    Address =
                        (pollingStation.PollingStationAddress != null)
                            ? pollingStation.PollingStationAddress.GetFullAddress(true, false, false)
                            : null,
                    PhoneNumber = pollingStation.ContactInfo,
                    LongY =
                        (pollingStation.PollingStationAddress != null) &&
                        (pollingStation.PollingStationAddress.GeoLocation != null)
                            ? pollingStation.PollingStationAddress.GeoLocation.Longitude
                            : null,
                    LatX =
                        (pollingStation.PollingStationAddress != null) &&
                        (pollingStation.PollingStationAddress.GeoLocation != null)
                            ? pollingStation.PollingStationAddress.GeoLocation.Latitude
                            : null
                };


            if (pollingStationInfo != null)
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

                    if (result.PollingStationNumber != null)
                    {
                        pollingStationInfo.Number = result.PollingStationNumber;
                    }
                    else
                    {
                        pollingStationInfo.Number = pollingStation.FullNumber;
                    }
                    pollingStationInfo.Circumscription = result.NameRo;
                }
                else
                {
                    pollingStationInfo.Number = pollingStation.FullNumber;
                }
            }

            return pollingStationInfo;
        }
        
        private string GetFilterWithDiacritics(string filter)
        {
            return filter.Aggregate(new StringBuilder(), (sbCurrent, chNext) =>
                ("Ss".Contains(chNext) ? sbCurrent.Append("[sș]") : (
                 "Tt".Contains(chNext) ? sbCurrent.Append("[tț]") : (
                 "Aa".Contains(chNext) ? sbCurrent.Append("[aăâ]") : (
                 "Ii".Contains(chNext) ? sbCurrent.Append("[iî]") : sbCurrent.Append(chNext)))))).ToString();
        }

        private string GetRegionFullName(Street street)
        {
            StringBuilder fullName = this.GetRegionFullName(street.Region);
            fullName.Insert(0, street.GetFullName(false));
            return fullName.ToString();
        }

        private StringBuilder GetRegionFullName(Region region)
        {
            StringBuilder fullName = new StringBuilder(")");
            
            while (region != null)
            {
                if (region.Parent != null)
                {
                    fullName.Insert(0, region.GetFullName());
                    if (region.Parent.Parent != null)
                    {
                        fullName.Insert(0, " / ");
                    }
                }
                else
                {
                    fullName.Insert(0, " (");
                }

                region = region.Parent;
            }

            return fullName;
        }
    }
}
