using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace OnDemandPurchase
{
    public static class EmailLicenseFile
    {
        [FunctionName("EmailLicenseFile")]
        public static void Run([BlobTrigger("licenses/{orderId}.lic", Connection = "AzureWebJobsStorage")]string licenseFileContent,
            [SendGrid(ApiKey = "SendGridApiKey")] ICollector<SendGridMessage> sender,
            [Table("orders", "orders", "{orderId}")] Order order,
            string orderId, 
            ILogger log)
        {
            var email = order.Email;
            log.LogInformation($"Got order from {email}\nLicense file name: {orderId}");

            var message = new SendGridMessage();

            //Sender
            message.From = new EmailAddress(Environment.GetEnvironmentVariable("EmailSender"));

            //Receiver
            message.AddTo(email);

            //Adding attachment
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(licenseFileContent);
            var base64 = Convert.ToBase64String(plainTextBytes);
            message.AddAttachment(orderId, base64, "text/plain");

            //Subject
            message.Subject = "Your license key";

            //Content
            message.HtmlContent = "Thank you for your order";

            if (!email.EndsWith("@test.com"))
            {
                sender.Add(message);
            }
        }
    }
}
