using System;
using System.Collections.Generic;

namespace CarService_API.Models.DB;

public partial class Avaliabledate
{
    public decimal Id { get; set; }

    public DateTime Avaliabledate1 { get; set; }

    public int? Avaliablecount { get; set; }

    public decimal? Companyid { get; set; }

    public virtual Company? Company { get; set; }
}
