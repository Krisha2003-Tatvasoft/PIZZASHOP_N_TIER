using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.ViewModels;

public class TaxTable
{
    public int Taxid { get; set; }

    public string Taxname { get; set; } = null!;

    public bool Isenabled { get; set; }

    public bool Isdefault { get; set; }

    public string Taxvalue { get; set; } = null!;

    public taxtype taxtype { get; set; }
}
