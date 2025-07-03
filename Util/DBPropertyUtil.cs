using System;
using System.IO;
using System.Collections.Generic;

namespace CarConnectApp.Util
{
    public class DBPropertyUtil
    {
        public static string GetConnectionString(string propertyFileName)
        {
            string connectionString = string.Empty;
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, propertyFileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The property file '{propertyFileName}' was not found at '{filePath}'.");
            }

            try
            {
                var properties = new Dictionary<string, string>();
                foreach (string line in File.ReadLines(filePath))
                {
                    if (!string.IsNullOrWhiteSpace(line) && line.Contains("="))
                    {
                        string[] parts = line.Split('=', 2);
                        if (parts.Length == 2)
                        {
                            properties[parts[0].Trim()] = parts[1].Trim();
                        }
                    }
                }

                // Construct the connection string from properties
                if (properties.TryGetValue("server", out string server) &&
                    properties.TryGetValue("database", out string database) &&
                    properties.TryGetValue("integratedSecurity", out string integratedSecurity) &&
                    properties.TryGetValue("trustServerCertificate", out string trustServerCertificate))
                {
                    connectionString = $"Server={server};Database={database};Integrated Security={integratedSecurity};TrustServerCertificate={trustServerCertificate};";
                }
                else
                {
                    throw new InvalidDataException("Missing required connection string properties in db.properties.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading properties from '{propertyFileName}': {ex.Message}", ex);
            }

            return connectionString;
        }
    }
}