using Blog2022_netcore.Data;
using Blog2022_netcore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog2022_netcore.Services
{
    public class MenuRepository : IMenuRepository
    {
        private readonly RoutineDbContext _context;

        public MenuRepository(RoutineDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public void AddChildMenu(ChildMenu childMenu)
        {

            if (childMenu == null)
            {
                throw new ArgumentNullException(nameof(childMenu));
            }
            _context.ChildMenu.Add(childMenu);
        }

        public void AddParentMenu(ParentMenu parentMenu)
        {
            if (parentMenu == null)
            {
                throw new ArgumentNullException(nameof(parentMenu));
            }
            _context.ParentMenu.Add(parentMenu);
        }

        public void DeleteChildMenu(ChildMenu childMenu)
        {
            if (childMenu == null)
            {
                throw new ArgumentNullException(nameof(childMenu));
            }
            _context.ChildMenu.Remove(childMenu);
        }

        public void DeleteParentMenu(ParentMenu parentMenu)
        {
            if (parentMenu == null)
            {
                throw new ArgumentNullException(nameof(parentMenu));
            }
            _context.ParentMenu.Remove(parentMenu);
        }

     
        public async Task<List<ChildMenu>> GetChildMenus()
        {
            return await _context.ChildMenu.Include(x=>x.Apis).Include(x => x.Icon)
                .ToListAsync();
        }

     

        public async Task<List<ParentMenu>> GetParentMenus()
        {
            return await _context.ParentMenu.Include(x => x.Icon)
                .Include(x => x.Children).ThenInclude(x => x.Icon)
                .Include(x=>x.Children).ThenInclude(x=>x.Apis)
                .ToListAsync();
        }

        public async Task<bool> IsMenuExist(string keyWord)
        {
            ChildMenu childMenu = await _context.ChildMenu.FirstOrDefaultAsync(x => x.Name == keyWord || x.Key == keyWord);
            ParentMenu parentMenu = await _context.ParentMenu.FirstOrDefaultAsync(x => x.Name == keyWord || x.Key == keyWord);
            return (childMenu != null || parentMenu != null);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public void UpdateChildMenu(ChildMenu childMenu)
        {
          
        }

        public void UpdateParentMenu(ParentMenu parentMenu)
        {
           
        }

        async Task<ChildMenu> IMenuRepository.GetChildMenu(Guid childMenuId)
        {
            return await _context.ChildMenu.Include(x => x.Icon).Include(x => x.Apis).FirstOrDefaultAsync(x => x.Id == childMenuId);
        }

      async   Task<ParentMenu> IMenuRepository.GetParentMenu(Guid parentMenuId)
        {
            return await _context.ParentMenu.Include(x => x.Icon)
                .Include(x => x.Children).ThenInclude(x => x.Icon)
                .Include(x => x.Children).ThenInclude(x => x.Apis)
                .FirstOrDefaultAsync(x => x.Id == parentMenuId);
        }
    }
}
