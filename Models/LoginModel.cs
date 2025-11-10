using System.ComponentModel.DataAnnotations;

namespace YardManagementApplication.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } 

        [Display(Name = "Remember me for 30 days")]
        public bool RememberMe { get; set; } = false;
    }

}