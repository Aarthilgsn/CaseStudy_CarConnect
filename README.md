# CarConnect: A Seamless Car Rental Solution

Welcome to CarConnect, a simple yet effective application designed to manage car rentals effortlessly. Whether you're booking a car, managing vehicles, or handling customer details, CarConnect aims to make the process smooth and efficient.

## About The Project

CarConnect is a console-based application built to demonstrate fundamental concepts of software development, including database interaction, service-oriented architecture, and robust error handling. It's designed to be easy to understand and extend.

## Key Features

* **Customer Management:** Register new customers and view existing profiles.
* **Vehicle Management:** Add new vehicles, track availability, and update vehicle details.
* **Reservation System:** Book, view, and cancel car reservations.
* **Admin Control:** Administrative functions for overall system management.
* **Exception Handling:** Gracefully handles common issues like database connection problems or invalid inputs.

## Technologies Used

* **Programming Language:** C# (C Sharp)
* **Framework:** .NET (specifically .NET 8.0, as indicated by project files)
* **Database:** SQL Server (for storing all application data)
* **Database Connectivity:** ADO.NET (for connecting C# application to SQL Server)
* **Unit Testing:** NUnit (for ensuring the code works as expected)

## Project Structure

```
CarConnectApp/
├── Dao/
│   ├── Implementations/
│   └── Interfaces/
├── Entity/
├── Exceptions/
├── Service/
│   ├── Implementations/
│   └── Interfaces/
├── Util/
├── CarConnectApp.csproj
└── Program.cs

CarConnectApp.Tests/
├── CustomerServiceTests.cs
├── ReservationServiceTests.cs
├── VehicleServiceTests.cs
├── AdminServiceTests.cs
└── CarConnectApp.Tests.csproj
```

## How to Get Started (For Developers)

### Requirements

* **Visual Studio**
* **SQL Server**
* **.NET 8.0 SDK**

### Steps

1. **Clone the Repository:**
    ```bash
    git clone https://github.com/your-username/CarConnect.git
    ```

2. **Set up Database:**
    * Create a SQL Server database (e.g., `CarConnectDB`).
    * Use your `.sql` script to create tables and insert sample data.

3. **Configure Database Connection:**
    * Edit `DBConnUtil.cs` in the `Util` folder to add your connection string.
    * Example:
    ```csharp
    return "Data Source=YourServerName;Initial Catalog=YourDatabaseName;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
    ```

4. **Open in Visual Studio:** Open `CarConnect.sln` file.

5. **Restore NuGet Packages:** Right-click the solution > "Restore NuGet Packages".

6. **Build the Solution:** Press `Ctrl+Shift+B`.

7. **Run the Application:** Run the `CarConnectApp` project.

8. **Run Tests:** Open `Test Explorer` and run all tests.


## Contribution

Feel free to fork this repository, suggest features, or contribute to the project. Any contributions are welcome!
