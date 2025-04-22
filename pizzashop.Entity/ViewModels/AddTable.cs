using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.ViewModels;

public class AddTable
{
    public int Tableid { get; set; }

    [Required(ErrorMessage = "Table name is required.")]
    [StringLength(100, ErrorMessage = "Table name cannot exceed 100 characters.")]
    [RegularExpression(@"^(?!\s)(?!\d)[^\s].*", ErrorMessage = "Table name cannot start with a space or number and cannot be only whitespace.")]

    public string Tablename { get; set; } = null!;

    [Required(ErrorMessage = "Section is required.")]
    public int? Sectionid { get; set; }

    [Required(ErrorMessage = "Capacity is required.")]
    [Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100.")]
    public decimal Capacity { get; set; }

    [Required(ErrorMessage = "Table status is required.")]
    public tablestatus tablestatus { get; set; }

    public List<SelectListItem>? SectionList { get; set; }
}
