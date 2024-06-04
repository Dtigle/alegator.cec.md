using CEC.SRV.Domain.Lookup;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CEC.SRV.Domain
{
    public class PollingStation : SRVBaseEntity, INotificationEntity
    {
        private readonly IList<Address> _addresses;
        private Region _region;
        private readonly int _owingCircumscription;
        private string _fullNumber;

        public PollingStation()
        {
            _addresses = new List<Address>();
        }

        /// <summary>
        /// Default ctor for polling station
        /// </summary>
        /// <param name="region">Region where polling station belongs to</param>
        public PollingStation(Region region)
        {
            if (region == null)
            {
                throw new ArgumentNullException("region");
            }

            _region = region;

            _addresses = new List<Address>();
        }

        /// <summary>
        /// Polling Station number
        /// </summary>
        public virtual string Number { get; set; }

        /// <summary>
        /// Polling Station sub number
        /// </summary>
        public virtual string SubNumber { get; set; }

        /// <summary>
        /// Region where pollinStation belongs to
        /// </summary>
        public virtual Region Region
        {
            get { return _region; }
        }

        /// <summary>
        /// Contact info e.g. Phone, email, etc.
        /// </summary>
        public virtual string ContactInfo { get; set; }

        /// <summary>
        /// List of addresses assigned to current polling station
        /// </summary>
        public virtual IReadOnlyList<Address> Addresses
        {
            get
            {
                return new ReadOnlyCollection<Address>(_addresses);
            }
        }

        /// <summary>
        /// Saise identifier
        /// </summary>
        public virtual long? SaiseId { get; set; }

        public virtual VotersListOrderType VotersListOrderType { get; set; }

        /// <summary>
        /// Geo location
        /// </summary>
        public virtual GeoLocation GeoLocation { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        public virtual string Location { get; set; }

        /// <summary>
        /// Address of the polling station
        /// </summary>
        public virtual Address PollingStationAddress { get; set; }

        public virtual int OwingCircumscription
        {
            get { return _owingCircumscription; }
        }

        public virtual PollingStationTypes PollingStationType { get; set; }

        /// <summary>
        /// Add an <see cref="Address"/> to polling station
        /// </summary>
        /// <param name="address"><see cref="Address"/> to be assigned</param>
        public virtual void AddAddress(Address address)
        {
            if (address == null) throw new ArgumentNullException("address");

            if (!_addresses.Contains(address))
            {
                _addresses.Add(address);
            }
        }

        /// <summary>
        /// Remove an <see cref="Address"/> from polling station
        /// </summary>
        /// <param name="address"><see cref="Address"/> to be removed</param>
        public virtual void RemoveAddress(Address address)
        {
            if (address == null) throw new ArgumentNullException("address");

            if (_addresses.Contains(address))
            {
                _addresses.Remove(address);
            }
        }

        public virtual string GetFullAddress()
        {
            return PollingStationAddress != null ? PollingStationAddress.GetFullAddress() : null;
        }

        public virtual void ChangeRegion(Region region)
        {
            if (_region != region)
            {
                _region = region;
            }
        }

        string INotificationEntity.GetNotificationType()
        {
            return "PollingStation_Notification";
        }

        public virtual string FullNumber
        {
            get { return _fullNumber; }
        }

    }
}