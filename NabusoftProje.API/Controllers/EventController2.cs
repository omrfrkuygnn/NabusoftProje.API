using EtkinlikOnay.API.Data;
using EtkinlikOnay.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EtkinlikOnay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Tüm etkinlikleri listele
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events.ToListAsync();
            return Ok(events); // 200 OK + JSON veri
        }

        // ✅ Belirli bir etkinliği onayla (Status = "Approved")
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveEvent(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt == null)
                return NotFound(); // 404 Not Found

            evt.Status = "Approved";             // Durumu onaylandı yap
            evt.RejectionNote = null;            // Reddetme notu varsa sıfırla

            await _context.SaveChangesAsync();   // Veritabanına kaydet
            return Ok(new { message = "Etkinlik onaylandı." });
        }

        // ✅ Belirli bir etkinliği reddet (Status = "Rejected") + Not bırak
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectEvent(int id, [FromBody] string rejectionNote)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt == null)
                return NotFound();

            evt.Status = "Rejected";             // Durumu reddedildi yap
            evt.RejectionNote = rejectionNote;   // Admin'in notunu kaydet

            await _context.SaveChangesAsync();
            return Ok(new { message = "Etkinlik reddedildi." });
        }
    }
}