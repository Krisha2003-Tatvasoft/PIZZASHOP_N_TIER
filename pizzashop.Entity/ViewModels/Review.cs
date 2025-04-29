namespace pizzashop.Entity.ViewModels;

public class Review
{
    public int Orderid { get; set; }

    public decimal? Foodrating { get; set; }

    public decimal? Servicerating { get; set; }

    public decimal? Ambiencerating { get; set; }

    public float? Avgrating { get; set; }

    public string? Comments { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Modifiedat { get; set; }
}
