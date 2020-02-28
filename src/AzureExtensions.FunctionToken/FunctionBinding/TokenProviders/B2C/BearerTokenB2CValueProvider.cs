using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AzureExtensions.FunctionToken.FunctionBinding.Options;
using Microsoft.AspNetCore.Http;
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
        public BearerTokenB2CValueProvider(
            HttpRequest request, 
            TokenAzureB2COptions options, 
            FunctionTokenAttribute attribute)
            : base(request, options, attribute)
        {
            this.options = options;
        }

        public override async Task<TokenValidationParameters> GetTokenValidationParametersAsync()
        {
            if (options.AzureB2CSingingKeyUri == null)
            {
                throw new ArgumentNullException(nameof(options.AzureB2CSingingKeyUri));
            }

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

        public override Task<ClaimsPrincipal> GetClaimsPrincipalAsync(
            string token,
            TokenValidationParameters validationParameters)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            jwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>
            {
                { "scp", ClaimTypes.Role }
            };
            jwtSecurityTokenHandler.MapInboundClaims = true;

            return Task.FromResult(
                jwtSecurityTokenHandler.ValidateToken(
                    token,
                    validationParameters,
                    out var securityToken
                )
            );
        }
        
        public override async Task<object> GetValueAsync()
        {
            var result = await base.GetValueAsync();
            var functionResult = (FunctionTokenResult)result;

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