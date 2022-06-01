using Blog2022_netcore.Common;
using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;
using Blog2022_netcore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog2022_netcore.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IIconRepository _iconRepository;
        public MenuController(IMenuRepository menuRepository, IIconRepository iconRepository)
        {
            _menuRepository = menuRepository ?? throw new ArgumentNullException(nameof(menuRepository));
            _iconRepository = iconRepository ?? throw new ArgumentNullException(nameof(iconRepository));
        }
        [HttpGet]
        public async Task<IActionResult> GetMenus()
        {
            var menus = await _menuRepository.GetParentMenus();
            var returnDto = MapperHelper.MenuDtoMapper(menus);
            return Ok(new ApiResult
            {
                Data = returnDto
            });
        }
        [HttpDelete("{menuId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> DeleteMenu(Guid menuId)
        {
            var childMenu = await _menuRepository.GetChildMenu(menuId);
            var parentMenu = await _menuRepository.GetParentMenu(menuId);
            if (childMenu == null && parentMenu == null)
            {
                return NotFound();
            }
            if (childMenu != null)
            {
                _menuRepository.DeleteChildMenu(childMenu);
                await _menuRepository.SaveAsync();
                return Ok(new ApiResult());
            }
            _menuRepository.DeleteParentMenu(parentMenu);
            await _menuRepository.SaveAsync();
            return Ok(new ApiResult());
        }
        [HttpPost]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> AddMenu(AddOrUpdateMenuDto addOrUpdateMenuDto)
        {
            if (addOrUpdateMenuDto == null)
            {
                throw new ArgumentNullException(nameof(addOrUpdateMenuDto));
            }
            if (await _menuRepository.IsMenuExist(addOrUpdateMenuDto.Name))
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该菜单名称已存在，请更换菜单名重试"
                });
            }
            if (await _menuRepository.IsMenuExist(addOrUpdateMenuDto.Key))
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该菜单路径已存在，请更换菜单路径重试"
                });
            }
            Icon icon = await _iconRepository.GetIconById(addOrUpdateMenuDto.IconId);
            string name = addOrUpdateMenuDto.Name;
            string key = addOrUpdateMenuDto.Key;
            string parentMenuIdStr = addOrUpdateMenuDto.ParentMenuId;
            int weight = addOrUpdateMenuDto.Weight;
            string createdDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string updatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            if (string.IsNullOrWhiteSpace(parentMenuIdStr))
            {
                //新增一级菜单

                var entity = new ParentMenu
                {
                    Name = name,
                    Key = key,
                    Icon = icon,
                    Weight = weight,
                    CreatedDate = createdDate,
                    UpdatedDate = updatedDate,
                };
                _menuRepository.AddParentMenu(entity);
            }
            else
            {
                //新增二级菜单
                Guid ParentMenuId = new Guid(parentMenuIdStr);
                var parentMenu = await _menuRepository.GetParentMenu(ParentMenuId);
                if (parentMenu == null)
                {
                    return BadRequest(new ApiResult
                    {
                        Code = 400,
                        Msg = "父级菜单查询失败，请检查后重试"
                    });
                }
                var entity = new ChildMenu
                {
                    Name = name,
                    KeepAlive = addOrUpdateMenuDto.KeepAlive,
                    Key = key,
                    ComponentPath = addOrUpdateMenuDto.ComponentPath,
                    Icon = icon,
                    Weight = weight,
                    ParentMenu = parentMenu,
                    CreatedDate = createdDate,
                    UpdatedDate = updatedDate,
                };
                _menuRepository.AddChildMenu(entity);
            }
            await _menuRepository.SaveAsync();
            return Ok(new ApiResult());
        }
        [HttpPut("{menuId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> UpdateMenu(Guid menuId, AddOrUpdateMenuDto addOrUpdateMenuDto)
        {
            if (addOrUpdateMenuDto == null)
            {
                throw new ArgumentNullException(nameof(addOrUpdateMenuDto));
            }
            ParentMenu ParentMenu = await _menuRepository.GetParentMenu(menuId);
            ChildMenu ChildMenu = await _menuRepository.GetChildMenu(menuId);
            Icon newIcon = await _iconRepository.GetIconById(addOrUpdateMenuDto.IconId);
            string newName = addOrUpdateMenuDto.Name;
            string newKey = addOrUpdateMenuDto.Key;
            int newWeight = addOrUpdateMenuDto.Weight;
            string updatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            if (ParentMenu == null && ChildMenu == null)
            {
                return NotFound();
            }
            if (ParentMenu != null)
            {
                //更新一级菜单
                if (await _menuRepository.IsMenuExist(addOrUpdateMenuDto.Name) && ParentMenu.Name != addOrUpdateMenuDto.Name)
                {
                    return BadRequest(new ApiResult
                    {
                        Code = 400,
                        Msg = "该菜单名称已存在，请更换菜单名重试"
                    });
                }
                if (await _menuRepository.IsMenuExist(addOrUpdateMenuDto.Key) && ParentMenu.Key != addOrUpdateMenuDto.Key)
                {
                    return BadRequest(new ApiResult
                    {
                        Code = 400,
                        Msg = "该菜单路径已存在，请更换菜单路径重试"
                    });
                }
                ParentMenu.Name = newName;
                ParentMenu.Key = newKey;
                ParentMenu.Icon = newIcon;
                ParentMenu.UpdatedDate = updatedDate;
                ParentMenu.Weight = newWeight;
                _menuRepository.UpdateParentMenu(ParentMenu);
            }
            else
            {
                //更新二级菜单
                if (await _menuRepository.IsMenuExist(addOrUpdateMenuDto.Name) && ChildMenu.Name != addOrUpdateMenuDto.Name)
                {
                    return BadRequest(new ApiResult
                    {
                        Code = 400,
                        Msg = "该菜单名称已存在，请更换菜单名重试"
                    });
                }
                if (await _menuRepository.IsMenuExist(addOrUpdateMenuDto.Key) && ChildMenu.Key != addOrUpdateMenuDto.Key)
                {
                    return BadRequest(new ApiResult
                    {
                        Code = 400,
                        Msg = "该菜单路径已存在，请更换菜单路径重试"
                    });
                }
                Guid ParentMenuId = new Guid(addOrUpdateMenuDto.ParentMenuId);
                var parentMenu = await _menuRepository.GetParentMenu(ParentMenuId);
                ChildMenu.Name = newName;
                ChildMenu.Key = newKey;
                ChildMenu.Icon = newIcon;
                ChildMenu.UpdatedDate = updatedDate;
                ChildMenu.Weight = newWeight;
                ChildMenu.ComponentPath = addOrUpdateMenuDto.ComponentPath;
                ChildMenu.KeepAlive = addOrUpdateMenuDto.KeepAlive;
                ChildMenu.ParentMenu = parentMenu;
                _menuRepository.UpdateChildMenu(ChildMenu);
            }
            await _menuRepository.SaveAsync();
            return Ok(new ApiResult());
        }
    }
}