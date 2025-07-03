using CarConnectApp.Dao.Implementations;
using CarConnectApp.Dao.Interfaces;
using CarConnectApp.Entity;
using NUnit.Framework;
using System;
using System.Collections.Generic; // Added for potential future use or to match other test files
using CarConnectApp.Exceptions; // Make sure this is included for cleanup (if needed later)

namespace CarConnectApp.Tests
{
    [TestFixture]
    public class CustomerServiceTests
    {
        private CustomerService customerService;
        private int testCustomerId; // To store the ID of the customer created in tests for cleanup

        [SetUp]
        public void Setup()
        {
            customerService = new CustomerService();
            testCustomerId = 0; // Initialize to 0
        }

        [TearDown]
        public void Teardown()
        {
            // Clean up the customer created in the test if it was successfully added
            if (testCustomerId != 0)
            {
                try
                {
                    customerService.DeleteCustomer(testCustomerId); // Assuming you have a DeleteCustomer method
                }
                catch (Exception ex)
                {
                    TestContext.WriteLine($"Warning: Failed to cleanup customer {testCustomerId}: {ex.Message}");
                }
            }
        }

        [Test]
        public void RegisterCustomer_ShouldInsertCustomerIntoDatabase()
        {
            // Arrange
            // Generate unique values for Email, PhoneNumber, and Username
            string uniqueId = Guid.NewGuid().ToString(); // Use the full GUID for maximum uniqueness

            Customer testCustomer = new Customer
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"test_{uniqueId}@example.com", // Make email unique
                PhoneNumber = $"987654{uniqueId.Substring(0, 4)}", // Make phone number unique
                Address = "Test Town",
                Username = $"testuser_{uniqueId.Substring(0, 8)}", // Make username unique, maybe shorten for readability
                Password = "pass123",
                RegistrationDate = DateTime.Now
            };

            // Act
            customerService.RegisterCustomer(testCustomer);

            // Assert
            var result = customerService.GetCustomerByUsername(testCustomer.Username);
            Assert.IsNotNull(result, "Customer was not found after registration.");
            Assert.AreEqual(testCustomer.Email, result.Email, "Registered customer email does not match.");
            Assert.AreEqual(testCustomer.PhoneNumber, result.PhoneNumber, "Registered customer phone number does not match.");
            Assert.AreEqual(testCustomer.Address, result.Address, "Registered customer address does not match.");
            Assert.AreEqual(testCustomer.FirstName, result.FirstName, "Registered customer first name does not match.");
            Assert.AreEqual(testCustomer.LastName, result.LastName, "Registered customer last name does not match.");

            // Store the CustomerID for teardown
            testCustomerId = result.CustomerID;
        }

        // It's also good practice to add more specific tests, e.g.,
        // [Test]
        // public void RegisterCustomer_ShouldThrowException_WhenDuplicateEmail() { /* ... */ }
        // [Test]
        // public void GetCustomerByUsername_ShouldReturnNull_WhenCustomerNotFound() { /* ... */ }
        // etc.
    }
}