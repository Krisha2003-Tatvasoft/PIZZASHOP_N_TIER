using System;
using System.Collections.Generic;

namespace pizzashop.Entity.Models;

public partial class Ordereditemmodifer
{
    public int Modifieditemid { get; set; }

    public int Ordereditemid { get; set; }

    public int Itemmodifiergroupid { get; set; }

      public int modifierid { get; set; }

    public virtual Itemmodifiergroupmap Itemmodifiergroup { get; set; } = null!;

     public virtual Modifier  Modifiers { get; set; } = null!;

    public virtual Ordereditem Ordereditem { get; set; } = null!;
}
