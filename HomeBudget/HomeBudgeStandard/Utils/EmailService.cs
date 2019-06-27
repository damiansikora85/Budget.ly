using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using MailKit.Net.Smtp;
using MimeKit;

namespace HomeBudgeStandard.Utils
{
    public class EmailService
    {
        public static async Task SendMessage(string messageText)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Budget.ly", "budgetlymobile@gmail.com"));
                message.To.Add(new MailboxAddress("Dark Tower Lab", "darktowerlab@gmail.com"));
                message.Subject = "[Budget.ly][Logi]";

                message.Body = new TextPart("plain")
                {
                    Text = messageText
                };

                using (var client = new SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect("smtp.gmail.com", 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("budgetlymobile", "damianiola");

                    client.Send(message);
                    client.Disconnect(true);
                }

                await UserDialogs.Instance.AlertAsync("Sukces", "Dane wysłane");
            }
            catch (Exception ex)
            {
                await UserDialogs.Instance.AlertAsync("Nie można wysłać danych: " + ex.Message, "Błąd");
            }
        }
    }
}
