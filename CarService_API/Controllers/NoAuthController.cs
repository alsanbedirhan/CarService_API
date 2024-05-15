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
            try
            {
                if (model == null)
                {
                    throw new Exception("Parametre hatalı");
                }
                model.mail = model.mail.Trim();
                model.name = model.name.Trim();
                model.surname = model.surname.Trim();
                model.psw = model.psw.Trim();
                if (!CustomFunctions.IsValidEmail(model.mail))
                {
                    throw new Exception("Mail adresi geçersiz");
                }
                if (string.IsNullOrEmpty(model.psw) || model.psw.Length <= 5)
                {
                    throw new Exception("Şifre geçersiz");
                }
                var u = await _context.Users.FirstOrDefaultAsync(x => x.Mail == model.mail);
                if (u != null && (u.Active == "Y" || u.Usertype != "C"))
                {
                    throw new Exception("Mail adresi zaten kullanılmakta");
                }

                CustomFunctions.CreatePasswordHash(model.psw, out var passwordHash, out var passwordSalt);
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
                await _context.SaveChangesAsync();
                string token = _extentsion.CreateToken(u.Id, (u.Companyid ?? 0m), u.Usertype);
                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("Giriş yapılamadı");
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
                        Token = token,
                        CompanyName = u.Company?.Companyname ?? "",
                        Mail = u.Mail
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel { Message = ex.Message, Status = false });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] clsLoginModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new Exception("Parametre hatalı");
                }
                var u = await _context.Users.AsNoTracking().Where(x => x.Mail == model.mail.Trim() && x.Active == "Y").Include(x => x.Company)
                    .FirstOrDefaultAsync(x => x.Usertype == "C" || (x.Company != null && x.Company.Active == "Y"));
                if (u == null)
                {
                    throw new Exception("Mail adresi ya da şifre yanlış");
                }
                if (CustomFunctions.VerifyPassword(model.psw, u.Passhash, u.Passsalt))
                {
                    throw new Exception("Mail adresi ya da şifre yanlış");
                }
                //CreatePasswordHash(model.psw, out var passwordHash, out var passwordSalt);
                //if (passwordHash != u.Passhash || passwordSalt != u.Passsalt)
                //{
                //    throw new Exception("Mail adresi ya da şifre yanlış");
                //}
                string token = _extentsion.CreateToken(u.Id, (u.Companyid ?? 0m), u.Usertype);
                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("Giriş yapılamadı");
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
                        Token = token,
                        CompanyName = u.Company?.Companyname ?? "",
                        Mail = u.Mail
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel { Message = ex.Message, Status = false });
            }
        }
    }
}
