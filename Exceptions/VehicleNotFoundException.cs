using System;
using CarConnectApp.Exceptions;

namespace CarConnectApp.Exceptions
{
    public class VehicleNotFoundException : Exception
    {
        public VehicleNotFoundException(string message) : base(message) { }
    }
}
