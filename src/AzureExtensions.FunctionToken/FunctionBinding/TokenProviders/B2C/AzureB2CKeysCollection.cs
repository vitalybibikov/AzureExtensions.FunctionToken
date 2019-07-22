using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace AzureExtensions.FunctionToken.FunctionBinding.TokenProviders.B2C
{
    /// <summary>
    /// Mapped list of keys, that can be retrieved from jwks_uri of Azure B2C OpenId connect policy (flow) metadata endpoint
    /// <see href="https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-reference-tokens#validation">Link</see>.
    /// </summary>
    internal class AzureB2CKeysCollection
    {
        /// <summary>
        /// List of JsonWebKey (JWK)
        /// <see href="https://tools.ietf.org/html/rfc7517#section-4">Link</see>.
        /// </summary>
        [JsonProperty("keys")]
        public List<JsonWebKey> Keys { get; set; }
    }
}
