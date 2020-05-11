using System;
using System.Collections.Generic;
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
    internal class SingingKeyValueProvider : BearerTokenValueProvider
    {
        private readonly TokenSigningKeyOptions options;

        /// <inheritdoc />
        public SingingKeyValueProvider(
            HttpRequest request,
            TokenSigningKeyOptions options,
            FunctionTokenAttribute attribute)
            : base(request, options, attribute)
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