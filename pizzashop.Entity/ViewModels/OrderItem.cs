namespace pizzashop.Entity.ViewModels;

public class OrderItem
{
    public int Itemid { get; set; }

    public string Itemname { get; set; } = null!;

    public decimal Rate { get; set; }

    public short Quantity { get; set; }

    public int ReadyQuantity { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Itemwisecomment { get; set; }

    public List<OrderModifier?> Modifiers { get; set; } = new List<OrderModifier>();
}
