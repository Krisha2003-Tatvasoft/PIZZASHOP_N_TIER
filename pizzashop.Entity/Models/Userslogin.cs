using System;
using System.Collections.Generic;
using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.Models;

public partial class Userslogin
{
   
    public object select;

    public int Userloginid { get; set; }

    public statustype status { get; set; }

    public string Email { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public int Userid { get; set; }

    public string? Refreshtoken { get; set; }

    public int Roleid { get; set; }

    public string Username { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual User? User { get; set; }
}
