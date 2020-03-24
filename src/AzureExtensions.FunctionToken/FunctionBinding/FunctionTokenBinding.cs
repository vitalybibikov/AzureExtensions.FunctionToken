using System;
using System.Threading.Tasks;
using AzureExtensions.FunctionToken.FunctionBinding.Options;
using AzureExtensions.FunctionToken.FunctionBinding.Options.Interface;
using AzureExtensions.FunctionToken.FunctionBinding.TokenProviders.B2C;
using AzureExtensions.FunctionToken.FunctionBinding.TokenProviders.Firebase;
using AzureExtensions.FunctionToken.FunctionBinding.TokenProviders.SigningKey;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace AzureExtensions.FunctionToken.FunctionBinding
{
    /// <summary>
    /// Binder obtains data from every Azure Function request's context. Creates a value provider on every call.
    /// </summary>
    internal sealed class FunctionTokenBinding : IBinding
    {
        private readonly ITokenOptions options;
        private readonly FunctionTokenAttribute attribute;

        public FunctionTokenBinding(ITokenOptions options, FunctionTokenAttribute attribute)
        {
            this.options = options;
            this.attribute = attribute;
        }

        /// <inheritdoc />
        public bool FromAttribute => true;

        /// <inheritdoc />
        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            var request = context.BindingData["$request"] as HttpRequest;

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (options is TokenAzureB2COptions tokenAzureB2COptions)
            {
                return Task.FromResult<IValueProvider>(
                    new BearerTokenB2CValueProvider(
                        request,
                        tokenAzureB2COptions,
                        attribute));
            }
            else if (options is TokenSinginingKeyOptions tokenSinginingKeyOptions)
            {
                return Task.FromResult<IValueProvider>(
                    new SigningKeyValueProvider(
                        request,
                        tokenSinginingKeyOptions,
                        attribute));
            }
            else if (options is FireBaseOptions fireOptions)
            {
                return Task.FromResult<IValueProvider>(
                    new FirebaseKeyValueProvider(
                        request,
                        fireOptions,
                        attribute));
            }
            else
            {
                throw new NotSupportedException(options.GetType().ToString());
            }
        }

        /// <inheritdoc />
        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            return null;
        }

        /// <inheritdoc />
        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor();
        }
    }
}
