using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
