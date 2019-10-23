using System.Diagnostics;

namespace RetroGamingWebAPI.Infrastructure
{
    public class MailService : IMailService
    {
        public void SendMail(string message)
        {
            Debug.WriteLine($"Sending mail from mail service: {message}");
        }
    }
}
