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
        IMemoryCache _cache;
        public CarsController(ModelContext context, Extentsion extentsion, IMemoryCache cache)
        {
            _context = context;
            _extentsion = extentsion;
            _cache = cache;
        }
        public class clsWorkCar
        {
            public decimal Idno { get; set; }
            public decimal ModelId { get; set; }
            public short Yil { get; set; }
            public string Plaka { get; set; }
            public decimal UserId { get; set; }
        }
        public class clsCars : clsWorkCar
        {
            public decimal MarkaId { get; set; }
            public string Marka { get; set; }
            public string Model { get; set; }
        }
        public class clsSearchCar
        {
            public decimal UserId { get; set; }
            public decimal MakeId { get; set; }
            public decimal MakeModelId { get; set; }
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
                (input.MakeModelId > 0 ? input.MakeModelId == x.Makemodelid : (input.MakeId > 0 ? input.MakeId == x.Makemodel.Makeid : true)))
                    .Select(x => new clsCars
                    {
                        Idno = x.Id,
                        Marka = x.Makemodel.Make.Explanation ?? "",
                        Model = x.Makemodel.Explanation ?? "",
                        ModelId = x.Makemodelid,
                        Plaka = x.Plate ?? "",
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
                var makes = _cache.Get<List<Make>>("makemodels");
                if (makes == null)
                {
                    makes = await _context.Makes.Include(x => x.Makemodels).AsNoTracking().ToListAsync();
                    _cache.Set("makes", makes);
                }
                var l = makes.Select(x => new clsSearchDetail
                {
                    Key = x.Id,
                    DisplayValue = x.Explanation ?? "",
                    Details = x.Makemodels.Select(y => new clsSearch { Key = y.Id, DisplayValue = y.Explanation ?? "" }).ToList()
                }).ToList();

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
                if (u == null || input == null || input.ModelId <= 0 || u.UserId <= 0 || string.IsNullOrEmpty(input.Plaka) || Convert.ToInt32(input.Yil) <= 1900)
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
                        Userid = input.UserId,
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
