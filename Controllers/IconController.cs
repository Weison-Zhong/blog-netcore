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
    public class IconController : ControllerBase
    {
        private readonly IIconRepository _iconRepository;

        public IconController(IIconRepository iconRepository)
        {
            _iconRepository = iconRepository ?? throw new ArgumentNullException(nameof(iconRepository));
        }
        [HttpPost]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> AddIcon(AddOrUpdateIconDto addOrUpdateIconDto)
        {
            if (addOrUpdateIconDto == null)
            {
                throw new ArgumentNullException(nameof(addOrUpdateIconDto));
            }
            if (await _iconRepository.IsIconExist(addOrUpdateIconDto.Name))
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该Icon名称已存在，请更换Icon名重试"
                });
            }
            if (await _iconRepository.IsIconExist(addOrUpdateIconDto.Key))
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该Icon的Key已存在，请更换Icon的Key值重试"
                });
            }
            var entity = new Icon
            {
                Id = Guid.NewGuid(),
                Name = addOrUpdateIconDto.Name,
                Key = addOrUpdateIconDto.Key,
                CreatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            };
            _iconRepository.AddIcon(entity);
            await _iconRepository.SaveAsync();
            return Ok(new ApiResult());
        }

        [HttpDelete("{IconId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> DeleteIcon(Guid IconId)
        {
            var Icon = await _iconRepository.GetIconById(IconId);
            if (Icon == null)
            {
                return NotFound();
            }
            _iconRepository.DeleteIcon(Icon);
            await _iconRepository.SaveAsync();
            return Ok(new ApiResult());
        }
        [HttpPut("{IconId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> UpdateIcon(Guid IconId, AddOrUpdateIconDto addOrUpdateIconDto)
        {
            var Icon = await _iconRepository.GetIconById(IconId);
            if (Icon == null)
            {
                return NotFound();
            }
            string newName = addOrUpdateIconDto.Name;
            if (await _iconRepository.IsIconExist(addOrUpdateIconDto.Name) && Icon.Name != newName)
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该Icon名称已存在，请更换Icon名重试"
                });
            }
            string newKey = addOrUpdateIconDto.Key;
            if (await _iconRepository.IsIconExist(addOrUpdateIconDto.Key) && Icon.Key != newKey)
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该Icon的Key已存在，请更换Icon的Key值重试"
                });
            }
            Icon.Name = addOrUpdateIconDto.Name;
            Icon.Key = addOrUpdateIconDto.Key;
            _iconRepository.UpdateIcon(Icon);
            await _iconRepository.SaveAsync();
            return Ok(new ApiResult());
        }

        [HttpGet]
        public async Task<IActionResult> GetIcons()
        {
            var icons = await _iconRepository.GetIcons();
            var iconsDto = new List<IconDto>();
            if (icons != null)
            {
                foreach (var icon in icons)
                {
                    var IconDto = new IconDto
                    {
                        Name = icon.Name,
                        Key = icon.Key,
                        Id = icon.Id,
                        CreatedDate=icon.CreatedDate
                    };
                    iconsDto.Add(IconDto);
                }
            }
            return Ok(new ApiResult
            {
                Data = iconsDto
            });
        }
    }
}
