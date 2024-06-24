using System.Net;

namespace CEC.SRV.BLL.Dto
{
    public class SSRSPrintParameters
    {
        public string ServerUrl { get; set; }

        public string ReportName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ExportPath { get; set; }

        public ICredentials GetCredentials()
        {
            if (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password))
            {
                return new NetworkCredential(UserName, Password);
            }

            return CredentialCache.DefaultNetworkCredentials;
        }
    }
}
