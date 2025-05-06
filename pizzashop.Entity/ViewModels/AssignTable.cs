using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.Entity.ViewModels;

public class AssignTable
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email format")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Customer name is required")]
    [RegularExpression(@"^[^\d][\w\s]*$", ErrorMessage = "Customer name cannot start with a number")]
    public string Customername { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
    public string Phoneno { get; set; } = null!;

    [Range(1, 100, ErrorMessage = "No. of persons must be at least 1")]
    public short? Noofperson { get; set; }

    // [Required(ErrorMessage = "Section is required")]
    public List<int>? Sectionid { get; set; }

    public List<SelectListItem>? SectionList { get; set; }

    public int? Waitingtokenid { get; set; }

     public string? TableIds { get; set; }
}
