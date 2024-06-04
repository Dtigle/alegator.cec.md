using System;

namespace CEC.SRV.BLL.Impl
{
    public class LocalityConflictException : Exception
    {
        public LocalityConflictException(string message)
            : base(message)
        {
        }
    }
}