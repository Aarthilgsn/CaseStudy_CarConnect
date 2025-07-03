using NUnit.Framework;
using CarConnectApp.Dao.Implementations;
using CarConnectApp.Dao.Interfaces;
using CarConnectApp.Entity;
using System;
using CarConnectApp.Exceptions; // Assuming AuthenticationException or AdminNotFoundException

namespace CarConnectApp.Tests
{
    [TestFixture]
    public class AdminServiceTests
    {
        private AdminService adminService;
        private string testAdminUsername; // To store username for cleanup or reference
        private int testAdminId;

        [SetUp]
        public void Setup()
        {
            adminService = new AdminService();
            // For Admin tests, you might pre-insert a test admin,
            // or rely on a known admin from your dev setup (less ideal for true unit testing).
            // For robustness, consider adding an AddAdmin method to AdminService for tests.
            // Or, if Admin details are static and known, directly use them.
        }

        [TearDown]
        public void Teardown()
        {
            // Cleanup any admin created if AdminService has a delete method
            if (testAdminId != 0)
            {
                try
                {
                    adminService.DeleteAdmin(testAdminId); // Assuming DeleteAdmin exists and is implemented
                    testAdminId = 0;
                    testAdminUsername = null;
                }
                catch (Exception ex)
                {
                    TestContext.WriteLine($"Warning: Failed to clean up test admin {testAdminId}: {ex.Message}");
                }
            }
        }

        [Test]
        public void GetAdminByUsername_ShouldReturnCorrectAdmin()
        {
            // Arrange: Assume you have a known admin in your database for testing.
            // For a robust test, you'd insert a temporary admin first.
            // Let's assume a static admin 'admin_user' exists for this example.
            // If your AdminService has an AddAdmin method, use that:
            Admin tempAdmin = new Admin
            {
                FirstName = "Test",
                LastName = "Admin",
                Email = "test.admin_" + Guid.NewGuid().ToString().Substring(0, 5) + "@example.com",
                PhoneNumber = "5551234567",
                Username = "testadmin_" + Guid.NewGuid().ToString().Substring(0, 5),
                Password = "adminpass",
                Role = "SuperAdmin", // Assuming 'Role' is a required field or has a default in DB
                JoinDate = DateTime.Now // *** CRITICAL FIX: Set JoinDate to a valid date ***
            };
            adminService.AddAdmin(tempAdmin); // Assuming AddAdmin exists in AdminService
            var addedAdmin = adminService.GetAdminByUsername(tempAdmin.Username);
            Assert.IsNotNull(addedAdmin, "Pre-test setup failed: Could not add test admin.");
            testAdminUsername = addedAdmin.Username;
            testAdminId = addedAdmin.AdminID;

            // Act
            Admin fetchedAdmin = adminService.GetAdminByUsername(testAdminUsername);

            // Assert
            Assert.IsNotNull(fetchedAdmin);
            Assert.AreEqual(testAdminUsername, fetchedAdmin.Username);
            Assert.AreEqual(tempAdmin.Email, fetchedAdmin.Email);
            // Assert.AreEqual(tempAdmin.JoinDate.Date, fetchedAdmin.JoinDate.Date); // Optional: Compare only date part if time might differ
        }

        [Test]
        public void UpdateAdmin_ShouldModifyAdminDetailsInDatabase()
        {
            // Arrange: Create a temporary admin to update
            Admin tempAdmin = new Admin
            {
                FirstName = "OldAdmin",
                LastName = "User",
                Email = "old.admin_" + Guid.NewGuid().ToString().Substring(0, 5) + "@example.com",
                PhoneNumber = "9998887776",
                Username = "oldadmin_" + Guid.NewGuid().ToString().Substring(0, 5),
                Password = "oldpass",
                Role = "Admin", // Assuming 'Role' is a required field or has a default in DB
                JoinDate = DateTime.Now.AddDays(-10) // *** CRITICAL FIX: Set JoinDate to a valid date ***
            };
            adminService.AddAdmin(tempAdmin);
            var addedAdmin = adminService.GetAdminByUsername(tempAdmin.Username);
            Assert.IsNotNull(addedAdmin, "Pre-test setup failed: Could not add admin for update.");
            testAdminId = addedAdmin.AdminID;
            testAdminUsername = addedAdmin.Username;

            // Modify the admin details
            addedAdmin.Email = "new.email@example.com";
            addedAdmin.PhoneNumber = "1110009998";

            // Act
            adminService.UpdateAdmin(addedAdmin); // Assuming UpdateAdmin takes an Admin object

            // Assert
            Admin updatedAdmin = adminService.GetAdminByUsername(testAdminUsername);
            Assert.IsNotNull(updatedAdmin);
            Assert.AreEqual("new.email@example.com", updatedAdmin.Email);
            Assert.AreEqual("1110009998", updatedAdmin.PhoneNumber);
        }

        // Add more tests for other AdminService methods (e.g., AddAdmin, DeleteAdmin)
        // You might need to add these methods to your IAdminService and AdminService implementations first.
        [Test]
        public void AddAdmin_ShouldInsertAdminIntoDatabase()
        {
            // Arrange
            Admin newAdmin = new Admin
            {
                FirstName = "New",
                LastName = "One",
                Email = "new.admin_" + Guid.NewGuid().ToString().Substring(0, 5) + "@example.com",
                PhoneNumber = "1231231234",
                Username = "newadmin_" + Guid.NewGuid().ToString().Substring(0, 5),
                Password = "newpass",
                Role = "Admin", // Assuming 'Role' is a required field or has a default in DB
                JoinDate = DateTime.Now // *** CRITICAL FIX: Set JoinDate to a valid date ***
            };

            // Act
            adminService.AddAdmin(newAdmin); // Assuming AddAdmin exists

            // Assert
            var result = adminService.GetAdminByUsername(newAdmin.Username);
            Assert.IsNotNull(result);
            Assert.AreEqual(newAdmin.Email, result.Email);
            testAdminId = result.AdminID;
            testAdminUsername = result.Username;
        }

        // Add a test for DeleteAdmin to ensure cleanup works
        [Test]
        public void DeleteAdmin_ShouldRemoveAdminFromDatabase()
        {
            // Arrange: Add an admin to be deleted
            Admin adminToDelete = new Admin
            {
                FirstName = "Delete",
                LastName = "Me",
                Email = "delete.me_" + Guid.NewGuid().ToString().Substring(0, 5) + "@example.com",
                PhoneNumber = "0001112222",
                Username = "deladmin_" + Guid.NewGuid().ToString().Substring(0, 5),
                Password = "delpass",
                Role = "Temp",
                JoinDate = DateTime.Now
            };
            adminService.AddAdmin(adminToDelete);
            var addedAdmin = adminService.GetAdminByUsername(adminToDelete.Username);
            Assert.IsNotNull(addedAdmin, "Pre-test setup failed: Could not add admin for deletion.");
            int adminIdToDelete = addedAdmin.AdminID;
            // No need to set testAdminId here as it will be deleted by this test.

            // Act
            adminService.DeleteAdmin(adminIdToDelete);

            // Assert
            Admin deletedAdmin = adminService.GetAdminById(adminIdToDelete); // Assuming GetAdminById exists
            Assert.IsNull(deletedAdmin, "Admin should be null after deletion.");
        }
    }
}