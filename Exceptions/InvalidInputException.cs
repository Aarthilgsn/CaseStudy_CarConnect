using System;
using CarConnectApp.Exceptions;

namespace CarConnectApp.Exceptions
{
    public class InvalidInputException : Exception
    {
        public InvalidInputException(string message) : base(message) { }
    }
}
