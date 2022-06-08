using Blog2022_netcore.Common;
using Blog2022_netcore.Data;
using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog2022_netcore.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class BlogConfigController : ControllerBase
    {
        private readonly RoutineDbContext _context;
        private readonly FileUploadHelper _fileUploadHelper;
        public BlogConfigController(RoutineDbContext routineDbContext, IHostEnvironment hostEnvironment)
        {
            _context = routineDbContext ?? throw new ArgumentNullException(nameof(routineDbContext));
            _fileUploadHelper = new FileUploadHelper(hostEnvironment);
        }
        [HttpGet]
        public async Task<ActionResult> GetBlogConfig()
        {
            var blogConfig = await _context.BlogConfig.FirstOrDefaultAsync(x => !String.IsNullOrEmpty(x.FirstText));
            return Ok(new ApiResult
            {
                Data = blogConfig,
            });
        }
        [HttpPut]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> UpdateBlogConfig([FromForm]UpdateBlogConfigDto newBlogConfig)
        {
            var oldBlogConfig = await _context.BlogConfig.FirstOrDefaultAsync(x => !String.IsNullOrEmpty(x.FirstText));

            if (oldBlogConfig == null)
            {
                //新增
                string resumeUrl = "";
                if (newBlogConfig.ResumeFile != null)
                {
                    resumeUrl = _fileUploadHelper.SingleFileUpload(newBlogConfig.ResumeFile);
                }
                BlogConfig config = new BlogConfig
                {
                    FirstText = newBlogConfig.FirstText,
                    SecondText = newBlogConfig.SecondText,
                    ThirdText = newBlogConfig.ThirdText,
                    IconLink = newBlogConfig.IconLink,
                    ResumeUrl = resumeUrl
                };
                _context.BlogConfig.Add(config);
                bool isSuccess = await _context.SaveChangesAsync() >= 0;
                if (isSuccess)
                {
                    return Ok(new ApiResult());
                }
                else return Ok(new ApiResult
                {
                    Code = 400,
                    Msg = "新增失败"
                });
            }
            else
            {
                if (newBlogConfig.ResumeFile != null)
                {
                    oldBlogConfig.ResumeUrl = _fileUploadHelper.SingleFileUpload(newBlogConfig.ResumeFile);
                }
                oldBlogConfig.FirstText = newBlogConfig.FirstText;
                oldBlogConfig.SecondText = newBlogConfig.SecondText;
                oldBlogConfig.ThirdText = newBlogConfig.ThirdText;
                oldBlogConfig.IconLink = newBlogConfig.IconLink;
                bool isSuccess = await _context.SaveChangesAsync() >= 0;
                if (isSuccess)
                {
                    return Ok(new ApiResult());
                }
                else return Ok(new ApiResult
                {
                    Code = 400,
                    Msg = "修改失败"
                });
            }

        }

        [HttpDelete]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> DeleteResume()
        {
            var blogConfig = await _context.BlogConfig.FirstOrDefaultAsync(x => !String.IsNullOrEmpty(x.FirstText));
            blogConfig.ResumeUrl = "";
            bool isSuccess = await _context.SaveChangesAsync() >= 0;
            if (isSuccess)
            {
                return Ok(new ApiResult());
            }
            else return Ok(new ApiResult
            {
                Code = 400,
                Msg = "删除简历失败"
            });
        }
    }

}
