using System.Collections.Generic;
using CarConnectApp.Entity;

namespace CarConnectApp.Dao.Interfaces
{
    public interface IVehicleService
    {
        void AddVehicle(Vehicle vehicle);
        Vehicle GetVehicleById(int vehicleId);
        List<Vehicle> GetAvailableVehicles();
        void UpdateVehicle(Vehicle vehicle);
        void RemoveVehicle(int vehicleId);
        Vehicle GetVehicleByRegistrationNumber(string registrationNumber);
    }
}