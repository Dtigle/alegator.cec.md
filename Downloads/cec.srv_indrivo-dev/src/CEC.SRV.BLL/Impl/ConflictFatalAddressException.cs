using System;

namespace CEC.SRV.BLL.Impl
{
    public class ConflictFatalAddressException : Exception
    {
        public ConflictFatalAddressException(string message): base(message)
        {
            
        }
    }
}