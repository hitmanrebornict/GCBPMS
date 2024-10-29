using System;
using System.Collections.Generic;

namespace GCBPMS.Models;

public partial class SupplierDetail
{
    public int Id { get; set; }

    public string SupplierName { get; set; } = null!;

    public DateTime? Eta { get; set; }

    public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();
}
