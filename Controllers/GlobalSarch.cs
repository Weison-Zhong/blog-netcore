using Blog2022_netcore.Data;
using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog2022_netcore.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class GlobalSarch : ControllerBase
    {
        private readonly RoutineDbContext _context;

        public GlobalSarch(RoutineDbContext routineDbContext)
        {
            _context = routineDbContext ?? throw new ArgumentNullException(nameof(routineDbContext));
        }
        [HttpGet]
        public async Task<IActionResult> GlobalSearch(string searchTerm)
        {
            var returnData = new GlobalSearchDto();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                //关键字筛选
                var matchArticles = await _context.Article.Where(x => x.Title.Contains(searchTerm) || x.Content.Contains(searchTerm)).ToListAsync();
                var matchDemos = await _context.Demo.Where(x => x.Title.Contains(searchTerm) || x.Description.Contains(searchTerm)).ToListAsync();
                returnData.Articles = matchArticles;
                returnData.Demos = matchDemos;
            }
            return Ok(new ApiResult
            {
                Data = returnData
            });
        }
    }

    public class GlobalSearchDto
    {
        public List<Article> Articles { get; set; }
        public List<Demo> Demos { get; set; }
    }
}
