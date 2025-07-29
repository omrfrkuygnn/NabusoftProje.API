using LocationManagement.API.Data;
using LocationManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LocationManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DistrictController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DistrictController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ DROPDOWN: Seçilen şehre ait ilçeleri getir (MVC için)
        // GET: api/District/dropdown/1
        [HttpGet("dropdown/{cityId}")]
        public async Task<IActionResult> GetDistrictsByCityId(int cityId)
        {
            var districts = await _context.Districts
                .Where(d => d.CityId == cityId)
                .OrderBy(d => d.DistrictName)
                .Select(d => new SelectListItem
                {
                    Value = d.DistrictId.ToString(),
                    Text = d.DistrictName
                })
                .ToListAsync();

            return Ok(districts);
        }

        // ✅ TÜM İLÇELERİ GETİR
        // GET: api/District
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var districts = await _context.Districts
                .Include(d => d.City)
                .Include(d => d.Neighborhoods)
                    .ThenInclude(n => n.Quarters)
                .OrderBy(d => d.DistrictName)
                .ToListAsync();

            return Ok(districts);
        }

        // ✅ ID’ye göre ilçe getir
        // GET: api/District/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var district = await _context.Districts
                .Include(d => d.City)
                .Include(d => d.Neighborhoods)
                    .ThenInclude(n => n.Quarters)
                .FirstOrDefaultAsync(d => d.DistrictId == id);

            if (district == null)
                return NotFound();

            return Ok(district);
        }

        // ✅ YENİ İLÇE OLUŞTUR
        // POST: api/District
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] District district)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Districts.Add(district);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = district.DistrictId }, district);
        }

        // ✅ VAR OLAN İLÇEYİ GÜNCELLE
        // PUT: api/District/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] District updatedDistrict)
        {
            if (id != updatedDistrict.DistrictId)
                return BadRequest("ID uyuşmuyor.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var district = await _context.Districts.FindAsync(id);
            if (district == null)
                return NotFound();

            district.DistrictName = updatedDistrict.DistrictName;
            district.CityId = updatedDistrict.CityId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ İLÇEYİ SİL
        // DELETE: api/District/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var district = await _context.Districts.FindAsync(id);
            if (district == null)
                return NotFound();

            _context.Districts.Remove(district);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}