using System;
using System.Collections.Generic;

namespace CarService_API.Models.DB;

public partial class Company
{
    public decimal Id { get; set; }

    public string? Companyname { get; set; }

    public string? Active { get; set; }

    public virtual ICollection<Avaliabledate> Avaliabledates { get; set; } = new List<Avaliabledate>();

    public virtual ICollection<Avaliableday> Avaliabledays { get; set; } = new List<Avaliableday>();

    public virtual ICollection<Userdateoffer> Userdateoffers { get; set; } = new List<Userdateoffer>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
