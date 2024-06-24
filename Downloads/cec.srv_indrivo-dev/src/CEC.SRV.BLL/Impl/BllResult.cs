using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SRV.BLL.Impl
{
    public class BllResult
    {
        public long StatusCode { get; set; }
        public String StatusMessage { get; set; }

        public BllResult(long statusCode, string statusMessage)
        {
            this.StatusCode = statusCode;
            this.StatusMessage = statusMessage;
        }

        public override string ToString()
        {
            return "[Status: ]" + StatusCode + "\n" +
                   "[StatusMessage: ] " + StatusMessage;
        }
    }
}
