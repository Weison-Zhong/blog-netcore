using Blog2022_netcore.Data;
using Blog2022_netcore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog2022_netcore.Services
{
    public class ApiRepository : IApiRepository
    {
        private readonly RoutineDbContext _context;

        public ApiRepository(RoutineDbContext routineDbContext)
        {
            _context = routineDbContext ?? throw new ArgumentNullException(nameof(routineDbContext));
        }
        public void AddApi(Api Api)
        {
            if (Api == null)
            {
                throw new ArgumentNullException(nameof(Api));
            }
            _context.Api.Add(Api);
        }

        public async Task<IEnumerable<Api>> GetApis()
        {
            return await _context.Api.Include(x=>x.Menu).ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        void IApiRepository.DeleteApi(Api Api)
        {
            if (Api == null)
            {
                throw new ArgumentNullException(nameof(Api));
            }
            _context.Api.Remove(Api);
        }

        async Task<Api> IApiRepository.GetApi(Guid ApiId)
        {
            return await _context.Api.Include(x => x.Menu).FirstOrDefaultAsync(x => x.Id == ApiId);
        }

        async Task<bool> IApiRepository.IsApiExist(string KeyWord)
        {
            var api = await _context.Api.FirstOrDefaultAsync(x => x.Title == KeyWord || x.Key == KeyWord);
            return api != null;
        }

        void IApiRepository.UpdateApi(Api Api)
        {

        }
    }
}
