namespace pizzashop.Entity.ViewModels;

public class CustomerTable
{
     public int Customerid { get; set; }

    public string Customername { get; set; } = null!;

    public string? Email { get; set; }

    public string Phoneno { get; set; } = null!;

    public int? Totalorder { get; set; }

     public DateTime? Orderdate { get; set; }
}
