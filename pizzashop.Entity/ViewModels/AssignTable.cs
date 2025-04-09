using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.Entity.ViewModels;

public class AssignTable
{
    public string? Email { get; set; }

    public string Customername { get; set; } = null!;

    public string Phoneno { get; set; } = null!;

    public short? Noofperson { get; set; }

    public int Sectionid { get; set; }

    public List<SelectListItem>? SectionList { get; set; }

    public int? Waitingtokenid { get; set; }

     public string? TableIds { get; set; }



}
