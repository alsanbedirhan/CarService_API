using System;
using System.Collections.Generic;

namespace CarService_API.Models.DB;

public partial class Make
{
    public decimal Id { get; set; }

    public decimal Cuser { get; set; }

    public DateTime? Cdate { get; set; }

    public string? Explanation { get; set; }

    public virtual ICollection<Makemodel> Makemodels { get; set; } = new List<Makemodel>();
}
