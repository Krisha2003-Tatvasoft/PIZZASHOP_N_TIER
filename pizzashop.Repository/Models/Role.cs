using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace pizzashop.Repository.Models;

public partial class Role
{
    public int Roleid { get; set; }

    public string Rolename { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    
    [JsonIgnore]
    public virtual ICollection<Userslogin> Userslogins { get; set; } = new List<Userslogin>();
}
