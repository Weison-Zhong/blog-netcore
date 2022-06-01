namespace Blog2022_netcore.Model
{
    public class ArticleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string? CoverImgUrl { get; set; }
        public int Weight { get; set; }
        public string ArticleType { get; set; }
        public int ReadCount { get; set; }
        public byte Status { get; set; }
        public string? RelatedLink { get; set; }
    }
}
