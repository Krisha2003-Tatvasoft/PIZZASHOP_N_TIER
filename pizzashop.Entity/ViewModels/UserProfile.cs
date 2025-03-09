using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.Entity.ViewModels
{
  public class UserProfile
  {
    public int Userid { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    public string Firstname { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    public string Lastname { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 digits.")]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "Please select a country.")]
    public int Countryid { get; set; }

    [Required(ErrorMessage = "Please select a state.")]
    public int Stateid { get; set; }

    [Required(ErrorMessage = "Please select a city.")]
    public int Cityid { get; set; }

    [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
    public string? Address { get; set; }

    [RegularExpression(@"^\d{5,10}$", ErrorMessage = "Zipcode must be between 5 to 10 digits")]
    public string? Zipcode { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(30, ErrorMessage = "Username cannot exceed 30 characters.")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Role name is required.")]
    public string Rolename { get; set; } = null!;

    public List<SelectListItem>? Countries { get; set; }
    public List<SelectListItem>? States { get; set; }
    public List<SelectListItem>? Cities { get; set; }
  }
}
