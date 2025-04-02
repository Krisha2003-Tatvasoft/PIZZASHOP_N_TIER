namespace pizzashop.Entity.ViewModels;

public class CustomerTable
{
     public int Customerid { get; set; }

     public string Customername { get; set; } = null!;

     public string? Email { get; set; }

     public string Phoneno { get; set; } = null!;

     public int? Totalorder { get; set; }

     public DateTime? Orderdate { get; set; }

     public decimal? AvgBill { get; set; }

     public int? MaxOrder { get; set; }

     public int? VisitCount { get; set; }

     public DateTime? Cominng_Since { get; set; }

     public List<OrderHistory> orders { get; set; } = new List<OrderHistory>();
}
