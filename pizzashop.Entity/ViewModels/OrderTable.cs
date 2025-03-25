using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.ViewModels;

public class OrderTable
{
    public int Orderid { get; set; }

    public DateTime? Orderdate { get; set; }

    public string? Customername { get; set; }

    public orderstatus orderstatus { get; set; }

    public paymentmode paymentmode { get; set; }

    public decimal Rating { get; set; }

    public decimal Totalamount { get; set; }
}
