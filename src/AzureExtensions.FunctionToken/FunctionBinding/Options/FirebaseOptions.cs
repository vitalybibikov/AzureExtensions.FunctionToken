using System;
using AzureExtensions.FunctionToken.FunctionBinding.Options.Interface;

namespace AzureExtensions.FunctionToken.FunctionBinding.Options
{
    public class FireBaseOptions : ITokenOptions
    {
        public string Audience { get; set; }

        public string Issuer { get; set; }

        public Uri GoogleServiceAccountJsonUri { get; set; }
    }
}
