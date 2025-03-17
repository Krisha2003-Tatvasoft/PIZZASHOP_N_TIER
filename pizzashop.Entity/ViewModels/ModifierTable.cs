namespace pizzashop.Entity.ViewModels;

public class ModifierTable
{
    public int Modifiergroupid { get; set; }
    public int Modifierid { get; set; }

    public string Modifiername { get; set; } = null!;

    public decimal Rate { get; set; }

    public short? Quantity { get; set; }

    public string Unitname { get; set; } = null!;
}
