using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AzureExtensions.FunctionToken.Exceptions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("AzureExtensions.FunctionToken.Tests")]
namespace AzureExtensions.FunctionToken.FunctionBinding.TokenProviders.B2C
{
    internal class AzureB2CTokensLoader : IAzureB2CTokensLoader
    {
        private static List<JsonWebKey> keys = new List<JsonWebKey>();

        /// <summary>
        /// Loads the keys by uri or from static cache.
        /// </summary>
        public async Task<List<JsonWebKey>> Load(Uri uri)
        {
            if (keys.Count > 0)
            {
                return keys;
            }

            return await Reload(uri);
        }

        /// <summary>
        /// Loads public keys and  updates static cache.
        /// As Azure B2C might update public keys, that are loaded by metadata Uri from time to time.
        /// </summary>
        public async Task<List<JsonWebKey>> Reload(Uri uri)
        {
            using (var client = new HttpClient())
            {
                using (var result = await client.GetAsync(uri))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        var json = await result.Content.ReadAsStringAsync();
                        keys = JsonConvert.DeserializeObject<AzureB2CKeysCollection>(json).Keys;

                        if (keys.Count == 0)
                        {
                            throw new AzureB2CTokenLoadException("Request for keys was successful, but nothing was retrieved");
                        }

                        return keys;
                    }

                    throw new AzureB2CTokenLoadException("Failed to load token.", result.StatusCode);
                }
            }
        }
    }

    public interface IAzureB2CTokensLoader
    {
        Task<List<JsonWebKey>> Load(Uri uri);
        Task<List<JsonWebKey>> Reload(Uri uri);
    }
}
