using System.ComponentModel.DataAnnotations;

namespace PaypalLab.ViewModels
{
public class RoleVM
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

    }
}
