using Event.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Event.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context;
        public NotificationController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("my")]
        public async Task<IActionResult> GetMyNotifications()
        {
            int userId = 3;
            var notifications = await _context.Notifications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
            return Ok(notifications);
        }
        [HttpPost("mark-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notif = await _context.Notifications.FindAsync(id);
            if (notif == null) return NotFound();

            notif.IsRead = true;
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notif = await _context.Notifications.FindAsync(id);
            if (notif == null) return NotFound();

            _context.Notifications.Remove(notif);
            await _context.SaveChangesAsync();
            return Ok("Bildirim silindi.");
        }
        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllRead()
        {
            int userId = 3; // Giriş kullanıcısı

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            notifications.ForEach(n => n.IsRead = true);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAll()
        {
            int userId = 3; // Giriş kullanıcısı

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .ToListAsync();

            _context.Notifications.RemoveRange(notifications);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
