using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AzureExtensions.FunctionToken.FunctionBinding.Enums;
using Microsoft.Azure.WebJobs.Description;

[assembly: InternalsVisibleTo("AzureExtensions.FunctionToken.Tests")]
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

        public string ScopeRequired { get; }
        public List<string> Roles { get; }

        public FunctionTokenAttribute(
            AuthLevel level,
            string scope,
            string[] roles
        )
        {
            Auth = level;
            ScopeRequired = scope;
            Roles = new List<string>();
            if (roles != null)
            {
                Roles.AddRange(roles);
            }
        }

        public FunctionTokenAttribute(AuthLevel level = AuthLevel.Authorized):
        this (
            level,
            "",
            null
        )
        {
        }

        public FunctionTokenAttribute(string[] roles) :
        this (
            roles != null ? AuthLevel.Authorized : AuthLevel.AllowAnonymous,
            "",
            roles
        )
        {
        }
    }
}
