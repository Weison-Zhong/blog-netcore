namespace Blog2022_netcore.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public List<RoleApi> Apis { get; set; } //角色可访问的Api列表命名为权限Permission，为了和菜单子级Api列表区分
    }
}
