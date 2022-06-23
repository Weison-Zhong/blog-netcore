using Blog2022_netcore.Common;
using Blog2022_netcore.Data;
using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;
using Blog2022_netcore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog2022_netcore.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly RoutineDbContext _context;
        private readonly IMenuRepository _menuRepository;

        public LoginController(RoutineDbContext routineDbContext, IMenuRepository menuRepository)
        {
            _context = routineDbContext ?? throw new ArgumentNullException(nameof(routineDbContext));
            _menuRepository = menuRepository ?? throw new ArgumentNullException(nameof(menuRepository));
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginParams loginParams)
        {
            if (string.IsNullOrEmpty(loginParams.username) || string.IsNullOrEmpty(loginParams.password))
            {
                return Ok(new ApiResult
                {
                    Msg = "账号或密码不能为空",
                    Code = 400
                });
            }
            string decodePassword = MD5Helper.MD5Encrypt32(loginParams.password);
            var administrator = await _context.Administrators.Include(x => x.Role).ThenInclude(x => x.Apis).ThenInclude(x => x.Api).ThenInclude(x => x.Menu).ThenInclude(x => x.ParentMenu).ThenInclude(x => x.Icon)
                                                   .Include(x => x.Role).ThenInclude(x => x.Apis).ThenInclude(x => x.Api).ThenInclude(x => x.Menu).ThenInclude(x => x.Icon)
                                               .FirstOrDefaultAsync(x => x.Name == loginParams.username && x.Password == decodePassword);
            if (administrator == null)
            {
                return Ok(new ApiResult
                {
                    Msg = "账号或密码错误",
                    Code = 400
                });
            }
            string roleName = administrator.Role.Name;
            JwtModel JwtModel = new JwtModel
            {
                UserName = loginParams.username,
                Password = decodePassword,
                RoleName = roleName
            };
            string token = "Bearer " + JwtHelper.CreateJwt(JwtModel);
            var roleMenusDto = new List<MenuDto>();
            if (administrator.Role.Name == "SuperAdmin")
            {
                var menus = await _menuRepository.GetParentMenus();
                roleMenusDto = MapperHelper.MenuDtoMapper(menus);
            }
            else if (administrator.Role.Apis != null)
            {
                //查找该角色拥有的权限菜单列表
                var rolePermissions = new List<ApiDto>();
                var roleChildMenus = new List<ChildMenu>();//角色拥有的权限对应的所有二级菜单列表
                                                           //遍历处理角色拥有的api权限
                foreach (var api in administrator.Role.Apis)
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
            LoginDto loginDto = new LoginDto
            {
                Token = token,
                RoleName = roleName,
                UserName = administrator.Name,
                MenuList = roleMenusDto,
                Id = administrator.Id,
                Avatar = administrator.Avatar
            };
            return Ok(new ApiResult
            {
                Data = loginDto
            });
        }
    }
    public class LoginParams
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
