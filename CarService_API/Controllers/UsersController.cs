using CarService_API.Models.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarService_API.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        ModelContext _context;
        Extentsion _extentsion;
        public UsersController(ModelContext context, Extentsion extentsion)
        {
            _context = context;
            _extentsion = extentsion;
        }
        public class clsUsers
        {
            public decimal Idno { get; set; }
            public string Ad { get; set; }
            public string Soyad { get; set; }
            public string Mail { get; set; }
            public string Tip { get; set; }
            public DateTime Cdate { get; set; }
        }
        public class clsSearchUser
        {
            public string ad { get; set; }
            public string soyad { get; set; }
            public string usertype { get; set; }
        }
        public class clsWorkUser : clsSearchUser
        {
            public decimal id { get; set; }
            public string mail { get; set; }
            public string psw { get; set; }
        }
        [HttpPost("allusers")]
        public async Task<IActionResult> AllUsers([FromBody] clsSearchUser input)
        {
            try
            {
                var u = _extentsion.GetTokenValues();
                if (u == null || u.UserType != "A")
                {
                    throw new Exception("Hata oluştu");
                }

                var l = await _context.Users.AsNoTracking().Where(x => x.Companyid == u.CompanyId && x.Active == "Y" &&
                (input != null && !string.IsNullOrEmpty(input.ad) ? x.Name.ToLower().Contains(input.ad.ToLower()) : true) &&
                (input != null && !string.IsNullOrEmpty(input.soyad) ? x.Surname.ToLower().Contains(input.soyad.ToLower()) : true) &&
                (input != null && !string.IsNullOrEmpty(input.usertype) ? x.Usertype == input.usertype : true))
                    .Select(x => new clsUsers
                    {
                        Ad = x.Name,
                        Soyad = x.Surname,
                        Mail = x.Mail,
                        Cdate = x.Cdate ?? DateTime.MinValue,
                        Idno = x.Id,
                        Tip = x.Usertype
                    }).OrderByDescending(x => x.Cdate).ToListAsync();

                return Ok(new ResultModel<List<clsUsers>> { Status = true, Data = l });
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
                if (input.id > 0)
                {
                    var f = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.id && x.Active == "Y");
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
                    string pass = Guid.NewGuid().ToString();
                    CustomFunctions.CreatePasswordHash(pass, out string passwordHash, out string passwordSalt);
                    var f = await _context.Users.FirstOrDefaultAsync(x => x.Mail == input.mail);
                    if (f == null)
                    {
                        await _context.Users.AddAsync(new Models.DB.User
                        {
                            Active = "Y",
                            Cdate = DateTime.Now,
                            Companyid = u.CompanyId,
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
                    }
                }
                await _context.SaveChangesAsync();
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
