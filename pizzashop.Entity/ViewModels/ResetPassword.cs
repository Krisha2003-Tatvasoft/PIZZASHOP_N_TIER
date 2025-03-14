using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pizzashop.Entity.ViewModels;



public class ResetPassword
{
    public string Email { get; set; } // To bind the email from the query string

   [Required(ErrorMessage = "password is required.")]
   [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be at least 8 characters, contain 1 uppercase letter, 1 number, and 1 special character.")]
    public string Password { get; set; } // New password

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } // Confirm password

    public string token {get ; set;}

}
