using System.ComponentModel.DataAnnotations;

namespace Blog2022_netcore.Model
{
    public class UpdateChildMenuDto
    {
        [Display(Name = "菜单名称")]
        [Required(ErrorMessage = "{0}是必填的")]
        [MaxLength(100, ErrorMessage = "{0}的最大长度是{1}")]
        public string Name { get; set; }

        [Display(Name = "菜单路径")]
        [Required(ErrorMessage = "{0}是必填的")]
        [MaxLength(200, ErrorMessage = "{0}的最大长度是{1}")]
        public string Key { get; set; }

        [Display(Name = "菜单组件存放路径")]
        [MaxLength(100, ErrorMessage = "{0}的最大长度是{1}")]
        public string? ComponentPath { get; set; }

        [Display(Name = "菜单图标Id")]
        [Required(ErrorMessage = "{0}是必填的")]
        public Guid IconId { get; set; }

        [Display(Name = "是否缓存该菜单页面")]
        public byte KeepAlive { get; set; }

        [Display(Name = "父级菜单Id")]
        [Required(ErrorMessage = "{0}是必填的")]
        public Guid ParentMenuId { get; set; }

        [Display(Name = "权重数值")]
        [Required(ErrorMessage = "{0}是必填的")]
        public int Weight { get; set; }
    }
}
