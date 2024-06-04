
namespace CEC.SRV.Domain.Lookup
{
    public class Circumscription : SRVBaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual string Number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string NameRo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string NameRu { get; set; }

        /// <summary>
        /// Primary region
        /// </summary>
        public virtual Region Region { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual CircumscriptionList CircumscriptionList { get; set; }
    }
}
