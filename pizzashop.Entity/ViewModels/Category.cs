using System.ComponentModel.DataAnnotations;

namespace pizzashop.Entity.ViewModels;

public class Category
{

     public int Categoryid { get; set; }

     [Required(ErrorMessage = "Category name is required")]
     [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
     [RegularExpression(@"^(?!\s)(?!\d)[^\s].*", ErrorMessage = "Category name cannot start with a space or number and cannot be only whitespace.")]

     public string Categoryname { get; set; } = null!;

     [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
     public string? Description { get; set; }

     public int Createdby { get; set; }

     public int Modifiedby { get; set; }

     public int? OrderCount { get; set; }
}
