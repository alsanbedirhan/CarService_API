using System;
using System.Collections.Generic;

namespace CarService_API.Models.DB;

public partial class Userdate
{
    public decimal Id { get; set; }

    public DateTime Datevalue { get; set; }

    public decimal Userid { get; set; }

    public decimal Carid { get; set; }

    public string? Active { get; set; }

    public DateTime? Cdate { get; set; }

    public string? Approved { get; set; }

    public string? Carok { get; set; }

    public string? Explanation { get; set; }

    public virtual Car Car { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Userdateoffer> Userdateoffers { get; set; } = new List<Userdateoffer>();
}
