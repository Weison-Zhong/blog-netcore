namespace Blog2022_netcore.Model.DtoParameters
{
    public class ArticleQueryParameters
    {
        private const int MaxPageSize = 100;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 5;
        //关键字
        public string? SearchTerm { get; set; }
        public string? ArticleType { get; set; }
        //是否在web端展示
        public byte? Status { get; set; }
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
