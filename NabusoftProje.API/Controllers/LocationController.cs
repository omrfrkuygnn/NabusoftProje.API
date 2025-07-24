using Event.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Event.Controllers
{
    [Route("api/location")]
    [ApiController]
    public class LocationController: ControllerBase
    {
        private readonly AppDbContext _context;
        public LocationController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("iller")]
        public async Task<IActionResult> GetIller()
        {
            var data = await _context.Illers.ToListAsync();
            return Ok(data);
        }

        [HttpGet("ilceler/{ilId}")]
        public async Task<IActionResult> GetIlcelerByIlId(int ilId)
        {
            var data = await _context.Ilcelers
                .Where(i=>i.IlId == ilId)
                .ToListAsync();
            return Ok(data);
        }
        [HttpGet("mahalleler/{ilceId}")]
        public async Task<IActionResult> GetMahallelerByIlceId(int ilceId)
        {
            var data = await _context.Mahallelers
                .Where(m=> m.IlceId == ilceId)
                .ToListAsync();
            return Ok(data);
        }
        [HttpGet("csbms/{mahalleId}")]
        public async Task<IActionResult> GetCsbmsByMahalleId(int mahalleId)
        {
            var data = await _context.Csbms
                .Where(c=> c.MahalleId == mahalleId)
                .ToListAsync();
            return Ok(data);
        }
    }
}
