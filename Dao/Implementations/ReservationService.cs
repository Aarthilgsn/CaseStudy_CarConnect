using CarConnectApp.Exceptions;
using CarConnectApp.Dao.Interfaces;
using CarConnectApp.Dao.Implementations;
using CarConnectApp.Entity;
using CarConnectApp.Util;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;


namespace CarConnectApp.Dao.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly string connectionString;

        public ReservationService()
        {
            connectionString = DBConnUtil.GetConnectionString();
        }

        public void BookReservation(Reservation reservation)
        {
            try
            {
                // Calculate and set TotalCost using the Reservation method
                reservation.CalculateTotalCost(GetDailyRate(reservation.VehicleID));

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Reservation (CustomerID, VehicleID, StartDate, EndDate, TotalCost, Status)
                                     VALUES (@CustomerID, @VehicleID, @StartDate, @EndDate, @TotalCost, @Status)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CustomerID", reservation.CustomerID);
                    cmd.Parameters.AddWithValue("@VehicleID", reservation.VehicleID);
                    cmd.Parameters.AddWithValue("@StartDate", reservation.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", reservation.EndDate);
                    cmd.Parameters.AddWithValue("@TotalCost", reservation.TotalCost);
                    cmd.Parameters.AddWithValue("@Status", reservation.Status);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error booking reservation: " + ex.Message);
            }
        }

        public Reservation GetReservationById(int reservationId)
        {
            Reservation reservation = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Reservation WHERE ReservationID = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", reservationId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        reservation = MapReservation(reader);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error fetching reservation: " + ex.Message);
            }

            return reservation;
        }

        public List<Reservation> GetReservationsByCustomerId(int customerId)
        {
            List<Reservation> reservations = new List<Reservation>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Reservation WHERE CustomerID = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", customerId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        reservations.Add(MapReservation(reader));
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error fetching customer reservations: " + ex.Message);
            }

            return reservations;
        }

        public void CancelReservation(int reservationId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Reservation SET Status = 'Cancelled' WHERE ReservationID = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", reservationId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error canceling reservation: " + ex.Message);
            }
        }

        private decimal GetDailyRate(int vehicleId)
        {
            decimal rate = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT DailyRate FROM Vehicle WHERE VehicleID = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", vehicleId);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    rate = Convert.ToDecimal(result);
                }
            }

            return rate;
        }

        private Reservation MapReservation(SqlDataReader reader)
        {
            return new Reservation
            {
                ReservationID = Convert.ToInt32(reader["ReservationID"]),
                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                VehicleID = Convert.ToInt32(reader["VehicleID"]),
                StartDate = Convert.ToDateTime(reader["StartDate"]),
                EndDate = Convert.ToDateTime(reader["EndDate"]),
                TotalCost = Convert.ToDecimal(reader["TotalCost"]),
                Status = reader["Status"].ToString()
            };
        }
    }
}
