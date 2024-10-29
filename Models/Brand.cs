using System;
using System.Collections.Generic;

namespace GCBPMS.Models;

public partial class Brand
{
    public int Id { get; set; }

    public string BrandName { get; set; } = null!;

    public bool Active { get; set; }

    public virtual ICollection<Plate> Plates { get; set; } = new List<Plate>();
}
