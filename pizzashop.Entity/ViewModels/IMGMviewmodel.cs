using pizzashop.Entity.Models;
using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.ViewModels;

public class IMGMviewmodel
{
    public int Itemmodifiergroupid { get; set; }

    public int? Itemid { get; set; }

    public string? Itemname { get; set; }

    public string? Modifiergroupname { get; set; }

    public int Modifiergroupid { get; set; }

    public short? Minselectionrequired { get; set; }

    public short? Maxselectionallowed { get; set; }

    public string? Modifiername { get; set; }

    public List<Modifier>? modifiers { get; set; }

     public itemtype itemtype { get; set; }
}
