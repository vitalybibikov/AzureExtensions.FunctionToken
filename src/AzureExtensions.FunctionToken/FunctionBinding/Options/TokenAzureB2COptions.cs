using System;
using AzureExtensions.FunctionToken.FunctionBinding.Options.Interface;

namespace AzureExtensions.FunctionToken.FunctionBinding.Options
{
    public sealed class TokenAzureB2COptions : ITokenOptions
    {
        public Uri AzureB2CSingingKeyUri { get; set; }

        public string Audience { get; set; }

        public string Issuer { get; set; }
    }
}
