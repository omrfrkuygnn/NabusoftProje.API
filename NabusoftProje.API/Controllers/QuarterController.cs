using LocationManagement.API.Data;
using LocationManagement.API.Models;
using LocationManagement.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocationManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuarterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuarterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ 1. DROPDOWN için: Seçilen mahalleye bağlı semt/köyleri getir
        // GET: api/Quarter/neighborhood/5
        // Geri dönen model: QuarterDropdownViewModel { Id, Name }
        [HttpGet("neighborhood/{neighborhoodId}")]
        public async Task<IActionResult> GetQuartersByNeighborhoodId(int neighborhoodId)
        {
            var quarters = await _context.Quarters
                .Where(q => q.NeighborhoodId == neighborhoodId)
                .OrderBy(q => q.QuarterName)
                .Select(q => new QuarterDropdownViewModel
                {
                    Id = q.QuarterId,
                    Name = q.QuarterName
                })
                .ToListAsync();

            return Ok(quarters);
        }

        // ✅ 2. Tüm Semt/Köyleri getir (admin panel gibi yerlerde listeleme için)
        // GET: api/Quarter
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var quarters = await _context.Quarters
                .Include(q => q.Neighborhood)
                .OrderBy(q => q.QuarterName)
                .ToListAsync();

            return Ok(quarters);
        }

        // ✅ 3. Belirli bir semt/köyü getir
        // GET: api/Quarter/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var quarter = await _context.Quarters
                .Include(q => q.Neighborhood)
                .FirstOrDefaultAsync(q => q.QuarterId == id);

            if (quarter == null)
                return NotFound();

            return Ok(quarter);
        }

        // ✅ 4. Yeni semt/köy ekle
        // POST: api/Quarter
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Quarter quarter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Quarters.Add(quarter);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = quarter.QuarterId }, quarter);
        }

        // ✅ 5. Semt/köy güncelle
        // PUT: api/Quarter/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Quarter updatedQuarter)
        {
            if (id != updatedQuarter.QuarterId)
                return BadRequest("ID uyuşmuyor.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var quarter = await _context.Quarters.FindAsync(id);
            if (quarter == null)
                return NotFound();

            quarter.QuarterName = updatedQuarter.QuarterName;
            quarter.NeighborhoodId = updatedQuarter.NeighborhoodId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ 6. Semt/köy sil
        // DELETE: api/Quarter/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var quarter = await _context.Quarters.FindAsync(id);
            if (quarter == null)
                return NotFound();

            _context.Quarters.Remove(quarter);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}