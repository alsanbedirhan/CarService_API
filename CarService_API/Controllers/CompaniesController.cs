using CarService_API.Models.DB;
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
        IMemoryCache _cache;
        public CompaniesController(ModelContext context, Extentsion extentsion, IMemoryCache cache)
        {
            _context = context;
            _extentsion = extentsion;
            _cache = cache;
        }
        public class clsSaveEntry
        {
            public decimal UserCarId { get; set; }
            public string Aciklama { get; set; }
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
    }
}
