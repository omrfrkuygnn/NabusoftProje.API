using Event.Data;
using Event.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EtkinlikKategorileri.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventCategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventCategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/eventcategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventCategory>>> GetEventCategories()
        {
            return await _context.EventCategories.ToListAsync();
        }

        // GET: api/eventcategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EventCategory>> GetEventCategory(int id)
        {
            var category = await _context.EventCategories.FindAsync(id);
            if (category == null)
                return NotFound();

            return category;
        }

        // POST: api/eventcategories
        [HttpPost]
        public async Task<ActionResult<EventCategory>> CreateEventCategory(EventCategory category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                return BadRequest("Kategori adı boş olamaz.");

            bool isExists = await _context.EventCategories
                .AnyAsync(c => c.Name.ToLower() == category.Name.ToLower());

            if (isExists)
                return Conflict("Aynı isimde bir kategori zaten var.");

            category.CreatedDate = DateTime.Now;
            _context.EventCategories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventCategory), new { id = category.EventCategoryId }, category);
        }

        // PUT: api/eventcategories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEventCategory(int id, EventCategory category)
        {
            if (id != category.EventCategoryId)
                return BadRequest();

            var existingCategory = await _context.EventCategories.FindAsync(id);
            if (existingCategory == null)
                return NotFound();

            // Kategori ismi başka bir kayıtta var mı kontrol et
            bool nameExists = await _context.EventCategories
                .AnyAsync(c => c.Name.ToLower() == category.Name.ToLower() && c.EventCategoryId != id);

            if (nameExists)
                return Conflict("Aynı isimde başka bir kategori zaten var."); // HTTP 409

            existingCategory.Name = category.Name;
            existingCategory.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/eventcategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventCategory(int id)
        {
            var category = await _context.EventCategories.FindAsync(id);
            if (category == null)
                return NotFound();

            _context.EventCategories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
