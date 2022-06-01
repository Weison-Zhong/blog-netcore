using Blog2022_netcore.Data;
using Blog2022_netcore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog2022_netcore.Services
{
    public class IconRepository : IIconRepository
    {
        private readonly RoutineDbContext _context;

        public IconRepository(RoutineDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public void AddIcon(Icon icon)
        {
            if (icon == null)
            {
                throw new ArgumentNullException(nameof(icon));
            }
            icon.Id = Guid.NewGuid();
            _context.Icon.Add(icon);
        }

        public void DeleteIcon(Icon icon)
        {
            if (icon == null)
            {
                throw new ArgumentNullException(nameof(icon));
            }
            _context.Icon.Remove(icon);
        }

        public async Task<IEnumerable<Icon>> GetIcons()
        {
            return await _context.Icon.ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public void UpdateIcon(Icon icon)
        {

        }

        async Task<Icon> IIconRepository.GetIconById(Guid id)
        {
            return await _context.Icon.FirstOrDefaultAsync(x => x.Id == id);
        }

        async Task<bool> IIconRepository.IsIconExist(string KeyWord)
        {
            var icon = await _context.Icon.FirstOrDefaultAsync(x => x.Name == KeyWord || x.Key == KeyWord);
            return icon != null;
        }
    }
}
