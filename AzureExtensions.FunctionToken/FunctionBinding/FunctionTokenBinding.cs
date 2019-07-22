﻿using System;
using System.Threading.Tasks;
using AzureExtensions.FunctionToken.FunctionBinding.Options;
using AzureExtensions.FunctionToken.FunctionBinding.Options.Interface;
using AzureExtensions.FunctionToken.FunctionBinding.TokenProviders.B2C;
using Microsoft.AspNetCore.Http.Internal;
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

        public FunctionTokenBinding(ITokenOptions options)
        {
            this.options = options;
        }

        /// <inheritdoc />
        public bool FromAttribute => true;

        /// <inheritdoc />
        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            var request = context.BindingData["$request"] as DefaultHttpRequest;

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (options is TokenAzureB2COptions tokenAzureB2COptions)
            {
                return Task.FromResult<IValueProvider>(
                    new BearerTokenB2CValueProvider(
                        request,
                        tokenAzureB2COptions));
            }
            else
            {
                throw new NotSupportedException();
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
