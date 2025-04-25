namespace pizzashop.Entity.ViewModels;

public class Bill
{
    public int Orderid { get; set; }

    public List<string> Tablenames { get; set; } = new List<string>();

    public string Sectionname { get; set; } = null!;

    public List<OrderItem> Items { get; set; } = new List<OrderItem>();

    public List<TaxTable> Taxes { get; set; } = new List<TaxTable>();

}
