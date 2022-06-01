using Blog2022_netcore.Common;
using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;
using Blog2022_netcore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog2022_netcore.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly IDemoRepository _demoRepository;
        private readonly FileUploadHelper _fileUploadHelper;

        public DemoController(IDemoRepository demoRepository, IHostEnvironment hostEnvironment)
        {
            _demoRepository = demoRepository ?? throw new ArgumentNullException(nameof(demoRepository));
            _fileUploadHelper = new FileUploadHelper(hostEnvironment);
        }
        [HttpGet]
        public async Task<IActionResult> GetDemos()
        {
            var demos = await _demoRepository.GetDemos();
            var demosDto = new List<DemoDto>();
            if (demos != null)
            {
                foreach (var demo in demos)
                {
                    var icons = new List<IconDto>();
                    if (demo.Icons != null)
                    {
                        foreach (var icon in demo.Icons)
                        {
                            var IconDto = new IconDto
                            {
                                Name = icon.Icon.Name,
                                Key = icon.Icon.Key,
                                Id = icon.Icon.Id
                            };
                            icons.Add(IconDto);
                        }
                    }
                    DemoDto demoDto = new DemoDto
                    {
                        Id = demo.Id,
                        Title = demo.Title,
                        Description = demo.Description,
                        CreatedDate = demo.CreatedDate,
                        UpdatedDate = demo.UpdatedDate,
                        CoverImgUrl = demo.CoverImgUrl,
                        Weight = demo.Weight,
                        Status = demo.Status,
                        RelatedLink = demo.RelatedLink,
                        Icons = icons
                    };
                    demosDto.Add(demoDto);
                }
            }
            return Ok(new ApiResult
            {
                Data = demosDto
            });
        }

        [HttpPost]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> AddDemo([FromForm] AddOrUpdateDemoDto addOrUpdateDemoDto)
        {
            if (addOrUpdateDemoDto == null)
            {
                throw new ArgumentNullException(nameof(addOrUpdateDemoDto));
            }
            if (await _demoRepository.IsDemoExist(addOrUpdateDemoDto.Title))
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该Demo标题已存在，请更换标题重试"
                });
            }
            string imgUrl = "";
            if (addOrUpdateDemoDto.CoverImage != null)
            {
                imgUrl = _fileUploadHelper.ImageUpload(addOrUpdateDemoDto.CoverImage);
            }
            Guid demoId = Guid.NewGuid();
            var entity = new Demo
            {
                Id = demoId,
                Title = addOrUpdateDemoDto.Title,
                Description = addOrUpdateDemoDto.Description,
                Weight = addOrUpdateDemoDto.Weight,
                Status = addOrUpdateDemoDto.Status,
                RelatedLink = addOrUpdateDemoDto.RelatedLink,
                CoverImgUrl = imgUrl,
                CreatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                UpdatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            };
            _demoRepository.AddDemo(entity);
            await _demoRepository.SaveAsync();
            if (addOrUpdateDemoDto.IconIds != null)
            {
                _demoRepository.UpdateIconForDemo(demoId, addOrUpdateDemoDto.IconIds);
            }
            await _demoRepository.SaveAsync();
            return Ok(new ApiResult());
        }

        [HttpPut("{DemoId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> UpdateDemo(Guid DemoId, [FromForm] AddOrUpdateDemoDto addOrUpdateDemoDto)
        {
            if (addOrUpdateDemoDto == null)
            {
                throw new ArgumentNullException(nameof(addOrUpdateDemoDto));
            }
            var demo = await _demoRepository.GetDemo(DemoId);
            if (demo == null)
            {
                return NotFound();
            }
            if (await _demoRepository.IsDemoExist(addOrUpdateDemoDto.Title) && demo.Title != addOrUpdateDemoDto.Title)
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该Demo标题已存在，请更换标题重试"
                });
            }
            if (addOrUpdateDemoDto.CoverImage != null)
            {
                string imgUrl = _fileUploadHelper.ImageUpload(addOrUpdateDemoDto.CoverImage);
                demo.CoverImgUrl = imgUrl;
            }
            demo.Title = addOrUpdateDemoDto.Title;
            demo.Description = addOrUpdateDemoDto.Description;
            demo.Weight = addOrUpdateDemoDto.Weight;
            demo.Status = addOrUpdateDemoDto.Status;
            demo.RelatedLink = addOrUpdateDemoDto.RelatedLink;
            demo.UpdatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            if (addOrUpdateDemoDto.IconIds != null)
            {
                _demoRepository.UpdateIconForDemo(DemoId, addOrUpdateDemoDto.IconIds);
            }
            _demoRepository.UpdateDemo(demo);
            await _demoRepository.SaveAsync();
            return Ok(new ApiResult());
        }

        [HttpDelete("{DemoId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> DeleteDemo(Guid DemoId)
        {
            var demo = await _demoRepository.GetDemo(DemoId);
            if (demo == null)
            {
                return NotFound();
            }
            _demoRepository.DeleteDemo(demo);
            await _demoRepository.SaveAsync();
            return Ok(new ApiResult());
        }

        [HttpPut("{demoId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> ToggleDemoStatus(Guid demoId)
        {

            var demo = await _demoRepository.GetDemo(demoId);
            if (demo == null)
            {
                return NotFound();
            }
            demo.Status = (byte)(demo.Status == 1 ? 0 : 1);
            demo.UpdatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            _demoRepository.UpdateDemo(demo);
            await _demoRepository.SaveAsync();
            return Ok(new ApiResult());
        }
    }
}
