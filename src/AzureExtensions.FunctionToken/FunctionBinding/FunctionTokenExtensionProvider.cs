using AzureExtensions.FunctionToken.FunctionBinding.Options.Interface;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;

namespace AzureExtensions.FunctionToken.FunctionBinding
{
    /// <summary>
    /// Defines an extension to provide Custom Azure Function Token.
    /// </summary>
    [Extension("FunctionTokenExtensionProvider")]
    internal sealed class FunctionTokenExtensionProvider : IExtensionConfigProvider
    {
        private readonly ITokenOptions options;

        public FunctionTokenExtensionProvider(ITokenOptions options)
        {
            this.options = options;
        }

        /// <inheritdoc />
        public void Initialize(ExtensionConfigContext context)
        {
            var provider = new FunctionTokenBindingProvider(options);
            var rule = context.AddBindingRule<FunctionTokenAttribute>()
                .Bind(provider);
        }
    }
}
