using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using MenuToShopping.Shared.Models;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using Microsoft.Azure.Documents;

namespace MenuToShopping.Api
{
    public static class MenuFunctions
    {
        [FunctionName("GetAllMenus")]
        public static IActionResult GetAllMenus(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req,
            [CosmosDB(
                databaseName: "%MyDatabase%",
                collectionName: "%MyContainer%",
                ConnectionStringSetting = "CosmosDBConnectionString",
                SqlQuery = "SELECT * FROM c")]
                IEnumerable<Menu> menuList, ILogger log)
        {
            return new OkObjectResult(menuList);
        }

        [FunctionName("GetAllMenus")]
        public static IActionResult GetMenuById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetMenu/{partitionKey}/{id}")]
            HttpRequest req,
            [CosmosDB(
                databaseName: "%MyDatabase%",
                collectionName: "%MyContainer%",
                ConnectionStringSetting = "CosmosDBConnectionString",
                Id = "{id}",
                PartitionKey = "{partitionKey}")]
                Menu menuItem, ILogger log)
        {
            return new OkObjectResult(menuItem);
        }


        [FunctionName("AddMenu")]
        public async static Task<IActionResult> AddMenu(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            [CosmosDB(
                databaseName: "%MyDatabase%",
                collectionName: "%MyContainer%",
                ConnectionStringSetting = "CosmosDBConnectionString")]IAsyncCollector<Menu> MenuOut,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                Menu data = JsonConvert.DeserializeObject<Menu>(requestBody);
                await MenuOut.AddAsync(data);

                // Add a JSON document to the output container.
                //var result = await MenuOut.AddAsync(new
                //{
                //    // create a random ID
                //    id = System.Guid.NewGuid().ToString(),
                //    // altri campi
                //});

                var databaseName = Environment.GetEnvironmentVariable("MyDatabase");
                var containerName = Environment.GetEnvironmentVariable("MyContainer");
                return new CreatedResult(UriFactory.CreateDocumentUri(databaseName, containerName, data.Id.ToString()), data);
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("UpdateMenu")]
        public async static Task<IActionResult> UpdateMenu(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "UpdateMenu/{id}")]
            HttpRequest req,
            [CosmosDB(
                databaseName: "%MyDatabase%",
                collectionName: "%MyContainer%",
                ConnectionStringSetting = "CosmosDBConnectionString",
                Id = "{id}")]IAsyncCollector<Menu> menuOut,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                Menu data = JsonConvert.DeserializeObject<Menu>(requestBody);
                await menuOut.AddAsync(data);

                var databaseName = Environment.GetEnvironmentVariable("MyDatabase");
                var containerName = Environment.GetEnvironmentVariable("MyContainer");
                return new OkObjectResult(data);
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("DeleteMenu")]
        public static async Task<IActionResult> DeleteMenu(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteMenu/{id}")]
            HttpRequest request,
            [CosmosDB(
                databaseName: "%MyDatabase%",
                collectionName: "%MyContainer%",
                ConnectionStringSetting = "CosmosDBConnectionString",
                SqlQuery = "SELECT * FROM c WHERE c.id = {id}")]
            IEnumerable<Menu> menus,
            [CosmosDB(
                databaseName: "%MyDatabase%",
                collectionName: "%MyContainer%",
                ConnectionStringSetting = "CosmosDBConnectionString")]
            DocumentClient documentClient,
            string id,
            ILogger log)
        {
            if (menus == null || !menus.Any())
            {
                return new NotFoundResult();
            }

            var menu = menus.First();
            var uri = UriFactory.CreateDocumentUri("%MyDatabase%", "%MyContainer%", id);
            var options = new RequestOptions
            {
                PartitionKey = new PartitionKey(menu.Id)
            };

            await documentClient.DeleteDocumentAsync(uri, options);

            return new OkResult();
        }
    }
}
}
