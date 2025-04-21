namespace pizzashop.Entity.ViewModels;

public class OrderItemStatus
{
    public int OrderId { get; set; }
    public string NewStatus { get; set; } // "Ready" or "Inprogress"
    public List<ItemQuantity> Items { get; set; }
}
