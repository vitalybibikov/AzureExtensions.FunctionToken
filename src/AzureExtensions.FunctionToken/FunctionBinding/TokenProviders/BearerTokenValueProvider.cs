using System;
using System.Security.AccessControl;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using AzureExtensions.FunctionToken.Extensions;
using AzureExtensions.FunctionToken.FunctionBinding.Options.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.IdentityModel.Tokens;

namespace AzureExtensions.FunctionToken.FunctionBinding.TokenProviders
{
    internal abstract class BearerTokenValueProvider : IValueProvider
    {
        public FunctionTokenAttribute InputAttribute { get; }

        public HttpRequest Request { get; }

        public ITokenOptions Options { get; }
        private ISecurityTokenValidator securityTokenValidator;

        /// <inheritdoc />
        protected BearerTokenValueProvider(
            HttpRequest request, 
            ITokenOptions options,
            FunctionTokenAttribute attribute,
            ISecurityTokenValidator tokenValidator)
        {
            InputAttribute = attribute;
            Request = request;
            Options = options;
            securityTokenValidator = tokenValidator;
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

                    if (!IsAuthorizedForAction(claimsPrincipal))
                    {
                        throw new PrivilegeNotHeldException($"User is not in a valid role. Valid roles include: {string.Join(" ", InputAttribute.Roles)}.");
                    }
                    else
                    {
                        result = FunctionTokenResult.Success(claimsPrincipal, InputAttribute.Auth);
                    }

                    Request.HttpContext.User = claimsPrincipal;
                }
                else
                {
                    throw new AuthenticationException("No authentication provided in request.");
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
            TokenValidationParameters validationParameters
        )
        {
            return Task.FromResult(
                securityTokenValidator.ValidateToken(
                    token,
                    validationParameters,
                    out var securityToken
                )
            );
        }

        public abstract Task<TokenValidationParameters> GetTokenValidationParametersAsync();

        /// <inheritdoc />
        public string ToInvokeString()
        {
            return string.Empty;
        }

        /// <summary>
        /// Overridable 
        /// </summary>
        protected virtual bool IsAuthorizedForAction(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.IsInScope(InputAttribute.ScopeRequired)
                && claimsPrincipal.IsInRole(InputAttribute.Roles);
        }
    }
}