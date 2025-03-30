using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.Entity.ViewModels;

public class AddModifier
{
    public int Modifierid { get; set; }

    public string Modifiername { get; set; } = null!;

    public decimal Rate { get; set; }

    public short? Quantity { get; set; }

    public int Unitid { get; set; }

    public string? Description { get; set; }

    public int? Createdby { get; set; }

    public string ModifierGroupIds { get; set; }

    public List<int> SelectedMGIds {get; set;}

    public List<SelectListItem>? MGList { get; set; }

    public List<SelectListItem>? Units { get; set; }


}
