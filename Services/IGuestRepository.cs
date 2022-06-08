using Blog2022_netcore.Entities;
using Blog2022_netcore.Model;
using Blog2022_netcore.Model.QueryParameters;

namespace Blog2022_netcore.Services
{
    public interface IGuestRepository
    {
        void AddGuest(IpAddressApiResult guestAddDto);
        Task<IEnumerable<Guest>> GetGuests(GuestQueryParameters guestQueryParameters);
        Task<Guest> GetGuest(string IP);
        void UpdateGuest(Guest guest);
        Task<bool> SaveAsync();
        Task<GuestStatistics> GetGuestAndAccessCount();
    }
}
