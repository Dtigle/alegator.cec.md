using System;

namespace CEC.SRV.BLL.Impl
{
    public class ConflictStatusException : Exception
    {
        public ConflictStatusException(string message) : base(message)
        {
        }
    }
}