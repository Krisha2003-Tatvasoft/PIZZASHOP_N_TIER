using Microsoft.AspNetCore.Mvc.Rendering;
using static pizzashop.Entity.Models.Userslogin;

namespace pizzashop.Entity.ViewModels;

public class AddNewUser
{
    public int Userid {get; set;}

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public int Roleid { get; set; }

    public string Password { get; set; } = null!;

    // public string? Profileimg { get; set; }

    public string Phone { get; set; } = null!;

    public int Countryid { get; set; }

    public int Stateid { get; set; }

    public int Cityid { get; set; }

    public string? Address { get; set; }

    public string? Zipcode { get; set; }

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public List<SelectListItem>? Countries { get; set; }
    public List<SelectListItem>? States { get; set; }
    public List<SelectListItem>? Cities { get; set; }

      public List<SelectListItem>? Roles { get; set; }

       public statustype status { get; set; }

}
