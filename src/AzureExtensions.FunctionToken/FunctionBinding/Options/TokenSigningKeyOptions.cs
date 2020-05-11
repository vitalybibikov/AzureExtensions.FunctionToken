using AzureExtensions.FunctionToken.FunctionBinding.Options.Interface;
using Microsoft.IdentityModel.Tokens;

namespace AzureExtensions.FunctionToken.FunctionBinding.Options
{
    public sealed class TokenSigningKeyOptions : ITokenOptions
    {
        public JsonWebKey SigningKey { get; set; }

        public string Audience { get; set; }

        public string Issuer { get; set; }
    }
}
