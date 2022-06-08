namespace Blog2022_netcore.Model
{
    public class GuestDto
    {
        public string Ip { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public int AccessCount { get; set; }
    }
}
