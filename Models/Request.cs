using System;
using System.Collections.Generic;

namespace GCBPMS.Models;

public partial class Request
{
    public int Id { get; set; }

    public int PlateId { get; set; }

    public DateTime RequestDatetime { get; set; }

    public string? RepairReason { get; set; }

    public string? RepairRemark { get; set; }

    public string? RequestStatus { get; set; }

    public int? PlateHistoryUsageId { get; set; }

    public virtual Plate Plate { get; set; } = null!;

    public virtual PlateHistoryUsage? PlateHistoryUsage { get; set; }

    public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();
}
