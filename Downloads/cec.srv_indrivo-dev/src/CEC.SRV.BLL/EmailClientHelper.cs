using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SRV.BLL
{
    public interface IEmailClientHelper
    {
        void Send(string recipient, string subject, string body);
        string ImagePath { get; set; }
    }
    public class EmailClientHelper : IEmailClientHelper
    {
        private readonly SmtpClient _client;
        private readonly string _username;

        public EmailClientHelper(string username, string password, string host, int port)
        {
            _username = username;
            _client = new SmtpClient();
            _client.UseDefaultCredentials = false;
            _client.Credentials = new System.Net.NetworkCredential(_username, password);
            _client.Port = port; // You can use Port 25 if 587 is blocked (mine is!)
            _client.Host = host;
            _client.DeliveryMethod = SmtpDeliveryMethod.Network;
            _client.EnableSsl = true;            
        }

        public string ImagePath { get; set; }

        public void Send(string recipient, string subject, string body)
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.To.Add(recipient);
            message.From = new System.Net.Mail.MailAddress(_username);
            message.Subject = subject;
            message.IsBodyHtml = true;

            var inlineLogo = new LinkedResource(ImagePath);
            inlineLogo.ContentId = Guid.NewGuid().ToString();

            
            var view = AlternateView.CreateAlternateViewFromString(string.Format(body, inlineLogo.ContentId), null, "text/html");
            view.LinkedResources.Add(inlineLogo);
            message.AlternateViews.Add(view);

            try
            {
                _client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                      ex.ToString());
            }             
        }
    }
}
