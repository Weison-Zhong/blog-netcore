namespace Blog2022_netcore.Entities
{
    public class ParentMenu
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public Icon Icon { get; set; }
        public int Weight { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public List<ChildMenu> Children { get; set; }
    }
}
