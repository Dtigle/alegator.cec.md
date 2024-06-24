using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using CEC.SAISE.EDayModule.Controllers;
using CEC.SAISE.EDayModule.LoggingServices;

namespace CEC.SAISE.EDayModule.Infrastructure
{
    public class LoggerUtil: BaseController
    {
        private readonly string _ip;
        private readonly string _userName;
        private readonly string _token;
        readonly MessageHeader _messageHeader;
        private readonly LoggingServiceClient _logEvent;

        public LoggerUtil()
        {
            _ip = GetIpAddress();
            _userName = System.Web.HttpContext.Current.User.Identity.Name;
            _messageHeader = MessageHeader.CreateHeader("AppToken", "", ConfigurationManager.AppSettings["ApiToken"]);
            _logEvent = new LoggingServiceClient();
            if (System.Web.HttpContext.Current.Session != null)
            {
                _token = System.Web.HttpContext.Current.Session.SessionID;
            }
            else
            {
                _token = " ";
            }

        }

        public bool MLogEvent(LogLevel logLevel, string code, string message, Dictionary<string, string> parameters)
        {
            var result = false;
            try
            {
                var logEvent = new LogEvent
                {
                    UserIdentity = _userName,
                    UserMachineIp = _ip,
                    UserSessionId = _token,
                    GeneratedAt = DateTime.Now,
                    Level = logLevel,
                    Code = code,
                    Message = message,
                    Parameters = parameters
                };
                using (new OperationContextScope(_logEvent.InnerChannel))
                {
                    OperationContext.Current.OutgoingMessageHeaders.Add(_messageHeader);
                    _logEvent.MLog(logEvent);
                }

            }
            catch
            {
                //
            }

            return result;
        }

        public static string GetIpAddress()
        {
            var context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}