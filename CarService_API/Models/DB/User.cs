using System;
using System.Collections.Generic;

namespace CarService_API.Models.DB;

public partial class User
{
    public decimal Id { get; set; }

    public string Mail { get; set; } = null!;

    public string Passsalt { get; set; } = null!;

    public string? Active { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public DateTime? Cdate { get; set; }

    public decimal Cuser { get; set; }

    public string Usertype { get; set; } = null!;

    public decimal? Companyid { get; set; }

    public string Passhash { get; set; } = null!;

    public virtual Company? Company { get; set; }

    public virtual ICollection<Requestlog> Requestlogs { get; set; } = new List<Requestlog>();

    public virtual ICollection<Userauth> Userauths { get; set; } = new List<Userauth>();

    public virtual ICollection<Usercar> Usercars { get; set; } = new List<Usercar>();

    public virtual ICollection<Userdateoffer> Userdateoffers { get; set; } = new List<Userdateoffer>();

    public virtual ICollection<Userdate> Userdates { get; set; } = new List<Userdate>();
}
