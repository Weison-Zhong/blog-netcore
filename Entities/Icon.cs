namespace Blog2022_netcore.Entities
{
    public class Icon
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string CreatedDate { get; set; }
        public List<DemoIcon> Demos { get; set; }
        public List<ChildMenu> ChildMenus { get; set; }
        public List<ParentMenu> ParentMenus { get; set; }
    }
}
