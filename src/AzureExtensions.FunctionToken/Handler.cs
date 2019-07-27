using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AzureExtensions.FunctionToken
{
    public class Handler
    {
        /// <summary>
        /// Catches AuthenticationException and returns UnauthorizedResult, otherwise BadRequestObjectResult. 
        /// </summary>
        public static async Task<IActionResult> WrapAsync(FunctionTokenResult token, Func<Task<IActionResult>> action)
        {
            try
            {
                token.ValidateThrow();
                var result = await action();
                return result;
            }
            catch (AuthenticationException)
            {
                return new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        /// <summary>
        /// Catches AuthenticationException and returns UnauthorizedResult, otherwise BadRequestObjectResult. 
        /// </summary>
        public static IActionResult Wrap(FunctionTokenResult token, Func<IActionResult> action)
        {
            try
            {
                token.ValidateThrow();
                var result = action();
                return result;
            }
            catch (AuthenticationException)
            {
                return new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
