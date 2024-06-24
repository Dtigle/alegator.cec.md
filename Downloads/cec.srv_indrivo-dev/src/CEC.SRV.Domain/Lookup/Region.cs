using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CEC.Web.SRV.Resources;

namespace CEC.SRV.Domain.Lookup
{
    public class Region : Lookup
    {
        public const long NoResidenceRegionId = -1;

        private Region _parent;
        private RegionType _regionType;
        private readonly IList<Region> _children;
        private readonly int _level;


        protected Region()
        {
            _children = new List<Region>();
        }
        
        /// <summary>
        /// default ctor
        /// </summary>
        /// <param name="regionType"><see cref="RegionType"/></param>
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

        /// <summary>
        /// Parent region. Can be null
        /// </summary>
        public virtual Region Parent
        {
            get { return _parent; }
        }
        
        /// <summary>
        /// Region type
        /// </summary>
        public virtual RegionType RegionType
        {
            get { return _regionType; }
        }

        /// <summary>
        /// Lists of children regions
        /// </summary>
        public virtual IReadOnlyCollection<Region> Children
        {
            get { return new ReadOnlyCollection<Region>(_children); }
        }
        
        /// <summary>
        /// Saise identifier
        /// </summary>
        public virtual long? SaiseId { get; set; }

        /// <summary>
        /// Registru identifier
        /// </summary>
        [Obsolete]        
        public virtual long? RegistruId { get; set; }

        public virtual long? StatisticCode { get; set; }

        public virtual long? StatisticIdentifier { get; set; }

        public virtual GeoLocation GeoLocation { get; set; }

        /// <summary>
        /// Specify if current Region could have streets
        /// </summary>
        public virtual bool HasStreets { get; set; }

        public virtual int Level
        {
            get { return CalculateLevel(); }
        }

        public virtual PublicAdministration PublicAdministration { get; set; }


        private int CalculateLevel()
        {
            if (Parent == null)
                return 0;
            
            return Parent.CalculateLevel() + 1;
        }

	    public virtual int? GetCircumscription()
	    {
		    if (Circumscription.HasValue || Parent == null)
		    {
			    return Circumscription;
		    }

		    return Parent.GetCircumscription();
	    }

	    /// <summary>
        /// Adds a child <see cref="Region"/> 
        /// </summary>
        /// <param name="region">Child region</param>
        /// <exception cref="NotSupportedException"></exception>
        public virtual void AddChild(Region region)
        {
            if(region == null)
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

        /// <summary>
        /// Removes <param name="region"></param> from children if exists
        /// </summary>
        /// <param name="region"><see cref="Region"/> to be removed</param>
        /// <remarks>Also its set null parent for <param name="region"></param></remarks>
        public virtual void RemoveChild(Region region)
        {
            if(region == null)
                throw new ArgumentNullException("region");
            
            if (_children.Contains(region))
            {
                _children.Remove(region);
            }
        }

        /// <summary>
        /// Sets <param name="region"></param> as parent for current region
        /// </summary>
        /// <param name="region">Parent region</param>
        public virtual void ChangeParent(Region region)
        {
            

            if (region == null)
            {
                _parent = null;
                return;
            }
            
            if (RegionType.Rank < region.RegionType.Rank)
            {
                throw new NotSupportedException("Parent cant have higher rank as child");
            }

            if (region.Equals(this))
            {
				throw new NotSupportedException(MUI.Region_NotAcceptParentWithSameInstance);
            }

            _parent = region;
            
            region.AddChild(this);
        }

        public virtual void ChangeRegionType(RegionType regionType)
        {
            if (regionType == null)
            {
                throw new ArgumentNullException("regionType");
            }

            if (Parent.RegionType.Rank > regionType.Rank)
            {
                throw new NotSupportedException("Parent can't have higher rank as child");
            }

            _regionType = regionType;
        }

        public virtual string GetFullName()
        {
            return string.Format("{0} {1}", RegionType.Name, Name);
        }

		public virtual int? Circumscription { get; set; }
    }
}