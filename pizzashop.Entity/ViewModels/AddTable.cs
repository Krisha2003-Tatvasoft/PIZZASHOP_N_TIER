using Microsoft.AspNetCore.Mvc.Rendering;
using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.ViewModels;

public class AddTable
{
    public int Tableid { get; set; }

    public string Tablename { get; set; } = null!;

    public int? Sectionid { get; set; }

    public decimal Capacity { get; set; }

    public tablestatus tablestatus { get; set; }

    public List<SelectListItem>? SectionList { get; set; }

}
