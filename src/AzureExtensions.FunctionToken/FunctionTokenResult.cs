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

        public AuthLevel Level { get; private set; }

        public static FunctionTokenResult Success(ClaimsPrincipal principal, AuthLevel level)
        {
            return new FunctionTokenResult
            {
                Principal = principal,
                Status = TokenStatus.Valid,
                Level = level
            };
        }

        public static FunctionTokenResult Expired(AuthLevel level)
        {
            return new FunctionTokenResult
            {
                Status = TokenStatus.Expired,
                Level = level
            };
        }

        public static FunctionTokenResult Error(Exception ex, AuthLevel level)
        {
            return new FunctionTokenResult
            {
                Status = TokenStatus.Error,
                Exception = ex,
                Level = level
            };
        }

        public static FunctionTokenResult Anonymous(AuthLevel level)
        {
            return new FunctionTokenResult
            {
                Status = TokenStatus.Anonymous,
                Level = level
            };
        }

        public void ValidateThrow()
        {
            if (Level == AuthLevel.Authorized && Status != TokenStatus.Valid)
            {
                throw Exception;
            }
        }
    }
}
