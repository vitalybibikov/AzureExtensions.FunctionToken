using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace AzureExtensions.FunctionToken.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        /// <summary>
        ///     Finds role if claim <see cref="ClaimTypes.Role" /> is set in the principal.
        /// </summary>
        public static bool IsInRole(this ClaimsPrincipal principal, string role)
        {
            var result = false;
            var claimRole = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);

            if (claimRole != null)
            {
                if (claimRole.ValueType == ClaimValueTypes.String)
                {
                    result = claimRole.Value.Equals(role, StringComparison.OrdinalIgnoreCase);
                }
            }

            return result;
        }

        /// <summary>
        ///     Finds role if claim <see cref="ClaimTypes.Role" /> is set in the principal.
        /// </summary>
        public static bool IsInRole(this ClaimsPrincipal principal, IEnumerable<string> roles)
        {
            var result = false;
            var claimRole = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);

            if (claimRole != null && roles != null)
            {
                if (claimRole.ValueType == ClaimValueTypes.String || claimRole.ValueType == typeof(string).ToString())
                {
                    result = roles.Any(s => s.Equals(claimRole.Value, StringComparison.OrdinalIgnoreCase));
                }
            }

            return result;
        }

        /// <summary>
        ///     Finds role if claim <see cref="ClaimTypes.Role" /> is set in the principal.
        /// </summary>
        internal static bool IsRoleSet(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role) != null;
        }
    }
}
