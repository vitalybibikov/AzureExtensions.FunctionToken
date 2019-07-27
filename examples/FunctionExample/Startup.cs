using System;
using AzureExtensions.FunctionToken.Extensions;
using AzureExtensions.FunctionToken.FunctionBinding.Options;
using FunctionExample;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

[assembly: FunctionsStartup(typeof(Startup))]

namespace FunctionExample
{
    internal class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            builder.AddAzureFunctionsToken(new TokenAzureB2COptions()
            {
                AzureB2CSingingKeyUri = new Uri("https://yourapp.b2clogin.com/yourapp.onmicrosoft.com/discovery/v2.0/keys?p=yourapp-policy"),
                Audience = "your audience",
                Issuer = "your issuer"
            });
        }
    }
}