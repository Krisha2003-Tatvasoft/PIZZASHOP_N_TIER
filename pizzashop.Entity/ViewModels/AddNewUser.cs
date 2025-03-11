using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.ViewModels;

public class AddNewUser
{
  public int Userid { get; set; }

  [Required(ErrorMessage = "First name is required")]
  [RegularExpression(@"^[A-Za-z]{2,50}$",
            ErrorMessage = "First name must contain only alphabets and be between 2 to 50 characters long.")]
  public string Firstname { get; set; } = null!;

  [Required(ErrorMessage = "Last name is required")]
  [RegularExpression(@"^[A-Za-z]{2,50}$",
            ErrorMessage = "Last name must contain only alphabets and be between 2 to 50 characters long.")]
  public string Lastname { get; set; } = null!;

  [Required(ErrorMessage = "Role is required")]
  public int Roleid { get; set; }

  [Required(ErrorMessage = "Password is required")]
  [DataType(DataType.Password)]
  [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be at least 8 characters, contain 1 uppercase letter, 1 number, and 1 special character.")]
  public string Password { get; set; } = null!;

  [Required(ErrorMessage = "Phone number is required")]
  [Phone(ErrorMessage = "Invalid phone number format")]
   [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
  public string Phone { get; set; } = null!;

  [Required(ErrorMessage = "Country is required")]
  public int Countryid { get; set; }

  [Required(ErrorMessage = "State is required")]
  public int Stateid { get; set; }

  [Required(ErrorMessage = "City is required")]
  public int Cityid { get; set; }

  [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
  public string? Address { get; set; }

  [RegularExpression(@"^\d{5,10}$", ErrorMessage = "Zipcode must be between 5 to 10 digits")]
  public string? Zipcode { get; set; }

  [Required(ErrorMessage = "Email is required")]
  [EmailAddress(ErrorMessage = "Invalid email format")]
  public string Email { get; set; } = null!;

  [Required(ErrorMessage = "Username is required")]
  [RegularExpression(@"^(?!^\d+$)(?!^_+$)[A-Za-z0-9_]{3,20}$",
  ErrorMessage = "Username must be 3 to 20 characters long and contain at least one letter. It can also include numbers and underscores.")]
  public string Username { get; set; } = null!;


  [Required(ErrorMessage = "Status is required")]
  public statustype status { get; set; }

  [Display(Name = "Profile Picture")]
  [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" })]
  [MaxFileSize(2 * 1024 * 1024)] // 2MB limit
  public IFormFile? ProfilePicture { get; set; }

  public string? Profileimg { get; set; }
  public List<SelectListItem>? Countries { get; set; }
  public List<SelectListItem>? States { get; set; }
  public List<SelectListItem>? Cities { get; set; }
  public List<SelectListItem>? Roles { get; set; }

 
}
