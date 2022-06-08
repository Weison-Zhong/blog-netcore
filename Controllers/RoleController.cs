using Blog2022_netcore.Common;
using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;
using Blog2022_netcore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog2022_netcore.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        }
        [HttpPost]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> AddRole(RoleAddOrUpdateDto newRole)
        {
            if (newRole == null)
            {
                throw new ArgumentNullException(nameof(newRole));
            }
            if (await _roleRepository.IsRoleExist(newRole.Name))
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该角色已存在，请更换角色名重试"
                });
            }
            var entity = new Role
            {
                Name = newRole.Name,
                CreatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                UpdatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            };
            _roleRepository.AddRole(entity);
            await _roleRepository.SaveAsync();
            return Ok(new ApiResult());
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleRepository.GetRoles();
            var roleDtos = new List<RoleDto>();
            foreach (var role in roles)
            {
                var rolePermissions = new List<ApiDto>();
                var roleChildMenus = new List<ChildMenu>();//角色拥有的权限对应的所有二级菜单列表
                var roleMenusDto = new List<MenuDto>();
                if (role.Apis != null)
                {
                    //遍历处理角色拥有的api权限
                    foreach (var api in role.Apis)
                    {
                        roleChildMenus.Add(api.Api.Menu);
                        BelongMenu belongMenu = new BelongMenu
                        {
                            MenuId = api.Api.Menu.Id,
                            MenuName = api.Api.Menu.Name,
                        };
                        var rolePermission = new ApiDto
                        {
                            Id = api.Api.Id,
                            Title = api.Api.Title,
                            Key = api.Api.Key,
                            Description = api.Api.Description,
                            CreatedDate = api.Api.CreatedDate,
                            UpdatedDate = api.Api.UpdatedDate,
                            BelongMenu = belongMenu
                        };
                        rolePermissions.Add(rolePermission);
                    }
                    roleMenusDto = MapperHelper.RoleMenuMapper(roleChildMenus);
                }
                RoleDto roleDto = new RoleDto
                {
                    Name = role.Name,
                    Id = role.Id,
                    CreatedDate = role.CreatedDate,
                    UpdatedDate = role.UpdatedDate,
                    Permissions = rolePermissions,
                    Menus = roleMenusDto
                };
                roleDtos.Add(roleDto);
            }
            ApiResult apiResult = new ApiResult
            {
                Data = roleDtos
            };
            return Ok(apiResult);
        }
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRole(Guid roleId)
        {
            var role = await _roleRepository.GetRole(roleId);
            var rolePermissions = new List<ApiDto>();
            var roleChildMenus = new List<ChildMenu>();//角色拥有的权限对应的所有二级菜单列表
            var roleMenusDto = new List<MenuDto>();
            if (role.Apis != null)
            {
                //遍历处理角色拥有的api权限
                foreach (var api in role.Apis)
                {
                    roleChildMenus.Add(api.Api.Menu);
                    BelongMenu belongMenu = new BelongMenu
                    {
                        MenuId = api.Api.Menu.Id,
                        MenuName = api.Api.Menu.Name,
                    };
                    var rolePermission = new ApiDto
                    {
                        Id = api.Api.Id,
                        Title = api.Api.Title,
                        Key = api.Api.Key,
                        Description = api.Api.Description,
                        CreatedDate = api.Api.CreatedDate,
                        UpdatedDate = api.Api.UpdatedDate,
                        BelongMenu = belongMenu
                    };
                    rolePermissions.Add(rolePermission);
                }
                roleMenusDto = MapperHelper.RoleMenuMapper(roleChildMenus);
            }
            RoleDto roleDto = new RoleDto
            {
                Name = role.Name,
                Id = role.Id,
                CreatedDate = role.CreatedDate,
                UpdatedDate = role.UpdatedDate,
                Permissions = rolePermissions,
                Menus = roleMenusDto,
            };
            ApiResult apiResult = new ApiResult
            {
                Data = roleDto
            };
            return Ok(apiResult);
        }

        [HttpDelete("{roleId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> DeleteRole(Guid roleId)
        {
            var role = await _roleRepository.GetRole(roleId);
            if (role == null)
            {
                return NotFound();
            }
            if (role.Name == "SuperAdmin")
            {
                return Ok(new ApiResult
                {
                    Code = 400,
                    Msg = "不能删除超级管理员"
                });
            }
            _roleRepository.DeleteRole(role);
            await _roleRepository.SaveAsync();
            return Ok(new ApiResult());
        }
        [HttpPut("{roleId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> UpdateRole(Guid roleId, RoleAddOrUpdateDto newRole)
        {
            var role = await _roleRepository.GetRole(roleId);
            if (role == null)
            {
                return NotFound();
            }
            if(role.Name == "SuperAdmin")
            {
                return Ok(new ApiResult
                {
                    Code = 400,
                    Msg = "不能修改超级管理员"
                });
            }
            string newName = newRole.Name;
            if (await _roleRepository.IsRoleExist(newName) && role.Name != newName)
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该角色已存在，请更换角色名重试"
                });
            }
            role.Name = newRole.Name;
            role.UpdatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            _roleRepository.UpdateRole(role);
            await _roleRepository.SaveAsync();
            return Ok(new ApiResult());
        }
        //更新角色API访问权限
        [HttpPost("{roleId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> UpdatePermissionForRole(Guid roleId, UpdateApiForRoleDto newApis)
        {
            var role = await _roleRepository.GetRole(roleId);
            if (role == null)
            {
                return NotFound();
            }
            if (role.Name == "SuperAdmin")
            {
                return Ok(new ApiResult
                {
                    Code = 400,
                    Msg = "不能修改超级管理员"
                });
            }
            _roleRepository.UpdateApiForRole(roleId, newApis);
            await _roleRepository.SaveAsync();
            return Ok(new ApiResult());
        }
    }


}
