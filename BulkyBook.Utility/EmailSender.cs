using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string _email, string subject, string htmlMessage)
        {
            // create email
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("dev.djvan@gmail.com"));
            email.To.Add(MailboxAddress.Parse(_email));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html){ Text = htmlMessage};

            // send email

            using (var mailClient = new SmtpClient())
            {
                mailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                mailClient.Authenticate("dev.djvan@gmail.com", "azhibiifkwncowfm");
                mailClient.Send(email);
                mailClient.Disconnect(true);
                mailClient.Dispose();
            }

            return Task.CompletedTask;
        }
    }
}
