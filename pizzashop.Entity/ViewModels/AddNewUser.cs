using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.ViewModels;

public class AddNewUser
{
  public int Userid { get; set; }

  [Required(ErrorMessage = "First name is required")]
  [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
  public string Firstname { get; set; } = null!;

  [Required(ErrorMessage = "Last name is required")]
  [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
  public string Lastname { get; set; } = null!;

  [Required(ErrorMessage = "Role is required")]
  public int Roleid { get; set; }

  [Required(ErrorMessage = "Password is required")]
  [DataType(DataType.Password)]
  [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
  public string Password { get; set; } = null!;

  [Required(ErrorMessage = "Phone number is required")]
  [Phone(ErrorMessage = "Invalid phone number format")]
  [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
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
  [StringLength(30, ErrorMessage = "Username cannot exceed 30 characters")]
  public string Username { get; set; } = null!;

  [Required(ErrorMessage = "Status is required")]
  public statustype status { get; set; }

  [Display(Name = "Profile Picture")]
  public IFormFile ProfilePicture { get; set; }

  public string? Profileimg { get; set; }
  public List<SelectListItem>? Countries { get; set; }
  public List<SelectListItem>? States { get; set; }
  public List<SelectListItem>? Cities { get; set; }
  public List<SelectListItem>? Roles { get; set; }

 
}
