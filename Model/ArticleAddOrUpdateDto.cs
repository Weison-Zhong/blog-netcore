namespace Blog2022_netcore.Model
{
    public class ArticleAddOrUpdateDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        //public int Weight { get; set; }
        public string ArticleType { get; set; }
        public byte Status { get; set; }
        public string? RelatedLink { get; set; }
        public IFormFile? CoverImage { get; set; }
    }
}
