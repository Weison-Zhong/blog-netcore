using Blog2022_netcore.Common;
using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;
using Blog2022_netcore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog2022_netcore.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly IAdministratorRepository _administratorRepository;
        private readonly FileUploadHelper _fileUploadHelper;
        private readonly IHttpContextAccessor _accessor;
        private readonly IRoleRepository _roleRepository;
        private readonly IMenuRepository _menuRepository;

        public AdministratorController(IAdministratorRepository administratorRepository, IHostEnvironment hostEnvironment, IHttpContextAccessor httpContextAccessor, IRoleRepository roleRepository, IMenuRepository menuRepository)
        {
            _administratorRepository = administratorRepository ?? throw new ArgumentNullException(nameof(administratorRepository));
            _fileUploadHelper = new FileUploadHelper(hostEnvironment);
            _accessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _menuRepository = menuRepository ?? throw new ArgumentNullException(nameof(menuRepository));
        }
        [HttpPost]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> AddAdministrator([FromForm] AdministratorAddOrUpdateDto administratorAddDto)
        {
            if (administratorAddDto == null)
            {
                throw new ArgumentNullException(nameof(administratorAddDto));
            }
            if (await _administratorRepository.IsAdministratorExist(administratorAddDto.Name))
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该管理员已存在，请更换管理员账号名重试"
                });
            }
            string avatar = "";
            if (administratorAddDto.AvatarImg != null)
            {
                avatar = _fileUploadHelper.ImageUpload(administratorAddDto.AvatarImg);
            }
            Role role = await _roleRepository.GetRole(administratorAddDto.RoleId);
            if (role.Name == "SuperAdmin")
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "不能新增超级管理员"
                });
            }
            var entity = new Administrator
            {
                Name = administratorAddDto.Name,
                Password = MD5Helper.MD5Encrypt32(administratorAddDto.Password),
                Role = role,
                CreatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                UpdatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                Avatar = avatar
            };
            _administratorRepository.AddAdministrator(entity);
            await _administratorRepository.SaveAsync();
            return Ok(new ApiResult());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Administrator>>> GetAdministrators()
        {
            var administrators = await _administratorRepository.GetAdministrators();
            var returnDto = new List<AdministratorDto>();

            foreach (var item in administrators)
            {
                AdministratorDto administratorDto = new AdministratorDto
                {
                    Name = item.Name,
                    Id = item.Id,
                    Role = new RoleDtoForAdministrator { RoleName = item.Role.Name, RoleId = item.Role.Id },
                    AvatarUrl = item.Avatar
                };
                returnDto.Add(administratorDto);
            }
            return Ok(new ApiResult
            {
                Data = returnDto
            });
        }
        [HttpDelete("{administratorId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> DeleteAdministrator(Guid administratorId)
        {
            var administrator = await _administratorRepository.GetAdministrator(administratorId);
            if (administrator == null)
            {
                return NotFound();
            }

            if (administrator.Role.Name == "SuperAdmin")
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "不能删除超级管理员"
                });
            }
            _administratorRepository.DeleteAdministrator(administrator);
            await _administratorRepository.SaveAsync();
            return Ok(new ApiResult());
        }
        [HttpPut("{AdministratorId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> UpdateAdministrator(Guid AdministratorId, [FromForm] AdministratorAddOrUpdateDto administratorAddOrUpdateDto)
        {
            var administrator = await _administratorRepository.GetAdministrator(AdministratorId);
            if (administrator == null)
            {
                return NotFound();
            }
            string newName = administratorAddOrUpdateDto.Name;
            if (await _administratorRepository.IsAdministratorExist(newName) && administrator.Name != newName)
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该管理员已存在，请更换管理员账号名重试"
                });
            }
            Role role = await _roleRepository.GetRole(administratorAddOrUpdateDto.RoleId);
            if (role.Name == "SuperAdmin")
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "不能修改超级管理员"
                });
            }
            string avatar = "";
            if (administratorAddOrUpdateDto.AvatarImg != null)
            {
                avatar = _fileUploadHelper.ImageUpload(administratorAddOrUpdateDto.AvatarImg);
            }
            administrator.Name = administratorAddOrUpdateDto.Name;
            administrator.Password = MD5Helper.MD5Encrypt32(administratorAddOrUpdateDto.Password);
            administrator.Role = role;
            administrator.UpdatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            administrator.Avatar = avatar;
            await _administratorRepository.SaveAsync();
            return Ok(new ApiResult());
        }
        [HttpGet]
        public async Task<IActionResult> GetUserMenus()
        {
            var httpContext = _accessor.HttpContext;
            //获取当前账号的角色
            var RoleName = (from item in httpContext.User.Claims
                            where item.Type == ClaimTypes.Role
                            select item.Value).FirstOrDefault();
            if (RoleName == null)
            {
                return Ok(new ApiResult
                {
                    Code = 400,
                    Msg = "角色查询失败，请重新登录重试"
                });
            }
            else
            {
                //查找该角色拥有的权限菜单列表
                var role = await _roleRepository.GetRoleByRoleName(RoleName);
                var rolePermissions = new List<ApiDto>();
                var roleChildMenus = new List<ChildMenu>();//角色拥有的权限对应的所有二级菜单列表
                var roleMenusDto = new List<MenuDto>();
                if (RoleName == "SuperAdmin")
                {
                    var menus = await _menuRepository.GetParentMenus();
                    roleMenusDto = MapperHelper.MenuDtoMapper(menus);
                }
                else if (role.Apis != null)
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
                return Ok(new ApiResult
                {
                    Data = roleMenusDto
                });
            }
        }
    }
}
