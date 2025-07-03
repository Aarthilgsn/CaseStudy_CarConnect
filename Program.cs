using CarConnectApp.Exceptions;
using CarConnectApp.Dao.Implementations;
using CarConnectApp.Dao.Interfaces;
using CarConnectApp.Entity;
using CarConnectApp.Service;
using CarConnectApp.Util;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Globalization; // For parsing dates

namespace CarConnectApp
{
    internal class Program
    {
        private static readonly AuthenticationService _authService = new AuthenticationService();
        private static readonly ICustomerService _customerService = new CustomerService();
        private static readonly IVehicleService _vehicleService = new VehicleService();
        private static readonly IReservationService _reservationService = new ReservationService();
        private static readonly IAdminService _adminService = new AdminService();
        private static readonly ReportGenerator _reportGenerator = new ReportGenerator();

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to CarConnect - Your Car Rental Platform!");

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n--- Main Menu ---");
                Console.WriteLine("1. Customer Login");
                Console.WriteLine("2. Admin Login");
                Console.WriteLine("3. New Customer Registration");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            CustomerLogin();
                            break;
                        case "2":
                            AdminLogin();
                            break;
                        case "3":
                            RegisterNewCustomer();
                            break;
                        case "4":
                            exit = true;
                            Console.WriteLine("Thank you for using CarConnect. Goodbye!");
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                }
            }
        }

        static void CustomerLogin()
        {
            Console.WriteLine("\n--- Customer Login ---");
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();

            try
            {
                Customer loggedInCustomer = _authService.CustomerLogin(username, password);
                Console.WriteLine($"Login successful! Welcome, {loggedInCustomer.FirstName}!");
                CustomerMenu(loggedInCustomer);
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error during login: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void AdminLogin()
        {
            Console.WriteLine("\n--- Admin Login ---");
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();

            try
            {
                Admin loggedInAdmin = _authService.AdminLogin(username, password);
                Console.WriteLine($"Login successful! Welcome, Admin {loggedInAdmin.FirstName}!");
                AdminMenu(loggedInAdmin);
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error during login: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void RegisterNewCustomer()
        {
            Console.WriteLine("\n--- New Customer Registration ---");
            Customer newCustomer = new Customer();

            Console.Write("Enter First Name: ");
            newCustomer.FirstName = Console.ReadLine();
            Console.Write("Enter Last Name: ");
            newCustomer.LastName = Console.ReadLine();
            Console.Write("Enter Email: ");
            newCustomer.Email = Console.ReadLine();
            Console.Write("Enter Phone Number: ");
            newCustomer.PhoneNumber = Console.ReadLine();
            Console.Write("Enter Address: ");
            newCustomer.Address = Console.ReadLine();
            Console.Write("Enter Desired Username: ");
            newCustomer.Username = Console.ReadLine();
            Console.Write("Enter Password: ");
            newCustomer.Password = Console.ReadLine(); // In a real app, hash this password!
            newCustomer.RegistrationDate = DateTime.Now;

            try
            {
                _customerService.RegisterCustomer(newCustomer);
                Console.WriteLine("Customer registered successfully!");
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error during registration: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during registration: {ex.Message}");
            }
        }

        static void CustomerMenu(Customer customer)
        {
            bool logout = false;
            while (!logout)
            {
                Console.WriteLine($"\n--- Customer Menu ({customer.Username}) ---");
                Console.WriteLine("1. Browse Available Vehicles");
                Console.WriteLine("2. Book a Reservation");
                Console.WriteLine("3. View My Reservations");
                Console.WriteLine("4. Update My Profile");
                Console.WriteLine("5. Cancel a Reservation");
                Console.WriteLine("6. Logout");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            BrowseAvailableVehicles();
                            break;
                        case "2":
                            BookReservation(customer.CustomerID);
                            break;
                        case "3":
                            ViewMyReservations(customer.CustomerID);
                            break;
                        case "4":
                            UpdateCustomerProfile(customer);
                            break;
                        case "5":
                            CancelReservation();
                            break;
                        case "6":
                            logout = true;
                            Console.WriteLine("Logged out successfully.");
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred in customer menu: {ex.Message}");
                }
            }
        }

        static void BrowseAvailableVehicles()
        {
            Console.WriteLine("\n--- Available Vehicles ---");
            try
            {
                List<Vehicle> vehicles = _vehicleService.GetAvailableVehicles();
                if (vehicles.Count == 0)
                {
                    Console.WriteLine("No vehicles currently available.");
                    return;
                }

                foreach (var vehicle in vehicles)
                {
                    Console.WriteLine($"Vehicle ID: {vehicle.VehicleID}");
                    Console.WriteLine($"  Model: {vehicle.Model}");
                    Console.WriteLine($"  Make: {vehicle.Make}");
                    Console.WriteLine($"  Year: {vehicle.Year}");
                    Console.WriteLine($"  Color: {vehicle.Color}");
                    Console.WriteLine($"  Registration Number: {vehicle.RegistrationNumber}");
                    Console.WriteLine($"  Daily Rate: ${vehicle.DailyRate:F2}");
                    Console.WriteLine("--------------------");
                }
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error while Browse vehicles: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        
        static void BookReservation(int customerId)
        {
            Console.WriteLine("\n--- Book a Reservation ---");
            Console.Write("Enter Vehicle ID you wish to reserve: ");
            if (!int.TryParse(Console.ReadLine(), out int vehicleId))
            {
                Console.WriteLine("Invalid Vehicle ID. Please enter a number.");
                return;
            }

            Vehicle selectedVehicle;
            try
            {
                selectedVehicle = _vehicleService.GetVehicleById(vehicleId);
                if (selectedVehicle == null || !selectedVehicle.Availability)
                {
                    throw new VehicleNotFoundException($"Vehicle with ID {vehicleId} is not found or not available.");
                }
            }
            catch (VehicleNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return;
            }

            Console.Write("Enter Start Date (YYYY-MM-DD): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
            {
                Console.WriteLine("Invalid Start Date format. Please use YYYY-MM-DD.");
                return;
            }

            Console.Write("Enter End Date (YYYY-MM-DD): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                Console.WriteLine("Invalid End Date format. Please use YYYY-MM-DD.");
                return;
            }

            if (startDate >= endDate)
            {
                Console.WriteLine("End Date must be after Start Date.");
                return;
            }
            if (startDate < DateTime.Today)
            {
                Console.WriteLine("Start Date cannot be in the past.");
                return;
            }


            Reservation newReservation = new Reservation
            {
                CustomerID = customerId,
                VehicleID = vehicleId,
                StartDate = startDate,
                EndDate = endDate,
                Status = "pending" // Initial status
            };

            try
            {
                _reservationService.BookReservation(newReservation);
                Console.WriteLine("Reservation booked successfully!");
                Console.WriteLine($"Estimated Total Cost: ${newReservation.TotalCost:F2}"); // TotalCost is calculated in service
            }
            catch (ReservationException ex)
            {
                Console.WriteLine($"Reservation failed: {ex.Message}");
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error during reservation: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during reservation: {ex.Message}");
            }
        }
        
       
        static void ViewMyReservations(int customerId)
        {
            Console.WriteLine("\n--- My Reservations ---");
            try
            {
                List<Reservation> reservations = _reservationService.GetReservationsByCustomerId(customerId);
                if (reservations.Count == 0)
                {
                    Console.WriteLine("You have no reservations.");
                    return;
                }

                foreach (var reservation in reservations)
                {
                    Vehicle vehicle = _vehicleService.GetVehicleById(reservation.VehicleID);
                    Console.WriteLine($"Reservation ID: {reservation.ReservationID}");
                    Console.WriteLine($"  Vehicle: {vehicle?.Make} {vehicle?.Model} (Reg: {vehicle?.RegistrationNumber})");
                    Console.WriteLine($"  Start Date: {reservation.StartDate.ToShortDateString()}");
                    Console.WriteLine($"  End Date: {reservation.EndDate.ToShortDateString()}");
                    Console.WriteLine($"  Total Cost: ${reservation.TotalCost:F2}");
                    Console.WriteLine($"  Status: {reservation.Status}");
                    Console.WriteLine("--------------------");
                }
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error while viewing reservations: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void UpdateCustomerProfile(Customer customer)
        {
            Console.WriteLine("\n--- Update Your Profile ---");
            Console.WriteLine($"Current First Name: {customer.FirstName}");
            Console.Write("Enter New First Name (leave blank to keep current): ");
            string newFirstName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newFirstName)) customer.FirstName = newFirstName;

            Console.WriteLine($"Current Last Name: {customer.LastName}");
            Console.Write("Enter New Last Name (leave blank to keep current): ");
            string newLastName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newLastName)) customer.LastName = newLastName;

            Console.WriteLine($"Current Email: {customer.Email}");
            Console.Write("Enter New Email (leave blank to keep current): ");
            string newEmail = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newEmail)) customer.Email = newEmail;

            Console.WriteLine($"Current Phone Number: {customer.PhoneNumber}");
            Console.Write("Enter New Phone Number (leave blank to keep current): ");
            string newPhoneNumber = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newPhoneNumber)) customer.PhoneNumber = newPhoneNumber;

            Console.WriteLine($"Current Address: {customer.Address}");
            Console.Write("Enter New Address (leave blank to keep current): ");
            string newAddress = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newAddress)) customer.Address = newAddress;

            Console.Write("Enter New Password (leave blank to keep current, CAUTION: not hashed here): ");
            string newPassword = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newPassword)) customer.Password = newPassword; // Again, in real app, hash this!

            try
            {
                _customerService.UpdateCustomer(customer);
                Console.WriteLine("Profile updated successfully!");
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error during profile update: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during profile update: {ex.Message}");
            }
        }

        static void CancelReservation()
        {
            Console.WriteLine("\n--- Cancel a Reservation ---");
            Console.Write("Enter Reservation ID to cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int reservationId))
            {
                Console.WriteLine("Invalid Reservation ID. Please enter a number.");
                return;
            }

            try
            {
                _reservationService.CancelReservation(reservationId);
                Console.WriteLine($"Reservation {reservationId} cancelled successfully.");
            }
            catch (ReservationException ex)
            {
                Console.WriteLine($"Cancellation failed: {ex.Message}");
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error during cancellation: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during cancellation: {ex.Message}");
            }
        }


        static void AdminMenu(Admin admin)
        {
            bool logout = false;
            while (!logout)
            {
                Console.WriteLine($"\n--- Admin Menu ({admin.Username}) ---");
                Console.WriteLine("1. Manage Vehicles (Add, Update, Remove)");
                Console.WriteLine("2. Manage Customers (Update, Delete)");
                Console.WriteLine("3. Manage Admins (Add, Update, Delete)");
                Console.WriteLine("4. Generate Reports");
                Console.WriteLine("5. Logout");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            ManageVehicles();
                            break;
                        case "2":
                            ManageCustomers();
                            break;
                        case "3":
                            ManageAdmins();
                            break;
                        case "4":
                            GenerateReports();
                            break;
                        case "5":
                            logout = true;
                            Console.WriteLine("Logged out successfully.");
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred in admin menu: {ex.Message}");
                }
            }
        }

        static void ManageVehicles()
        {
            Console.WriteLine("\n--- Manage Vehicles ---");
            Console.WriteLine("1. Add New Vehicle");
            Console.WriteLine("2. Update Vehicle Details");
            Console.WriteLine("3. Remove Vehicle");
            Console.WriteLine("4. View All Vehicles (Available and Unavailable)");
            Console.WriteLine("5. Back to Admin Menu");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();
            try
            {
                switch (choice)
                {
                    case "1":
                        AddNewVehicle();
                        break;
                    case "2":
                        UpdateVehicleDetails();
                        break;
                    case "3":
                        RemoveVehicle();
                        break;
                    case "4":
                        ViewAllVehicles();
                        break;
                    case "5":
                        break; // Go back to admin menu
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while managing vehicles: {ex.Message}");
            }
        }

        static void AddNewVehicle()
        {
            Console.WriteLine("\n--- Add New Vehicle ---");
            Vehicle newVehicle = new Vehicle();

            Console.Write("Enter Model: ");
            newVehicle.Model = Console.ReadLine();
            Console.Write("Enter Make: ");
            newVehicle.Make = Console.ReadLine();
            Console.Write("Enter Year: ");
            if (!int.TryParse(Console.ReadLine(), out int year))
            {
                throw new InvalidInputException("Invalid Year. Please enter a number.");
            }
            newVehicle.Year = year;
            Console.Write("Enter Color: ");
            newVehicle.Color = Console.ReadLine();
            Console.Write("Enter Registration Number: ");
            newVehicle.RegistrationNumber = Console.ReadLine();
            Console.Write("Is Available (true/false): ");
            if (!bool.TryParse(Console.ReadLine(), out bool availability))
            {
                throw new InvalidInputException("Invalid Availability. Please enter 'true' or 'false'.");
            }
            newVehicle.Availability = availability;
            Console.Write("Enter Daily Rate: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal dailyRate))
            {
                throw new InvalidInputException("Invalid Daily Rate. Please enter a number.");
            }
            newVehicle.DailyRate = dailyRate;

            try
            {
                _vehicleService.AddVehicle(newVehicle);
                Console.WriteLine("Vehicle added successfully!");
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
        }

        static void UpdateVehicleDetails()
        {
            Console.WriteLine("\n--- Update Vehicle Details ---");
            Console.Write("Enter Vehicle ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int vehicleId))
            {
                Console.WriteLine("Invalid Vehicle ID.");
                return;
            }

            try
            {
                Vehicle existingVehicle = _vehicleService.GetVehicleById(vehicleId);
                if (existingVehicle == null)
                {
                    throw new VehicleNotFoundException($"Vehicle with ID {vehicleId} not found.");
                }

                Console.WriteLine($"Current Model: {existingVehicle.Model}");
                Console.Write("Enter New Model (leave blank to keep current): ");
                string newModel = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newModel)) existingVehicle.Model = newModel;

                Console.WriteLine($"Current Make: {existingVehicle.Make}");
                Console.Write("Enter New Make (leave blank to keep current): ");
                string newMake = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newMake)) existingVehicle.Make = newMake;

                Console.WriteLine($"Current Year: {existingVehicle.Year}");
                Console.Write("Enter New Year (leave blank to keep current, enter 0 to skip): ");
                string newYearStr = Console.ReadLine();
                if (int.TryParse(newYearStr, out int newYear) && newYear != 0) existingVehicle.Year = newYear;

                Console.WriteLine($"Current Color: {existingVehicle.Color}");
                Console.Write("Enter New Color (leave blank to keep current): ");
                string newColor = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newColor)) existingVehicle.Color = newColor;

                Console.WriteLine($"Current Registration Number: {existingVehicle.RegistrationNumber}");
                Console.Write("Enter New Registration Number (leave blank to keep current): ");
                string newRegNumber = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newRegNumber)) existingVehicle.RegistrationNumber = newRegNumber;

                Console.WriteLine($"Current Availability: {existingVehicle.Availability}");
                Console.Write("Enter New Availability (true/false, leave blank to keep current): ");
                string newAvailabilityStr = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newAvailabilityStr) && bool.TryParse(newAvailabilityStr, out bool newAvailability)) existingVehicle.Availability = newAvailability;

                Console.WriteLine($"Current Daily Rate: {existingVehicle.DailyRate}");
                Console.Write("Enter New Daily Rate (leave blank to keep current, enter 0 to skip): ");
                string newDailyRateStr = Console.ReadLine();
                if (decimal.TryParse(newDailyRateStr, out decimal newDailyRate) && newDailyRate != 0) existingVehicle.DailyRate = newDailyRate;


                _vehicleService.UpdateVehicle(existingVehicle);
                Console.WriteLine("Vehicle updated successfully!");
            }
            catch (VehicleNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (InvalidInputException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void RemoveVehicle()
        {
            Console.WriteLine("\n--- Remove Vehicle ---");
            Console.Write("Enter Vehicle ID to remove: ");
            if (!int.TryParse(Console.ReadLine(), out int vehicleId))
            {
                Console.WriteLine("Invalid Vehicle ID. Please enter a number.");
                return;
            }

            try
            {
                _vehicleService.RemoveVehicle(vehicleId);
                Console.WriteLine($"Vehicle {vehicleId} removed successfully.");
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error during removal: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during removal: {ex.Message}");
            }
        }

        static void ViewAllVehicles()
        {
            Console.WriteLine("\n--- All Vehicles in System ---");
            try
            {
                // This would require a GetAllVehicles method in IVehicleService
                // For demonstration, let's use GetAvailableVehicles and also show how to fetch unavailable ones.
                List<Vehicle> vehicles = new List<Vehicle>();
                using (SqlConnection conn = DBConnUtil.GetDBConnection())
                {
                    string query = "SELECT * FROM Vehicle";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        vehicles.Add(new Vehicle
                        {
                            VehicleID = Convert.ToInt32(reader["VehicleID"]),
                            Model = reader["Model"].ToString(),
                            Make = reader["Make"].ToString(),
                            Year = Convert.ToInt32(reader["Year"]),
                            Color = reader["Color"].ToString(),
                            RegistrationNumber = reader["RegistrationNumber"].ToString(),
                            Availability = Convert.ToBoolean(reader["Availability"]),
                            DailyRate = Convert.ToDecimal(reader["DailyRate"])
                        });
                    }
                    reader.Close();
                }

                if (vehicles.Count == 0)
                {
                    Console.WriteLine("No vehicles found in the system.");
                    return;
                }

                foreach (var vehicle in vehicles)
                {
                    Console.WriteLine($"Vehicle ID: {vehicle.VehicleID}");
                    Console.WriteLine($"  Model: {vehicle.Model}");
                    Console.WriteLine($"  Make: {vehicle.Make}");
                    Console.WriteLine($"  Year: {vehicle.Year}");
                    Console.WriteLine($"  Color: {vehicle.Color}");
                    Console.WriteLine($"  Registration Number: {vehicle.RegistrationNumber}");
                    Console.WriteLine($"  Daily Rate: ${vehicle.DailyRate:F2}");
                    Console.WriteLine($"  Availability: {(vehicle.Availability ? "Available" : "Not Available")}");
                    Console.WriteLine("--------------------");
                }
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error while viewing all vehicles: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }


        static void ManageCustomers()
        {
            Console.WriteLine("\n--- Manage Customers ---");
            Console.WriteLine("1. Update Customer Details");
            Console.WriteLine("2. Delete Customer");
            Console.WriteLine("3. View All Customers");
            Console.WriteLine("4. Back to Admin Menu");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();
            try
            {
                switch (choice)
                {
                    case "1":
                        UpdateCustomerDetailsByAdmin();
                        break;
                    case "2":
                        DeleteCustomer();
                        break;
                    case "3":
                        ViewAllCustomers();
                        break;
                    case "4":
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while managing customers: {ex.Message}");
            }
        }

        static void UpdateCustomerDetailsByAdmin()
        {
            Console.WriteLine("\n--- Update Customer Details (Admin) ---");
            Console.Write("Enter Customer ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid Customer ID.");
                return;
            }

            try
            {
                Customer existingCustomer = _customerService.GetCustomerById(customerId);
                if (existingCustomer == null)
                {
                    throw new InvalidInputException($"Customer with ID {customerId} not found.");
                }

                Console.WriteLine($"Current First Name: {existingCustomer.FirstName}");
                Console.Write("Enter New First Name (leave blank to keep current): ");
                string newFirstName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newFirstName)) existingCustomer.FirstName = newFirstName;

                Console.WriteLine($"Current Last Name: {existingCustomer.LastName}");
                Console.Write("Enter New Last Name (leave blank to keep current): ");
                string newLastName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newLastName)) existingCustomer.LastName = newLastName;

                Console.WriteLine($"Current Email: {existingCustomer.Email}");
                Console.Write("Enter New Email (leave blank to keep current): ");
                string newEmail = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newEmail)) existingCustomer.Email = newEmail;

                Console.WriteLine($"Current Phone Number: {existingCustomer.PhoneNumber}");
                Console.Write("Enter New Phone Number (leave blank to keep current): ");
                string newPhoneNumber = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newPhoneNumber)) existingCustomer.PhoneNumber = newPhoneNumber;

                Console.WriteLine($"Current Address: {existingCustomer.Address}");
                Console.Write("Enter New Address (leave blank to keep current): ");
                string newAddress = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newAddress)) existingCustomer.Address = newAddress;

                // Admin can also reset password for customer, though not hashing here for simplicity
                Console.Write("Enter New Password (leave blank to keep current): ");
                string newPassword = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newPassword)) existingCustomer.Password = newPassword;

                _customerService.UpdateCustomer(existingCustomer);
                Console.WriteLine("Customer updated successfully!");
            }
            catch (InvalidInputException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void DeleteCustomer()
        {
            Console.WriteLine("\n--- Delete Customer ---");
            Console.Write("Enter Customer ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid Customer ID. Please enter a number.");
                return;
            }

            try
            {
                _customerService.DeleteCustomer(customerId);
                Console.WriteLine($"Customer {customerId} deleted successfully.");
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error during deletion: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during deletion: {ex.Message}");
            }
        }

        static void ViewAllCustomers()
        {
            Console.WriteLine("\n--- All Customers in System ---");
            try
            {
                // This would require a GetAllCustomers method in ICustomerService
                // For demonstration, let's just fetch existing customers by iterating IDs or from a dummy list
                // In a real scenario, you'd add a GetAllCustomers() to ICustomerService.
                Console.WriteLine("Note: A comprehensive 'All Customers' list would require a GetAllCustomers method in ICustomerService.");
                Console.WriteLine("Fetching a sample customer (ID 1) as an example:");
                Customer sampleCustomer = _customerService.GetCustomerById(1); // Example

                if (sampleCustomer != null)
                {
                    Console.WriteLine($"Customer ID: {sampleCustomer.CustomerID}");
                    Console.WriteLine($"  Name: {sampleCustomer.FirstName} {sampleCustomer.LastName}");
                    Console.WriteLine($"  Email: {sampleCustomer.Email}");
                    Console.WriteLine($"  Phone: {sampleCustomer.PhoneNumber}");
                    Console.WriteLine($"  Address: {sampleCustomer.Address}");
                    Console.WriteLine($"  Username: {sampleCustomer.Username}");
                    Console.WriteLine($"  Registration Date: {sampleCustomer.RegistrationDate.ToShortDateString()}");
                    Console.WriteLine("--------------------");
                }
                else
                {
                    Console.WriteLine("No customers found or sample customer not found.");
                }
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error while viewing customers: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void ManageAdmins()
        {
            Console.WriteLine("\n--- Manage Admins ---");
            Console.WriteLine("1. Add New Admin");
            Console.WriteLine("2. Update Admin Details");
            Console.WriteLine("3. Delete Admin");
            Console.WriteLine("4. View All Admins");
            Console.WriteLine("5. Back to Admin Menu");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();
            try
            {
                switch (choice)
                {
                    case "1":
                        AddNewAdmin();
                        break;
                    case "2":
                        UpdateAdminDetails();
                        break;
                    case "3":
                        DeleteAdmin();
                        break;
                    case "4":
                        ViewAllAdmins();
                        break;
                    case "5":
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while managing admins: {ex.Message}");
            }
        }

        static void AddNewAdmin()
        {
            Console.WriteLine("\n--- Add New Admin ---");
            Admin newAdmin = new Admin();

            Console.Write("Enter First Name: ");
            newAdmin.FirstName = Console.ReadLine();
            Console.Write("Enter Last Name: ");
            newAdmin.LastName = Console.ReadLine();
            Console.Write("Enter Email: ");
            newAdmin.Email = Console.ReadLine();
            Console.Write("Enter Phone Number: ");
            newAdmin.PhoneNumber = Console.ReadLine();
            Console.Write("Enter Desired Username: ");
            newAdmin.Username = Console.ReadLine();
            Console.Write("Enter Password: ");
            newAdmin.Password = Console.ReadLine(); // In a real app, hash this!
            Console.Write("Enter Role (e.g., super admin, fleet manager): ");
            newAdmin.Role = Console.ReadLine();
            newAdmin.JoinDate = DateTime.Now;

            try
            {
                _adminService.AddAdmin(newAdmin);
                Console.WriteLine("Admin added successfully!");
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void UpdateAdminDetails()
        {
            Console.WriteLine("\n--- Update Admin Details ---");
            Console.Write("Enter Admin ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int adminId))
            {
                Console.WriteLine("Invalid Admin ID.");
                return;
            }

            try
            {
                Admin existingAdmin = _adminService.GetAdminById(adminId);
                if (existingAdmin == null)
                {
                    throw new AdminNotFoundException($"Admin with ID {adminId} not found.");
                }

                Console.WriteLine($"Current First Name: {existingAdmin.FirstName}");
                Console.Write("Enter New First Name (leave blank to keep current): ");
                string newFirstName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newFirstName)) existingAdmin.FirstName = newFirstName;

                Console.WriteLine($"Current Last Name: {existingAdmin.LastName}");
                Console.Write("Enter New Last Name (leave blank to keep current): ");
                string newLastName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newLastName)) existingAdmin.LastName = newLastName;

                Console.WriteLine($"Current Email: {existingAdmin.Email}");
                Console.Write("Enter New Email (leave blank to keep current): ");
                string newEmail = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newEmail)) existingAdmin.Email = newEmail;

                Console.WriteLine($"Current Phone Number: {existingAdmin.PhoneNumber}");
                Console.Write("Enter New Phone Number (leave blank to keep current): ");
                string newPhoneNumber = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newPhoneNumber)) existingAdmin.PhoneNumber = newPhoneNumber;

                Console.WriteLine($"Current Username: {existingAdmin.Username}");
                Console.Write("Enter New Username (leave blank to keep current): ");
                string newUsername = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newUsername)) existingAdmin.Username = newUsername;

                Console.Write("Enter New Password (leave blank to keep current, CAUTION: not hashed here): ");
                string newPassword = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newPassword)) existingAdmin.Password = newPassword;

                Console.WriteLine($"Current Role: {existingAdmin.Role}");
                Console.Write("Enter New Role (leave blank to keep current): ");
                string newRole = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newRole)) existingAdmin.Role = newRole;

                _adminService.UpdateAdmin(existingAdmin);
                Console.WriteLine("Admin updated successfully!");
            }
            catch (AdminNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void DeleteAdmin()
        {
            Console.WriteLine("\n--- Delete Admin ---");
            Console.Write("Enter Admin ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int adminId))
            {
                Console.WriteLine("Invalid Admin ID. Please enter a number.");
                return;
            }

            try
            {
                _adminService.DeleteAdmin(adminId);
                Console.WriteLine($"Admin {adminId} deleted successfully.");
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error during deletion: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during deletion: {ex.Message}");
            }
        }

        static void ViewAllAdmins()
        {
            Console.WriteLine("\n--- All Admins in System ---");
            try
            {
                // This would require a GetAllAdmins method in IAdminService
                // For demonstration, let's just fetch existing admin by iterating IDs or from a dummy list
                // In a real scenario, you'd add a GetAllAdmins() to IAdminService.
                Console.WriteLine("Note: A comprehensive 'All Admins' list would require a GetAllAdmins method in IAdminService.");
                Console.WriteLine("Fetching a sample admin (ID 1) as an example:");
                Admin sampleAdmin = _adminService.GetAdminById(1); // Example

                if (sampleAdmin != null)
                {
                    Console.WriteLine($"Admin ID: {sampleAdmin.AdminID}");
                    Console.WriteLine($"  Name: {sampleAdmin.FirstName} {sampleAdmin.LastName}");
                    Console.WriteLine($"  Email: {sampleAdmin.Email}");
                    Console.WriteLine($"  Phone: {sampleAdmin.PhoneNumber}");
                    Console.WriteLine($"  Username: {sampleAdmin.Username}");
                    Console.WriteLine($"  Role: {sampleAdmin.Role}");
                    Console.WriteLine($"  Join Date: {sampleAdmin.JoinDate.ToShortDateString()}");
                    Console.WriteLine("--------------------");
                }
                else
                {
                    Console.WriteLine("No admins found or sample admin not found.");
                }
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error while viewing admins: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void GenerateReports()
        {
            Console.WriteLine("\n--- Generate Reports ---");
            Console.WriteLine("1. Reservation History Report");
            Console.WriteLine("2. Vehicle Utilization Report (Current Availability)");
            Console.WriteLine("3. Revenue Report");
            Console.WriteLine("4. Back to Admin Menu");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();
            try
            {
                switch (choice)
                {
                    case "1":
                        _reportGenerator.GenerateReservationHistoryReport();
                        break;
                    case "2":
                        _reportGenerator.GenerateVehicleUtilizationReport();
                        break;
                    case "3":
                        _reportGenerator.GenerateRevenueReport();
                        break;
                    case "4":
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while generating reports: {ex.Message}");
            }
        }
    }
}