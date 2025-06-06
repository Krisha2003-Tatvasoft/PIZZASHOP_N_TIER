using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.ViewModels;

public class Ticket
{
    public int orderitemid { get; set; }
    public int Categoryid { get; set; }

    public string Categoryname { get; set; } = null!;

    public int Orderid { get; set; }
    public List<string> Tablenames { get; set; } = new List<string>();

    public List<string> Sectionname { get; set; }= new List<string>();

    public List<OrderItem> Items { get; set; } = new List<OrderItem>();

    public string? Orderwisecomment { get; set; }

    public string? RunningSince { get; set; }

    public orderstatus orderstatus { get; set; }


}
