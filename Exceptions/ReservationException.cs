using System;
using CarConnectApp.Exceptions;
namespace CarConnectApp.Exceptions
{
    public class ReservationException : Exception
    {
        public ReservationException(string message) : base(message) { }
    }
}
