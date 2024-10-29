using System;
using System.Collections.Generic;

namespace GCBPMS.Models;

public partial class Repair
{
    public int Id { get; set; }

    public int? PlateId { get; set; }

    public string? RepairType { get; set; }

    public DateTime RequestDatetime { get; set; }

    public DateTime? FinishDatetime { get; set; }

    public string? TechnicianName { get; set; }

    public int? SupplierDetailsId { get; set; }

    public string? RepairStatus { get; set; }

    public virtual Plate? Plate { get; set; }

    public virtual ICollection<RepairCost> RepairCosts { get; set; } = new List<RepairCost>();

    public virtual SupplierDetail? SupplierDetails { get; set; }
}
