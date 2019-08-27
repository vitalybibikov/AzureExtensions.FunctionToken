using System;
using System.Collections.Generic;
using AzureExtensions.FunctionToken.FunctionBinding.Enums;
using Microsoft.Azure.WebJobs.Description;

namespace AzureExtensions.FunctionToken
{
    /// <summary>
    ///  Attribute that supplies ClaimsPrincipal out of token on every request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public sealed class FunctionTokenAttribute : Attribute
    {
        public AuthLevel Auth { get;  }

        public List<string> Roles { get; } = new List<string>();

        public FunctionTokenAttribute(AuthLevel level = AuthLevel.Authorized)
        {
            Auth = level;
        }

        public FunctionTokenAttribute(AuthLevel level = AuthLevel.Authorized, params string[] roles)
        {
            if (roles != null)
            {
                Roles.AddRange(roles);
            }
           
            Auth = level;
        }

        public FunctionTokenAttribute(params string[] roles)
        {
            if (roles != null)
            {
                Auth = AuthLevel.Authorized;
                Roles.AddRange(roles);
            }
            else
            {
                Auth = AuthLevel.AllowAnonymous;
            }
        }
    }
}
