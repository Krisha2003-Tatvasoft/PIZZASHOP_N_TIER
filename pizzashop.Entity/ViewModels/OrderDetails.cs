using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.ViewModels;

public class OrderDetails
{
    public int Orderid { get; set; }

    public string? Customername { get; set; }

    public orderstatus orderstatus { get; set; }

    public DateTime? PlacedOn { get; set; }

    public string? Email { get; set; }

    public string Phoneno { get; set; } = null!;

    public short? Noofperson { get; set; }

    public string Invoicenumber { get; set; } = null!;

    public List<string> Tablenames { get; set; } = new List<string>();

    public List<string> Sectionname { get; set; } = new List<string>();

    public List<OrderItem> Items { get; set; } = new List<OrderItem>();

    public List<TaxTable> Taxes { get; set; } = new List<TaxTable>();

    public decimal Totalamount { get; set; }

    public decimal? Subamount { get; set; }

    public paymentmode paymentmode { get; set; }




}
