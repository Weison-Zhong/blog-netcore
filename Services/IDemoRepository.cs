using Blog2022_netcore.Entities;

namespace Blog2022_netcore.Services
{
    public interface IDemoRepository
    {
        void AddDemo(Demo demo);
        Task<Demo> GetDemo(Guid demoId);
        Task<IEnumerable<Demo>> GetDemos();
        void DeleteDemo(Demo demo);
        void UpdateDemo(Demo demo);
        Task<bool> SaveAsync();
        Task<bool> IsDemoExist(string KeyWord);
        void UpdateIconForDemo(Guid DemoId, List<Guid> iconIds);
    }
}
