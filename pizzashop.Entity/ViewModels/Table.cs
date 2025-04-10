using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.ViewModels;

public class Table
{
  public int Tableid { get; set; }

  public string Tablename { get; set; } = null!;

  public int? Sectionid { get; set; }

  public decimal Capacity { get; set; }

  public tablestatus tablestatus { get; set; }

  public bool? Isdeleted { get; set; }

  public orderstatus orderstatus { get; set; }

  public decimal Totalamount { get; set; }

   public string? RunningSince { get; set; }

   public int? Orderid {get; set;}


}
