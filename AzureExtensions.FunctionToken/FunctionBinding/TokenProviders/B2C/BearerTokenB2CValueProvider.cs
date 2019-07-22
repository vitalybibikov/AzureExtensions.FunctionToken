using System.Threading.Tasks;
using AzureExtensions.FunctionToken.FunctionBinding.Options;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.IdentityModel.Tokens;

namespace AzureExtensions.FunctionToken.FunctionBinding.TokenProviders.B2C
{
    /// <summary>
    /// Provides values loaded from Azure B2C.
    /// </summary>
    internal class BearerTokenB2CValueProvider : BearerTokenValueProvider
    {
        private readonly TokenAzureB2COptions options;

        /// <inheritdoc />
        public BearerTokenB2CValueProvider(DefaultHttpRequest request, TokenAzureB2COptions options)
            : base(request, options)
        {
            this.options = options;
        }

        public override async Task<TokenValidationParameters> GetTokenValidationParametersAsync()
        {
            var rsaWebKeys = await new AzureB2CTokensLoader()
                .Load(options.AzureB2CSingingKeyUri);

            var tokenParams = new TokenValidationParameters
            {
                RequireSignedTokens = true,

                ValidAudience = options.Audience,
                ValidateAudience = true,

                ValidIssuer = options.Issuer,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,

                ValidateLifetime = true,
                IssuerSigningKeys = rsaWebKeys,
            };
            return tokenParams;
        }

        public virtual async Task<object> GetValueAsync()
        {
            var result = await base.GetValueAsync();
            var functionResult = (FunctionTokenResult) result;

            if (functionResult.Exception != null)
            {
                await new AzureB2CTokensLoader()
                    .Reload(options.AzureB2CSingingKeyUri);

                result = await base.GetValueAsync();
            }

            return result;
        }
    }
}