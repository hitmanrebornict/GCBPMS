using System;
using System.Collections.Generic;

namespace GCBPMS.Models;

public partial class Plate
{
    public int Id { get; set; }

    public string PlateName { get; set; } = null!;

    public bool Active { get; set; }

    public int? PlateBrand { get; set; }

    public string PlateStatus { get; set; } = null!;

    public DateTime PlateInstallDatetime { get; set; }

    public int? PhaseType { get; set; }

    public virtual Phase? PhaseTypeNavigation { get; set; }

    public virtual Brand? PlateBrandNavigation { get; set; }

    public virtual ICollection<PlateHistoryUsage> PlateHistoryUsages { get; set; } = new List<PlateHistoryUsage>();

    public virtual ICollection<Pot> Pots { get; set; } = new List<Pot>();

    public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();
}
