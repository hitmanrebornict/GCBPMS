using System;
using System.Collections.Generic;

namespace GCBPMS.Models;

public partial class UserAction
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public DateTime? ActionDatetime { get; set; }

    public int? PlateId { get; set; }

    public int Action { get; set; }

    public virtual ActionCodeDetail ActionNavigation { get; set; } = null!;

    public virtual Plate? Plate { get; set; }

    public virtual AspNetUser? UsernameNavigation { get; set; }
}
