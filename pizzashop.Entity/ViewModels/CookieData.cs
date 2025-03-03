using System.ComponentModel.DataAnnotations;

namespace pizzashop.Entity.ViewModels;

public partial class CookieData
{
    public string Email { get; set; } = null!;
    public int Userid { get; set; }
   public string Rolename { get; set; } = null!;
    public string Username { get; set; } = null!;

}
