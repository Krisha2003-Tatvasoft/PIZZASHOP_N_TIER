namespace pizzashop.Entity.ViewModels;

public class TicketDetailResult
{
    public int Orderid { get; set; }
    public int Orderitemid { get; set; }
    public int Itemid { get; set; }
    public string Itemname { get; set; } = null!;
    public short Quantity { get; set; }
    public int? Modifierid { get; set; }
    public string? Modifiername { get; set; }
}
