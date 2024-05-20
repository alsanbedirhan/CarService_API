using System;
using System.Collections.Generic;

namespace CarService_API.Models.DB;

public partial class Companywork
{
    public decimal Id { get; set; }

    public decimal Usercarid { get; set; }

    public decimal Companyid { get; set; }

    public decimal Cuser { get; set; }

    public DateTime? Cdate { get; set; }

    public string? Isdone { get; set; }

    public string? Active { get; set; }

    public string? Explanation { get; set; }

    public string? Isout { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<Companyworkdetail> Companyworkdetails { get; set; } = new List<Companyworkdetail>();

    public virtual Usercar Usercar { get; set; } = null!;
}
