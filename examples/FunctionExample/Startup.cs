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

            builder.AddAzureFunctionsToken(new TokenSinginingKeyOptions()
            {
                //AzureB2CSingingKeyUri = new Uri("https://loyaltyprogramapp.b2clogin.com/loyaltyprogramapp.onmicrosoft.com/discovery/v2.0/keys?p=b2c_1_google-web-dev-policy"),
                SigningKey = new JsonWebKey("{\"kid\":\"X5eXk4xyojNFum1kl2Ytv8dlNP4-c57dO6QGTVBwaNk\",\"nbf\":1493763266,\"use\":\"sig\",\"kty\":\"RSA\",\"e\":\"AQAB\",\"n\":\"tVKUtcx_n9rt5afY_2WFNvU6PlFMggCatsZ3l4RjKxH0jgdLq6CScb0P3ZGXYbPzXvmmLiWZizpb-h0qup5jznOvOr-Dhw9908584BSgC83YacjWNqEK3urxhyE2jWjwRm2N95WGgb5mzE5XmZIvkvyXnn7X8dvgFPF5QwIngGsDG8LyHuJWlaDhr_EPLMW4wHvH0zZCuRMARIJmmqiMy3VD4ftq4nS5s8vJL0pVSrkuNojtokp84AtkADCDU_BUhrc2sIgfnvZ03koCQRoZmWiHu86SuJZYkDFstVTVSR0hiXudFlfQ2rOhPlpObmku68lXw-7V-P7jwrQRFfQVXw\"}"),
                Audience = "7dff948f-203c-452c-9f46-f8254cb61009",
                Issuer = "https://loyaltyprogramapp.b2clogin.com/73878e96-491d-4eac-97b2-5c77688cbbed/v2.0/"
            });
        }
    }
}