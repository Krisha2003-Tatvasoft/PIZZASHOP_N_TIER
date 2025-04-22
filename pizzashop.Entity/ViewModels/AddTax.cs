using System.ComponentModel.DataAnnotations;
using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.ViewModels;

public class AddTax
{
    public int Taxid { get; set; }

    [Required(ErrorMessage = "Tax name is required")]
    [StringLength(100, ErrorMessage = "Tax name cannot exceed 100 characters")]
    [RegularExpression(@"^(?!\s)(?!\d)[^\s].*", ErrorMessage = "Tax name cannot start with a space or number and cannot be only whitespace.")]

    public string Taxname { get; set; } = null!;

    public bool Isenabled { get; set; }

    public bool Isdefault { get; set; }

    [Required(ErrorMessage = "Tax type is required")]
    public taxtype taxtype { get; set; }

    [Required(ErrorMessage = "Tax value is required")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Tax value must be a valid number with up to 2 decimal places")]
    public string Taxvalue { get; set; } = null!;
}



