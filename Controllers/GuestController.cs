using Blog2022_netcore.Data;
using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;
using Blog2022_netcore.Model.QueryParameters;
using Blog2022_netcore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Blog2022_netcore.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly IGuestRepository _guestRepository;
        private readonly IHttpContextAccessor _accessor;
        private readonly RoutineDbContext _context;

        public GuestController(IGuestRepository guestRepository, IHttpContextAccessor httpContextAccessor, RoutineDbContext routineDbContext)
        {
            _guestRepository = guestRepository ?? throw new ArgumentNullException(nameof(guestRepository));
            _accessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = routineDbContext ?? throw new ArgumentNullException(nameof(routineDbContext));
        }
        [HttpGet]
        public async Task<IActionResult> GuestConfig()
        {

            string IP = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            Guest guest = await _guestRepository.GetGuest(IP);
            if (guest == null)
            {
                try
                {
                    HttpClient client = new HttpClient();
                    string getIpAddressUrl = $"https://www.douyacun.com/api/openapi/geo/location?ip={IP}&token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJBY2NvdW50SWQiOiJkOTQyY2RhMTVkOWMzZjQ3MzhiZTA5MDI2YTM1MTU4NSJ9.jCGrZzseH_EkWWFX8xippvMfmgcaXhbH6L2W7yJrKUU";
                    client.BaseAddress = new Uri(getIpAddressUrl);
                    var response = await client.GetAsync(getIpAddressUrl);
                    string result = await response.Content.ReadAsStringAsync();
                    Res res = JsonConvert.DeserializeObject<Res>(result);
                    if (string.IsNullOrEmpty(res.data.ip))
                    {
                        res.data.ip = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();//若ip没有则取时间戳代替
                    }
                    _guestRepository.AddGuest(res.data);
                }
                catch (Exception e)
                {

                    throw new ArgumentException(e.Message);
                }
            }
            else
            {
                guest.AccessCount += 1;
                guest.UpdateDateTime = DateTime.Now;
            }

            await _guestRepository.SaveAsync();
            GuestStatistics guestStatisticsDto = await _guestRepository.GetGuestAndAccessCount();
            BlogConfig blogConfig = await _context.BlogConfig.FirstOrDefaultAsync(x => !String.IsNullOrEmpty(x.FirstText));
            GuestConfigDto guestConfigDto = new GuestConfigDto
            {
                guestStatistics = guestStatisticsDto,
                blogConfig = blogConfig
            };
            return Ok(new ApiResult
            {
                Data = guestConfigDto
            });
        }
        [HttpGet]
        [Authorize("ApiPolicy")]
        public async Task<IActionResult> GetGuests([FromQuery] GuestQueryParameters guestQueryParameters)
        {
            var guests = await _guestRepository.GetGuests(guestQueryParameters);
            int totalCount = await _context.Guest.CountAsync();
            var guestsDto = new List<GuestDto>();
            if (guests != null)
            {
                foreach (var item in guests)
                {
                    GuestDto guest = new GuestDto
                    {
                        Ip = item.Ip,
                        Country = item.Country,
                        Province = item.Province,
                        City = item.City,
                        Latitude = item.Latitude,
                        Longitude = item.Longitude,
                        CreatedDate = item.CreateDateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                        UpdatedDate = item.UpdateDateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                        AccessCount = item.AccessCount,
                    };
                    guestsDto.Add(guest);
                }
            }
            GuestListDto returnDto = new GuestListDto
            {
                Guests = guestsDto,
                TotalCount = totalCount
            };
            return Ok(new ApiResult
            {
                Data = returnDto
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetGuestStatistics()
        {
            GuestStatistics guestStatisticsDto = await _guestRepository.GetGuestAndAccessCount();
            return Ok(new ApiResult
            {
                Data = guestStatisticsDto
            });
        }
    }

}

public class Res
{
    public int code;
    public string message;
    public IpAddressApiResult data;
}
public class GuestConfigDto
{
    public GuestStatistics guestStatistics { get; set; }
    public BlogConfig blogConfig { get; set; }
}

public class GuestListDto
{
    public List<GuestDto> Guests { get; set; }
    public int TotalCount { get; set; }
}