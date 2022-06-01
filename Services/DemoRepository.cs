using Blog2022_netcore.Data;
using Blog2022_netcore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog2022_netcore.Services
{
    public class DemoRepository : IDemoRepository
    {
        private readonly RoutineDbContext _context;

        public DemoRepository(RoutineDbContext routineDbContext)
        {
            _context = routineDbContext ?? throw new ArgumentNullException(nameof(routineDbContext));
        }
        public void AddDemo(Demo demo)
        {
            if (demo == null)
            {
                throw new ArgumentNullException(nameof(demo));
            }
            _context.Demo.Add(demo);
        }

        public void DeleteDemo(Demo demo)
        {
            if (demo == null)
            {
                throw new ArgumentNullException(nameof(demo));
            }
            _context.Demo.Remove(demo);
        }

        public async Task<Demo> GetDemo(Guid demoId)
        {
            return await _context.Demo
                .FirstOrDefaultAsync(x => x.Id == demoId);
        }

        public async Task<IEnumerable<Demo>> GetDemos()
        {
            return await _context.Demo
                .Include(x => x.Icons).ThenInclude(x=>x.Icon)
                .ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public void UpdateDemo(Demo demo)
        {

        }

        async Task<bool> IDemoRepository.IsDemoExist(string KeyWord)
        {
            var demo = await _context.Demo.FirstOrDefaultAsync(x => x.Title == KeyWord);
            return demo != null;
        }

        void IDemoRepository.UpdateIconForDemo(Guid demoId, List<Guid> iconIds)
        {

            if (demoId == null)
            {
                throw new ArgumentNullException(nameof(demoId));
            }
            if (iconIds == null)
            {
                throw new ArgumentNullException(nameof(iconIds));
            }
            //1，先删除该角色所有接口访问权限
            var oldIcons = _context.DemoIcon.Where(x => x.DemoId == demoId);
            _context.DemoIcon.RemoveRange(oldIcons);
            //2，重新赋予该角色新的接口访问权限
            foreach (Guid iconId in iconIds)
            {
                var DemoIcon = new DemoIcon
                {
                    DemoId = demoId,
                    IconId = iconId
                };
                _context.Add(DemoIcon);
            }
            _context.SaveChanges();
        }
    }
}
