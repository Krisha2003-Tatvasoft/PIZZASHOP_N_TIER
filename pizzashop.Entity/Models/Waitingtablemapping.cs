using System;
using System.Collections.Generic;

namespace pizzashop.Entity.Models;

public partial class Waitingtablemapping
{
    public int Waitingtableid { get; set; }

    public int Waitingtokenid { get; set; }

    public int Tableid { get; set; }

    public virtual Table Table { get; set; } = null!;

    public virtual Waitingtoken Waitingtoken { get; set; } = null!;
}
