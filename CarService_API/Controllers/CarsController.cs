using CarService_API.Models.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using static CarService_API.Controllers.UsersController;

namespace CarService_API.Controllers
{
    [Route("api/[controller]")]
    public class CarsController : Controller
    {
        ModelContext _context;
        Extentsion _extentsion;
        public CarsController(ModelContext context, Extentsion extentsion)
        {
            _context = context;
            _extentsion = extentsion;
        }
        public class clsWorkCar
        {
            public decimal Idno { get; set; }
            public decimal ModelId { get; set; }
            public byte Yil { get; set; }
            public string Plaka { get; set; }
            public decimal UserId { get; set; }
        }
        public class clsCars
        {
            public decimal Idno { get; set; }
            public decimal ModelId { get; set; }
            public decimal MarkaId { get; set; }
            public decimal UserId { get; set; }
            public string Marka { get; set; }
            public string Model { get; set; }
            public byte Yil { get; set; }
            public string Plaka { get; set; }
        }
        public class clsSearchCar
        {
            public decimal UserId { get; set; }
            public List<decimal> MakeIds { get; set; }
            public List<decimal> MakeModelIds { get; set; }
        }

        [HttpPost("allcars")]
        public async Task<IActionResult> AllCars([FromBody] clsSearchCar input)
        {
            try
            {
                if (input == null || input.UserId <= 0)
                {
                    throw new Exception("Hata oluştu");
                }
                var u = _extentsion.GetTokenValues();
                if (u == null)
                {
                    throw new Exception("Hata oluştu");
                }

                var l = await _context.Usercars.AsNoTracking().Include(x => x.Makemodel).Where(x => x.Userid == input.UserId &&
                (input.MakeModelIds.Any() ? input.MakeIds.Contains(x.Makemodelid) : (input.MakeIds.Any() ? input.MakeIds.Contains(x.Makemodel.Makeid) : true)))
                    .Select(x => new clsCars
                    {
                        Idno = x.Id,
                        Marka = x.Makemodel.Make.Explanation,
                        Model = x.Makemodel.Explanation,
                        ModelId = x.Makemodelid,
                        Plaka = x.Plate,
                        Yil = x.Pyear ?? 0,
                        MarkaId = x.Makemodel.Makeid,
                        UserId = x.Userid
                    }).OrderByDescending(x => x.Idno).ToListAsync();

                return Ok(new ResultModel<List<clsCars>> { Status = true, Data = l });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel { Status = false, Message = ex.Message });
            }
        }
        [HttpGet("makemodels")]
        public async Task<IActionResult> Makes()
        {
            try
            {
                var u = _extentsion.GetTokenValues();
                if (u == null)
                {
                    throw new Exception("Hata oluştu");
                }

                var l = await _context.Makes.Include(x => x.Makemodels).AsNoTracking().Select(x => new clsSearchDetail
                {
                    Key = x.Id,
                    DisplayValue = x.Explanation,
                    Details = x.Makemodels.Select(y => new clsSearch { Key = y.Id, DisplayValue = y.Explanation }).ToList()
                }).ToListAsync();

                return Ok(new ResultModel<List<clsSearchDetail>> { Status = true, Data = l });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel { Status = false, Message = ex.Message });
            }
        }

        [HttpPost("workcar")]
        public async Task<IActionResult> WorkCars([FromBody] clsWorkCar input)
        {
            try
            {
                var u = _extentsion.GetTokenValues();
                if (u == null || input == null || input.ModelId <= 0 || u.UserId <= 0)
                {
                    throw new Exception("Hata oluştu");
                }
                var t = await _context.Users.AsNoTracking().Where(x => x.Id == u.UserId && x.Active == "Y").Select(x => new { x.Usertype }).FirstOrDefaultAsync();
                if (t == null)
                {
                    throw new Exception("Kullanıcı geçersiz");
                }
                input.Plaka = input.Plaka?.Trim() ?? "";
                if (input.Idno > 0)
                {
                    var f = await _context.Usercars.FirstOrDefaultAsync(x => x.Id == input.Idno);
                    if (f == null)
                    {
                        throw new Exception("Kullanıcı bulunamadı");
                    }
                    f.Plate = input.Plaka;
                    f.Makemodelid = input.ModelId;
                    f.Pyear = input.Yil;
                }
                else
                {
                    await _context.Usercars.AddAsync(new Models.DB.Usercar
                    {
                        Userid = u.UserId,
                        Makemodelid = input.ModelId,
                        Plate = input.Plaka,
                        Pyear = input.Yil,
                        Cdate = DateTime.Now,
                        Cuser = u.UserId
                    });
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
