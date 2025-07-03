using CarConnectApp.Exceptions;
using CarConnectApp.Util;
using Microsoft.Data.SqlClient;
using System;
using System.IO;

namespace CarConnectApp.Util
{
    public static class DBConnUtil
    {
        private static string connectionString;

        static DBConnUtil()
        {
            // This static constructor runs once when the class is first accessed.
            // It tries to load the connection string from db.properties.
            // In a real application, you might use app.config/web.config for connection strings.
            try
            {
                connectionString = DBPropertyUtil.GetConnectionString("db.properties");
            }
            catch (FileNotFoundException ex)
            {
                throw new DatabaseConnectionException("Database property file not found: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException("Error loading database connection string: " + ex.Message);
            }
        }

        public static string GetConnectionString()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                // This case should ideally not be hit if the static constructor runs correctly,
                // but it's a fallback for robust error handling.
                throw new DatabaseConnectionException("Database connection string is not initialized.");
            }
            return connectionString;
        }

        public static SqlConnection GetDBConnection()
        {
            try
            {
                return new SqlConnection(GetConnectionString());
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException("Failed to create database connection object: " + ex.Message);
            }
        }
    }
}