namespace Blog2022_netcore.Model
{
    //返回给前端的DTO
    public class MenuDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public int Weight { get; set; }
        public IconDto Icon { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public List<ChildMenuDto> Children { get; set; }
    }
}
