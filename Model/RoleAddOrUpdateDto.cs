using System.ComponentModel.DataAnnotations;

namespace Blog2022_netcore.Model
{
    public class RoleAddOrUpdateDto
    {
        [Display(Name = "角色名称")]
        [Required(ErrorMessage = "{0}是必填的")]
        [MaxLength(100, ErrorMessage = "{0}的最大长度是{1}")]
        public string Name { get; set; }

    }
}
