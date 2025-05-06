using System.ComponentModel.DataAnnotations;

namespace pizzashop.Entity.ViewModels;

public class WaitingListAssignTable
{
    public int? Waitingtokenid { get; set; }

    // [Required(ErrorMessage = "Section is required")]
    public int Sectionid { get; set; }

    public string? TableIds { get; set; }

}
