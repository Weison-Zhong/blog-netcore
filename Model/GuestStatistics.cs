namespace Blog2022_netcore.Model
{
    public class GuestStatistics
    {
        public int TotalGuestCount { get; set; }
        public int TotalAccessCount { get; set; }
        public int TodayNewGuestCount { get; set; }
        public int TodayAccssCount { get; set; }
        public List<NewGuestTrendItem> NewGuestTrendList { get; set; }
        public List<GuestCityItem> GuestCityList { get; set; }
    }
}

public class NewGuestTrendItem
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int Count { get; set; }
}

public class GuestCityItem
{
    public string City { get; set; }
    public int Count { get; set; }
}