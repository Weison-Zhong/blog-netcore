using Blog2022_netcore.Common;
using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;
using Blog2022_netcore.Model.DtoParameters;
using Blog2022_netcore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog2022_netcore.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly FileUploadHelper _fileUploadHelper;

        public ArticleController(IArticleRepository articleRepository, IHostEnvironment hostEnvironment)
        {
            _articleRepository = articleRepository ?? throw new ArgumentNullException(nameof(articleRepository));
            _fileUploadHelper = new FileUploadHelper(hostEnvironment);
        }

        [HttpGet("{articleId}")]
        public async Task<IActionResult> GetArticle(Guid articleId, [FromQuery] string? origin)
        {
            var article = await _articleRepository.GetArticle(articleId);
            if (origin != "admin")
            {
                article.ReadCount += 1;
            }
            _articleRepository.UpdateArticle(article);
            await _articleRepository.SaveAsync();
            ApiResult apiResult = new ApiResult
            {
                Data = article
            };
            return Ok(apiResult);
        }

        [HttpGet]
        public async Task<IActionResult> GetArticles([FromQuery] ArticleQueryParameters parameters)
        {
            var articlesResult = await _articleRepository.GetArticles(parameters);
            var articlesList = new List<ArticleDto>();
            if (articlesResult.Articles != null)
            {
                foreach (var item in articlesResult.Articles)
                {
                    var articleDto = new ArticleDto
                    {
                        Id = item.Id,
                        Title = item.Title,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        CoverImgUrl = item.CoverImgUrl,
                        Weight = item.Weight,
                        ArticleType = item.ArticleType,
                        ReadCount = item.ReadCount,
                        Status = item.Status,
                        RelatedLink = item.RelatedLink
                    };
                    articlesList.Add(articleDto);
                }
            }
            ArticlesDto articlesDto = new ArticlesDto
            {
                Articles = articlesList,
                TotalCount = articlesResult.TotalCount
            };
            return Ok(new ApiResult
            {
                Data = articlesDto
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetArticleTypes()
        {
            var types = await _articleRepository.GetArticleTypes();
            ApiResult res = new ApiResult
            {
                Data = types
            };
            return Ok(res);
        }
        [HttpPost]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> AddArticle([FromForm] ArticleAddOrUpdateDto articleAddDto)
        {
            if (articleAddDto == null)
            {
                throw new ArgumentNullException(nameof(articleAddDto));
            }
            if (await _articleRepository.IsArticleExist(articleAddDto.Title))
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该标题已存在，请更换标题重试"
                });
            }
            string imgUrl = "";
            if (articleAddDto.CoverImage != null)
            {
                imgUrl = _fileUploadHelper.ImageUpload(articleAddDto.CoverImage);
            }
            var entity = new Article
            {
                Title = articleAddDto.Title,
                Content = articleAddDto.Content,
                //Weight = articleAddDto.Weight,
                ArticleType = articleAddDto.ArticleType,
                Status = articleAddDto.Status,
                RelatedLink = articleAddDto.RelatedLink,
                CoverImgUrl = imgUrl
            };
            _articleRepository.AddArticle(entity);
            await _articleRepository.SaveAsync();
            return Ok(new ApiResult());
        }
        [HttpPut("{articleId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> UpdateArticle(Guid articleId, [FromForm] ArticleAddOrUpdateDto articleUpdateDto)
        {
            if (articleUpdateDto == null)
            {
                throw new ArgumentNullException(nameof(articleUpdateDto));
            }
            var article = await _articleRepository.GetArticle(articleId);
            if (article == null)
            {
                return NotFound();
            }
            if (await _articleRepository.IsArticleExist(articleUpdateDto.Title) && articleUpdateDto.Title != article.Title)
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该标题已存在，请更换标题重试"
                });
            }
            if (articleUpdateDto.CoverImage != null)
            {
                string imgUrl = _fileUploadHelper.ImageUpload(articleUpdateDto.CoverImage);
                article.CoverImgUrl = imgUrl;
            }

            article.Title = articleUpdateDto.Title;
            article.Content = articleUpdateDto.Content;
            article.ArticleType = articleUpdateDto.ArticleType;
            article.Status = articleUpdateDto.Status;
            article.RelatedLink = articleUpdateDto.RelatedLink;
            article.UpdatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            _articleRepository.UpdateArticle(article);
            await _articleRepository.SaveAsync();
            return Ok(new ApiResult());
        }
        [HttpPut("{articleId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> ToggleArticleStatus(Guid articleId)
        {

            var article = await _articleRepository.GetArticle(articleId);
            if (article == null)
            {
                return NotFound();
            }
            article.Status = (byte)(article.Status == 1 ? 0 : 1);
            article.UpdatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            _articleRepository.UpdateArticle(article);
            await _articleRepository.SaveAsync();
            return Ok(new ApiResult());
        }
        [HttpPut("{articleId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> UpdateArticleWeight(Guid articleId, UpdateWeightDto updateArticleWeightDto)
        {
            if (updateArticleWeightDto.Diff == 0)
            {
                return Ok(new ApiResult());
            }
            var article = await _articleRepository.UpdateArticleWeight(articleId, updateArticleWeightDto.Diff);
            await _articleRepository.SaveAsync();
            return Ok(new ApiResult());
        }
        [HttpDelete("{articleId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> DeleteArticle(Guid articleId)
        {
            var article = await _articleRepository.GetArticle(articleId);
            if (article == null)
            {
                return NotFound();
            }
            _articleRepository.DeleteArticle(article);
            await _articleRepository.SaveAsync();
            return Ok(new ApiResult());
        }
    }
}
