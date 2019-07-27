using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AzureExtensions.FunctionToken;
using AzureExtensions.FunctionToken.FunctionBinding.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FunctionExample
{
    public class Example
    {
        [FunctionName("Example")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestMessage req,
            [FunctionToken(AuthLevel.Authorized)] FunctionTokenResult token,
            ILogger log)
        {
            return await Handler.WrapAsync(token,async () =>
            {
                log.LogInformation("C# HTTP trigger function processed a request.");
                return new OkObjectResult($"Hello, {token}");
            });
        }

    }
}