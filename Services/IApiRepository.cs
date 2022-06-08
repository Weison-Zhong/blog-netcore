using Blog2022_netcore.Entities;

namespace Blog2022_netcore.Services
{
    public interface IApiRepository
    {
        void AddApi(Api api);
        Task<IEnumerable<Api>> GetApis();
        Task<Api> GetApi(Guid apiId);
        Task<bool> SaveAsync();
        void DeleteApi(Api api);
        void UpdateApi(Api api);
        Task<bool> IsApiExist(string KeyWord);
    }
}
