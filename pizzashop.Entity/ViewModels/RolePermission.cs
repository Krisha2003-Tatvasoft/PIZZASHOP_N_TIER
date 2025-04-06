namespace pizzashop.Entity.ViewModels;

public partial class RolePermission
{

    public int Permissionid { get; set; }

    public string Rolename { get; set; } = null!;

    public string Permissionname { get; set; } = null!;

    public bool Canview { get; set; }

    public bool Canaddedit { get; set; }

    public bool Candelete { get; set; }

    public bool CanPermission { get; set; }
 
      public int Moduleid { get; set; }
   

}
