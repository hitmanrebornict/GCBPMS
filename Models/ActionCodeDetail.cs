using System;
using System.Collections.Generic;

namespace GCBPMS.Models;

public partial class ActionCodeDetail
{
    public int Id { get; set; }

    public string FullCode { get; set; } = null!;

    public virtual ICollection<UserAction> UserActions { get; set; } = new List<UserAction>();
}
