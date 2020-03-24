using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using AzureExtensions.FunctionToken.FunctionBinding.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.IdentityModel.Tokens;

namespace AzureExtensions.FunctionToken.FunctionBinding.TokenProviders.SigningKey
{
    /// <summary>
    /// Provides values loaded from Azure B2C.
    /// </summary>
    internal class SigningKeyValueProvider : BearerTokenValueProvider
    {
        private readonly TokenSinginingKeyOptions options;

        /// <inheritdoc />
        public SigningKeyValueProvider(
            HttpRequest request,
            TokenSinginingKeyOptions options,
            FunctionTokenAttribute attribute)
            : this(request, options, attribute, new JwtSecurityTokenHandler())
        {
            this.options = options;
        }

        public SigningKeyValueProvider(
            HttpRequest request,
            TokenSinginingKeyOptions options,
            FunctionTokenAttribute attribute,
            ISecurityTokenValidator securityHandler)
            : base(request, options, attribute, securityHandler)
        {
            this.options = options;
        }

        public override Task<TokenValidationParameters> GetTokenValidationParametersAsync()
        {
            if (options.SigningKey == null)
            {
                throw new ArgumentNullException(nameof(options.SigningKey));
            }

            var tokenParams = new TokenValidationParameters
            {
                RequireSignedTokens = true,

                ValidAudience = options.Audience,
                ValidateAudience = true,

                ValidIssuer = options.Issuer,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,

                ValidateLifetime = true,
                IssuerSigningKeys = new List<SecurityKey> { options.SigningKey },
            };

            return Task.FromResult(tokenParams);
        }
    }
}