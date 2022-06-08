using Blog2022_netcore.Common;
using Blog2022_netcore.Data;
using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blog2022_netcore.Services
{
    public class AdministratorRepository : IAdministratorRepository
    {
        private readonly RoutineDbContext _context;
        public AdministratorRepository(RoutineDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
 
        }
        void IAdministratorRepository.AddAdministrator(Administrator administrator)
        {
            if (administrator == null)
            {
                throw new ArgumentNullException(nameof(administrator));
            }
            administrator.Id = Guid.NewGuid();
            _context.Administrators.Add(administrator);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        async Task<IEnumerable<Administrator>> IAdministratorRepository.GetAdministrators()
        {
            return await _context.Administrators.Include(x => x.Role).ToListAsync();
        }

        void IAdministratorRepository.DeleteAdministrator(Administrator administrator)
        {
            if (administrator == null)
            {
                throw new ArgumentNullException(nameof(administrator));
            }
            _context.Administrators.Remove(administrator);
        }

        async Task<Administrator> IAdministratorRepository.GetAdministrator(Guid administratorId)
        {
            return await _context.Administrators.Include(x=>x.Role).FirstOrDefaultAsync(x => x.Id == administratorId);
        }

        async Task<bool> IAdministratorRepository.IsAdministratorExist(string AdministratorName)
        {
            var administrator = await _context.Administrators.FirstOrDefaultAsync(x => x.Name == AdministratorName);
            return administrator != null;
        }
}
}
