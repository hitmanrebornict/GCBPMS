using System;
using System.Collections.Generic;

namespace GCBPMS.Models;

public partial class Phase
{
    public int Id { get; set; }

    public string PhaseName { get; set; } = null!;

    public bool Active { get; set; }

    public int PotNumber { get; set; }

    public virtual ICollection<Plate> Plates { get; set; } = new List<Plate>();

    public virtual ICollection<Press> Presses { get; set; } = new List<Press>();
}
