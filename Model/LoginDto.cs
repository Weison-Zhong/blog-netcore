namespace Blog2022_netcore.Model
{
    public class LoginDto
    {
        public Guid Id { get; set; }
        public string Avatar { get; set; }
        //public string CreatedDate { get; set; }
        //public string UpdatedDate { get; set; }
        public string Token { get; set; }
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public List<MenuDto> MenuList { get; set; }
    }
}
