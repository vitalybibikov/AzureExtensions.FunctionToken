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

            builder.AddAzureFunctionsToken(new FireBaseOptions()
            {
                GoogleServiceAccountJsonUri = new Uri("https://secretstorage.blob.core.windows.net/firebase/zalik-243111-firebase-adminsdk-83897-987d10f2db.json?sp=r&st=2019-08-17T10:10:57Z&se=2099-08-17T18:10:57Z&spr=https&sv=2018-03-28&sig=nNylTGVvFHvEr3gvuxD5V8w1B4yC1zvFXB2p9AOP%2BC0%3D&sr=b")
            });
        }
    }
}