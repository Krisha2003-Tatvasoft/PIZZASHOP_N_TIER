﻿using System;
using System.Collections.Generic;

namespace pizzashop.Entity.Models;

public partial class Userslogin
{
    public enum statustype
    {
        Active = 0,
        Inactive = 1
    }
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
