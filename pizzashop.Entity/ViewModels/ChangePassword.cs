using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pizzashop.Entity.ViewModels;

public class ChangePassword
{
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "New password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Newpassword { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("Newpassword", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }

}
