using Blog2022_netcore.Data;
using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;
using Blog2022_netcore.Model.QueryParameters;
using Microsoft.EntityFrameworkCore;
namespace Blog2022_netcore.Services
{
    public class GuestRepository : IGuestRepository
    {
        private readonly RoutineDbContext _context;

        public GuestRepository(RoutineDbContext routineDbContext)
        {
            _context = routineDbContext ?? throw new ArgumentNullException(nameof(routineDbContext));
        }

        public void AddGuest(IpAddressApiResult ipAddressApiResult)
        {

            Guest guest = new Guest
            {
                Id = Guid.NewGuid(),
                AccessCount = 1,
                Ip = ipAddressApiResult.ip,
                Country = ipAddressApiResult.country,
                Province = ipAddressApiResult.province,
                City = ipAddressApiResult.city,
                Latitude = ipAddressApiResult.latitude,
                Longitude = ipAddressApiResult.longitude,
                CreateDateTime = DateTime.Now,
                UpdateDateTime = DateTime.Now,
            };
            _context.Guest.Add(guest);
        }



        public async Task<IEnumerable<Guest>> GetGuests(GuestQueryParameters guestQueryParameters)
        {
            return await _context.Guest.Skip(guestQueryParameters.PageSize * (guestQueryParameters.PageNumber - 1))
                .Take(guestQueryParameters.PageSize).ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public void UpdateGuest(Guest guest)
        {

        }

        async Task<Guest> IGuestRepository.GetGuest(string IP)
        {
            return await _context.Guest.FirstOrDefaultAsync(x => x.Ip == IP);
        }

        async Task<GuestStatistics> IGuestRepository.GetGuestAndAccessCount()
        {
            DateTime now = DateTime.Now;
            int guestCount = await _context.Guest.CountAsync();
            int accessCount = await _context.Guest.SumAsync(x => x.AccessCount);
            int todayNewGuestCount = await _context.Guest.Where(d => d.CreateDateTime.Year == now.Year && d.CreateDateTime.Month == now.Month && d.CreateDateTime.Day == now.Day).CountAsync();
            //这个只是粗略值，如果该用户当天重复进入也只是计算了一次，若要精确的需要建个表记录用户每一次都录的时间日志
            int todayAccessCount = await _context.Guest.Where(d => d.UpdateDateTime.Year == now.Year && d.UpdateDateTime.Month == now.Month && d.UpdateDateTime.Day == now.Day).CountAsync();
            var newGuestGroup = await _context.Guest
                 .GroupBy(c => new { Year = c.CreateDateTime.Year, Month = c.CreateDateTime.Month })
            .Select(c => new NewGuestTrendItem
            {
                Month = c.Key.Month,
                Year = c.Key.Year,
                Count = c.Count()
            })
            .OrderByDescending(a => a.Year)
            .ThenByDescending(a => a.Month)
            .Take(12)
            .Reverse()
            .ToListAsync();
            var guestProvinceGroup = await _context.Guest
            .GroupBy(c => new { City = c.City })
            .Select(c => new GuestCityItem
            {
                City = c.Key.City,
                Count = c.Count()
            })
            .ToListAsync();
            int len = newGuestGroup.Count();
            int minMonth = now.Month;
            int minYear = now.Year;
            if (len != 0)
            {
                minMonth = newGuestGroup.First().Month;
                minYear = newGuestGroup.First().Year;
            }
            if (len < 12)
            {
                int a = 1;
                for (int i = 0; i < 12 - len; i++)
                {
                    int month = minMonth - a;
                    a++;
                    if (month < 1)
                    {
                        minYear--;
                        minMonth = 12;
                        month = 12;
                        a = 1;
                    }

                    newGuestGroup.Insert(0, new NewGuestTrendItem
                    {
                        Month = month,
                        Year = minYear,
                        Count = 0
                    });
                }
            }
            return new GuestStatistics
            {
                TotalGuestCount = guestCount,//访客总数
                TotalAccessCount = accessCount, //访问总次数
                TodayNewGuestCount = todayNewGuestCount,
                TodayAccssCount = todayAccessCount,
                NewGuestTrendList = newGuestGroup,
                GuestCityList = guestProvinceGroup
            };
        }
    }
}