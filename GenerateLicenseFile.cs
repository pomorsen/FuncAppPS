using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace OnDemandPurchase
{
    public static class GenerateLicenseFile
    {
        [FunctionName("GenerateLicenseFile")]
        public static async System.Threading.Tasks.Task RunAsync(
            [QueueTrigger("orders", Connection = "AzureWebJobsStorage")]Order order,
            IBinder binder,
            ILogger log
            )
        {

            var outputBlob = await binder.BindAsync<TextWriter>(
                new BlobAttribute($"licenses/{order.OrderId}.lic")
                {
                    Connection = "AzureWebJobsStorage"
            });

            outputBlob.WriteLine($"OrderId:  {order.OrderId}");
            outputBlob.WriteLine($"Email:  {order.Email}");
            outputBlob.WriteLine($"ProductId:  {order.ProductId}");
            outputBlob.WriteLine($"PurchaseDate:  {DateTime.UtcNow}");

            var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(order.Email + "secret"));
            outputBlob.WriteLine($"SecretCode: {BitConverter.ToString(hash).Replace("-", "")}");

        }
    }
}
