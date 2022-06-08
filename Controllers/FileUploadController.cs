using Blog2022_netcore.Model;
using Microsoft.AspNetCore.Mvc;
using Blog2022_netcore.Common;
using Microsoft.AspNetCore.Authorization;

namespace Blog2022_netcore.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment;

        public FileUploadController(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }
        [HttpPost]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> FileUpload(IFormFile formFile)
        {
            var uploadHelper = new FileUploadHelper(_hostEnvironment);
            string url = uploadHelper.SingleFileUpload(formFile);
            var result = new ApiResult
            {
                Data = url
            };
            return Ok(result);
        }
    }
}