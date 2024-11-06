using System;
using System.Collections.Generic;

namespace GCBPMS.Models;

public partial class Press
{
    public int Id { get; set; }

    public string PressName { get; set; } = null!;

    public bool Active { get; set; }

    public int? PhaseId { get; set; }

    public virtual Phase? Phase { get; set; }

    public virtual ICollection<Pot> Pots { get; set; } = new List<Pot>();
}
