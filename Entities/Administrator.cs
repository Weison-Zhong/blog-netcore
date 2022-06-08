namespace Blog2022_netcore.Entities
{
    public class Administrator
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string? Avatar { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}
