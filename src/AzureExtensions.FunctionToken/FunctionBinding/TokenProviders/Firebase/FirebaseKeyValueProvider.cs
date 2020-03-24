using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AzureExtensions.FunctionToken.Exceptions;
using AzureExtensions.FunctionToken.FunctionBinding.Options;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.IdentityModel.Tokens;
using FirebaseAuthException = AzureExtensions.FunctionToken.Exceptions.FirebaseAuthException;

namespace AzureExtensions.FunctionToken.FunctionBinding.TokenProviders.Firebase
{
    /// <summary>
    /// Provides values loaded from Azure B2C.
    /// </summary>
    internal class FirebaseKeyValueProvider : BearerTokenValueProvider
    {
        private readonly FireBaseOptions options;

        /// <inheritdoc />
        public FirebaseKeyValueProvider(
            HttpRequest request,
            FireBaseOptions options,
            FunctionTokenAttribute attribute)
            : base(request, options, attribute, null)
        {
            this.options = options;
        }

        public override Task<TokenValidationParameters> GetTokenValidationParametersAsync()
        {
            var tokenParams = new TokenValidationParameters
            {
                ValidAudience = options.Audience,
                ValidIssuer = options.Issuer
            };

            return Task.FromResult(tokenParams);
        }

        public override async Task<ClaimsPrincipal> GetClaimsPrincipalAsync(string token, TokenValidationParameters validationParameters)
        {
            //currently there is no need to validate parameters
            // as this is performed on google's side.
            await LoadGoogleAuthJsonIfNotLoaded(options.GoogleServiceAccountJsonUri);

            try
            {
                var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                var claims = decoded.Claims.Select(claim => claim.ToClaim()).ToList();
                var identity = new ClaimsIdentity(claims, "Bearer");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                return claimsPrincipal;
            }
            catch (FirebaseException ex)
            {
                //todo: unfortunately  google does not provide neither proper exception nor an error code.
                //need to parse token and validate expiration manually
                if (ex.Message.Contains("expired"))
                {
                    throw new SecurityTokenExpiredException(ex.Message, ex);
                }
                else
                {
                    throw new SecurityTokenException(ex.Message, ex);
                }
            }
        }

        private async Task LoadGoogleAuthJsonIfNotLoaded(Uri uri)
        {
            try
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    using var httpClient = new HttpClient();
                    var result = await httpClient.GetAsync(uri);

                    if (result.IsSuccessStatusCode)
                    {
                        var stream = await result.Content.ReadAsStreamAsync();
                        FirebaseApp.Create(new AppOptions
                        {
                            Credential = GoogleCredential.FromStream(stream),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FirebaseAuthException("Failed to load Google Auth json file.", ex);
            }
        }
    }
}
