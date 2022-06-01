using System.ComponentModel.DataAnnotations;

namespace Blog2022_netcore.Model
{
    public class AdministratorAddOrUpdateDto
    {
        [Display(Name ="管理员名称")]
        [Required(ErrorMessage = "{0}是必填的")]
        [MaxLength(100, ErrorMessage = "{0}的最大长度是{1}")]
        public string Name { get; set; }

        [Display(Name = "密码")]
        [Required(ErrorMessage = "{0}是必填的")]
        [MaxLength(100, ErrorMessage = "{0}的最大长度是{1}")]
        public string Password { get; set; }

        [Display(Name = "角色Id")]
        [Required(ErrorMessage = "{0}是必填的")]
        public Guid RoleId { get; set; }

        [Display(Name = "用户头像")]
        public IFormFile? AvatarImg { get; set; }
    }
}
