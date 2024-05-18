using System;
using System.Collections.Generic;

namespace CarService_API.Models.DB;

public partial class Usercar
{
    public decimal Id { get; set; }

    public decimal Userid { get; set; }

    public decimal Makemodelid { get; set; }

    public decimal Cuser { get; set; }

    public DateTime? Cdate { get; set; }

    public string? Uniquekey { get; set; }

    public string? Plate { get; set; }

    public short? Pyear { get; set; }

    public string? Explanation { get; set; }

    public string? Active { get; set; }

    public virtual ICollection<Companywork> Companyworks { get; set; } = new List<Companywork>();

    public virtual Makemodel Makemodel { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
