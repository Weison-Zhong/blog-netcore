using Blog2022_netcore.Data;
using Blog2022_netcore.Entities;
using Blog2022_netcore.Model.DtoParameters;
using Microsoft.EntityFrameworkCore;

namespace Blog2022_netcore.Services
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly RoutineDbContext _context;

        public ArticleRepository(RoutineDbContext routineDbContext)
        {
            _context = routineDbContext ?? throw new ArgumentNullException(nameof(routineDbContext));
        }
        public async void AddArticle(Article article)
        {
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }
            int maxWeight = 1;
            int len = _context.Article.Count();
            if (len > 0)
            {
                maxWeight = len + 1;
            }
            article.Id = Guid.NewGuid();
            article.CreatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            article.UpdatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            article.ReadCount = 0;
            article.Weight = maxWeight;
            _context.Article.Add(article);
        }

        public void DeleteArticle(Article article)
        {
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }
            _context.Article.Remove(article);
        }

        public async Task<ArticlesResult> GetArticles(ArticleQueryParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            var queryExpression = _context.Article as IQueryable<Article>;
            string type = "";
            int totleCount = await _context.Article.CountAsync();
            if (!string.IsNullOrWhiteSpace(parameters.ArticleType))
            {
                type = parameters.ArticleType.Trim();
            }
            if (!string.IsNullOrWhiteSpace(type))
            {
                //若筛选文章类型字段不为空，则往表达式中增加文章类型过滤
                queryExpression = queryExpression.Where(x => x.ArticleType == type);
                totleCount = await _context.Article.Where(x => x.ArticleType == type).CountAsync();
            }
            if (parameters.Status != null)
            {
                //筛选文章是否展示
                queryExpression = queryExpression.Where(x => x.Status == parameters.Status);
                totleCount = await _context.Article.Where(x => x.Status == parameters.Status).CountAsync();
            }
            if(!string.IsNullOrWhiteSpace(type) && parameters.Status != null)
            {
                totleCount = await _context.Article.Where(x => x.Status == parameters.Status && x.ArticleType == type).CountAsync();
            }
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                //关键字筛选
                string search = parameters.SearchTerm.Trim();
                queryExpression = queryExpression.Where(x => x.Title.Contains(search) || x.Content.Contains(search));
            }
            //fen页
            queryExpression = queryExpression.Skip(parameters.PageSize * (parameters.PageNumber - 1))// 跳过
                .Take(parameters.PageSize)//跳过后取多少
                .OrderByDescending(x => x.Weight); //按权重倒序
            var articles = await queryExpression.ToListAsync(); //这一行才是真正查询数据库


            return new ArticlesResult
            {
                Articles = articles,
                TotalCount = totleCount
            };
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public void UpdateArticle(Article article)
        {

        }

        async Task<Article> IArticleRepository.GetArticle(Guid articleId)
        {
            var article = await _context.Article.FirstOrDefaultAsync(x => x.Id == articleId);
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }
            return article;
        }

        async Task<IEnumerable<string>> IArticleRepository.GetArticleTypes()
        {
            return await _context.Article.Select(x => x.ArticleType).Distinct().ToListAsync();
        }

        async Task<bool> IArticleRepository.IsArticleExist(string keyWord)
        {
            Article article = await _context.Article.FirstOrDefaultAsync(x => x.Title == keyWord);
            return article != null;
        }

        async Task<Article> IArticleRepository.UpdateArticleWeight(Guid articleId, int diff)
        {
            Article matchArticle = await _context.Article.FirstOrDefaultAsync(x => x.Id == articleId);
            if (matchArticle == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
            //移动a那么对应直接改a的权重对应移动大小
            //a若向后移动，则需要把移动前位置和移动后所在位置中间区间(a,b]的都遍历改权重+1
            //a若向前移动，则需要改变[b,a)区间的遍历-1
            if (diff == 0)
            {
                return matchArticle;
            }
            int matchArticleWeight = matchArticle.Weight;
            matchArticle.Weight = matchArticleWeight + diff;
            if (diff > 0)
            {
                //向前移动diff位置，则要找到matchArticle前面diff个的文章，将其遍历-1 
                var beforeArticles = await _context.Article.Where(x => x.Weight > matchArticleWeight && x.Weight <= matchArticleWeight + diff).ToListAsync();
                foreach (Article article in beforeArticles)
                {
                    article.Weight = article.Weight - 1;
                }
            }
            else
            {
                //向后移动diff位置，则要找到matchArticle后面diff个的文章，将其遍历+1
                var afterArticles = await _context.Article.Where(x => x.Weight < matchArticleWeight && x.Weight >= matchArticleWeight + diff).ToListAsync();
                foreach (Article article in afterArticles)
                {
                    article.Weight = article.Weight + 1;
                }
            }
            await _context.SaveChangesAsync();
            return matchArticle;
        }
    }

    public class ArticlesResult
    {
        public List<Article> Articles { get; set; }
        public int TotalCount { get; set; }
    }
}

