namespace Blog2022_netcore.Model
{
    public class ChildMenuDto
    {
        public Guid Id { get; set; }
        public Guid? ParentMenuId { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string ComponentPath { get; set; }
        public int Weight { get; set; }
        public IconDto Icon { get; set; }
        public byte KeepAlive { get; set; }
        public List<ApiDto> Apis { get; set; }
    }
}
