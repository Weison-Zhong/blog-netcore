namespace Blog2022_netcore.Model
{
    public class UpdateBlogConfigDto
    {
        public string FirstText { get; set; }
        public string? SecondText { get; set; }
        public string? ThirdText { get; set; }
        public string? IconLink { get; set; }
        public IFormFile? ResumeFile { get; set; }
    }
}
