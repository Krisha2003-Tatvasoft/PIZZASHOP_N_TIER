using System.ComponentModel.DataAnnotations;

namespace pizzashop.Repository.ViewModels;

public partial class Login
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please Enter Valid Syntax")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;

    public Boolean RememberMe { get; set; }
}
