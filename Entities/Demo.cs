namespace Blog2022_netcore.Entities
{
    public class Demo
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string? CoverImgUrl { get; set; }
        public int Weight { get; set; }
        public byte Status { get; set; }
        public string? RelatedLink { get; set; }
        public List<DemoIcon> Icons { get; set; }
    }
}
