using System;
using System.Collections.Generic;

namespace GCBPMS.Models;

public partial class RepairCost
{
    public int Id { get; set; }

    public string CostName { get; set; } = null!;

    public decimal Cost { get; set; }

    public string? CostRemark { get; set; }

    public int? RepairId { get; set; }

    public virtual Repair? Repair { get; set; }
}
