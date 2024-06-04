using System.Linq;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain
{
    public class Address : SRVBaseEntity
    {
        /// <summary>
        /// The street of the address
        /// </summary>
        public virtual Street Street { get; set; }

        /// <summary>
        /// House' number
        /// </summary>
        public virtual int? HouseNumber { get; set; }

        /// <summary>
        /// Sufix
        /// </summary>
        public virtual string Suffix { get; set; }

        /// <summary>
        /// <see cref="PollingStation"/> under address bellows to
        /// </summary>
        public virtual PollingStation PollingStation { get; set; }

        /// <summary>
        /// Specify the type of buildings
        /// </summary>
        public virtual BuildingTypes BuildingType { get; set; }

        /// <summary>
        /// Returns a full formatted address
        /// </summary>
        public virtual string GetFullAddress(bool includeRegion = true, bool setStreetNameFirst = true, bool showpollingStation = false)
        {
            string region = "";
            if (includeRegion)
            {
                region = Street.Region.GetFullName() + ",";
            }
            string pollingStationNumber = "";
            if (showpollingStation && PollingStation != null)
            {
                pollingStationNumber = " -> " + PollingStation.FullNumber;
            }
            return string.Format("{0} {1}, {2}{3}", region, Street.GetFullName(setStreetNameFirst), GetBuildingNumber(), pollingStationNumber);
        }

        /// <summary>
        /// Returns formated Building number. Possible formats are variants of: 1/1, 2a
        /// </summary>
        public virtual string GetBuildingNumber()
        {
            return string.IsNullOrWhiteSpace(Suffix)
                ? HouseNumber.ToString()
                : string.Format(char.IsDigit(Suffix.First()) ? "{0}/{1}" : "{0}{1}", HouseNumber, Suffix);
        }

		/// <summary>
		/// Geo location
		/// </summary>
		public virtual GeoLocation GeoLocation { get; set; }
    }
}