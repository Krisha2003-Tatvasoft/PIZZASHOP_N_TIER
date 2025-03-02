using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace pizzashop.Repository.Models;

public partial class User
{
    public int Userid { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string? Profileimg { get; set; }

    public string Phone { get; set; } = null!;

    public int Countryid { get; set; }

    public int Stateid { get; set; }

    public int Cityid { get; set; }

    public string? Address { get; set; }

    public string? Zipcode { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int Modifiedby { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public virtual State State { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Userslogin> Userslogins { get; set; } = new List<Userslogin>();
}
