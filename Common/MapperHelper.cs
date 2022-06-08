using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;

namespace Blog2022_netcore.Common
{
    public class MapperHelper
    {
        public static List<MenuDto> MenuDtoMapper(List<ParentMenu> menus)
        {
            var returnDto = new List<MenuDto>();
            if (menus != null)
            {
                foreach (var parentMenu in menus)
                {
                    var children = new List<ChildMenuDto>();
                    if (parentMenu.Children != null)
                    {
                        foreach (var child in parentMenu.Children)
                        {
                            //处理二级菜单下的API
                            var apis = new List<ApiDto>();
                            if (child.Apis != null)
                            {
                                foreach (var api in child.Apis)
                                {
                                    ApiDto apiDto = new ApiDto
                                    {
                                        Id = api.Id,
                                        Title = api.Title,
                                        Key = api.Key,
                                        CreatedDate = api.CreatedDate,
                                        UpdatedDate = api.UpdatedDate,
                                        Description = api.Description
                                    };
                                    apis.Add(apiDto);
                                }
                            }
                            IconDto icon = new IconDto();
                            if (child.Icon != null)
                            {
                                icon.Id = child.Icon.Id;
                                icon.Name = child.Icon.Name;
                                icon.Key = child.Icon.Key;
                                icon.CreatedDate = child.Icon.CreatedDate;
                            };
                            ChildMenuDto childMenu = new ChildMenuDto
                            {
                                Name = child.Name,
                                KeepAlive = child.KeepAlive,
                                Key = child.Key,
                                Icon = icon,
                                CreatedDate = child.CreatedDate,
                                UpdatedDate = child.UpdatedDate,
                                ComponentPath = child.ComponentPath,
                                Id = child.Id,
                                Weight = child.Weight,
                                ParentMenuId = parentMenu.Id,
                                Apis = apis
                            };
                            children.Add(childMenu);
                        }
                    }
                    children.Sort((a, b) => b.Weight - a.Weight);
                    IconDto icon2 = new IconDto();
                    if (parentMenu.Icon != null)
                    {
                        icon2.Id = parentMenu.Icon.Id;
                        icon2.Name = parentMenu.Icon.Name;
                        icon2.Key = parentMenu.Icon.Key;
                        icon2.CreatedDate = parentMenu.Icon.CreatedDate;
                    };
                    MenuDto menu = new MenuDto
                    {
                        Id = parentMenu.Id,
                        Key = parentMenu.Key,
                        Name = parentMenu.Name,
                        Weight = parentMenu.Weight,
                        Icon = icon2,
                        CreatedDate = parentMenu.CreatedDate,
                        UpdatedDate = parentMenu.UpdatedDate,
                        Children = children,
                    };
                    returnDto.Add(menu);
                }
            }
            returnDto.Sort((a, b) => b.Weight - a.Weight);
            return returnDto;
        }

        public static List<MenuDto> RoleMenuMapper(List<ChildMenu> childMenus)
        {
            var roleMenusDto = new List<MenuDto>();
            if (childMenus != null)
            {
                var roleParentMenus = new List<ParentMenu>();//该角色权限对应的二级菜单下的所有父级菜单（仅一级菜单）
                foreach (ChildMenu childMenu in childMenus)
                {
                    ParentMenu parentMenu = childMenu.ParentMenu;
                    if (!roleParentMenus.Contains(parentMenu))
                    {
                        roleParentMenus.Add(parentMenu);
                    }
                }
                roleMenusDto = MenuDtoMapper(roleParentMenus);//处理遍历该角色一级菜单下所有二级菜单，后续还需过滤掉没有权限的二级菜单
                foreach (var parent in roleMenusDto)
                {
                    foreach (var child in parent.Children)
                    {
                        if (childMenus.Where(x => x.Key == child.Key) == null)
                        {
                            parent.Children.Remove(child);
                        }
                    }
                }
            }
            return roleMenusDto;
        }
    }
}
