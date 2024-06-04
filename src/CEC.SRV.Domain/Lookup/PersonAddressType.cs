namespace CEC.SRV.Domain.Lookup
{
    public class PersonAddressType : Lookup
    {
        /// <summary>
        /// Represent a main residence
        /// </summary>
        public static long Residence = 1;
        public static long NoResidence = 0;
        public static long Temporary = 2;
        public static long Declaration = 3;
    }
}
