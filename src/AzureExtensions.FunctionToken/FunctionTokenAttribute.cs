using System;
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

        public FunctionTokenAttribute(AuthLevel level = AuthLevel.Authorized)
        {
            Auth = level;
        }
    }
}
