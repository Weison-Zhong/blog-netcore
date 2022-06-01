using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;

namespace Blog2022_netcore.Services
{
    public interface IRoleRepository
    {
        void AddRole(Role role);
        void DeleteRole(Role role);
        Task<IEnumerable<Role>> GetRoles();
        Task<Role> GetRole(Guid roleID);
        Task<Role> GetRoleByRoleName(string roleName);
        Task<bool> SaveAsync();
        Task<bool> IsRoleExist(string RoleName);
        void UpdateRole(Role role);
        void UpdateApiForRole(Guid roleId, UpdateApiForRoleDto newApiIds);
    }
}
