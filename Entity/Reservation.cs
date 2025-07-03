using System;

namespace CarConnectApp.Entity
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        public int CustomerID { get; set; }
        public int VehicleID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; }

        public void CalculateTotalCost(decimal dailyRate)
        {
            TimeSpan duration = EndDate - StartDate;
            TotalCost = (decimal)duration.TotalDays * dailyRate;
        }
    }
}