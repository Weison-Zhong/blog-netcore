using System.ComponentModel.DataAnnotations;

namespace Blog2022_netcore.Model
{
    public class AddOrUpdateDemoDto
    {
        [Display(Name = "Demo标题")]
        [Required(ErrorMessage = "{0}是必填的")]
        [MaxLength(100, ErrorMessage = "{0}的最大长度是{1}")]
        public string Title { get; set; }

        [Display(Name = "Demo描述")]
        [Required(ErrorMessage = "{0}是必填的")]
        public string Description { get; set; }

        [Display(Name = "Demo排序权重")]
        [Required(ErrorMessage = "{0}是必填的")]
        public int Weight { get; set; }
        [Display(Name = "Demo展示状态")]
        [Required(ErrorMessage = "{0}是必填的")]
        public byte Status { get; set; }
        public IFormFile? CoverImage { get; set; }
        public string? RelatedLink { get; set; }

        public List<Guid>? IconIds { get; set; }
    }
}
