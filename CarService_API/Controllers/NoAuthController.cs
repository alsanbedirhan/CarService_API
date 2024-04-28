using CarService_API.Models.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CarService_API.Controllers
{
    [Route("api/[controller]")]
    public class NoAuthController : Controller
    {
        ModelContext _context;
        Extentsion _extentsion;
        public NoAuthController(ModelContext context, Extentsion extentsion)
        {
            _context = context;
            _extentsion = extentsion;
        }
        public class clsLoginModel
        {
            public string mail { get; set; }
            public string psw { get; set; }
        }
        public class clsRegisterModel : clsLoginModel
        {
            public string name { get; set; }
            public string surname { get; set; }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] clsRegisterModel model)
        {
            if (model == null)
            {
                return BadRequest(new ResultModel { Message = "Parametre hatalı", Status = false });
            }
            model.mail = model.mail.Trim();
            model.name = model.name.Trim();
            model.surname = model.surname.Trim();
            if (!IsValidEmail(model.mail))
            {
                return BadRequest(new ResultModel { Message = "Mail adresi geçersiz", Status = false });
            }
            var u = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Mail == model.mail);
            if (u != null && (u.Active == "Y" || u.Usertype != "C"))
            {
                return BadRequest(new ResultModel { Message = "Mail adresi zaten kullanılmakta", Status = false });
            }

            CreatePasswordHash(model.psw, out var passwordHash, out var passwordSalt);
            if (u != null)
            {
                u.Name = model.name;
                u.Surname = model.surname;
                u.Active = "Y";
                u.Passhash = passwordHash;
                u.Passsalt = passwordSalt;
            }
            else
            {
                u = new Models.DB.User
                {
                    Mail = model.mail,
                    Usertype = "C",
                    Active = "Y",
                    Cdate = DateTime.Now,
                    Name = model.name,
                    Surname = model.surname,
                    Passhash = passwordHash,
                    Passsalt = passwordSalt
                };
                await _context.Users.AddAsync(u);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel { Message = ex.Message, Status = false });
            }
            return Ok(new ResultModel<LoginResultUser>
            {
                Status = true,
                Data = new LoginResultUser
                {
                    UserId = u.Id,
                    Ad = u.Name,
                    Soyad = u.Surname,
                    CompanyId = u.Companyid ?? 0m,
                    UserType = u.Usertype,
                    Token = _extentsion.CreateToken(u.Id, (u.Companyid ?? 0m), u.Usertype),
                    CompanyName = u.Company?.Companyname ?? ""
                }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] clsLoginModel model)
        {
            if (model == null)
            {
                return BadRequest(new ResultModel { Message = "Parametre hatalı", Status = false });
            }
            var u = await _context.Users.AsNoTracking().Where(x => x.Mail == model.mail.Trim() && x.Active == "Y").Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.Usertype == "C" || (x.Company != null && x.Company.Active == "Y"));
            if (u == null)
            {
                return BadRequest(new ResultModel { Message = "Mail adresi ya da şifre yanlış", Status = false });
            }
            CreatePasswordHash(model.psw, out var passwordHash, out var passwordSalt);
            if (passwordHash != u.Passhash || passwordSalt != u.Passsalt)
            {
                return BadRequest(new ResultModel { Message = "Mail adresi ya da şifre yanlış", Status = false });
            }

            return Ok(new ResultModel<LoginResultUser>
            {
                Status = true,
                Data = new LoginResultUser
                {
                    UserId = u.Id,
                    Ad = u.Name,
                    Soyad = u.Surname,
                    CompanyId = u.Companyid ?? 0m,
                    UserType = u.Usertype,
                    Token = _extentsion.CreateToken(u.Id, 0m, "C"),
                    CompanyName = u.Company?.Companyname ?? ""
                }
            });
        }
        private void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = Convert.ToBase64String(hmac.Key);
                passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }
        public bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }
    }
}
