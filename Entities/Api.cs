namespace Blog2022_netcore.Entities
{
    //Api即权限，角色可访问哪些api就有哪些权限
    public class Api
    {
        public Guid Id { get; set; }
        public string Title { get; set; } //这个是接口名,写成title为了跟antd的tree树形控件保持同名，这样前端不用再二次处理
        public string Key { get; set; } //这个是路径path,写成key为了跟antd的tree树形控件保持同名，这样前端不用再二次处理
        public string? Description { get; set; } //说明
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public ICollection<RoleApi> Roles { get; set; } //角色和可访问的api多对多
        //public Menu? Menu { get; set; } //一个api属于一个menu，这里足够体现menu和api的一对多关系
        //public Guid? MenuId { get; set; }

        public ChildMenu Menu { get; set; }
        //public Guid MenuId { get; set; }
    }
}
