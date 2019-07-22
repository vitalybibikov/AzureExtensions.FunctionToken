using System;
using System.Threading.Tasks;
using AzureExtensions.FunctionToken.FunctionBinding.Options.Interface;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace AzureExtensions.FunctionToken.FunctionBinding
{
    internal sealed class FunctionTokenBindingProvider : IBindingProvider
    {
        private readonly ITokenOptions options;

        public FunctionTokenBindingProvider(ITokenOptions options)
        {
            this.options = options;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            IBinding binding = new FunctionTokenBinding(options);
            return Task.FromResult(binding);
        }
    }
}
