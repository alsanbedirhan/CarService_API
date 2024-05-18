using CarService_API.Models.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CarService_API.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        ModelContext _context;
        Extentsion _extentsion;
        IMemoryCache _cache;
        public UsersController(ModelContext context, Extentsion extentsion, IMemoryCache cache)
        {
            _context = context;
            _extentsion = extentsion;
            _cache = cache;
        }
        public class clsSearchUserInfo
        {
            public string ad { get; set; }
            public string soyad { get; set; }
        }
        public class clsSearchCustomer : clsSearchUserInfo
        {
            public string mail { get; set; }
        }
        public class clsSearchCustomerCar
        {
            public decimal UserId { get; set; }
            public decimal MakeId { get; set; }
            public decimal MakeModelId { get; set; }
            public string Plaka { get; set; }
            public short Yil { get; set; }
        }
        public class clsSearchUser : clsSearchUserInfo
        {
            public string usertype { get; set; }
        }
        public class clsWorkUser : clsSearchUser
        {
            public decimal idno { get; set; }
            public string mail { get; set; }
        }
        public class Customer
        {
            public decimal Idno { get; set; }
            public string AdSoyad { get; set; }
            public string Mail { get; set; }
        }
        public class UserCar
        {
            public decimal Idno { get; set; }
            public string Marka { get; set; }
            public string Model { get; set; }
            public decimal MarkaModelId { get; set; }
            public string Plaka { get; set; }
        }
        [HttpPost("companyusers")]
        public async Task<IActionResult> CompanyUsers([FromBody] clsSearchUser input)
        {
            try
            {
                var u = _extentsion.GetTokenValues();
                if (u == null || u.UserType != "A")
                {
                    throw new Exception("Hata oluştu");
                }

                var users = _cache.Get<List<User>>("users");
                if (users == null)
                {
                    users = await _context.Users.Include(x => x.Company).AsNoTracking().ToListAsync();
                    _cache.Set("users", users);
                }

                var l = users.Where(x => x.Companyid == u.CompanyId && x.Active == "Y" &&
                (input != null && !string.IsNullOrEmpty(input.ad) ? x.Name.ToLower().Contains(input.ad.ToLower()) : true) &&
                (input != null && !string.IsNullOrEmpty(input.soyad) ? x.Surname.ToLower().Contains(input.soyad.ToLower()) : true) &&
                (input != null && !string.IsNullOrEmpty(input.usertype) ? x.Usertype == input.usertype : true))
                    .Select(x => new UserInfo
                    {
                        Ad = x.Name,
                        Soyad = x.Surname,
                        Mail = x.Mail,
                        Cdate = x.Cdate ?? DateTime.MinValue,
                        UserId = x.Id,
                        UserType = x.Usertype
                    }).OrderByDescending(x => x.Cdate).ToList();

                return Ok(new ResultModel<List<UserInfo>> { Status = true, Data = l });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel { Status = false, Message = ex.Message });
            }
        }

        [HttpPost("searchcustomer")]
        public async Task<IActionResult> Customers([FromBody] clsSearchCustomer input)
        {
            try
            {
                if (input == null || (string.IsNullOrEmpty(input.ad?.Trim() ?? "") && string.IsNullOrEmpty(input.soyad?.Trim() ?? "") && string.IsNullOrEmpty(input.mail?.Trim() ?? "")))
                {
                    throw new Exception("Koşul girmelisiniz");
                }
                var u = _extentsion.GetTokenValues();
                if (u == null || u.UserType == "C")
                {
                    throw new Exception("Hata oluştu");
                }
                input.ad = input.ad?.Trim() ?? "";
                input.soyad = input.soyad?.Trim() ?? "";
                input.mail = input.mail?.Trim() ?? "";

                var users = _cache.Get<List<User>>("users");
                if (users == null)
                {
                    users = await _context.Users.Include(x => x.Company).AsNoTracking().ToListAsync();
                    _cache.Set("users", users);
                }

                var l = users.Where(x => x.Usertype == "C" && x.Active == "Y" &&
                (!string.IsNullOrEmpty(input.ad) ? x.Name.Contains(input.ad, StringComparison.CurrentCultureIgnoreCase) : true) &&
                (!string.IsNullOrEmpty(input.soyad) ? x.Surname.Contains(input.soyad, StringComparison.CurrentCultureIgnoreCase) : true) &&
                (!string.IsNullOrEmpty(input.mail) ? x.Mail.Contains(input.mail) : true))
                    .Select(x => new Customer
                    {
                        Idno = x.Id,
                        AdSoyad = string.Concat(x.Name, " ", x.Surname),
                        Mail = x.Mail
                    }).OrderBy(x => x.AdSoyad).Take(10).ToList();

                return Ok(new ResultModel<List<Customer>> { Status = true, Data = l });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel { Status = false, Message = ex.Message });
            }
        }

        [HttpPost("searchcustomercars")]
        public async Task<IActionResult> CustomerCars([FromBody] clsSearchCustomerCar input)
        {
            try
            {
                if (input == null || input.UserId <= 0)
                {
                    throw new Exception("Hata oluştu");
                }
                var u = _extentsion.GetTokenValues();
                if (u == null || u.UserType == "C")
                {
                    throw new Exception("Hata oluştu");
                }
                input.Plaka = input.Plaka?.Trim() ?? "";

                var l = _context.Usercars.Include(x => x.Makemodel.Make).AsNoTracking().Where(x => x.Userid == input.UserId && x.Active == "Y" &&
                (!string.IsNullOrEmpty(input.Plaka) && !string.IsNullOrEmpty(x.Plate) ? x.Plate.Contains(input.Plaka, StringComparison.CurrentCultureIgnoreCase) : true) &&
                (input.MakeModelId > 0 ? x.Makemodelid == input.MakeModelId : (input.MakeId > 0 ? input.MakeId == x.Makemodel.Makeid : true)) &&
                (input.Yil > 0 ? input.Yil == x.Pyear : true))
                    .Select(x => new UserCar
                    {
                        Marka = x.Makemodel.Make.Explanation ?? "",
                        Model = x.Makemodel.Explanation ?? "",
                        MarkaModelId = x.Makemodelid,
                        Idno = x.Id,
                        Plaka = x.Plate ?? ""
                    }).Take(10).ToList();

                return Ok(new ResultModel<List<UserCar>> { Status = true, Data = l });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel { Status = false, Message = ex.Message });
            }
        }

        [HttpPost("workuser")]
        public async Task<IActionResult> WorkUser([FromBody] clsWorkUser input)
        {
            try
            {
                var u = _extentsion.GetTokenValues();
                if (u == null || input == null || string.IsNullOrEmpty(input.ad) || string.IsNullOrEmpty(input.soyad) || string.IsNullOrEmpty(input.mail) || string.IsNullOrEmpty(input.usertype))
                {
                    throw new Exception("Hata oluştu");
                }
                input.mail = input.mail.Trim();
                input.ad = input.ad.Trim();
                input.soyad = input.soyad.Trim();
                if (!CustomFunctions.IsValidEmail(input.mail))
                {
                    throw new Exception("Mail adresi hatalı");
                }
                if (input.idno != u.UserId && u.UserType == "C")
                {
                    throw new Exception("Yetkiniz bulunamadı");
                }
                if (input.idno > 0)
                {
                    var f = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.idno && x.Active == "Y");
                    if (f == null)
                    {
                        throw new Exception("Kullanıcı bulunamadı");
                    }
                    f.Mail = input.mail;
                    f.Name = input.ad;
                    f.Surname = input.soyad;
                }
                else
                {
                    string pass = Guid.NewGuid().ToString("n").Substring(0, 8);
                    CustomFunctions.CreatePasswordHash(pass, out string passwordHash, out string passwordSalt);
                    var f = await _context.Users.FirstOrDefaultAsync(x => x.Mail == input.mail);
                    if (f == null)
                    {
                        await _context.Users.AddAsync(new Models.DB.User
                        {
                            Active = "Y",
                            Cdate = DateTime.Now,
                            Cuser = u.UserId,
                            Companyid = input.usertype != "C" ? u.CompanyId : null,
                            Name = input.ad,
                            Surname = input.soyad,
                            Mail = input.mail,
                            Usertype = input.usertype,
                            Passsalt = passwordSalt,
                            Passhash = passwordHash
                        });
                    }
                    else if (f.Active == "Y")
                    {
                        throw new Exception("Mail adresi kullanılmakta");
                    }
                    else
                    {
                        f.Active = "Y";
                        f.Passhash = passwordHash;
                        f.Passsalt = passwordSalt;
                    }
                    await _context.Requestlogs.AddAsync(new Requestlog
                    {
                        Rdate = DateTime.Now,
                        Userid = u.UserId,
                        UserAgent = pass
                    });
                }
                await _context.SaveChangesAsync();
                _cache.Remove("users");
                return Ok(new ResultModel { Status = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }
    }
}
