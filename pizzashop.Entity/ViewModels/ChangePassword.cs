using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pizzashop.Entity.ViewModels;

public class ChangePassword
{
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "New password is required.")]
   [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be at least 8 characters, contain 1 uppercase letter, 1 number, and 1 special character.")]
    public string Newpassword { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
   [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be at least 8 characters, contain 1 uppercase letter, 1 number, and 1 special character.")]
    public string ConfirmPassword { get; set; }

}
