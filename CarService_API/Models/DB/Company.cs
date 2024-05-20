using System;
using System.Collections.Generic;

namespace CarService_API.Models.DB;

public partial class Company
{
    public decimal Id { get; set; }

    public string? Companyname { get; set; }

    public string? Active { get; set; }

    public virtual ICollection<Companywork> Companyworks { get; set; } = new List<Companywork>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
