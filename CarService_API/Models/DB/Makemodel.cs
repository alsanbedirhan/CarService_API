using System;
using System.Collections.Generic;

namespace CarService_API.Models.DB;

public partial class Makemodel
{
    public decimal Id { get; set; }

    public decimal Makeid { get; set; }

    public decimal Cuser { get; set; }

    public DateTime? Cdate { get; set; }

    public string? Explanation { get; set; }

    public virtual Make Make { get; set; } = null!;

    public virtual ICollection<Usercar> Usercars { get; set; } = new List<Usercar>();
}
