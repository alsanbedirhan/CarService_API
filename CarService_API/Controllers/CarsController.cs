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
        public class clsCars
        {
            public decimal Idno { get; set; }
            public decimal ModelId { get; set; }
            public string Marka { get; set; }
            public string Model { get; set; }
            public byte Yil { get; set; }
            public string Plaka { get; set; }
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
                    Details = x.Makemodels.Select(y => new clsSearch { Key = y.Id, DisplayValue = x.Explanation + " - " + y.Explanation }).ToList()
                }).ToListAsync();

                return Ok(new ResultModel<List<clsSearchDetail>> { Status = true, Data = l });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultModel { Status = false, Message = ex.Message });
            }
        }
    }
}
