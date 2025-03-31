using System.ComponentModel.DataAnnotations;

namespace pizzashop.Entity.ViewModels
{
    public class Section
    {
        public int Sectionid { get; set; }

        [Required(ErrorMessage = "Section name is required.")]
        [StringLength(50, ErrorMessage = "Section name cannot exceed 50 characters.")]
        [RegularExpression(@"^[^\d].*", ErrorMessage = "Section name cannot start with a number.")]
        public string Sectionname { get; set; } = null!;

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string? Description { get; set; }
    }
}
