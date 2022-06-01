using Blog2022_netcore.Entities;
using Blog2022_netcore.Model.DtoParameters;

namespace Blog2022_netcore.Services
{
    public interface IArticleRepository
    {
        void AddArticle(Article article);
        Task<ArticlesResult> GetArticles(ArticleQueryParameters articleQueryParameters);
        Task<Article> GetArticle(Guid articleId);
        Task<IEnumerable<string>> GetArticleTypes();
        void DeleteArticle(Article article);
        void UpdateArticle(Article article);
        Task<Article> UpdateArticleWeight(Guid articleId,int diff);
        Task<bool> SaveAsync();
        Task<bool> IsArticleExist(string keyWord);
    }
}
