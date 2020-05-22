using System;
using System.Security.Claims;
using AzureExtensions.FunctionToken.Extensions;
using AzureExtensions.FunctionToken.FunctionBinding.Options;
using FunctionExample;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            builder.Services.AddHttpContextAccessor();

            builder.AddFirebase(new FireBaseOptions()
            {
                GoogleServiceAccountJsonUri = new Uri("https://secretstorage.blob.core.windows.net/firebase-client-local/zalik-client-local-firebase-adminsdk-ujhgt-944902da1e.json?sp=r&st=2020-02-15T14:22:54Z&se=2099-02-15T22:22:54Z&spr=https&sv=2019-02-02&sr=b&sig=etW2CaJzVFr%2F4SaS9jDpbLNTtDXFC8BWp7FAJGV%2FU40%3D")
            });
        }
    }
}