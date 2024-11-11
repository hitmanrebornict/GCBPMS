using System;
using System.Collections.Generic;

namespace GCBPMS.Models;

public partial class PlateHistoryUsage
{
    public int Id { get; set; }

    public int? PlateId { get; set; }

    public DateTime InstallDateTime { get; set; }

    public DateTime? RemoveDateTime { get; set; }

    public int? PotId { get; set; }

    public virtual Plate? Plate { get; set; }

    public virtual Pot? Pot { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
