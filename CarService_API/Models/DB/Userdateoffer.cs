using System;
using System.Collections.Generic;

namespace CarService_API.Models.DB;

public partial class Userdateoffer
{
    public decimal Id { get; set; }

    public decimal Userdateid { get; set; }

    public decimal? Userid { get; set; }

    public decimal Cuser { get; set; }

    public DateTime? Cdate { get; set; }

    public string? Astatus { get; set; }

    public string? Explanation { get; set; }

    public decimal Companyid { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual User? User { get; set; }

    public virtual Userdate Userdate { get; set; } = null!;
}
