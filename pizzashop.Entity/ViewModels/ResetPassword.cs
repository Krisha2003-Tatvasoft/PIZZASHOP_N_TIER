using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pizzashop.Entity.ViewModels;



public class ResetPassword
{
    public string Email { get; set; } // To bind the email from the query string

    [Required(ErrorMessage = "password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Password { get; set; } // New password

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } // Confirm password
}
