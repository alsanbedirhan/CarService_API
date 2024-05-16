using CarService_API.Models.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

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
        class clsSearchCar
        {
            public decimal MakeId { get; set; }
            public decimal MakeModelId { get; set; }
        }
        class clsCars
        {
            public decimal Idno { get; set; }
            public decimal ModelId { get; set; }
            public string Marka { get; set; }
            public string Model { get; set; }
            public byte Yil { get; set; }
            public string Plaka { get; set; }
        }
        [HttpPost("allcars")]
        async Task<IActionResult> AllCars([FromBody] clsSearchCar input)
        {
            try
            {
                var u = _extentsion.GetTokenValues();
                if (u == null || u.UserType != "A")
                {
                    throw new Exception("Hata oluştu");
                }

                var l = await _context.Usercars.AsNoTracking().Include(x => x.Makemodel).Where(x => x.Userid == u.UserId &&
                (input != null && input.MakeId > 0 ? (input.MakeModelId > 0 ? x.Makemodelid == x.Makemodelid : x.Makemodel.Makeid == input.MakeId) : true))
                    .Select(x => new clsCars
                    {
                        Idno = x.Id,
                        Marka = x.Makemodel.Make.Explanation,
                        Model = x.Makemodel.Explanation,
                        ModelId = x.Makemodelid,
                        Plaka = x.Plate,
                        Yil = x.Pyear ?? 0
                    }).OrderByDescending(x => x.Idno).ToListAsync();

                return Ok(new ResultModel<List<clsCars>> { Status = true, Data = l });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel { Status = false, Message = ex.Message });
            }
        }
    }
}
