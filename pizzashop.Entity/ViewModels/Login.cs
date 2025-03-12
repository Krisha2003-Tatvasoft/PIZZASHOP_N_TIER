using System.ComponentModel.DataAnnotations;

namespace pizzashop.Repository.ViewModels;

public partial class Login
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please Enter Valid Syntax")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be at least 8 characters, contain 1 uppercase letter, 1 number, and 1 special character.")]
    public string Password { get; set; } = null!;

    public Boolean RememberMe { get; set; }
}
