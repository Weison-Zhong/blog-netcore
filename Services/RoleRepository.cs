using Blog2022_netcore.Data;
using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;
using Microsoft.EntityFrameworkCore;

namespace Blog2022_netcore.Services
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoutineDbContext _context;

        public RoleRepository(RoutineDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public void AddRole(Role role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            _context.Roles.Add(role);
        }

        public async Task<IEnumerable<Role>> GetRoles()
        {
            return await _context.Roles
                .Include(x => x.Apis).ThenInclude(x => x.Api).ThenInclude(x => x.Menu).ThenInclude(x => x.ParentMenu).ThenInclude(x=>x.Icon)
                .ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        void IRoleRepository.UpdateApiForRole(Guid roleId, UpdateApiForRoleDto updateApiForRoleDto)
        {
            if (roleId == null)
            {
                throw new ArgumentNullException(nameof(roleId));
            }
            if (updateApiForRoleDto.ApiIds == null)
            {
                throw new ArgumentNullException(nameof(updateApiForRoleDto.ApiIds));
            }
            //1，先删除该角色所有接口访问权限
            var oldApis = _context.RoleApi.Where(x => x.RoleId == roleId);
            _context.RoleApi.RemoveRange(oldApis);
            //2，重新赋予该角色新的接口访问权限
            foreach (Guid apiId in updateApiForRoleDto.ApiIds)
            {
                var RoleApi = new RoleApi
                {
                    RoleId = roleId,
                    ApiId = apiId
                };
                _context.Add(RoleApi);
            }
            _context.SaveChanges();
        }

        void IRoleRepository.DeleteRole(Role role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            _context.Roles.Remove(role);
        }

        async Task<Role> IRoleRepository.GetRole(Guid roleID)
        {
            return await _context.Roles
                .Include(x => x.Apis).ThenInclude(x => x.Api).ThenInclude(x => x.Menu).ThenInclude(x => x.ParentMenu)
                                         .FirstOrDefaultAsync(x => x.Id == roleID);
        }

        async Task<Role> IRoleRepository.GetRoleByRoleName(string roleName)
        {
            return await _context.Roles
                  .Include(x => x.Apis).ThenInclude(x => x.Api).ThenInclude(x => x.Menu).ThenInclude(x => x.ParentMenu)
                .FirstOrDefaultAsync(x => x.Name == roleName);
        }

        async Task<bool> IRoleRepository.IsRoleExist(string roleName)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == roleName);
            return role != null;
        }

        void IRoleRepository.UpdateRole(Role role)
        {

        }
    }
}
