using Blog2022_netcore.Entities;

namespace Blog2022_netcore.Model
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public List<ApiDto> Permissions { get; set; }
        public List<MenuDto> Menus { get; set; }
    }
}
