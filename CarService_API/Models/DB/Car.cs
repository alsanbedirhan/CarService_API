using System;
using System.Collections.Generic;

namespace CarService_API.Models.DB;

public partial class Car
{
    public decimal Id { get; set; }

    public string Plate { get; set; } = null!;

    public string? Explanation { get; set; }

    public string? Uniquekey { get; set; }

    public decimal Userid { get; set; }

    public DateTime? Cdate { get; set; }

    public decimal Cuser { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Userdate> Userdates { get; set; } = new List<Userdate>();
}
