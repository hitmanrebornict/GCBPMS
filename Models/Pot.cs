using System;
using System.Collections.Generic;

namespace GCBPMS.Models;

public partial class Pot
{
    public int Id { get; set; }

    public string PotName { get; set; } = null!;

    public bool Active { get; set; }

    public int? PlateId { get; set; }

    public int? PressId { get; set; }

    public virtual Plate? Plate { get; set; }

    public virtual Press? Press { get; set; }
}
