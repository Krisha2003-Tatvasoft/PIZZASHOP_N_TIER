using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.ViewModels;

public class AddTax
{
    public int Taxid { get; set; }

    public string Taxname { get; set; } = null!;

    public bool Isenabled { get; set; }

    public bool Isdefault { get; set; }

     public taxtype taxtype { get; set; }

    public string Taxvalue { get; set; } = null!;

}
