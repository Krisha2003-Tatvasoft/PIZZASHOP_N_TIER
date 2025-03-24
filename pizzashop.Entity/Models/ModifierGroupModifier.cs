namespace pizzashop.Entity.Models;

public class ModifierGroupModifier
{
    public int Id { get; set; }  // Primary Key

    public int ModifierGroupId { get; set; }
    public virtual Modifiergroup Modifiergroup { get; set; } = null!;

    public int ModifierId { get; set; }
    public virtual Modifier Modifier { get; set; }  = null!;
}
