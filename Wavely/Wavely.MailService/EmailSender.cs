
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfigration emailConfigration;

        public EmailSender(EmailConfigration _emailConfigration)
        {
            emailConfigration = _emailConfigration;
        }
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailmessage(message);
            Send(emailMessage);

        }

        private void Send(MimeMessage emailMessage)
        {
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    client.Connect(emailConfigration.SmtpServer, emailConfigration.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(emailConfigration.UserName, emailConfigration.Password);
                    client.Send(emailMessage);
                }
              
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }

        }

        private MimeMessage CreateEmailmessage(Message message)
        {

            var verfication = new Random().Next(500, 90000);
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("From TabibApp", emailConfigration.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;



            emailMessage.Body = new TextPart(TextFormat.Html)
            {

                Text = message.Content
            };

            return emailMessage;
        }
    }
}
