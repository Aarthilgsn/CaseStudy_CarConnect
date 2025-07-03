using System;
using CarConnectApp.Exceptions;

namespace CarConnectApp.Exceptions
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message) : base(message)
        {
        }
       
        
    }
}
