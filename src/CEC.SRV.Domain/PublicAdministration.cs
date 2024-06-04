using System;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.Domain
{
    public class PublicAdministration : SRVBaseEntity
    {
        private Region _region;
            
        [Obsolete("Nhibernate usage only")]
        protected PublicAdministration()
        {
        }

        /// <summary>
        /// Default ctor for public administration
        /// </summary>
        /// <param name="region">Region where administration belongs to</param>
        /// <param name="managerType">manager type</param>
        public PublicAdministration(Region region, ManagerType managerType)
        {
            if (region == null)
            {
                throw new ArgumentNullException("region");
            }

            if (managerType == null)
            {
                throw new ArgumentNullException("managerType");
            }

            _region = region;
            _region.PublicAdministration = this;
            ManagerType = managerType;
        }
        
        /// <summary>
        /// Name of the manager
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Surname of the manager
        /// </summary>
        public virtual string Surname { get; set; }

        /// <summary>
        /// Region where administration belongs to
        /// </summary>
        public virtual Region Region
        {
            get { return _region; }
        }

        /// <summary>
        /// Manager type <see cref="ManagerType"/>
        /// </summary>
        public virtual ManagerType ManagerType { get; set; }

        public virtual string GetManagerFullName()
        {
            return string.Format("{0} {1}", Surname, Name);
        }
    }
}