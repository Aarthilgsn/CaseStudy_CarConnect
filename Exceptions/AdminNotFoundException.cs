using System;
using CarConnectApp.Exceptions;

namespace CarConnectApp.Exceptions
{
    public class AdminNotFoundException : Exception
    {
        public AdminNotFoundException(string message) : base(message) { }
    }
}
