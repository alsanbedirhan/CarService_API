using System;
using System.Collections.Generic;

namespace CarService_API.Models.DB;

public partial class Companyworkdetail
{
    public decimal Id { get; set; }

    public decimal Companyworkid { get; set; }

    public decimal Userid { get; set; }

    public decimal Cuser { get; set; }

    public DateTime? Cdate { get; set; }

    public string? Explanation { get; set; }

    public decimal? Price { get; set; }

    public string? Active { get; set; }

    public virtual Companywork Companywork { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
