using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Filters;

namespace pizzashop.Entity.ViewModels;

public class AddModifierGroup
{
    public int Modifiergroupid { get; set; }

    [Required(ErrorMessage = "Modifier group name is required")]
    [StringLength(100, ErrorMessage = "Modifier group name cannot exceed 100 characters")]
    [RegularExpression(@"^[^\d].*", ErrorMessage = "ModifierGroup name cannot start with a number")] 
    public string Modifiergroupname { get; set; } = null!;

    [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
    public string? Description { get; set; }

    public string? selectedModifier { get; set; }

    public List<ModifierList>? SelectedModifiers { get; set; }

}
