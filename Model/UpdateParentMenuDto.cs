using System.ComponentModel.DataAnnotations;

namespace Blog2022_netcore.Model
{
    public class UpdateParentMenuDto
    {
        [Display(Name = "菜单名称")]
        [Required(ErrorMessage = "{0}是必填的")]
        [MaxLength(100, ErrorMessage = "{0}的最大长度是{1}")]
        public string Name { get; set; }

        [Display(Name = "菜单路径")]
        [Required(ErrorMessage = "{0}是必填的")]
        [MaxLength(200, ErrorMessage = "{0}的最大长度是{1}")]
        public string Key { get; set; }


        [Display(Name = "菜单图标Id")]
        [Required(ErrorMessage = "{0}是必填的")]
        public Guid IconId { get; set; }

    }
}
