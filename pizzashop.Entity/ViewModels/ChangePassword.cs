using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pizzashop.Entity.ViewModels;

public class ChangePassword
{
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "New password is required.")]
   [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be contain 1 special chracter , 1 uppercase letter and  at least 8 characters.")]
    public string Newpassword { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
     [Compare("Newpassword", ErrorMessage = "Passwords do not match.")]
   [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be contain 1 special chracter , 1 uppercase letter and  at least 8 characters.")]
    public string ConfirmPassword { get; set; }

}
