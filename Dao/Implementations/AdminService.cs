using CarConnectApp.Exceptions;
using CarConnectApp.Dao.Interfaces;
using CarConnectApp.Entity;
using CarConnectApp.Util;
using CarConnectApp.Dao.Implementations;
using System;
using Microsoft.Data.SqlClient;

namespace CarConnectApp.Dao.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly string connectionString;

        public AdminService()
        {
            connectionString = DBConnUtil.GetConnectionString();
        }

        public void AddAdmin(Admin admin)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Admin (FirstName, LastName, Email, PhoneNumber, Username, Password, Role, JoinDate)
                                     VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @Username, @Password, @Role, @JoinDate)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FirstName", admin.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", admin.LastName);
                    cmd.Parameters.AddWithValue("@Email", admin.Email);
                    cmd.Parameters.AddWithValue("@PhoneNumber", admin.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Username", admin.Username);
                    cmd.Parameters.AddWithValue("@Password", admin.Password);
                    cmd.Parameters.AddWithValue("@Role", admin.Role);
                    cmd.Parameters.AddWithValue("@JoinDate", admin.JoinDate);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error adding admin: " + ex.Message);
            }
        }

        public Admin GetAdminById(int id)
        {
            Admin admin = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Admin WHERE AdminID = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        admin = MapAdmin(reader);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error fetching admin by ID: " + ex.Message);
            }

            return admin;
        }

        public Admin GetAdminByUsername(string username)
        {
            Admin admin = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Admin WHERE Username = @username";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        admin = MapAdmin(reader);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error fetching admin by username: " + ex.Message);
            }

            return admin;
        }

        public void UpdateAdmin(Admin admin)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE Admin SET FirstName=@FirstName, LastName=@LastName, Email=@Email, 
                                     PhoneNumber=@PhoneNumber, Username=@Username, Password=@Password, Role=@Role 
                                     WHERE AdminID=@AdminID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AdminID", admin.AdminID);
                    cmd.Parameters.AddWithValue("@FirstName", admin.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", admin.LastName);
                    cmd.Parameters.AddWithValue("@Email", admin.Email);
                    cmd.Parameters.AddWithValue("@PhoneNumber", admin.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Username", admin.Username);
                    cmd.Parameters.AddWithValue("@Password", admin.Password);
                    cmd.Parameters.AddWithValue("@Role", admin.Role);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error updating admin: " + ex.Message);
            }
        }

        public void DeleteAdmin(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Admin WHERE AdminID = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Error deleting admin: " + ex.Message);
            }
        }

        private Admin MapAdmin(SqlDataReader reader)
        {
            return new Admin
            {
                AdminID = Convert.ToInt32(reader["AdminID"]),
                FirstName = reader["FirstName"].ToString(),
                LastName = reader["LastName"].ToString(),
                Email = reader["Email"].ToString(),
                PhoneNumber = reader["PhoneNumber"].ToString(),
                Username = reader["Username"].ToString(),
                Password = reader["Password"].ToString(),
                Role = reader["Role"].ToString(),
                JoinDate = Convert.ToDateTime(reader["JoinDate"])
            };
        }
    }
}