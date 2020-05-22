using System;
using AzureExtensions.FunctionToken.FunctionBinding;
using AzureExtensions.FunctionToken.FunctionBinding.Options;
using AzureExtensions.FunctionToken.FunctionBinding.Options.Interface;
using AzureExtensions.FunctionToken.FunctionBinding.TokenProviders.Firebase;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;

namespace AzureExtensions.FunctionToken.Extensions
{
    /// <summary>
    ///     Setup token by providing <see cref="ITokenOptions" />.
    /// </summary>
    public static class FunctionTokenExtensions
    {
        public static IWebJobsBuilder AddAzureFunctionsToken(this IWebJobsBuilder builder, ITokenOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            builder.AddExtension<FunctionTokenExtensionProvider>();
            builder.Services.AddSingleton(options);

            return builder;
        }

        public static IWebJobsBuilder AddFirebase(this IWebJobsBuilder builder, FireBaseOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            FirebaseFactory.Load(options.GoogleServiceAccountJsonUri).GetAwaiter().GetResult();

            builder.AddExtension<FunctionTokenExtensionProvider>();
            builder.Services.AddSingleton<ITokenOptions>(options);

            return builder;
        }
    }
}
