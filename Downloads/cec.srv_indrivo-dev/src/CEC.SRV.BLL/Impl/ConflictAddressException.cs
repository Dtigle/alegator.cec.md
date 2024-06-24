using System;

namespace CEC.SRV.BLL.Impl
{
    public class ConflictAddressException : Exception
    {
        public ConflictAddressException(string message) : base(message)
        {
        }
    }
}