using CarConnectApp.Exceptions;
using CarConnectApp.Dao.Implementations;
using CarConnectApp.Dao.Interfaces;
//using CarConnectApp.Exceptions;
using CarConnectApp.Entity;
using CarConnectApp.Util;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarConnectApp.Service
{
    public class ReportGenerator
    {
        private readonly IReservationService _reservationService;
        private readonly IVehicleService _vehicleService;
        private readonly ICustomerService _customerService;

        public ReportGenerator()
        {
            _reservationService = new ReservationService();
            _vehicleService = new VehicleService();
            _customerService = new CustomerService();
        }

        public void GenerateReservationHistoryReport()
        {
            Console.WriteLine("\n--- Reservation History Report ---");
            try
            {
                List<Reservation> allReservations = new List<Reservation>();
                // This would ideally fetch all reservations. Assuming a method like GetAllReservations() exists.
                // For demonstration, let's assume we iterate through customer IDs or have a direct DAO method.
                // Since IReservationService doesn't have GetAllReservations, we'll demonstrate based on customer.
                // In a real scenario, you'd add a GetAllReservations() to IReservationService and its implementation.

                // Placeholder: For a real report, you'd need a DAO method to get all reservations.
                // For now, this just illustrates structure.
                Console.WriteLine("Note: A comprehensive 'All Reservations' report would require a GetAllReservations method in IReservationService.");
                Console.WriteLine("Displaying reservations by a dummy customer ID (e.g., 1) as an example:");
                List<Reservation> sampleReservations = _reservationService.GetReservationsByCustomerId(1); // Example

                if (sampleReservations.Any())
                {
                    foreach (var reservation in sampleReservations)
                    {
                        Customer customer = _customerService.GetCustomerById(reservation.CustomerID);
                        Vehicle vehicle = _vehicleService.GetVehicleById(reservation.VehicleID);

                        Console.WriteLine($"Reservation ID: {reservation.ReservationID}");
                        Console.WriteLine($"  Customer: {customer?.FirstName} {customer?.LastName} (ID: {customer?.CustomerID})");
                        Console.WriteLine($"  Vehicle: {vehicle?.Make} {vehicle?.Model} (Reg: {vehicle?.RegistrationNumber}, ID: {vehicle?.VehicleID})");
                        Console.WriteLine($"  Start Date: {reservation.StartDate.ToShortDateString()}");
                        Console.WriteLine($"  End Date: {reservation.EndDate.ToShortDateString()}");
                        Console.WriteLine($"  Total Cost: ${reservation.TotalCost:F2}");
                        Console.WriteLine($"  Status: {reservation.Status}");
                        Console.WriteLine("--------------------");
                    }
                }
                else
                {
                    Console.WriteLine("No reservations found.");
                }
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Error generating reservation history report: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        public void GenerateVehicleUtilizationReport()
        {
            Console.WriteLine("\n--- Vehicle Utilization Report ---");
            try
            {
                List<Vehicle> allVehicles = _vehicleService.GetAvailableVehicles(); // Assuming GetAvailableVehicles can be repurposed or a GetAllVehicles is added

                if (!allVehicles.Any())
                {
                    Console.WriteLine("No vehicles found in the system.");
                    return;
                }

                foreach (var vehicle in allVehicles)
                {
                    // For a true utilization report, you'd query reservations for this vehicle
                    // and calculate the percentage of time it's been reserved over a period.
                    // For simplicity, we'll just show current availability.
                    string availabilityStatus = vehicle.Availability ? "Available" : "Not Available (currently rented)";

                    Console.WriteLine($"Vehicle: {vehicle.Make} {vehicle.Model} (Reg: {vehicle.RegistrationNumber})");
                    Console.WriteLine($"  Current Availability: {availabilityStatus}");
                    // To get full utilization, you'd need to query past reservations for this vehicle
                    // SELECT COUNT(*) FROM Reservation WHERE VehicleID = @VehicleID AND Status = 'completed'
                    Console.WriteLine("--------------------");
                }
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Error generating vehicle utilization report: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        public void GenerateRevenueReport()
        {
            Console.WriteLine("\n--- Revenue Report ---");
            try
            {
                // This would sum up TotalCost from completed reservations.
                // Assuming a method to get all completed reservations or total revenue from DAO.
                // For demonstration, let's sum from a hypothetical list or directly query the database.
                decimal totalRevenue = 0;
                using (SqlConnection conn = new SqlConnection(DBConnUtil.GetConnectionString()))
                {
                    string query = "SELECT SUM(TotalCost) FROM Reservation WHERE Status = 'completed'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        totalRevenue = Convert.ToDecimal(result);
                    }
                }

                Console.WriteLine($"Total Revenue Generated: ${totalRevenue:F2}");
                Console.WriteLine("--------------------");
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Error generating revenue report: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}