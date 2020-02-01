using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OnDemandPurchase
{
    public static class HttpTriggerPurchase
    {
        [FunctionName("HttpTriggerPurchase")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Queue("orders")] IAsyncCollector<Order> orderQueue,//pozwala na wysyłanie do FA wiadomości z kolejki
            [Table("orders")] IAsyncCollector<Order> orderTable,
            ILogger log)
        {
            log.LogInformation("Received a payment");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var order = JsonConvert.DeserializeObject<Order>(requestBody);

            await orderQueue.AddAsync(order); // asynchroniczne wysyłanie wiadomośći do kolejki

            order.PartitionKey = "orders"; // just one partition
            order.RowKey = order.OrderId;
            await orderTable.AddAsync(order); 


            log.LogInformation($"Order {order.OrderId} recieved from {order.Email} for product {order.ProductId}");
            
            return new OkObjectResult($"Thank you for your purchase!");
        }
    }

public class Order{
        private string orderId;
        private string productId;
        private string email;
        private string partitionKey;
        private string rowKey;

        public string PartitionKey { get => partitionKey; set => partitionKey = value; }
        public string RowKey { get => rowKey; set => rowKey = value; }
        public string Email { get => email; set => email = value; }
        public string ProductId { get => productId; set => productId = value; }
        public string OrderId { get => orderId; set => orderId = value; }




    }


}

