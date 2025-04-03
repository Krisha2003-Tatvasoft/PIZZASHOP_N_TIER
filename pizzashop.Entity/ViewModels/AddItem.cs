using Microsoft.AspNetCore.Mvc.Rendering;
using static pizzashop.Entity.Models.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace pizzashop.Entity.ViewModels;

public class AddItem
{
  // public int Itemid { get; set; }

  public int Itemid { get; set; }

  [Required(ErrorMessage = "Item name is required")]
  [RegularExpression(@"^[^\d].*", ErrorMessage = "Item name cannot start with a number")]
  public string Itemname { get; set; } = null!;

  [Required(ErrorMessage = "Category is required")]
  public int Categoryid { get; set; }

  [Required(ErrorMessage = "Rate is required")]
  [Range(0.01, double.MaxValue, ErrorMessage = "Rate must be greater than 0")]
  public decimal Rate { get; set; }

  [Range(1, short.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
  public short? Quantity { get; set; }

  [Required(ErrorMessage = "Unit is required")]
  public int Unitid { get; set; }

  public bool Isavailable { get; set; }

  [Range(0, 100, ErrorMessage = "Tax percentage must be between 0 and 100")]
  public decimal? Taxpercentage { get; set; }

  public bool Isdefaulttax { get; set; }

  [StringLength(10, ErrorMessage = "Shortcode cannot exceed 10 characters")]
  public string? Shortcode { get; set; }

  [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
  public string? Description { get; set; }

  public List<SelectListItem>? Categories { get; set; }

  public List<SelectListItem>? Units { get; set; }

  [Required(ErrorMessage = "Item type is required")]
  public itemtype itemtype { get; set; }

  public List<SelectListItem>? MGList { get; set; }

  public List<IMGMviewmodel> ModifierGroups { get; set; } = new List<IMGMviewmodel>();

  public int Createdby { get; set; }

  public List<int>? selectedMGList { get; set; }

  public IFormFile? ItemPicture { get; set; }

  public string? Itemimg { get; set; }


}
