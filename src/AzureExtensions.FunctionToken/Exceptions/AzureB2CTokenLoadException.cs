using System.Net;
using AzureExtensions.FunctionToken.Exceptions.Abstract;

namespace AzureExtensions.FunctionToken.Exceptions
{
    /// <summary>
    /// Signals that Azure B2C token wasn't retrieved from Azure B2C.
    /// </summary>
    public sealed class AzureB2CTokenLoadException : FunctionTokenException
    {
        public HttpStatusCode Code { get; }

        public AzureB2CTokenLoadException()
        {
        }

        public AzureB2CTokenLoadException(string message)
            : base(message)
        {
        }

        public AzureB2CTokenLoadException(string message, HttpStatusCode code)
            : base(message)
        {
            Code = code;
        }
    }
}
