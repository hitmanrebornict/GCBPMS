using System;
using System.Collections.Generic;

namespace GCBPMS.Models;

public partial class Repair
{
    public int Id { get; set; }

    public string? RepairType { get; set; }

    public DateTime? FinishDatetime { get; set; }

    public string? TechnicianName { get; set; }

    public int? SupplierDetailsId { get; set; }

    public string? RepairStatus { get; set; }

    public string? RepairRemark { get; set; }

    public DateTime StartDatetime { get; set; }

    public int RequestId { get; set; }

    public string? AcceptedBy { get; set; }

    public string? CompletedBy { get; set; }

    public virtual ICollection<RepairCost> RepairCosts { get; set; } = new List<RepairCost>();

    public virtual Request Request { get; set; } = null!;

    public virtual SupplierDetail? SupplierDetails { get; set; }
}
