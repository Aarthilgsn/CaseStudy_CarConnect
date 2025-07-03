using CarConnectApp.Entity;

namespace CarConnectApp.Dao.Interfaces
{
    public interface IAdminService
    {
        void AddAdmin(Admin admin);
        Admin GetAdminById(int id);
        Admin GetAdminByUsername(string username);
        void UpdateAdmin(Admin admin);
        void DeleteAdmin(int id);
    }
}