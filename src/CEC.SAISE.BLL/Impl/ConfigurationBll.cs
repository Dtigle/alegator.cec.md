using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Impl
{
    public class ConfigurationBll : IConfigurationBll
    {
        private const string Enable_PS_Openning = "SAISE.Enable_PS_Openning";
        private const string Enable_PS_Turnouts = "SAISE.Enable_PS_Turnouts";
        private const string Enable_ElectionResults_Input = "SAISE.Enable_ElectionResults_Input";
        private const string Debug_Mode_Enabled = "SAISE.Debug_Mode_Enabled";

        public TimeSpan GetPSOpenningTime()
        {
            var paramValue = ConfigurationManager.AppSettings[Enable_PS_Openning];
            if (string.IsNullOrWhiteSpace(paramValue))
            {
                throw new ConfigurationErrorsException("Missing parameter or value for 'SAISE.Enable_PS_Openning'");
            }

            return TimeSpan.Parse(paramValue);
        }

        public TimeSpan GetPSTurnoutsTime()
        {
            var paramValue = ConfigurationManager.AppSettings[Enable_PS_Turnouts];
            if (string.IsNullOrWhiteSpace(paramValue))
            {
                throw new ConfigurationErrorsException("Missing parameter or value for 'SAISE.Enable_PS_Turnouts'");
            }

            return TimeSpan.Parse(paramValue);
        }

        public TimeSpan GetPSElectionResultsTime()
        {
            var paramValue = ConfigurationManager.AppSettings[Enable_ElectionResults_Input];
            if (string.IsNullOrWhiteSpace(paramValue))
            {
                throw new ConfigurationErrorsException("Missing parameter or value for 'SAISE.Enable_ElectionResults_Input'");
            }

            return TimeSpan.Parse(paramValue);
        }

        public bool DebugModeEnabled()
        {
            var paramValue = ConfigurationManager.AppSettings[Debug_Mode_Enabled];
            if (string.IsNullOrWhiteSpace(paramValue))
            {
                throw new ConfigurationErrorsException("Missing parameter or value for 'SAISE.Debug_Mode_Enabled'");
            }

            return bool.Parse(paramValue);
        }
    }
}
