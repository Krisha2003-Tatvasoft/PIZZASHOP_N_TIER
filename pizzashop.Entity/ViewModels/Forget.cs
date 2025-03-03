
using System.ComponentModel.DataAnnotations;

namespace pizzashop.Entity.ViewModels;

public class Forget
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please Enter Valid Syntax")]
    public string Email { get; set; }
}
