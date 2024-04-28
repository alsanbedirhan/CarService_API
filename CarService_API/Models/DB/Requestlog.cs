using System;
using System.Collections.Generic;

namespace CarService_API.Models.DB;

public partial class Requestlog
{
    public decimal Id { get; set; }

    public decimal Userid { get; set; }

    public DateTime Rdate { get; set; }

    public string? Hostip { get; set; }

    public string? Deviceid { get; set; }

    public string? UserAgent { get; set; }

    public virtual User User { get; set; } = null!;
}
