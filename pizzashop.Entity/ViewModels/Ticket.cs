namespace pizzashop.Entity.ViewModels;

public class Ticket
{
    public int Categoryid { get; set; }

    public string Categoryname { get; set; } = null!;

    public int Orderid { get; set; }
    public List<string> Tablenames { get; set; } = new List<string>();

    public string Sectionname { get; set; } = null!;

    public List<OrderItem> Items { get; set; } = new List<OrderItem>();

    public string? Orderwisecomment { get; set; }
      

}
