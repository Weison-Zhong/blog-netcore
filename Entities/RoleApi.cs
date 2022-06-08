namespace Blog2022_netcore.Entities
{
    //角色和api多对多中间表
    public class RoleApi
    {
        public Guid RoleId { get; set; }
        public Guid ApiId { get; set; }
        public Role Role { get; set; }
        public Api Api { get; set; }
    }
}
