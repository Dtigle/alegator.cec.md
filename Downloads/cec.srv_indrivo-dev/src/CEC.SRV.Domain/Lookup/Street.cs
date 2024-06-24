using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CEC.SRV.Domain.Lookup
{
    public class Street : Lookup
    {
        private Region _region;
        private IList<Address> _addresses;
            
        [Obsolete("Nhibernate usage only")]
        protected Street()
        {
            _addresses = new List<Address>();
        }

        /// <summary>
        /// Default ctor for street
        /// </summary>
        /// <param name="region">Region where street belongs to</param>
        /// <param name="streetType">street type</param>
        /// <param name="name">street's name</param>
        /// <param name="overrideHasStreets">overrides check of HasRegion property on Validation. Default value 'False'</param>
        public Street(Region region, StreetType streetType, string name, bool overrideHasStreets = false)
        {
            ValidateRegion(region, overrideHasStreets);

            if (streetType == null)
            {
                throw new ArgumentNullException("streetType");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            _region = region;
            StreetType = streetType;
            Name = name;
            _addresses = new List<Address>();
        }


        /// <summary>
        /// External id of Registry of Population
        /// </summary>
        public virtual long? RopId { get; set; }

        /// <summary>
        /// External id to SAISE app
        /// </summary>
        public virtual long? SaiseId { get; set; }

        /// <summary>
        /// Region where street belongs to
        /// </summary>
        public virtual Region Region
        {
            get { return _region; }
        }

        /// <summary>
        /// Street type <see cref="StreetType"/>
        /// </summary>
        public virtual StreetType StreetType { get; set; }

        /// <summary>
        /// List of <see cref="Address"/>es under this Street
        /// </summary>
        public virtual IReadOnlyCollection<Address> Addresses
        {
            get
            {
                return new ReadOnlyCollection<Address>(_addresses);
            }
        }

        /// <summary>
        /// Change the <see cref="Region"/> 
        /// </summary>
        /// <param name="region">new region</param>
        /// <param name="overrideHasStreets">overrides check of HasRegion property on Validation. Default value 'False'</param>
        public virtual void ChangeRegion(Region region, bool overrideHasStreets = false)
        {
            ValidateRegion(region, overrideHasStreets);

            if (_region != region)
            {
                _region = region;
            }
        }

        public virtual string GetFullName(bool setStreetNameFirst = true)
        {
            return setStreetNameFirst
				? string.Format("{0} {1}", Name.StartsWith("$")?">":Name, StreetType.Name)
				: string.Format("{0} {1}", StreetType.Name, Name.StartsWith("$") ?">": Name);
        }

        private void ValidateRegion(Region region, bool overrideHasStreets = false)
        {
            if (region == null)
            {
                throw new ArgumentNullException("region");
            }

            if (!region.HasStreets && !overrideHasStreets)
            {
                throw new NotSupportedException(string.Format("Cant add street to a region with hasStreets equals false. Region : {0}", region.GetFullName()));
            }
        }
    }
}