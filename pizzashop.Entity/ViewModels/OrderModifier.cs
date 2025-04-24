namespace pizzashop.Entity.ViewModels;

public class OrderModifier
{
    public int Modifierid { get; set; }

    public string Modifiername { get; set; } = null!;

    public decimal Rate { get; set; }

    public decimal? TotalAmount { get; set; }

    public short Quantity { get; set; }
}
