using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;

namespace Blog2022_netcore.Services
{
    public interface IAdministratorRepository
    {
        void AddAdministrator(Administrator administrator);
        void DeleteAdministrator(Administrator administrator);
        Task<IEnumerable<Administrator>> GetAdministrators();
        Task<Administrator> GetAdministrator(Guid administratorId);
        Task<bool> SaveAsync();
        Task<bool> IsAdministratorExist(string AdministratorName);
    }
}
