using System;
using AzureExtensions.FunctionToken.FunctionBinding;
using AzureExtensions.FunctionToken.FunctionBinding.Options.Interface;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;

namespace AzureExtensions.FunctionToken.Extensions
{
    /// <summary>
    ///     Setup token by providing <see cref="ITokenOptions" />.
    /// </summary>
    public static class FunctionTokenExtensions
    {
        public static void AddAzureFunctionsToken(this IWebJobsBuilder builder, ITokenOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            builder.AddExtension<FunctionTokenExtensionProvider>();
            builder.Services.AddSingleton(options);
        }
    }
}
