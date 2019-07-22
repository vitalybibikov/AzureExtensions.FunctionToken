using System;
using System.Security.Claims;
using AzureExtensions.FunctionToken.FunctionBinding.Enums;

namespace AzureExtensions.FunctionToken
{
    public struct FunctionTokenResult
    {
        public ClaimsPrincipal Principal { get; private set; }

        public TokenStatus Status { get; private set; }

        public Exception Exception { get; private set; }

        public static FunctionTokenResult Success(ClaimsPrincipal principal)
        {
            return new FunctionTokenResult
            {
                Principal = principal,
                Status = TokenStatus.Valid
            };
        }

        public static FunctionTokenResult Expired()
        {
            return new FunctionTokenResult
            {
                Status = TokenStatus.Expired
            };
        }

        public static FunctionTokenResult Error(Exception ex)
        {
            return new FunctionTokenResult
            {
                Status = TokenStatus.Error,
                Exception = ex,
            };
        }

        public static FunctionTokenResult Anonymous()
        {
            return new FunctionTokenResult
            {
                Status = TokenStatus.Anonymous
            };
        }
    }
}
