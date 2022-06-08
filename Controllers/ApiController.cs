using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;
using Blog2022_netcore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Blog2022_netcore.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IApiRepository _apiRepository;
        private readonly IMenuRepository _menuRepository;

        public ApiController(IApiRepository apiRepository,IMenuRepository menuRepository)
        {
            _apiRepository = apiRepository ?? throw new ArgumentNullException(nameof(apiRepository));
            _menuRepository = menuRepository ?? throw new ArgumentNullException(nameof(menuRepository));
        }
        [HttpGet]
        public async Task<IActionResult> GetApis()
        {
            var Apis = await _apiRepository.GetApis();
            var returnDto = new List<ApiDto>();
            foreach (var item in Apis)
            {
                BelongMenu belongMenu = new BelongMenu();
                if (item.Menu != null)
                {

                    belongMenu.MenuId = item.Menu.Id;
                    belongMenu.MenuName = item.Menu.Name;
                }
                ApiDto ApiDto = new ApiDto
                {
                    Title = item.Title,
                    Id = item.Id,
                    Key = item.Key,
                    Description = item.Description,
                    CreatedDate = item.CreatedDate,
                    UpdatedDate = item.UpdatedDate,
                    BelongMenu = belongMenu
                };
                returnDto.Add(ApiDto);
            }
            return Ok(new ApiResult
            {
                Data = returnDto
            });

        }
        [HttpPost]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> AddApi(ApiAddOrUpdateDto ApiAddDto)
        {
            if (ApiAddDto == null)
            {
                throw new ArgumentNullException(nameof(ApiAddDto));
            }
            if (await _apiRepository.IsApiExist(ApiAddDto.Title))
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该Api名称已存在，请更换Api名重试"
                });
            }
            if (await _apiRepository.IsApiExist(ApiAddDto.Key))
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该Api路径已存在，请更换Api路径重试"
                });
            }
            Guid MenuId = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(ApiAddDto.MenuId))
            {
                MenuId = new Guid(ApiAddDto.MenuId);
            }
            var menu = await _menuRepository.GetChildMenu(MenuId);
            var entity = new Api
            {
                Id = Guid.NewGuid(),
                Title = ApiAddDto.Title,
                Description = ApiAddDto.Description,
                Key = ApiAddDto.Key,
                Menu = menu,
                CreatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                UpdatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            };
            _apiRepository.AddApi(entity);
            await _apiRepository.SaveAsync();
            return Ok(new ApiResult());
        }
        [HttpDelete("{ApiId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> DeleteApi(Guid ApiId)
        {
            var Api = await _apiRepository.GetApi(ApiId);
            if (Api == null)
            {
                return NotFound();
            }
            _apiRepository.DeleteApi(Api);
            await _apiRepository.SaveAsync();
            return Ok(new ApiResult());
        }

        [HttpPut("{ApiId}")]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> UpdateApi(Guid ApiId, ApiAddOrUpdateDto ApiUpdateDto)
        {
            var Api = await _apiRepository.GetApi(ApiId);
            if (Api == null)
            {
                return NotFound();
            }
            string newTitle = ApiUpdateDto.Title;
            string newKey = ApiUpdateDto.Key;
            if (await _apiRepository.IsApiExist(newTitle) && Api.Title != newTitle)
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该Api名称已存在，请更换Api名重试"
                });
            }
            if (await _apiRepository.IsApiExist(newKey) && Api.Key != newKey)
            {
                return BadRequest(new ApiResult
                {
                    Code = 400,
                    Msg = "该Api路径已存在，请更换Api路径重试"
                });
            }
            Guid MenuId = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(ApiUpdateDto.MenuId))
            {
                MenuId = new Guid(ApiUpdateDto.MenuId);
            }
            var menu = await _menuRepository.GetChildMenu(MenuId);
            Api.Title = ApiUpdateDto.Title;
            Api.Description = ApiUpdateDto.Description;
            Api.Key = ApiUpdateDto.Key;
            Api.Menu = menu;
            Api.UpdatedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            _apiRepository.UpdateApi(Api);
            await _apiRepository.SaveAsync();
            return Ok(new ApiResult());
        }
    }
}
