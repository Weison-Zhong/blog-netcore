using Blog2022_netcore.Entities;

namespace Blog2022_netcore.Model
{
    public class ApiDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Key { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string Description { get; set; }
        public BelongMenu BelongMenu { get; set; } 
    }
    public class BelongMenu
    {
        public Guid MenuId { get; set; }
        public string MenuName { get; set; }
    }
}

