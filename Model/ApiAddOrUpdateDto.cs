using System.ComponentModel.DataAnnotations;

namespace Blog2022_netcore.Model
{
    public class ApiAddOrUpdateDto
    {
        [Display(Name = "Api名称")]
        [Required(ErrorMessage = "{0}是必填的")]
        [MaxLength(100, ErrorMessage = "{0}的最大长度是{1}")]
        public string Title { get; set; }

        [Display(Name = "Api路径")]
        [Required(ErrorMessage = "{0}是必填的")]
        [MaxLength(200, ErrorMessage = "{0}的最大长度是{1}")]
        public string Key { get; set; }

        [Display(Name = "MenuId")]
        public string MenuId { get; set; }

        [Display(Name = "Api说明")]
        [MaxLength(300, ErrorMessage = "{0}的最大长度是{1}")]
        public string? Description { get; set; }
    }
}
