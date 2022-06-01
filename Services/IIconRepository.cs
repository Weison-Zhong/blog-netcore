using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;

namespace Blog2022_netcore.Services
{
    public interface IIconRepository
    {
        void AddIcon(Icon icon);
        void DeleteIcon(Icon icon);
        Task<IEnumerable<Icon>> GetIcons();
        Task<Icon> GetIconById(Guid id);
        Task<bool> IsIconExist(string KeyWord);
        Task<bool> SaveAsync();
        void UpdateIcon(Icon icon);
    }
}
