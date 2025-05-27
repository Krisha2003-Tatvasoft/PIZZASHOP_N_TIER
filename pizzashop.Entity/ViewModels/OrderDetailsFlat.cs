using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.ViewModels;

public class OrderDetailsFlat
{
    public int Orderid { get; set; }
    public int Ordereditemid { get; set; }
    public int Itemid { get; set; }
    public string Itemname { get; set; }
    public decimal Rate { get; set; }
    public short Quantity { get; set; }
    public int Readyquantity { get; set; }
    public bool Isdefaulttax { get; set; }
    public decimal? Taxpercentage { get; set; }
    public string? Itemwisecomment { get; set; }
    public int? Modifierid { get; set; }
    public string? Modifiername { get; set; }
    public decimal? Modifierrate { get; set; }
    public string? Tablename { get; set; }
    public string? Sectionname { get; set; }
    public int? Taxid { get; set; }
    public decimal? Taxvalue { get; set; }
}
