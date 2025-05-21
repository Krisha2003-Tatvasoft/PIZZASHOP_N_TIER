namespace pizzashop.Entity.ViewModels;

public class TicketResult
{
    public int OrderId { get; set; }
    public int Orderitemid { get; set; }
    public string Tablenames { get; set; }
    public string Sectionname { get; set; }
    public string? Orderwisecomment { get; set; }
    public int Itemid { get; set; }
    public string Itemname { get; set; }
    public short Quantity { get; set; }
    public string? Itemwisecomment { get; set; }
    public int? Modifierid { get; set; }
    public string? Modifiername { get; set; }
    public DateTime Orderdate { get; set; }

    public int OrderStatus { get; set; }
}
