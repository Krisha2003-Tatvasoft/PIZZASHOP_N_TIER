using System.ComponentModel.DataAnnotations;

namespace pizzashop.Entity.ViewModels;

public class CustomerDetail
{
    public int? customerId { get; set; }

    public int? Orderid { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email format")]
    public string Email { get; set; }= null!;

    public string? oldemail { get; set; }


    [Required(ErrorMessage = "Customer name is required")]
    [RegularExpression(@"^[^\d][\w\s]*$", ErrorMessage = "Customer name cannot start with a number")]
    public string Customername { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
    public string Phoneno { get; set; } = null!;

    [Range(1, 100, ErrorMessage = "No. of persons must be at least 1")]
    public short? Noofperson { get; set; }

}
