using System;
using System.Collections.Generic;
using System.Text;
using AzureExtensions.FunctionToken.Exceptions.Abstract;

namespace AzureExtensions.FunctionToken.Exceptions
{
    public class FirebaseAuthException : FunctionTokenException
    {
        public FirebaseAuthException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
