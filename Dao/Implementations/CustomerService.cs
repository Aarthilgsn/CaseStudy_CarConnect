using CarConnectApp.Exceptions;
using CarConnectApp.Entity;
using CarConnectApp.Util;
using CarConnectApp.Dao.Interfaces;
using CarConnectApp.Dao.Implementations;
using System;
using Microsoft.Data.SqlClient;

namespace CarConnectApp.Dao.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly string connectionString;

        public CustomerService()
        {
            connectionString = DBConnUtil.GetConnectionString();
        }

        public void RegisterCustomer(Customer customer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Customer (FirstName, LastName, Email, PhoneNumber, Address, Username, Password, RegistrationDate) 
                                     VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @Address, @Username, @Password, @RegistrationDate)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", customer.LastName);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Address", customer.Address);
                    cmd.Parameters.AddWithValue("@Username", customer.Username);
                    cmd.Parameters.AddWithValue("@Password", customer.Password);
                    cmd.Parameters.AddWithValue("@RegistrationDate", customer.RegistrationDate);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error registering customer: " + ex.Message);
            }
        }

        public Customer GetCustomerById(int id)
        {
            Customer customer = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Customer WHERE CustomerID = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        customer = MapCustomer(reader);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error fetching customer by ID: " + ex.Message);
            }

            return customer;
        }

        public Customer GetCustomerByUsername(string username)
        {
            Customer customer = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Customer WHERE Username = @username";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        customer = MapCustomer(reader);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error fetching customer by username: " + ex.Message);
            }

            return customer;
        }

        public void UpdateCustomer(Customer customer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE Customer 
                                     SET FirstName = @FirstName, LastName = @LastName, Email = @Email, 
                                         PhoneNumber = @PhoneNumber, Address = @Address, Password = @Password 
                                     WHERE CustomerID = @CustomerID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                    cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", customer.LastName);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Address", customer.Address);
                    cmd.Parameters.AddWithValue("@Password", customer.Password);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error updating customer: " + ex.Message);
            }
        }
/*
        public void DeleteCustomer(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Customer WHERE CustomerID = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error deleting customer: " + ex.Message);
            }
        }
*/
        public void DeleteCustomer(int customerId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Customer WHERE CustomerID = @customerId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@customerId", customerId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error deleting customer: " + ex.Message, ex);
            }
        }

        private Customer MapCustomer(SqlDataReader reader)
        {
            return new Customer
            {
                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                FirstName = reader["FirstName"].ToString(),
                LastName = reader["LastName"].ToString(),
                Email = reader["Email"].ToString(),
                PhoneNumber = reader["PhoneNumber"].ToString(),
                Address = reader["Address"].ToString(),
                Username = reader["Username"].ToString(),
                Password = reader["Password"].ToString(),
                RegistrationDate = Convert.ToDateTime(reader["RegistrationDate"])
            };
        }
    }
}