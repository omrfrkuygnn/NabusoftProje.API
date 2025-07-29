using LocationManagement.API.Data;
using LocationManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LocationManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Tüm şehirleri getir (ilişkilerle) - GET: api/City
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cities = await _context.Cities
                .Include(c => c.Districts)
                    .ThenInclude(d => d.Neighborhoods)
                        .ThenInclude(n => n.Quarters)
                .ToListAsync();

            return Ok(cities);
        }

        // ✅ Sadece dropdown için şehir listesi (id-name) - GET: api/City/dropdown
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var cityList = await _context.Cities
                .OrderBy(c => c.CityName)
                .Select(c => new SelectListItem
                {
                    Value = c.CityId.ToString(),
                    Text = c.CityName
                })
                .ToListAsync();

            return Ok(cityList);
        }

        // ✅ ID'ye göre şehir getir - GET: api/City/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var city = await _context.Cities
                .Include(c => c.Districts)
                    .ThenInclude(d => d.Neighborhoods)
                        .ThenInclude(n => n.Quarters)
                .FirstOrDefaultAsync(c => c.CityId == id);

            if (city == null)
                return NotFound();

            return Ok(city);
        }

        // ✅ Yeni şehir ekle - POST: api/City
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] City city)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = city.CityId }, city);
        }

        // ✅ Şehir güncelle - PUT: api/City/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] City updatedCity)
        {
            if (id != updatedCity.CityId)
                return BadRequest();

            var city = await _context.Cities.FindAsync(id);
            if (city == null)
                return NotFound();

            city.CityName = updatedCity.CityName;
            city.PlateCode = updatedCity.PlateCode;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ Şehir sil - DELETE: api/City/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
                return NotFound();

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}