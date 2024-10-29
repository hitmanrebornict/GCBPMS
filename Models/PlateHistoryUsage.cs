using System;
using System.Collections.Generic;

namespace GCBPMS.Models;

public partial class PlateHistoryUsage
{
    public int Id { get; set; }

    public int? PlateId { get; set; }

    public int? PressId { get; set; }

    public DateTime InstallDateTime { get; set; }

    public DateTime? RemoveDateTime { get; set; }

    public virtual Plate? Plate { get; set; }

    public virtual Press? Press { get; set; }
}
