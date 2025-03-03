﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace pizzashop.Entity.Models;

public partial class Role
{
    public int Roleid { get; set; }

    public string Rolename { get; set; } = null!;


    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    
  
    public virtual ICollection<Userslogin> Userslogins { get; set; } = new List<Userslogin>();
}
