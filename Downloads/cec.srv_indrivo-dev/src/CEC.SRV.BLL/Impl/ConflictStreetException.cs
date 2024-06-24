using System;

namespace CEC.SRV.BLL.Impl
{
    public class ConflictStreetException : Exception
    {
        public ConflictStreetException(string message)
            : base(message)
        {
        }
    }
}