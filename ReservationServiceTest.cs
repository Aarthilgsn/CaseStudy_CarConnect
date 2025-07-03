using NUnit.Framework;
using CarConnectApp.Dao.Implementations;
using CarConnectApp.Dao.Interfaces;
using CarConnectApp.Entity;
using System;
using System.Collections.Generic;
using CarConnectApp.Exceptions;

namespace CarConnectApp.Tests
{
    [TestFixture]
    public class ReservationServiceTests
    {
        private ReservationService reservationService;
        private CustomerService customerService; // Needed to setup test customer
        private VehicleService vehicleService;    // Needed to setup test vehicle

        private int testCustomerId;
        private int testVehicleId;
        private int testReservationId; // Stores the ID of the LAST reservation booked by a test

        [SetUp]
        public void Setup()
        {
            reservationService = new ReservationService();
            customerService = new CustomerService();
            vehicleService = new VehicleService();

            // Arrange: Create a test customer and vehicle for reservations
            // This is crucial for isolating tests.
            string uniqueId = Guid.NewGuid().ToString().Substring(0, 8); // Use a slightly longer GUID substring for better uniqueness
            Customer tempCustomer = new Customer
            {
                FirstName = "ResTest",
                LastName = "Customer",
                Email = $"restest_{uniqueId}@example.com", // Make email unique
                PhoneNumber = $"111222{uniqueId.Substring(0, 4)}", // Make phone number unique
                Address = "TestAddress",
                Username = $"resuser_{uniqueId}", // Make username unique
                Password = "respass",
                RegistrationDate = DateTime.Now
            };
            customerService.RegisterCustomer(tempCustomer);
            var addedCustomer = customerService.GetCustomerByUsername(tempCustomer.Username);
            Assert.IsNotNull(addedCustomer, "Pre-test setup failed: Could not add test customer.");
            testCustomerId = addedCustomer.CustomerID;

            string uniqueVehicleId = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            Vehicle tempVehicle = new Vehicle
            {
                Model = "ResModel",
                Make = "ResMake",
                Year = 2024,
                Color = "White",
                RegistrationNumber = $"RESVEH_{uniqueVehicleId}", // Make registration number unique
                Availability = true,
                DailyRate = 90.00m
            };
            vehicleService.AddVehicle(tempVehicle);
            // It's safer to retrieve the added vehicle by its unique registration number directly from the DB
            // as GetAvailableVehicles might return many and finding the exact one by just properties can be tricky.
            // Assuming VehicleService has a GetVehicleByRegistrationNumber method.
            var addedVehicle = vehicleService.GetVehicleByRegistrationNumber(tempVehicle.RegistrationNumber);
            Assert.IsNotNull(addedVehicle, "Pre-test setup failed: Could not add test vehicle.");
            testVehicleId = addedVehicle.VehicleID;
        }

        [TearDown]
        public void Teardown()
        {
            // Clean up test data in reverse order of creation
            // IMPORTANT: If a test fails before setting testReservationId, this might not clean up.
            // Consider more robust transaction-based testing for a real-world scenario.
            if (testReservationId != 0)
            {
                try
                {
                    // Assuming CancelReservation also removes the reservation, or you have a direct delete method in DAO
                    // If CancelReservation just changes status, you might need a ReservationService.DeleteReservation(id)
                    // For now, let's assume CancelReservation makes it safe for future inserts,
                    // or for very robust cleanup, you'd use a direct DAO delete.
                    // If your CancelReservation only updates status, and you need to delete for actual cleanup,
                    // you'll need to add a method to ReservationService (and its DAO) to physically delete the reservation.
                    reservationService.CancelReservation(testReservationId);
                }
                catch (Exception ex)
                {
                    TestContext.WriteLine($"Warning: Failed to cleanup reservation {testReservationId}: {ex.Message}");
                }
                testReservationId = 0; // Reset for next test
            }

            if (testVehicleId != 0)
            {
                try { vehicleService.RemoveVehicle(testVehicleId); } // Assuming RemoveVehicle exists and deletes from DB
                catch (Exception ex) { TestContext.WriteLine($"Warning: Failed to cleanup vehicle {testVehicleId}: {ex.Message}"); }
                testVehicleId = 0;
            }
            if (testCustomerId != 0)
            {
                try { customerService.DeleteCustomer(testCustomerId); } // Assuming DeleteCustomer exists and deletes from DB
                catch (Exception ex) { TestContext.WriteLine($"Warning: Failed to cleanup customer {testCustomerId}: {ex.Message}"); }
                testCustomerId = 0;
            }
        }

        [Test]
        public void BookReservation_ShouldInsertReservationIntoDatabase()
        {
            // Arrange
            Reservation newReservation = new Reservation
            {
                CustomerID = testCustomerId,
                VehicleID = testVehicleId,
                StartDate = DateTime.Today.AddDays(5),
                EndDate = DateTime.Today.AddDays(10),
                Status = "Pending"
                // TotalCost will be calculated by the service
            };

            // Act
            reservationService.BookReservation(newReservation);

            // Assert
            // Need a way to retrieve the newly booked reservation.
            // If BookReservation adds an ID to the object, use that. Otherwise, fetch by CustomerID and VehicleID.
            // For now, let's assume we can fetch all by customer and find the newest one.
            var customerReservations = reservationService.GetReservationsByCustomerId(testCustomerId);
            Assert.IsNotNull(customerReservations);
            Assert.Greater(customerReservations.Count, 0, "No reservations found for the test customer.");

            // Find the most recent reservation or one matching the criteria
            // Filter by VehicleID, CustomerID, and date range for better accuracy
            Reservation bookedReservation = customerReservations
                .Find(r => r.VehicleID == newReservation.VehicleID &&
                           r.CustomerID == newReservation.CustomerID && // Added CustomerID to filter
                           r.StartDate.Date == newReservation.StartDate.Date &&
                           r.EndDate.Date == newReservation.EndDate.Date);

            Assert.IsNotNull(bookedReservation, "Booked reservation not found after insertion.");
            Assert.AreEqual(newReservation.Status, bookedReservation.Status);
            Assert.Greater(bookedReservation.TotalCost, 0m); // Ensure cost was calculated
            testReservationId = bookedReservation.ReservationID; // Store ID for teardown
        }

        [Test]
        public void GetReservationById_ShouldReturnCorrectReservation()
        {
            // Arrange: Book a reservation first
            Reservation tempReservation = new Reservation
            {
                CustomerID = testCustomerId,
                VehicleID = testVehicleId,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(3),
                Status = "Confirmed"
            };
            reservationService.BookReservation(tempReservation);
            var booked = reservationService.GetReservationsByCustomerId(testCustomerId)
                         .Find(r => r.VehicleID == tempReservation.VehicleID &&
                                    r.StartDate.Date == tempReservation.StartDate.Date &&
                                    r.EndDate.Date == tempReservation.EndDate.Date); // Ensure you find the correct one
            Assert.IsNotNull(booked, "Pre-test setup failed: Could not book reservation for get by ID.");
            int reservationIdToFetch = booked.ReservationID;
            testReservationId = reservationIdToFetch; // Store for cleanup

            // Act
            Reservation fetchedReservation = reservationService.GetReservationById(reservationIdToFetch);

            // Assert
            Assert.IsNotNull(fetchedReservation);
            Assert.AreEqual(reservationIdToFetch, fetchedReservation.ReservationID);
            Assert.AreEqual(tempReservation.Status, fetchedReservation.Status);
        }

        [Test]
        public void GetReservationsByCustomerId_ShouldReturnCorrectReservations()
        {
            // Arrange: Book multiple reservations for the test customer
            // Use distinct dates for multiple reservations for the same customer/vehicle if that's a constraint
            Reservation res1 = new Reservation { CustomerID = testCustomerId, VehicleID = testVehicleId, StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(2), Status = "Pending" };
            Reservation res2 = new Reservation { CustomerID = testCustomerId, VehicleID = testVehicleId, StartDate = DateTime.Today.AddDays(3), EndDate = DateTime.Today.AddDays(4), Status = "Pending" };
            reservationService.BookReservation(res1);
            reservationService.BookReservation(res2);

            // To ensure teardown cleans both, you might need to track multiple reservation IDs
            // or ensure your cleanup method is smart enough to find all for the customer/vehicle and delete them.
            // For simplicity here, we rely on the customer/vehicle cleanup.
            // However, a more robust teardown would query for all reservations for testCustomerId and delete them.
            // For now, let's get the IDs for potential teardown.
            var allCustomerReservations = reservationService.GetReservationsByCustomerId(testCustomerId);
            foreach (var res in allCustomerReservations)
            {
                // This is a simplistic way to make sure testReservationId holds one of the IDs for cleanup.
                // In a real scenario, you'd delete all reservations associated with testCustomerId and testVehicleId.
                testReservationId = res.ReservationID; // This will only store the last one
            }


            // Act
            List<Reservation> customerReservations = reservationService.GetReservationsByCustomerId(testCustomerId);

            // Assert
            Assert.IsNotNull(customerReservations);
            // Verify that at least the two reservations we just added are there.
            // Note: If previous test runs left data, count might be higher than 2.
            Assert.GreaterOrEqual(customerReservations.Count, 2, "Should have at least 2 reservations for the customer.");
            foreach (var res in customerReservations)
            {
                Assert.AreEqual(testCustomerId, res.CustomerID);
            }
        }

        [Test]
        public void CancelReservation_ShouldUpdateStatusToCancelled()
        {
            // Arrange: Book a reservation to cancel
            Reservation tempReservation = new Reservation
            {
                CustomerID = testCustomerId,
                VehicleID = testVehicleId,
                StartDate = DateTime.Today.AddDays(15),
                EndDate = DateTime.Today.AddDays(20),
                Status = "Confirmed" // Start as confirmed
            };
            reservationService.BookReservation(tempReservation);
            var booked = reservationService.GetReservationsByCustomerId(testCustomerId)
                         .Find(r => r.VehicleID == tempReservation.VehicleID &&
                                    r.StartDate.Date == tempReservation.StartDate.Date &&
                                    r.EndDate.Date == tempReservation.EndDate.Date); // Ensure correct one is found
            Assert.IsNotNull(booked, "Pre-test setup failed: Could not book reservation for cancellation.");
            int reservationIdToCancel = booked.ReservationID;
            testReservationId = reservationIdToCancel; // Store for cleanup

            // Act
            reservationService.CancelReservation(reservationIdToCancel);

            // Assert
            Reservation cancelledReservation = reservationService.GetReservationById(reservationIdToCancel);
            Assert.IsNotNull(cancelledReservation);
            Assert.AreEqual("Cancelled", cancelledReservation.Status);
        }

        // Add a test for scenario where no reservation is found for ID
        [Test]
        public void GetReservationById_ShouldReturnNullForInvalidId()
        {
            // Arrange
            int invalidReservationId = -1; // An ID that should not exist

            // Act
            Reservation fetchedReservation = reservationService.GetReservationById(invalidReservationId);

            // Assert
            Assert.IsNull(fetchedReservation, "Should return null for an invalid reservation ID.");
        }

        // Add a test for scenario where no reservations are found for a customer
        [Test]
        public void GetReservationsByCustomerId_ShouldReturnEmptyListForInvalidId()
        {
            // Arrange
            int invalidCustomerId = -1; // An ID that should not exist

            // Act
            List<Reservation> customerReservations = reservationService.GetReservationsByCustomerId(invalidCustomerId);

            // Assert
            Assert.IsNotNull(customerReservations);
            Assert.IsEmpty(customerReservations, "Should return an empty list for an invalid customer ID.");
        }
    }
}