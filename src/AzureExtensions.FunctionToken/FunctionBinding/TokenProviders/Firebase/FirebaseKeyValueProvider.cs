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
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.IdentityModel.Tokens;

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
            DefaultHttpRequest request,
            FireBaseOptions options,
            FunctionTokenAttribute attribute)
            : base(request, options, attribute)
        {
            this.options = options;
        }

        public override Task<TokenValidationParameters> GetTokenValidationParametersAsync()
        {
            var tokenParams = new TokenValidationParameters
            {
                RequireSignedTokens = true,

                ValidAudience = options.Audience,
                ValidateAudience = true,

                ValidIssuer = options.Issuer,
                ValidateIssuer = true,

                ValidateLifetime = true,
            };
            return Task.FromResult(tokenParams);
        }

        public override async Task<object> GetValueAsync()
        {
            var result = await base.GetValueAsync();
            var functionResult = (FunctionTokenResult)result;
            return result;
        }

        public override async Task<ClaimsPrincipal> GetClaimsPrincipalAsync(string token, TokenValidationParameters validationParameters)
        {
            await LoadGoogleAuthJsonIfNotLoaded(options.GoogleServiceAccountJsonUri);

            var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            var claims = decoded.Claims.Select(claim => claim.ToClaim()).ToList();

            var identity = new ClaimsIdentity(claims, "Bearer");

            var claimsPrincipal = new ClaimsPrincipal(identity);
            return claimsPrincipal;
        }

        private async Task LoadGoogleAuthJsonIfNotLoaded(Uri uri)
        {
            try
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    using (var client = new HttpClient())
                    {
                        using (var result = await client.GetAsync(uri))
                        {
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
                }
            }
            catch (Exception ex)
            {
                throw new FirebaseAuthException("Failed to load Google Auth json file.", ex);
            }
        }
    }
}
