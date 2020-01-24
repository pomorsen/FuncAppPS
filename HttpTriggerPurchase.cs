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
            ILogger log)
        {
            log.LogInformation("Received a payment");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var order = JsonConvert.DeserializeObject<Order>(requestBody);

            await orderQueue.AddAsync(order); // asynchroniczne wysyłanie wiadomośći do kolejki

            log.LogInformation($"Order {order.OrderId} recieved from {order.Email} for product {order.ProductId}");
            
            return new OkObjectResult($"Thank you for your purchase!");
        }
    }

public class Order{
        public string OrderId;
        public string ProductId;
        public string Email;


        public string getOrderId()
        {
            return this.OrderId;
        }

        public void setOrderId(string OrderId)
        {
            this.OrderId = OrderId;
        }

        public string getProductId()
        {
            return this.ProductId;
        }

        public void setProductId(string ProductId)
        {
            this.ProductId = ProductId;
        }

        public string getEmail()
        {
            return this.Email;
        }

        public void setEmail(string Email)
        {
            this.Email = Email;
        }


    }


}

