using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Infrastructure
{
    public class MNotifyUtils
    {
        public static void Mnotify(string sender, string message, string subject)
        {


            Notifyservice.NotifyServiceClient client = new Notifyservice.NotifyServiceClient();
            
            client.MNotificationSystemEvent(sender,message,subject);
            
        }
    }
}