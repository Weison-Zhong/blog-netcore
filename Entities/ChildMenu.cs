namespace Blog2022_netcore.Entities
{
    public class ChildMenu
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string? ComponentPath { get; set; }
        public Icon Icon { get; set; }
        public byte KeepAlive { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public int Weight { get; set; }
        public ParentMenu ParentMenu { get; set; }
        public Guid ParentMenuId { get; set; }
        public List<Api> Apis { get; set; }
    }
}
