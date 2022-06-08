using Blog2022_netcore.Data;
using Blog2022_netcore.Model;
using Blog2022_netcore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blog2022_netcore.Policy
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly RoutineDbContext _dbContext;
        private readonly IRoleRepository _roleRepository;

        public PermissionHandler(IHttpContextAccessor httpContextAccessor, IRoleRepository roleRepository)
        {
            _accessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var httpContext = _accessor.HttpContext;
            //获取当前账号的角色
            var RoleName = (from item in httpContext.User.Claims
                            where item.Type == ClaimTypes.Role
                            select item.Value).FirstOrDefault();
            if (RoleName == "SuperAdmin")
            {
                //若为超级管理员则直接拥有所有权限直接放行
                context.Succeed(requirement);
                return;
            }
            //查找该角色拥有的权限API列表
            var role = await _roleRepository.GetRoleByRoleName(RoleName);
            if (role == null)
            {
                context.Fail();
                return;
            }
            var rolePermissions = new List<ApiDto>();
            foreach (var api in role.Apis)
            {
                var roleApi = new ApiDto
                {
                    Id = api.Api.Id,
                    Title = api.Api.Title,
                    Key = api.Api.Key,
                    Description = api.Api.Description
                };
                rolePermissions.Add(roleApi);
            }
            //获取当前API的URL
            string questUrl = httpContext.Request.Path.Value.ToLower();
            //匹配正确则放行
            foreach (var item in rolePermissions)
            {
                if (questUrl.Contains(item.Key.ToLower()))
                {
                    context.Succeed(requirement);
                    return;
                }
            }
            context.Fail();
        }
    }
}
