using Blog2022_netcore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog2022_netcore.Data
{
    public class RoutineDbContext : DbContext
    {
        public RoutineDbContext(DbContextOptions<RoutineDbContext> options) : base(options)
        {

        }

        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Api> Api { get; set; }
        public DbSet<RoleApi> RoleApi { get; set; }
        public DbSet<ParentMenu> ParentMenu { get; set; }
        public DbSet<ChildMenu> ChildMenu { get; set; }
        public DbSet<Article> Article { get; set; }
        public DbSet<Guest> Guest { get; set; }
        public DbSet<BlogConfig> BlogConfig { get; set; }
        public DbSet<Icon> Icon { get; set; }
        public DbSet<Demo> Demo { get; set; }
        public DbSet<DemoIcon> DemoIcon { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleApi>().HasKey(x => new { x.RoleId, x.ApiId });
            modelBuilder.Entity<DemoIcon>().HasKey(x => new { x.DemoId, x.IconId });
        }
    }
}
