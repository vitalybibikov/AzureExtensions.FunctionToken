using System;
using System.Reflection;
using System.Threading.Tasks;
using AzureExtensions.FunctionToken.FunctionBinding.Options.Interface;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Triggers;

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
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            ParameterInfo parameter = context.Parameter;
            var attribute = parameter.GetCustomAttribute<FunctionTokenAttribute>(inherit: false);

            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            IBinding binding = new FunctionTokenBinding(options, attribute);
            return Task.FromResult(binding);
        }
    }
}
