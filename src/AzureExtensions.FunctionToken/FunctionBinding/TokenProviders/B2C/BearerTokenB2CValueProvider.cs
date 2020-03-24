using System;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using AzureExtensions.FunctionToken.Extensions;
using AzureExtensions.FunctionToken.FunctionBinding.Options;

[assembly: InternalsVisibleTo("AzureExtensions.FunctionToken.Tests")]
namespace AzureExtensions.FunctionToken.FunctionBinding.TokenProviders.B2C
{
    /// <summary>
    /// Provides values loaded from Azure B2C.
    /// </summary>
    internal class BearerTokenB2CValueProvider : BearerTokenValueProvider
    {
        private const string ScopeClaimNameFromPrincipal = "http://schemas.microsoft.com/identity/claims/scope";
        private readonly TokenAzureB2COptions options;
        private IAzureB2CTokensLoader azureB2CTokensLoader;

        /// <inheritdoc />
        public BearerTokenB2CValueProvider(
            HttpRequest request, 
            TokenAzureB2COptions options, 
            FunctionTokenAttribute attribute)
            : this(request, options, attribute, new AzureB2CTokensLoader(), new JwtSecurityTokenHandler())
        {
        }

        public BearerTokenB2CValueProvider(
            HttpRequest request, 
            TokenAzureB2COptions options, 
            FunctionTokenAttribute attribute,
            IAzureB2CTokensLoader loader,
            ISecurityTokenValidator securityHandler)
            : base(request, options, attribute, securityHandler)
        {
            this.options = options;
            azureB2CTokensLoader = loader;
        }

        public override async Task<TokenValidationParameters> GetTokenValidationParametersAsync()
        {
            if (options.AzureB2CSingingKeyUri == null)
            {
                throw new ArgumentNullException(nameof(options.AzureB2CSingingKeyUri));
            }

            var rsaWebKeys = await azureB2CTokensLoader
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
        
        public override async Task<object> GetValueAsync()
        {
            var result = await base.GetValueAsync();
            var functionResult = (FunctionTokenResult)result;

            if (functionResult.Exception != null)
            {
                await azureB2CTokensLoader
                    .Reload(options.AzureB2CSingingKeyUri);

                result = await base.GetValueAsync();
            }

            return result;
        }

        protected override bool IsAuthorizedForAction(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.IsInScope(InputAttribute.ScopeRequired, ScopeClaimNameFromPrincipal)
                && claimsPrincipal.IsInRole(InputAttribute.Roles);
        }
    }
}