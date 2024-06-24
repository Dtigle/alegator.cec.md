using System;
using System.Collections.Generic;

namespace CEC.SAISE.Domain
{
    public class Region : SaiseBaseEntity
    {
        private readonly int _level;
        private IList<Region> _children;
        private Region _parent;
        private RegionType _regionType;

        public Region()
        {
            _children = new List<Region>();
        }

        public Region(RegionType regionType)
        {
            _children = new List<Region>();
            _regionType = regionType;
            _level = 0;
        }

        public Region(Region parent, RegionType regionType) : this(regionType)
        {
            parent.AddChild(this);
            _parent = parent;
        }

        public virtual IList<Region> Children
        {
            get { return _children ?? (_children = new List<Region>()); }
            set { _children = value; }
        }
        public virtual Region Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        public virtual RegionType RegionType
        {
            get { return _regionType; }
            set { _regionType = value; }
        }
        public virtual string Name { get; set; }
        public virtual string NameRu { get; set; }
        public virtual string Description { get; set; }
        public virtual long? RegistryId { get; set; }
        public virtual long? StatisticCode { get; set; }
        public virtual long? StatisticIdentifier { get; set; }
        public virtual decimal? GeoLatitude { get; set; }
        public virtual decimal? GeoLongitude { get; set; }
        public virtual bool HasStreets { get; set; }

        

        /// <summary>
		/// Adds a child <see cref="Region"/>
		/// </summary>
		/// <param name="region">Child region</param>
		/// <exception cref="NotSupportedException"></exception>
		public virtual void AddChild(Region region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            if (region.RegionType.Rank < RegionType.Rank)
            {
                throw new NotSupportedException("Child cant have a lower rank");
            }

            if (!_children.Contains(region))
            {
                _children.Add(region);
            }

            if (region.Parent == null || !region.Parent.Equals(this))
            {
                region._parent = this;
            }
        }
        public virtual string GetFullName()
        {
            return $"{RegionType.Name} {Name}";
        }

        public virtual string GetFullPath()
        {
            return GetFullPath(false);
        }

        public virtual string GetFullPath(bool addRm)
        {
            return (Parent != null && (addRm && Parent.Id == 1 || !addRm && Parent.Id != 1) ? Parent.GetFullPath() + ", " : string.Empty) + GetFullName();
        }

        public virtual string GetRegionName()
        {
            return GetRegionName(false);
        }
        public virtual string GetRegionName(bool addRm)
        {
            string name = (Parent != null && (addRm && Parent.Id == 1 || !addRm && Parent.Id != 1) ? Parent.GetFullPath().Split(',')[0] : string.Empty);
            return name;
        }

        /// <summary>
        /// Removes <param name="region"></param> from children if exists
        /// </summary>
        /// <param name="region"><see cref="Region"/> to be removed</param>
        /// <remarks>Also its set null parent for <param name="region"></param></remarks>
        public virtual void RemoveChild(Region region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            if (_children.Contains(region))
            {
                _children.Remove(region);
            }
        }
        private int CalculateLevel()
        {
            if (Parent == null)
                return 0;

            return Parent.CalculateLevel() + 1;
        }
    }
}
