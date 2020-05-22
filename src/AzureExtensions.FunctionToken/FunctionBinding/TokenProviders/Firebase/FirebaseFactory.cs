using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AzureExtensions.FunctionToken.Exceptions;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace AzureExtensions.FunctionToken.FunctionBinding.TokenProviders.Firebase
{
    public class FirebaseFactory
    {
        public static async Task Load(Uri uri)
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
