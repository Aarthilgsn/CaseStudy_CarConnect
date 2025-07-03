using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConnectApp.Entity
{

    public class Vehicle
    {
        public int VehicleID { get; set; }
        public string Model { get; set; }
        public string Make { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public string RegistrationNumber { get; set; }
        public bool Availability { get; set; }
        public decimal DailyRate { get; set; }

        public Vehicle() { }

        public Vehicle(int vehicleID, string model, string make, int year, string color,
                       string registrationNumber, bool availability, decimal dailyRate)
        {
            VehicleID = vehicleID;
            Model = model;
            Make = make;
            Year = year;
            Color = color;
            RegistrationNumber = registrationNumber;
            Availability = availability;
            DailyRate = dailyRate;
        }
    }
}