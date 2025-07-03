using CarConnectApp.Exceptions;
using CarConnectApp.Dao.Implementations;
using CarConnectApp.Entity;
using CarConnectApp.Util;
using CarConnectApp.Dao.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace CarConnectApp.Dao.Implementations
{
    public class VehicleService : IVehicleService
    {
        private readonly string connectionString;

        public VehicleService()
        {
            connectionString = DBConnUtil.GetConnectionString();
        }

        public void AddVehicle(Vehicle vehicle)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Vehicle (Model, Make, Year, Color, RegistrationNumber, Availability, DailyRate)
                                     VALUES (@Model, @Make, @Year, @Color, @RegistrationNumber, @Availability, @DailyRate)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Model", vehicle.Model);
                    cmd.Parameters.AddWithValue("@Make", vehicle.Make);
                    cmd.Parameters.AddWithValue("@Year", vehicle.Year);
                    cmd.Parameters.AddWithValue("@Color", vehicle.Color);
                    cmd.Parameters.AddWithValue("@RegistrationNumber", vehicle.RegistrationNumber);
                    cmd.Parameters.AddWithValue("@Availability", vehicle.Availability);
                    cmd.Parameters.AddWithValue("@DailyRate", vehicle.DailyRate);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error adding vehicle: " + ex.Message);
            }
        }

        public Vehicle GetVehicleById(int vehicleId)
        {
            Vehicle vehicle = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Vehicle WHERE VehicleID = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", vehicleId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        vehicle = MapVehicle(reader);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error fetching vehicle: " + ex.Message);
            }

            return vehicle;
        }

        public List<Vehicle> GetAvailableVehicles()
        {
            List<Vehicle> vehicles = new List<Vehicle>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Vehicle WHERE Availability = 1";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        vehicles.Add(MapVehicle(reader));
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error retrieving available vehicles: " + ex.Message);
            }

            return vehicles;
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE Vehicle SET Model=@Model, Make=@Make, Year=@Year, Color=@Color, 
                                     RegistrationNumber=@RegistrationNumber, Availability=@Availability, DailyRate=@DailyRate 
                                     WHERE VehicleID=@VehicleID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@VehicleID", vehicle.VehicleID);
                    cmd.Parameters.AddWithValue("@Model", vehicle.Model);
                    cmd.Parameters.AddWithValue("@Make", vehicle.Make);
                    cmd.Parameters.AddWithValue("@Year", vehicle.Year);
                    cmd.Parameters.AddWithValue("@Color", vehicle.Color);
                    cmd.Parameters.AddWithValue("@RegistrationNumber", vehicle.RegistrationNumber);
                    cmd.Parameters.AddWithValue("@Availability", vehicle.Availability);
                    cmd.Parameters.AddWithValue("@DailyRate", vehicle.DailyRate);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error updating vehicle: " + ex.Message);
            }
        }

        public void RemoveVehicle(int vehicleId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Vehicle WHERE VehicleID = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", vehicleId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error removing vehicle: " + ex.Message);
            }
        }

        // New method to implement
        public Vehicle GetVehicleByRegistrationNumber(string registrationNumber) // Add this method
        {
            Vehicle vehicle = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Vehicle WHERE RegistrationNumber = @registrationNumber";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@registrationNumber", registrationNumber);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        vehicle = MapVehicle(reader);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error fetching vehicle by registration number: " + ex.Message, ex);
            }
            return vehicle;
        }

        private Vehicle MapVehicle(SqlDataReader reader)
        {
            return new Vehicle
            {
                VehicleID = Convert.ToInt32(reader["VehicleID"]),
                Model = reader["Model"].ToString(),
                Make = reader["Make"].ToString(),
                Year = Convert.ToInt32(reader["Year"]),
                Color = reader["Color"].ToString(),
                RegistrationNumber = reader["RegistrationNumber"].ToString(),
                Availability = Convert.ToBoolean(reader["Availability"]),
                DailyRate = Convert.ToDecimal(reader["DailyRate"])
            };
        }
    }
}
