namespace pizzashop.Entity.ViewModels;

public class ModifiersGroup
{
    public int Modifiergroupid { get; set; }

    public string Modifiergroupname { get; set; } = null!;

    public string? Description { get; set; }

    public int Createdby { get; set; }

    public int Modifiedby { get; set; }
    
}
