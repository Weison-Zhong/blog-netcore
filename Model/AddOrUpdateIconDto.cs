using System.ComponentModel.DataAnnotations;

namespace Blog2022_netcore.Model
{
    public class AddOrUpdateIconDto
    {
        [Display(Name = "Icon名称")]
        [Required(ErrorMessage = "{0}是必填的")]
        [MaxLength(50, ErrorMessage = "{0}的最大长度是{1}")]
        public string Name { get; set; }

        [Display(Name = "Icon的Key值")]
        [Required(ErrorMessage = "{0}是必填的")]
        public string Key { get; set; }
    }
}
