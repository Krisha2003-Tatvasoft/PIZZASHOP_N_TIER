using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.ViewModels;

public class OrderHistory
{
    public int Orderid { get; set; }

    public DateTime? Orderdate { get; set; }

    public int orderType { get; set; }

    public paymentmode paymentmode { get; set; }

    public short Quantity { get; set; }

    public decimal TotalAmount { get; set; }

}
