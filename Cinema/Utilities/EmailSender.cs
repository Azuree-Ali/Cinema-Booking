using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace Cinema.Utilities
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("a.prins2004@gmail.com", "dusd mexn vlpx lffw")
            };

            var mail = new MailMessage(from: "a.prins2004@gmail.com",
                                to: email,
                                subject,
                                message
                                )
            {
                IsBodyHtml = true
            };
            return client.SendMailAsync(mail);
        }
    }
}
