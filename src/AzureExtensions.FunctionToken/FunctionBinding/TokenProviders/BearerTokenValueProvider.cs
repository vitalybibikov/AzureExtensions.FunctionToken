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
        public FunctionTokenAttribute InputAttribute { get; }

        public DefaultHttpRequest Request { get; }

        public ITokenOptions Options { get; }

        /// <inheritdoc />
        protected BearerTokenValueProvider(
            DefaultHttpRequest request, 
            ITokenOptions options,
            FunctionTokenAttribute attribute)
        {
            InputAttribute = attribute;
            Request = request;
            Options = options;
        }

        /// <inheritdoc />
        public Type Type => typeof(ClaimsPrincipal);

        /// <inheritdoc />
        public virtual async Task<object> GetValueAsync()
        {
            var result = FunctionTokenResult.Anonymous(InputAttribute.Auth);

            try
            {
                if (Request.TryGetBearerToken(out var token))
                {
                    Request.HttpContext.User = null;
                    var validationParameters = await GetTokenValidationParametersAsync();

                    var claimsPrincipal = await GetClaimsPrincipalAsync(token, validationParameters);

                    result = FunctionTokenResult.Success(claimsPrincipal, InputAttribute.Auth);
                    Request.HttpContext.User = claimsPrincipal;
                }
            }
            catch (SecurityTokenExpiredException)
            {
                result = FunctionTokenResult.Expired(InputAttribute.Auth);
            }
            catch (Exception ex)
            {
                result = FunctionTokenResult.Error(ex, InputAttribute.Auth);
            }

            return result;
        }

        public virtual Task<ClaimsPrincipal> GetClaimsPrincipalAsync(
            string token,
            TokenValidationParameters validationParameters)
        {
            var claimsPrincipal = new JwtSecurityTokenHandler()
                .ValidateToken(token, validationParameters, out var securityToken);
            return Task.FromResult(claimsPrincipal);
        }

        public abstract Task<TokenValidationParameters> GetTokenValidationParametersAsync();

        /// <inheritdoc />
        public string ToInvokeString()
        {
            return string.Empty;
        }
    }
}
