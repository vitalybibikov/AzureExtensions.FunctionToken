using System;

namespace AzureExtensions.FunctionToken.Exceptions.Abstract
{
    /// <summary>
    /// Base FunctionToken exception.
    /// </summary>
    public abstract class FunctionTokenException : Exception
    {
        protected FunctionTokenException()
        {
        }

        protected FunctionTokenException(string message)
            : base(message)
        {
        }

        protected FunctionTokenException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
