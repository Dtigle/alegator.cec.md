using System;

namespace CEC.SRV.BLL.Impl
{
    public class ConflictPollingException : Exception
    {
        public ConflictPollingException(string message) : base(message)
        {
        }
    }
}