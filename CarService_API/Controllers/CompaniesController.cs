using CarService_API.Models.DB;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CarService_API.Controllers
{
    [Route("api/[controller]")]
    public class CompaniesController : Controller
    {
        ModelContext _context;
        Extentsion _extentsion;
        public CompaniesController(ModelContext context, Extentsion extentsion)
        {
            _context = context;
            _extentsion = extentsion;
        }
        public class clsSaveEntry
        {
            public decimal UserCarId { get; set; }
            public string Aciklama { get; set; }
        }
        public class clsSearchCompanyWork : clsCompanyWorkList
        {
            public string Plaka { get; set; }
            public string Isdone { get; set; }
            public string Isout { get; set; }
            public DateTime StartDate { get; set; } = DateTime.MinValue;
            public DateTime EndDate { get; set; } = DateTime.MinValue;
        }
        public class SearchCompanyWorkList
        {
            public decimal Idno { get; set; }
            public string Ad { get; set; }
            public string Soyad { get; set; }
            public string Marka { get; set; }
            public string Model { get; set; }
            public string Plaka { get; set; }
            public decimal TotalPrice { get; set; }
        }
        public class CompanyWorkDetail
        {
            public decimal Idno { get; set; }
            public decimal Price { get; set; }
            public string Aciklama { get; set; }
            public DateTime Cdate { get; set; }
            public decimal Cuser { get; set; }
            public string Ad { get; set; }
            public string Soyad { get; set; }
        }
        public class clsCompanyWorkDetail
        {
            public decimal Idno { get; set; }
            public decimal CompanyWorkId { get; set; }
            public decimal Price { get; set; }
            public string Aciklama { get; set; }
        }
        public class clsIdno
        {
            public decimal Idno { get; set; }
        }
        public class clsCompanyWorkList
        {
            public string Ad { get; set; }
            public string Soyad { get; set; }
            public decimal MakeId { get; set; }
            public decimal MakeModelId { get; set; }
        }
        public class clsUpdateStatus
        {
            public decimal Idno { get; set; }
            public string Isdone { get; set; }
            public string Isout { get; set; }
        }
        [HttpPost("serviceentry")]
        public async Task<IActionResult> Entry([FromBody] clsSaveEntry input)
        {
            try
            {
                var u = _extentsion.GetTokenValues();
                if (u == null || u.UserType == "C")
                {
                    throw new Exception("Hata oluştu");
                }

                if (!(await _context.Usercars.AnyAsync(x => x.Id == input.UserCarId && x.Active == "Y")))
                {
                    throw new Exception("Araç bulunamadı");
                }

                if (!(await _context.Companyworks.AnyAsync(x => x.Usercarid == input.UserCarId && x.Active == "Y" && x.Isout == "N")))
                {
                    throw new Exception("Araç zaten içeridedir");
                }

                await _context.Companyworks.AddAsync(new Companywork
                {
                    Active = "Y",
                    Cdate = DateTime.Now,
                    Companyid = u.CompanyId,
                    Cuser = u.UserId,
                    Explanation = input.Aciklama?.Trim() ?? "",
                    Isdone = "N",
                    Usercarid = input.UserCarId
                });

                await _context.SaveChangesAsync();

                return Ok(new ResultModel { Status = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel { Status = false, Message = ex.Message });
            }
        }
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] clsSearchCompanyWork input)
        {
            try
            {
                if (input == null)
                {
                    throw new Exception("Hata oluştu");
                }
                var u = _extentsion.GetTokenValues();
                if (u == null)
                {
                    throw new Exception("Hata oluştu");
                }
                input.Ad = input.Ad?.Trim() ?? "";
                input.Soyad = input.Soyad?.Trim() ?? "";
                input.Plaka = input.Plaka?.Trim() ?? "";
                var l = await _context.Companyworks.Include(x => x.Usercar.Makemodel.Make).Include(x => x.Usercar.User).AsNoTracking()
                    .Where(x => x.Active == "Y" &&
                    (input.MakeModelId > 0 ? input.MakeModelId == x.Usercar.Makemodelid : input.MakeId > 0 ? input.MakeId == x.Usercar.Makemodel.Makeid : true) &&
                    (u.UserType == "C" ? x.Usercar.Userid == u.UserId : x.Companyid == u.CompanyId) &&
                    (!string.IsNullOrEmpty(input.Ad) ? x.Usercar.User.Name.ToLower().Contains(input.Ad.ToLower()) : true) &&
                    (!string.IsNullOrEmpty(input.Soyad) ? x.Usercar.User.Surname.ToLower().Contains(input.Soyad.ToLower()) : true) &&
                    (!string.IsNullOrEmpty(input.Plaka) ? x.Usercar.Plate != null && x.Usercar.Plate.ToLower().Contains(input.Plaka.ToLower()) : true) &&
                    (!string.IsNullOrEmpty(input.Isdone) ? x.Isdone == input.Isdone : true) &&
                    (!string.IsNullOrEmpty(input.Isout) ? x.Isout == input.Isout : true) &&
                    (input.StartDate.Year > 2010 && x.Cdate.HasValue ? x.Cdate.Value.Date >= input.StartDate : true) &&
                    (input.EndDate.Year > 2010 && x.Cdate.HasValue ? x.Cdate.Value.Date <= input.EndDate : true))
                    .Select(x => new SearchCompanyWorkList
                    {
                        Ad = x.Usercar.User.Name,
                        Soyad = x.Usercar.User.Surname,
                        Idno = x.Id,
                        Marka = x.Usercar.Makemodel.Make.Explanation ?? "",
                        Model = x.Usercar.Makemodel.Explanation ?? "",
                        Plaka = x.Usercar.Plate ?? "",
                        TotalPrice = x.Companyworkdetails.Where(y => y.Active == "Y").Sum(y => y.Price) ?? 0m
                    }).ToListAsync();

                return Ok(new ResultModel<List<SearchCompanyWorkList>> { Status = true, Data = l });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel { Status = false, Message = ex.Message });
            }
        }
        [HttpGet("detail")]
        public async Task<IActionResult> Detail(decimal headerid)
        {
            try
            {
                if (headerid <= 0)
                {
                    throw new Exception("Hata oluştu");
                }
                var u = _extentsion.GetTokenValues();
                if (u == null || u.UserType == "C")
                {
                    throw new Exception("Hata oluştu");
                }

                var l = await _context.Companyworkdetails.Include(x => x.Companywork).Include(x => x.User).AsNoTracking()
                    .Where(x => x.Companyworkid == headerid && x.Active == "Y" && x.Companywork.Active == "Y").Select(x => new CompanyWorkDetail
                    {
                        Aciklama = x.Explanation ?? "",
                        Cdate = x.Cdate ?? DateTime.MinValue,
                        Ad = x.User.Name,
                        Soyad = x.User.Surname,
                        Cuser = x.Userid,
                        Idno = x.Id,
                        Price = x.Price ?? 0m
                    }).ToListAsync();

                return Ok(new ResultModel<List<CompanyWorkDetail>> { Status = true, Data = l });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel { Status = false, Message = ex.Message });
            }
        }
        [HttpPost("workdetail")]
        public async Task<IActionResult> Work([FromBody] clsCompanyWorkDetail input)
        {
            try
            {
                if (input == null || string.IsNullOrEmpty(input.Aciklama?.Trim()))
                {
                    throw new Exception("Hata oluştu");
                }
                var u = _extentsion.GetTokenValues();
                if (u == null || u.UserType == "C")
                {
                    throw new Exception("Hata oluştu");
                }
                if (input.Idno > 0)
                {
                    var r = await _context.Companyworkdetails.FirstOrDefaultAsync(x => x.Id == input.Idno && x.Active == "Y");
                    if (r == null)
                    {
                        throw new Exception("Kayıt bulunamadı");
                    }
                    r.Price = input.Price;
                    r.Explanation = input.Aciklama?.Trim() ?? "";
                }
                else
                {
                    if (!(await _context.Companyworks.AnyAsync(x => x.Id == input.CompanyWorkId && x.Active == "Y" && x.Isdone == "N")))
                    {
                        throw new Exception("Kayıt bulunamadı");
                    }
                    await _context.Companyworkdetails.AddAsync(new Companyworkdetail
                    {
                        Companyworkid = input.CompanyWorkId,
                        Cdate = DateTime.Now,
                        Cuser = u.UserId,
                        Explanation = input.Aciklama?.Trim() ?? "",
                        Price = input.Price,
                        Userid = u.UserId,
                        Active = "Y"
                    });
                }
                await _context.SaveChangesAsync();

                return Ok(new ResultModel { Status = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel { Status = false, Message = ex.Message });
            }
        }
        [HttpPost("deletedetail")]
        public async Task<IActionResult> Delete([FromBody] clsIdno input)
        {
            try
            {
                if (input == null || input.Idno <= 0)
                {
                    throw new Exception("Hata oluştu");
                }
                var u = _extentsion.GetTokenValues();
                if (u == null || u.UserType == "C")
                {
                    throw new Exception("Hata oluştu");
                }
                var r = await _context.Companyworkdetails.FirstOrDefaultAsync(x => x.Id == input.Idno);
                if (r == null)
                {
                    throw new Exception("Kayıt bulunamadı");
                }
                if (r.Userid != u.UserId && u.UserType != "A")
                {
                    throw new Exception("Yetkiniz bulunamadı");
                }
                r.Active = "N";
                await _context.SaveChangesAsync();
                return Ok(new ResultModel { Status = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel { Status = false, Message = ex.Message });
            }
        }
        [HttpPost("status")]
        public async Task<IActionResult> Status([FromBody] clsUpdateStatus input)
        {
            try
            {
                if (input == null || input.Idno <= 0)
                {
                    throw new Exception("Hata oluştu");
                }
                var u = _extentsion.GetTokenValues();
                if (u == null || u.UserType == "C")
                {
                    throw new Exception("Yetkiniz bulunamadı");
                }
                var r = await _context.Companyworks.FirstOrDefaultAsync(x => x.Id == input.Idno && x.Active == "Y");
                if (r == null)
                {
                    throw new Exception("Kayıt bulunamadı");
                }
                r.Isdone = input.Isdone;
                r.Isout = input.Isout;
                await _context.SaveChangesAsync();

                return Ok(new ResultModel { Status = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel { Status = false, Message = ex.Message });
            }
        }
    }
}
