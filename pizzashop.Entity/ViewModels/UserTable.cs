using System;
using System.Collections.Generic;
namespace pizzashop.Entity.ViewModels;

public partial class UserTable
{
    public int userId { get; set; }
    public string Email { get; set; } = null!;
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Rolename { get; set; } = null!;
}
