using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AsapTasks.Data;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AsapTasks.Services
{
    static class EmailService
    {
        private static string apiKey = Constants.SendGridApiKey;
        private static SendGridClient client = new SendGridClient(apiKey);

        private static SendGridMessage fn_CreateMessage(EmailAddress recipient, string verificationCode)
        {
            var msg = new SendGridMessage();

            msg.SetFrom(new EmailAddress("no-reply@asaptasks.com", "Asap Tasks"));

            //var recipients = new List<EmailAddress>{
            //    new EmailAddress("jeff@example.com", "Jeff Smith"),
            //    new EmailAddress("anna@example.com", "Anna Lidman"),
            //    new EmailAddress("peter@example.com", "Peter Saddow")
            //};

            msg.AddTo(recipient);

            msg.SetSubject("Verification Code");

            //msg.AddContent(MimeType.Text, "Your Verification Code is:");
            //msg.AddContent(MimeType.Html, "<h2>" + "Your Verification Code is:" + "</h2><br>");
            msg.AddContent(MimeType.Html, "<h2>Your Verification Code is: <b>" + verificationCode + "</b></h2>");
            return msg;
        }

        public static async Task SendEmail(Developer d, string verificationCode)
        {
            EmailAddress recipient = new EmailAddress(d.Email, d.Name);
            var msg = fn_CreateMessage(recipient, verificationCode);
            //msg.AddTo(new EmailAddress("test@example.com", "Test User"));
            var response = await client.SendEmailAsync(msg);
            System.Diagnostics.Debug.WriteLine("-------Email Response : " + response.StatusCode);
            System.Diagnostics.Debug.WriteLine("-------Email Response : " + response.Headers);
            System.Diagnostics.Debug.WriteLine("-------Email Response : " + response.Body);
        }
    }
}