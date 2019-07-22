using System.Linq;
using AzureExtensions.FunctionToken.FunctionBinding.Constants;
using Microsoft.AspNetCore.Http.Internal;

namespace AzureExtensions.FunctionToken.Extensions
{
    internal static class DefaultHttpRequestExtensions
    {
        public static bool TryGetBearerToken(this DefaultHttpRequest message, out string token)
        {
            token = null;
            var isAuthHeaderPresented = message.Headers.Any(x => x.Key == BearerConstants.AUTH_TYPE) &&
                message.Headers[BearerConstants.AUTH_TYPE].ToString().StartsWith(BearerConstants.BEARER);

            if (isAuthHeaderPresented)
            {
                message.Headers.TryGetValue(BearerConstants.AUTH_TYPE, out var headerValues);
                token = headerValues.FirstOrDefault()?.Substring(BearerConstants.BEARER.Length);
            }

            return token != null;
        }
    }
}
