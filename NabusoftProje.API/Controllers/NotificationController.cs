using EtkinlikKatilimApi.Data;
using EtkinlikKatilimApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly EventDbContext _context;

    public NotificationController(EventDbContext context)
    {
        _context = context;
    }

    [HttpPost("haberdar-et")]
    public async Task<IActionResult> HaberdarEt([FromBody] int eventId)
    {
     var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userIdClaim))
    return Unauthorized("Kullanıcı kimliği bulunamadı.");

var userId = int.Parse(userIdClaim);

        var evt = await _context.Events.Include(e => e.Organizer).FirstOrDefaultAsync(e => e.EventId == eventId);
        if (evt == null)
            return NotFound("Etkinlik bulunamadı.");

        var bildirim = new Notification
        {
            EventId = eventId,
            UserId = userId,
            Message = $"Kullanıcı {userId}, '{evt.Title}' etkinliği hakkında haberdar olmak istiyor.",
            CreatedAt = DateTime.Now
        };

        _context.Notifications.Add(bildirim);
        await _context.SaveChangesAsync();

        return Ok("Etkinlik sahibine bildirim gönderildi.");
    }

    // Organizer için tüm bildirimler
    [HttpGet("organizer-bildirimler")]
    public IActionResult GetNotificationsForOrganizer()
    {
        var organizerId = int.Parse(User.Identity.Name);

        var bildirimler = _context.Notifications
            .Include(n => n.User)
            .Include(n => n.Event)
            .Where(n => n.Event.OrganizerId == organizerId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new {
                n.Id,
                n.Message,
                n.CreatedAt,
                n.IsRead
            })
            .ToList();

        return Ok(bildirimler);
    }

    [HttpPost("yanitla")]
    public async Task<IActionResult> Yanitla([FromBody] NotificationResponseModel model)
    {
        var organizerIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(organizerIdClaim))
            return Unauthorized("Kullanıcı kimliği bulunamadı.");

        var organizerId = int.Parse(organizerIdClaim);

        var notification = await _context.Notifications.Include(n => n.Event)
            .FirstOrDefaultAsync(n => n.Id == model.NotificationId);

        if (notification == null)
            return NotFound("Bildirim bulunamadı.");

        if (notification.Event.OrganizerId != organizerId)
            return Forbid("Yetkiniz yok.");

        notification.Response = model.Response;
        notification.ResponseDate = DateTime.Now;

        await _context.SaveChangesAsync();

        return Ok("Yanıt kaydedildi.");
    }
    [HttpGet("user-bildirimler")]
    public async Task<IActionResult> GetNotificationsForUser()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized("Kullanıcı kimliği bulunamadı.");

        var userId = int.Parse(userIdClaim);

        var bildirimler = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new
            {
                n.Id,
                n.Message,
                n.CreatedAt,
                n.IsRead
            })
            .ToListAsync();

        return Ok(bildirimler);
    }
    [HttpPost("okundu")]
public async Task<IActionResult> MarkAsRead(int id)
{
    var notification = await _context.Notifications.FindAsync(id);
    if (notification == null)
        return NotFound();

    notification.IsRead = true;
    await _context.SaveChangesAsync();
    return Ok();
}

// api/notification/sil?id=5
[HttpDelete("sil")]
public async Task<IActionResult> Delete(int id)
{
    var notification = await _context.Notifications.FindAsync(id);
    if (notification == null)
        return NotFound();

    _context.Notifications.Remove(notification);
    await _context.SaveChangesAsync();
    return Ok();
}

// api/notification/tumunu-sil
[HttpDelete("tumunu-sil")]
public async Task<IActionResult> DeleteAll()
{
    var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

    var userId = int.Parse(userIdClaim);
    var bildirimler = _context.Notifications.Where(n => n.UserId == userId);
    _context.Notifications.RemoveRange(bildirimler);
    await _context.SaveChangesAsync();

    return Ok();
}
    [HttpGet("okunmamis-sayisi")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var count = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync();

        return Ok(count);
    }
  
}
