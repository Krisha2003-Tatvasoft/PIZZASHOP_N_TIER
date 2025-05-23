namespace pizzashop.Entity.ViewModels;

public class RawOrderTableView
{
    public int Sectionid { get; set; }
    public string Sectionname { get; set; }
    public int Tableid { get; set; }
    public string Tablename { get; set; }
    public decimal Capacity { get; set; }
    public int tablestatus { get; set; }
    public int orderstatus { get; set; }
    public decimal Totalamount { get; set; }
    public string? RunningSince { get; set; }
    public int? Orderid { get; set; }
}
