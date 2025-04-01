using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.ViewModels;

public class ItemTable
{
  public int Itemid { get; set; }

  public string Itemname { get; set; } = null!;

  public decimal Rate { get; set; }

  public short? Quantity { get; set; }

  public string Unitname { get; set; } = null!;

  public itemtype itemtype { get; set; }

  public bool Isavailable { get; set; }

 public int Categoryid { get; set; }

  public string? Itemimg { get; set; }



}
