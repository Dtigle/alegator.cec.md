using System;

namespace CEC.SRV.BLL.Impl
{
    public class ConflictRegionException : Exception
    {
        public ConflictRegionException(string message) : base(message)
        {
        }
    }
}