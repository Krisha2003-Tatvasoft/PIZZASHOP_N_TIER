using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.Entity.ViewModels;

public class AddModifier
{
    public int Modifierid { get; set; }

    [Required(ErrorMessage = "Modifier name is required")]
    [StringLength(100, ErrorMessage = "Modifier name cannot exceed 100 characters")]
    [RegularExpression(@"^[^\d].*", ErrorMessage = "Modifier name cannot start with a number")]
    public string Modifiername { get; set; } = null!;

    [Required(ErrorMessage = "Rate is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Rate must be a non-negative value")]
    public decimal Rate { get; set; }

    [Range(1, short.MaxValue, ErrorMessage = "Quantity must be at least 1")]

    public short? Quantity { get; set; }

    [Required(ErrorMessage = "Unit is required")]
    public int Unitid { get; set; }

    [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
    public string? Description { get; set; }

    public int? Createdby { get; set; }

    public string? ModifierGroupIds { get; set; }

    public List<int>? SelectedMGIds { get; set; }

    public List<SelectListItem>? MGList { get; set; }

    public List<SelectListItem>? Units { get; set; }


}
