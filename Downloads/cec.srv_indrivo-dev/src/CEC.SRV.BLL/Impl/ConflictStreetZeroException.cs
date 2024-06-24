using System;

namespace CEC.SRV.BLL.Impl
{
    public class ConflictStreetZeroException : Exception
    {
        public ConflictStreetZeroException(string message)
            : base(message)
        {

        }
    }
}