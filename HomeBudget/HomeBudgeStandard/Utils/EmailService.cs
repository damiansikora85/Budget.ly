using System;
using System.Collections.Generic;
using System.Text;
using MailKit.Net.Smtp;
using MimeKit;

namespace HomeBudgeStandard.Utils
{
    public class EmailService
    {
        public static void SendMessage()
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("SmartMonitor", "damiansikora@vivaldi.net"));
                message.To.Add(new MailboxAddress("Certas", "dsikora85@gmail.com"));
                message.Subject = "Smart Monitor - contact";

                message.Body = new TextPart("plain")
                {
                    Text = $"Name: {nameEntry.Text}\n" +
                    $"Phone: {phoneEntry.Text}\n" +
                    $"Email: {emailEntry.Text}\n" +
                    $"Message: {messageEntry.Text}"
                };

                using (var client = new SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect("smtp.vivaldi.net", 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("damiansikora", "********");

                    client.Send(message);
                    client.Disconnect(true);
                }

                await UserDialogs.Instance.AlertAsync(AppResources.CONTACT_EMAIL_SEND, AppResources.SUCCESS_TITLE);
            }
            catch (Exception ex)
            {
                await UserDialogs.Instance.AlertAsync("Failed: " + ex.Message, AppResources.ERROR_TITLE);
            }
        }
    }
}
