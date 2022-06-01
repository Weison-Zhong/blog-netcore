namespace Blog2022_netcore.Model
{
    public class ArticlesDto
    {
        public List<ArticleDto> Articles { get; set; }
        public int TotalCount { get; set; } = 0;
    }
}
