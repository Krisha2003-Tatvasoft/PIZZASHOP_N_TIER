﻿using System;
using System.Collections.Generic;
using static pizzashop.Entity.Models.Enums;

namespace pizzashop.Entity.Models;

public partial class Table
{
    public int Tableid { get; set; }

    public string Tablename { get; set; } = null!;

    public int? Sectionid { get; set; }

    public decimal Capacity { get; set; }

     public tablestatus tablestatus { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int? Modifiedby { get; set; }

    public virtual ICollection<Ordertable> Ordertables { get; set; } = new List<Ordertable>();

    public virtual Section? Section { get; set; }

    public virtual ICollection<Waitingtablemapping> Waitingtablemappings { get; set; } = new List<Waitingtablemapping>();
}
