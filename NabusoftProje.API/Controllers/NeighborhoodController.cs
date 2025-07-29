using LocationManagement.API.Data;
using LocationManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LocationManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NeighborhoodController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NeighborhoodController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ DROPDOWN: İlçeye bağlı mahalleleri getir (MVC tarafında kullanılır)
        // GET: api/Neighborhood/dropdown/5
        [HttpGet("dropdown/{districtId}")]
        public async Task<IActionResult> GetDropdownByDistrictId(int districtId)
        {
            var result = await _context.Neighborhoods
                .Where(n => n.DistrictId == districtId)
                .OrderBy(n => n.NeighborhoodName)
                .Select(n => new SelectListItem
                {
                    Value = n.NeighborhoodId.ToString(),
                    Text = n.NeighborhoodName
                })
                .ToListAsync();

            return Ok(result);
        }

        // ✅ TÜM MAHALLELERİ GETİR
        // GET: api/Neighborhood
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var neighborhoods = await _context.Neighborhoods
                .Include(n => n.District)
                .Include(n => n.Quarters)
                .OrderBy(n => n.NeighborhoodName)
                .ToListAsync();

            return Ok(neighborhoods);
        }

        // ✅ MAHALLE ID'YE GÖRE GETİR
        // GET: api/Neighborhood/7
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var neighborhood = await _context.Neighborhoods
                .Include(n => n.District)
                .Include(n => n.Quarters)
                .FirstOrDefaultAsync(n => n.NeighborhoodId == id);

            if (neighborhood == null)
                return NotFound();

            return Ok(neighborhood);
        }

        // ✅ MAHALLE OLUŞTUR
        // POST: api/Neighborhood
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Neighborhood neighborhood)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Neighborhoods.Add(neighborhood);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = neighborhood.NeighborhoodId }, neighborhood);
        }

        // ✅ MAHALLE GÜNCELLE
        // PUT: api/Neighborhood/7
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Neighborhood updated)
        {
            if (id != updated.NeighborhoodId)
                return BadRequest("ID eşleşmiyor.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var neighborhood = await _context.Neighborhoods.FindAsync(id);
            if (neighborhood == null)
                return NotFound();

            neighborhood.NeighborhoodName = updated.NeighborhoodName;
            neighborhood.DistrictId = updated.DistrictId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ MAHALLE SİL
        // DELETE: api/Neighborhood/7
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var neighborhood = await _context.Neighborhoods.FindAsync(id);
            if (neighborhood == null)
                return NotFound();

            _context.Neighborhoods.Remove(neighborhood);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}