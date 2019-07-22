using System;
using System.IO;
using System.Threading.Tasks;
using AzureExtensions.FunctionToken;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionExample
{
    public class Example
    {
        [FunctionName("Example")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [FunctionToken] FunctionTokenResult token,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            return (ActionResult) new OkObjectResult($"Hello, {token}");
        }
    }
}
