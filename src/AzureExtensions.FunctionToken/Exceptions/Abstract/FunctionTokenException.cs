using System;

namespace AzureExtensions.FunctionToken.Exceptions.Abstract
{
    /// <summary>
    /// Base FunctionToken exception.
    /// </summary>
    public abstract class FunctionTokenException : Exception
    {
        public FunctionTokenException()
        {
        }

        public FunctionTokenException(string message)
            : base(message)
        {
        }

        public FunctionTokenException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
