using Blog2022_netcore.Entities;

namespace Blog2022_netcore.Services
{
    public interface IMenuRepository
    {
        void AddParentMenu(ParentMenu parentMenu);
        void DeleteParentMenu(ParentMenu parentMenu);
        void UpdateParentMenu(ParentMenu parentMenu);
        Task<ParentMenu> GetParentMenu(Guid parentMenuId);
        Task<List<ParentMenu>> GetParentMenus();
        void AddChildMenu(ChildMenu childMenu);
        void DeleteChildMenu(ChildMenu childMenu);
        void UpdateChildMenu(ChildMenu childMenu);
        Task<ChildMenu> GetChildMenu(Guid childMenuId);
        Task<bool> IsMenuExist(string keyWord);
        Task<List<ChildMenu>> GetChildMenus();
        Task<bool> SaveAsync();
    }
}
