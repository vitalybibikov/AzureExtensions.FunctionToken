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
            var claimRole = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
            if (roles == null // Not Defined
                || roles.Count() == 0 // No Roles Required
            )
            {
                return true;
            }
            else if (claimRole != null) // A required role is defined and there is at least one available in principal
            {
                if (claimRole.ValueType == ClaimValueTypes.String || claimRole.ValueType == typeof(string).ToString()) // Ensure it is the right type
                {
                    return roles.Any(s => s.Equals(claimRole.Value, StringComparison.OrdinalIgnoreCase)); // Check if the required roles are equal to any of the claimed roles
                }
            }

            return false; // not in role
        }

        /// <summary>
        ///     Finds scp (Scope) if claim is set in the principal.
        /// </summary>
        public static bool IsInScope(this ClaimsPrincipal principal, string requiredScope, string scopeFieldName = "scp")
        {
            // Acquire the scopes reported by ClaimsPrincipal
            var scopesClaim = principal
                .Identities
                .FirstOrDefault()?
                .Claims
                .FirstOrDefault(x => x.Type == scopeFieldName);

            // If there are no scopes listed then accept anything
            if (requiredScope == null || requiredScope == "")
            {
                return true;
            }
            else if (scopesClaim != null) // see if there are any scopes
            {
                if (scopesClaim.ValueType == ClaimValueTypes.String || scopesClaim.ValueType == typeof(string).ToString())
                {
                    // Scopes are returned as whitespace delimited strings
                    // There could be many scopes on a single claim so we separate and lowercase them
                    IEnumerable<string> scopesFromClaim = scopesClaim.Value.Split(' ').Select(c => c.ToLowerInvariant());

                    // Iterate over the scopes from the Claim checking if appropriate
                    foreach (string possibleScope in scopesFromClaim)
                    {
                        // Make the assumption that scopes are case invariant
                        if (requiredScope.Equals(possibleScope, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }

            // Otherwise the required scopes do not exist
            return false;
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
