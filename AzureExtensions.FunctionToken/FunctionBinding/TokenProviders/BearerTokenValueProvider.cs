using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AzureExtensions.FunctionToken.Extensions;
using AzureExtensions.FunctionToken.FunctionBinding.Options.Interface;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.IdentityModel.Tokens;

namespace AzureExtensions.FunctionToken.FunctionBinding.TokenProviders
{
    internal abstract class BearerTokenValueProvider : IValueProvider
    {
        public DefaultHttpRequest Request { get; }

        public ITokenOptions Options { get; }

        /// <inheritdoc />
        protected BearerTokenValueProvider(DefaultHttpRequest request, ITokenOptions options)
        {
            Request = request;
            Options = options;
        }

        /// <inheritdoc />
        public Type Type => typeof(ClaimsPrincipal);

        /// <inheritdoc />
        public virtual async Task<object> GetValueAsync()
        {
            try
            {
                if (Request.TryGetBearerToken(out var token))
                {
                    var validationParameters = await GetTokenValidationParametersAsync();
                    var claimsPrincipal = new JwtSecurityTokenHandler()
                        .ValidateToken(token, validationParameters, out var securityToken);

                    return FunctionTokenResult.Success(claimsPrincipal);
                }

                return FunctionTokenResult.Anonymous();
            }
            catch (SecurityTokenExpiredException)
            {
                return FunctionTokenResult.Expired();
            }
            catch (Exception ex)
            {
                return FunctionTokenResult.Error(ex);
            }
        }

        public abstract Task<TokenValidationParameters> GetTokenValidationParametersAsync();

        /// <inheritdoc />
        public string ToInvokeString()
        {
            return string.Empty;
        }
    }
}
