using NabusoftProje.API.Data;
using NabusoftProje.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly EventDbContext _context;
    public CalendarController(EventDbContext context)
    {
        _context = context;
    }

    // POST: /api/calendar/save
    [HttpPost("save")]
    public async Task<IActionResult> SaveToCalendar([FromBody] CalendarSaveDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        // K3: Aynı etkinlik birden fazla kez eklenemez
        var exists = await _context.UserCalendars.AnyAsync(x => x.UserId == userId && x.EventId == dto.EventId && x.CalendarType == dto.CalendarType);
        if (exists)
            return BadRequest("Bu etkinlik zaten takviminize eklenmiş.");

        var entry = new UserCalendar
        {
            UserId = userId,
            EventId = dto.EventId,
            CalendarType = dto.CalendarType
        };
        _context.UserCalendars.Add(entry);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Etkinlik takvime eklendi." });
    }

    // DELETE: /api/calendar/remove/{id}
    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> RemoveFromCalendar(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var entry = await _context.UserCalendars.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (entry == null)
            return NotFound();
        _context.UserCalendars.Remove(entry);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Etkinlik takvimden kaldırıldı." });
    }

    // (Opsiyonel) Kullanıcının takvimine eklediği etkinlikleri getir
    [HttpGet("my")] 
    public async Task<IActionResult> GetMyCalendar()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var list = await _context.UserCalendars.Where(x => x.UserId == userId).ToListAsync();
        return Ok(list);
    }
}

