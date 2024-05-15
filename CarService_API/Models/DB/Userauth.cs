using System;
using System.Collections.Generic;

namespace CarService_API.Models.DB;

public partial class Userauth
{
    public decimal Id { get; set; }

    public decimal Userid { get; set; }

    public DateTime? Cdate { get; set; }

    public decimal? Cuser { get; set; }

    public DateTime? Udate { get; set; }

    public decimal? Uuser { get; set; }

    public byte? Authbit { get; set; }

    public string? Active { get; set; }

    public virtual User User { get; set; } = null!;
}
