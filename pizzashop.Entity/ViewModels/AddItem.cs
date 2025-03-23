using Microsoft.AspNetCore.Mvc.Rendering;
using static pizzashop.Entity.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace pizzashop.Entity.ViewModels;

public class AddItem
{
  // public int Itemid { get; set; }

  public int Itemid { get; set; }

  public string Itemname { get; set; } = null!;

  public int Categoryid { get; set; }

  public decimal Rate { get; set; }

  public short? Quantity { get; set; }

  public int Unitid { get; set; }

  public bool Isavailable { get; set; }

  public decimal? Taxpercentage { get; set; }

  public bool Isdefaulttax { get; set; }

  public string? Shortcode { get; set; }

  public string? Description { get; set; }

  public List<SelectListItem>? Categories { get; set; }

  public List<SelectListItem>? Units { get; set; }

  public itemtype itemtype { get; set; }

  public List<SelectListItem>? MGList { get; set; }

  public List<IMGMviewmodel> ModifierGroups { get; set; } = new List<IMGMviewmodel>();

  public int Createdby { get; set; }

  public List<int>? selectedMGList {get; set;}

  public IFormFile? ItemPicture { get; set; }

  public string? Itemimg { get; set; }


}
