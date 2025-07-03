using NUnit.Framework;
using CarConnectApp.Dao.Implementations; // Adjust namespace if your service is in .DAO.Implementations
using CarConnectApp.Entity;
using System;
using System.Collections.Generic;
using CarConnectApp.Exceptions; // Assuming you have VehicleNotFoundException or similar
using CarConnectApp.Dao.Interfaces;
namespace CarConnectApp.Tests
{
    [TestFixture] // Use TestFixture for classes containing tests
    public class VehicleServiceTests
    {
        private VehicleService vehicleService;
        private int testVehicleId; // To store ID of a vehicle created for tests

        [SetUp]
        public void Setup()
        {
            vehicleService = new VehicleService();
            // Optional: Insert a known test vehicle for operations like Get, Update, Delete
            // This is good practice for ensuring test isolation.
            // For simplicity, we might create a vehicle in each test that needs one
            // or ensure a robust tear-down.
        }

        [TearDown]
        public void Teardown()
        {
            // Clean up any test data inserted by tests if not handled within test methods
            // For instance, if testVehicleId was used, delete that vehicle from DB.
            if (testVehicleId != 0)
            {
                try
                {
                    vehicleService.RemoveVehicle(testVehicleId);
                    testVehicleId = 0; // Reset for next test
                }
                catch (Exception ex)
                {
                    TestContext.WriteLine($"Warning: Failed to clean up test vehicle {testVehicleId}: {ex.Message}");
                }
            }
        }

        [Test]
        public void AddVehicle_ShouldInsertVehicleIntoDatabase()
        {
            // Arrange
            Vehicle newVehicle = new Vehicle
            {
                Model = "TestModel",
                Make = "TestMake",
                Year = 2023,
                Color = "TestColor",
                RegistrationNumber = "TESTREG_" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
                Availability = true,
                DailyRate = 75.00m
            };

            // Act
            // Assuming AddVehicle returns the ID or you fetch by unique identifier
            vehicleService.AddVehicle(newVehicle);

            // Assert
            // To assert insertion, we need a way to retrieve the vehicle.
            // Assuming VehicleService has a GetVehicleByRegistrationNumber or GetAllVehicles
            var retrievedVehicles = vehicleService.GetAvailableVehicles();
            var addedVehicle = retrievedVehicles.Find(v => v.RegistrationNumber == newVehicle.RegistrationNumber);

            Assert.IsNotNull(addedVehicle);
            Assert.AreEqual(newVehicle.Make, addedVehicle.Make);
            Assert.AreEqual(newVehicle.Model, addedVehicle.Model);
            Assert.AreEqual(newVehicle.RegistrationNumber, addedVehicle.RegistrationNumber);

            testVehicleId = addedVehicle.VehicleID; // Store ID for teardown
        }

        [Test]
        public void GetVehicleById_ShouldReturnCorrectVehicle()
        {
            // Arrange - Create a vehicle specifically for this test
            Vehicle tempVehicle = new Vehicle
            {
                Model = "GetTestModel",
                Make = "GetTestMake",
                Year = 2020,
                Color = "Blue",
                RegistrationNumber = "GETVEH_" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
                Availability = true,
                DailyRate = 50.00m
            };
            vehicleService.AddVehicle(tempVehicle); // Add it to DB
            var added = vehicleService.GetAvailableVehicles().Find(v => v.RegistrationNumber == tempVehicle.RegistrationNumber);
            Assert.IsNotNull(added, "Pre-test setup failed: Could not add vehicle.");
            int vehicleIdToFetch = added.VehicleID;
            testVehicleId = vehicleIdToFetch; // Store for cleanup

            // Act
            Vehicle fetchedVehicle = vehicleService.GetVehicleById(vehicleIdToFetch);

            // Assert
            Assert.IsNotNull(fetchedVehicle);
            Assert.AreEqual(vehicleIdToFetch, fetchedVehicle.VehicleID);
            Assert.AreEqual(tempVehicle.RegistrationNumber, fetchedVehicle.RegistrationNumber);
        }

        [Test]
        public void UpdateVehicle_ShouldModifyVehicleInDatabase()
        {
            // Arrange - Create a vehicle to update
            Vehicle tempVehicle = new Vehicle
            {
                Model = "UpdateOldModel",
                Make = "UpdateOldMake",
                Year = 2018,
                Color = "Green",
                RegistrationNumber = "UPDVH_" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
                Availability = true,
                DailyRate = 30.00m
            };
            vehicleService.AddVehicle(tempVehicle);
            var added = vehicleService.GetAvailableVehicles().Find(v => v.RegistrationNumber == tempVehicle.RegistrationNumber);
            Assert.IsNotNull(added, "Pre-test setup failed: Could not add vehicle for update.");
            testVehicleId = added.VehicleID; // Store for cleanup

            // Modify the vehicle
            added.Color = "UpdatedColor";
            added.DailyRate = 35.00m;
            added.Availability = false;

            // Act
            vehicleService.UpdateVehicle(added);

            // Assert
            Vehicle updatedVehicle = vehicleService.GetVehicleById(testVehicleId);
            Assert.IsNotNull(updatedVehicle);
            Assert.AreEqual("UpdatedColor", updatedVehicle.Color);
            Assert.AreEqual(35.00m, updatedVehicle.DailyRate);
            Assert.AreEqual(false, updatedVehicle.Availability);
        }

        [Test]
        public void DeleteVehicle_ShouldRemoveVehicleFromDatabase()
        {
            // Arrange - Create a vehicle to delete
            Vehicle tempVehicle = new Vehicle
            {
                Model = "DeleteTestModel",
                Make = "DeleteTestMake",
                Year = 2015,
                Color = "Black",
                RegistrationNumber = "DELVEH_" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
                Availability = true,
                DailyRate = 20.00m
            };
            vehicleService.AddVehicle(tempVehicle);
            var added = vehicleService.GetAvailableVehicles().Find(v => v.RegistrationNumber == tempVehicle.RegistrationNumber);
            Assert.IsNotNull(added, "Pre-test setup failed: Could not add vehicle for delete.");
            int vehicleIdToDelete = added.VehicleID;
            // No need to set testVehicleId for TearDown as it will be deleted here

            // Act
            vehicleService.RemoveVehicle(vehicleIdToDelete);

            // Assert
            // Try to fetch the deleted vehicle, it should be null
            Vehicle deletedVehicle = vehicleService.GetVehicleById(vehicleIdToDelete);
            Assert.IsNull(deletedVehicle);
        }

        [Test]
        public void GetAvailableVehicles_ShouldReturnOnlyAvailableVehicles()
        {
            // Arrange: Ensure some available and some not-available vehicles exist
            // This setup is crucial. For robust testing, you'd insert specific vehicles here.
            // Example:
            // vehicleService.AddVehicle(new Vehicle { /* ... Available ... */ });
            // vehicleService.AddVehicle(new Vehicle { /* ... NotAvailable ... */ });

            // Act
            List<Vehicle> availableVehicles = vehicleService.GetAvailableVehicles();

            // Assert
            Assert.IsNotNull(availableVehicles);
            CollectionAssert.AllItemsAreNotNull(availableVehicles);
            // Check that all returned vehicles actually have Availability = "Available"
            foreach (var vehicle in availableVehicles)
            {
                Assert.AreEqual(true, vehicle.Availability, $"Vehicle {vehicle.VehicleID} should be Available but is {vehicle.Availability}");
            }
        }
    }
}