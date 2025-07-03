using CarConnectApp.Exceptions;
using CarConnectApp.Dao.Implementations;
using CarConnectApp.Dao.Interfaces;
using CarConnectApp.Entity;
using System;

namespace CarConnectApp.Service
{
    public class AuthenticationService
    {
        private readonly IAdminService _adminService;
        private readonly ICustomerService _customerService;

        public AuthenticationService()
        {
            _adminService = new AdminService();
            _customerService = new CustomerService();
        }

        public Admin AdminLogin(string username, string password)
        {
            try
            {
                Admin admin = _adminService.GetAdminByUsername(username);
                if (admin == null || !admin.Authenticate(password))
                {
                    throw new AuthenticationException("Invalid admin username or password.");
                }
                return admin;
            }
            catch (DatabaseConnectionException ex)
            {
                throw new AuthenticationException("Authentication failed due to database error: " + ex.Message);
            }
        }

        public Customer CustomerLogin(string username, string password)
        {
            try
            {
                Customer customer = _customerService.GetCustomerByUsername(username);
                if (customer == null || !customer.Authenticate(password))
                {
                    throw new AuthenticationException("Invalid customer username or password.");
                }
                return customer;
            }
            catch (DatabaseConnectionException ex)
            {
                throw new AuthenticationException("Authentication failed due to database error: " + ex.Message);
            }
        }
    }
}