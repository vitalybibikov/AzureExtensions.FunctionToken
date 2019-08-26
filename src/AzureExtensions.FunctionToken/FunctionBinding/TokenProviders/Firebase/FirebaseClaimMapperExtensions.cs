using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AzureExtensions.FunctionToken.FunctionBinding.TokenProviders.Firebase
{
    public static class FirebaseClaimMapperExtensions
    {
        private static readonly Dictionary<string, string> Claims = new Dictionary<string, string>()
        {
            { "phone_number", ClaimTypes.MobilePhone },
            { "user_id", ClaimTypes.NameIdentifier },
            { "email", JwtRegisteredClaimNames.Email },
            { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", ClaimTypes.Email },
            { "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", ClaimTypes.Role }
        };

        public static Claim ToClaim(this KeyValuePair<string, object> dictionaryClaim)
        {
            Claim claim;
            if (Claims.ContainsKey(dictionaryClaim.Key))
            {
                var value = Claims[dictionaryClaim.Key];
                claim = new Claim(value, dictionaryClaim.Value.ToString(), dictionaryClaim.Value.GetType().ToString()); 
            }
            else
            {
                claim = new Claim(dictionaryClaim.Key, dictionaryClaim.Value.ToString(), dictionaryClaim.Value.GetType().ToString());
            }

            return claim;
        }
    }
}
