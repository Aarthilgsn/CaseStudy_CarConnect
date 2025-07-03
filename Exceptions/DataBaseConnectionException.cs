using System;
using CarConnectApp.Exceptions;
namespace CarConnectApp.Exceptions
{
    public class DatabaseConnectionException : Exception
    {
        // Default constructor
        public DatabaseConnectionException() { }

        // Constructor that takes a message
        public DatabaseConnectionException(string message) : base(message) { }

        // Constructor that takes a message and an inner exception
        public DatabaseConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }
}