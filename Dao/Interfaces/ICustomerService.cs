using CarConnectApp.Entity;

namespace CarConnectApp.Dao.Interfaces
{
    public interface ICustomerService
    {
        void RegisterCustomer(Customer customer);
        Customer GetCustomerById(int id);
        Customer GetCustomerByUsername(string username);
        void UpdateCustomer(Customer customer);
        //void DeleteCustomer(int id);
        void DeleteCustomer(int customerId);
    }
}