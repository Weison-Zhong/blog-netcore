namespace Blog2022_netcore.Model
{
    public class AdministratorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public RoleDtoForAdministrator Role { get; set; }
        public string AvatarUrl { get; set; }
    }

    public class RoleDtoForAdministrator
    {
        public string RoleName { get; set; }
        public Guid RoleId { get; set; }
    }
}
