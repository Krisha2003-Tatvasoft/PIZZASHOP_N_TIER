using Microsoft.AspNetCore.Mvc.Filters;

namespace pizzashop.Entity.ViewModels;

public class AddModifierGroup
{
    public int Modifiergroupid { get; set; }

    public string Modifiergroupname { get; set; } = null!;

    public string? Description { get; set; }

    public string selectedModifier {get; set;}

    public List<ModifierList> SelectedModifiers  {get; set;}

}
